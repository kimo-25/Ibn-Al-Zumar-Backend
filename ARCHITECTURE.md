# ARCHITECTURE.md — Ibn Al-Zomar ERP & POS System

> **Purpose:** This file is the **single source of truth (System Context)** for the Ibn Al-Zomar
> platform. Attach it to any prompt sent to an AI coding tool (Claude, Qwen, Manus, Cursor, Copilot…).
> It describes **what exists today in the codebase** and **what is planned but not yet built**.
>
> **Rule for AI tools:** Never invent entities, folders, enums, or endpoints. Anything not listed
> here does not exist. Every item below is tagged:
>
> | Tag | Meaning |
> |---|---|
> | `[IMPL]` | Implemented and present in the repository today. |
> | `[PARTIAL]` | Code/schema exists, but the end-to-end feature is incomplete, unregistered, unmounted, or blocked by a verified defect. |
> | `[PLANNED]` | Target design. **Does not exist yet.** Build it exactly as specified here. |
>
> **Repository snapshot (2026-10-02):** Backend `kimo-25/Ibn-Al-Zumar-Backend` at `8686125` and
> Frontend `kimo-25/IbnAlZumar-Frontend` at `0709df2b` (both `main` at review time). This file was
> not present in either repository; the canonical copy now lives at the Backend repository root.
> Re-check `git log -1` in both repositories before treating this snapshot as current. See §12 for
> the verified changes and outstanding integration gaps.

---

> **Canonical-document note:** At the 2026-10-02 repository review, neither selected repository
> contained `ARCHITECTURE.md`; this copy is added at the Backend repository root as the single
> cross-repository reference. Do not maintain a second divergent copy in the Frontend repository.

## 1. System Overview

| Item | Value |
|---|---|
| **App name** | Ibn Al-Zomar ERP & POS System (ابن الزمر) |
| **Business** | Hardware / tools retail store in Egypt: retail storefront, in-store POS, wholesale, warehousing, purchasing, maintenance workshop, HR & payroll |
| **Currency** | EGP (Egyptian Pound) |
| **Primary language** | Arabic (RTL) with English (LTR) as secondary — every user-facing string is bilingual |
| **Repositories** | `kimo-25/Ibn-Al-Zumar-Backend` (API) · `kimo-25/IbnAlZumar-Frontend` (Web SPA/PWA) |

### 1.1 Core stack — as actually built `[IMPL]`

| Concern | Technology |
|---|---|
| API runtime | **ASP.NET Core Web API, `net9.0`** (target framework in `Ibn al-Zumar.API.csproj` is `net9.0`, **not** net8.0 — do not downgrade) |
| Language | C# 13, `<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>` |
| Architecture style | **Layered "Clean-ish" single project**: `Domain/` → `Persistence/` → `Services/` → `Controllers/` inside one csproj (see §2.1). A strict 4-project Clean Architecture split is `[PLANNED]`. |
| ORM | Entity Framework Core 9 (`Microsoft.EntityFrameworkCore.SqlServer`), code-first migrations |
| Database | **SQL Server / Azure SQL** (`options.UseSqlServer(...)`). PostgreSQL is `[PLANNED]` — any raw SQL must stay provider-neutral or be guarded. |
| AuthN | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`), `PasswordHasher<User>` |
| AuthZ | Dynamic permission-code policies (`PermissionPolicyProvider` + `PermissionAuthorizationHandler`) |
| Validation | DataAnnotations on DTOs + `FluentValidation.AspNetCore` **package referenced but no `AbstractValidator` written yet** → `[PARTIAL]` |
| Mapping | `Mapster` referenced; current services map **manually** in service classes → `[PARTIAL]` |
| Excel / files | `ClosedXML`, `DocumentFormat.OpenXml` |
| Email | MailKit + **Brevo** (`EmailSettings`, config section `Brevo`) |
| HTTP clients | `RestSharp`, typed `HttpClient` via `AddHttpClient`; Quartz 3.8 packages are referenced, but no scheduler/job registration is active in the actual startup project. |
| API docs | Swashbuckle / Swagger at `/swagger` (enabled in all environments) |
| Frontend | **React 18 + Vite 5 + JavaScript (JSX, no TypeScript)** |
| Styling | **Tailwind CSS 3** with a custom design token theme (§6.2) |
| Routing | `react-router-dom` v6 |
| HTTP | `axios` with a single shared instance + interceptors |
| Offline | **PWA** (`vite-plugin-pwa`) + **Dexie (IndexedDB)** offline POS queue with idempotent batch sync |
| Icons | `lucide-react` |
| Client validation | **Zod** schemas (`src/validators/`) |
| Print / export | Frontend format renderers in `src/utils/print/` (A4, A5, thermal 80/58 mm) plus legacy `printInvoice.js`; `xlsx`, `html2pdf.js`, `html2canvas`. Backend QuestPDF service code exists for shareable PDFs but is not wired into DI yet. |
| Hosting | API → Azure App Service (`Dockerfile`, .NET 9 image, `PORT` env var). Frontend → **GitHub Pages** (`base: '/IbnAlZumar-Frontend/'`) via `.github/workflows/deploy.yml` |

### 1.2 High-level runtime topology

┌──────────────────────────┐ HTTPS / JWT Bearer ┌───────────────────────────┐
│ React PWA (GitHub Pages)│ ───────────────────────────────► │ ASP.NET Core API (Azure) │
│ • Storefront (customer) │ ◄─────────── JSON ───────────── │ Controllers → Services │
│ • Admin / Owner console │ │ → EF Core → Azure SQL │
│ • POS (offline-capable) │ └────────────┬──────────────┘
│ • Dexie queue + SW │ │
└──────────────────────────┘ │
offline writes ──► /api/orders/sync (idempotent by ClientUuid) │
▼
External: Paymob (payments) · Gemini (AI assistant) · Brevo (email) · Hugging Face (voice biometrics) · Google OAuth
Translation provider client is registered. WhatsApp transport/notification and Quartz debt-job code exist but are not registered/scheduled in the actual startup project; see §5.2 and §10.1.


---

## 2. Architecture Layers & Directory Structure

### 2.1 Backend — `Ibn-Al-Zumar-Backend`

Solution root: `Ibn al-Zumar.API/Ibn al-Zumar.API.sln`.
**All production code lives in the single project `Ibn al-Zumar.API/Ibn al-Zumar.API/`.**
`Ibn al-Zumar.Domain/`, `Controllers/` and `Persistence/` at the *solution* root are **legacy/dead
folders not referenced by the csproj — never edit or add files there.**

Ibn al-Zumar.API/ # solution folder
├── Ibn al-Zumar.API.sln
├── Dockerfile # .NET 9 SDK build → aspnet:9.0 runtime, EXPOSE 8080
└── Ibn al-Zumar.API/ # ★ THE project — all new code goes here
├── Program.cs # composition root: DI, JWT, CORS, Swagger, pipeline
├── appsettings*.json # ⚠ see §9.3 secret hygiene warning
│
├── Domain/ # ── DOMAIN LAYER (no EF/ASP.NET dependencies)
│ ├── Common/BaseEntity.cs # Id, CreatedAt, UpdatedAt, IsDeleted
│ ├── Enums/Enums.cs # ALL enums live in this one file
│ └── Entities/
│ ├── Catalog/ # Product, Category, Brand, ProductVariant, ProductPrice, images, attributes,
│ │ # ProductVariantAttributeValue, UnitConversion
│ ├── Inventory/ # Warehouse, ProductStock, InventoryTransaction, StockTransfer(+Item),
│ │ # ProductBatch
│ ├── Sales/ # Customer, Order, OrderItem, Payment, CustomerLedgerEntry, Invoice,
│ │ # CustomerDebtSchedule, ShippingZone
│ ├── Maintenance/ # MaintenanceRequest, MaintenanceNote, MaintenancePartUsage
│ ├── Purchasing/ # Supplier, PurchaseOrder(+Item), SupplierPayment, SupplierLedgerEntry
│ ├── Identity/ # User, Role, Permission, UserRole, RolePermission, UserPermission
│ ├── Attendance/ # AttendanceLog, PayrollRecord
│ ├── Reminders/ # Reminder (Quran / Dhikr banner)
│ ├── Notifications/ # NotificationLog
│ └── Ai/ # AiAuditLog
│
├── Persistence/ # ── INFRASTRUCTURE (data)
│ ├── ApplicationDbContext.cs # DbSets, global soft-delete filter, decimal(18,2) convention
│ ├── Configurations/<Module>/ # IEntityTypeConfiguration<T>, auto-applied via
│ │ # ApplyConfigurationsFromAssembly
│ └── Seed/DataSeeder.cs # PermissionCodes constants, roles, admin user, Warehouse Id=1,
│ # Products.csv, Reminders.csv, Opening Balance Supplier
├── Migrations/ # EF Core migrations (auto-applied at startup)
├── Infrastructure/Jobs/ # DebtReminderJob class (not yet registered/scheduled)
│
├── Services/ # ── APPLICATION LAYER (business logic, one folder per module)
│ ├── Auth/ Catalog/ Customers/ Identity/ Inventory/ Purchasing/ Sales/
│ ├── Attendance/ # AttendanceService, VoiceVerificationService
│ ├── Invoices/ # InvoiceService, InvoicePdfService (code present; DI not registered)
│ ├── Maintenance/ # MaintenanceWorkflowService
│ ├── Notifications/ # NotificationComposer, NotificationSender, WhatsApp transport (DI not registered)
│ ├── Payments/ # PaymobService + PaymobOptions
│ ├── Email/ # Brevo/MailKit EmailService
│ ├── Reminders/
│ └── Ai/ # AiAssistantService, VoiceCommandService,
│ ├── Tools/ # IAiTool implementations + AiToolRegistry
│ └── Files/ # AiFileProcessingService (multimodal uploads)
│
├── DTOs/<Module>/ # request/response contracts, mirrors Services/ folders
├── Controllers/ # ── API LAYER, thin: validate → call service → return; includes POS, pricing,
│ # translation, maintenance-workflow, invoice, and customer-debt controllers
├── Authorization/ # PermissionPolicyProvider, PermissionAuthorizationHandler,
│ # PermissionRequirement
├── Middleware/ExceptionHandlingMiddleware.cs
├── Common/
│ ├── Settings/ # JwtSettings, EmailSettings, GeminiSettings (IOptions pattern)
│ ├── Helpers/SlugHelper.cs
│ └── Exceptions/AppExceptions.cs # NotFoundException, BadRequestException
└── wwwroot/uploads/ , uploads/ # static files; /uploads is mapped to ContentRoot/uploads

> **Project-root caveat:** the compiled project is `Ibn al-Zumar.API/Ibn al-Zumar.API/`. The
> sibling file `Ibn al-Zumar.API/IbnAlZumar.API/Program.cs` is outside that `.csproj` and is not
> the runtime composition root. Edit and audit the `Program.cs` inside the compiled project only.


**Dependency direction (must never be violated):**
`Controllers → Services → Persistence(DbContext) → Domain`. `Domain` depends on nothing.
Controllers must **not** inject `ApplicationDbContext` in new code (some legacy controllers such as
`MaintenanceController` and `ExpensesController` still do — treat those as debt to refactor).

### 2.2 Frontend — `IbnAlZumar-Frontend`

src/
├── main.jsx # providers: Auth, Theme, Language, Router (basename = import.meta.env.BASE_URL)
├── App.jsx # ★ ALL routes in one file, grouped: storefront / auth / pos / moderator / admin
├── index.css # Tailwind layers + global RTL rules
│
├── api/ # ONE module per backend area; only these files may call axios
│ ├── axiosInstance.js # baseURL from config, JWT injection, 401 → clear session + redirect,
│ │ # normalizes errors to { statusCode, message, errors, traceId }
│ ├── adminApi.js moderatorApi.js storefrontApi.js userApi.js
│ ├── inventoryApi.js purchasingApi.js reportsApi.js attendanceApi.js
│ ├── posApi.js debtApi.js invoicesApi.js maintenanceWorkflowApi.js translationApi.js
│ └── AiApi.js reminders.js
├── config/apiConfig.js # getApiBaseUrl() / getApiOrigin() — dev: https://localhost:7223/api
├── context/ # AuthContext, CartContext, LanguageContext (ar/en + dir), ThemeContext,
│ # StorefrontSearchContext
├── routes/ # AdminRoute, ModeratorRoute, CashierRoute, CustomerRoute (role gates)
├── components/
│ ├── layout/ # DashboardLayout, StorefrontLayout, Navbar, Sidebar
│ ├── auth/ # ProtectedRoute, RoleGuard, Register/Verify/Reset pages
│ ├── ui/ # Card, StatCard, EmptyState, Pagination, ReminderBanner (design-system atoms)
│ ├── storefront/ operations/ Purchasing/ profile/ ai/
│ │ # operations/ProductUnitConversionModal.jsx — dual-unit (piece/box/carton)
│ │ # conversion-rate editor (FromUnit/ToUnit/Factor/IsBaseUnit), used from the product form
├── pages/<Area>/ # Shop, Pos, Catalog, Products, Inventory, Customers, Purchasing,
│ # Reports, Operations, Owner, Moderator, admin, Dashboard, Profile, Login
│ ├── admin/Inventory/ # WarehouseHierarchyView.jsx, BatchesManagementPage.jsx — Sheet 1
├── hooks/ # useAutoSync, useOnlineStatus, useOperationsHub
├── services/syncService.js # offline → /api/orders/sync batch push (via shared axiosInstance — see §5.3)
├── db/db.js # Dexie 'IbnAlZumarDB': transactions(++id, syncStatus, createdAt, clientUuid,
│ # syncedAt), products(id, name) — see §5.3 for the syncStatus state machine
├── validators/ # Zod schemas (productSchema, customerSchema…) + XSS sanitizer
├── utils/ # printInvoice.js + print/ A4/A5/thermal renderers, roles.js, auth.js, secureStorage.js
│ # (crypto-js encrypted auth), mediaUrl.js, imageHelper.js, catalog.js,
│ # audioToWav.js, serviceWorker.js
├── constants/maintenance.js
└── pages/{Maintenance,Debt}/ # workflow UI; DebtDashboardPage currently not routed


**Frontend layering rule:** `pages` compose `components`; **only `src/api/*` performs network I/O**;
domain/format helpers live in `utils`; cross-cutting state lives in `context`. A component must never
import `axios` directly.

---

## 3. Domain Entities & Relations

All persisted entities (except `ShippingZone`, `Reminder`, `AiAuditLog`, and the join tables) inherit:

```csharp
public abstract class BaseEntity          // IbnAlZumar.Domain.Common
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;   // global query filter → soft delete only
}
```

Conventions: `decimal` is globally `decimal(18,2)`; soft delete is enforced by a global query filter —
**never hard-delete**; money is EGP; all timestamps are **UTC**.

### 3.1 Catalog `[IMPL]`

| Entity | Key fields | Relations |
|---|---|---|
| `Category` | `Name`, `NameAr`, `Slug`, `Description`, `ParentCategoryId` | self-referencing tree; `1—* Product` |
| `Brand` | `Name`, `LogoUrl` | `1—* Product` |
| `Product` | `SKU`, `Barcode`, `Name`, `NameAr`, `Description`, `SellingPrice`, `CurrentCostPrice`, `QuantityPerCarton`, `MinStockThreshold` (default 5), `IsActive`, `TrackInventory`, `IsAutoTranslated`, `ImageUrl` | `*—1 Category`, `*—1 Brand`, `1—* ProductImage`, `1—* ProductAttributeValue`, `1—* ProductStock`, `1—* ProductVariant`, `1—* ProductBatch`, `1—* UnitConversion`, `1—* ProductPrice`, `1—* OrderItem`, `1—* PurchaseOrderItem` |
| `ProductVariant` | `SKU`, `Price`, `StockQuantity`, `Color`, `Finish`, `Material`, `Size`, `IsActive` | `*—1 Product`, `1—* ProductVariantAttributeValue`, `1—* ProductPrice` |
| `ProductVariantAttributeValue` | `Value` (string) — structured, arbitrary variant attribute (e.g. "Voltage" → "220V") drawn from the shared `ProductAttributeDefinition` catalog | `*—1 ProductVariant`, `*—1 ProductAttributeDefinition` |
| `ProductImage` | `ImageUrl`, `IsPrimary`, `DisplayOrder` | `*—1 Product` |
| `ProductAttributeDefinition` | `Name`, `Unit`, `DataType` (`AttributeDataType`) | `1—* ProductAttributeValue`, `1—* ProductVariantAttributeValue` |
| `ProductAttributeValue` | `Value` (string, parsed per `DataType`) | `*—1 Product`, `*—1 ProductAttributeDefinition` |
| `ProductPrice` `[IMPL]` | `ProductId`, `ProductVariantId?`, `Tier`, `Price`, `MinQuantity`, `IsActive` | Tier price break; variant-specific row outranks product row; highest qualifying `MinQuantity` wins. `PricingTierType` currently includes Retail, Wholesale, FirstWholesale, **Distributor**. |

> `ProductVariant.StockQuantity` is **denormalized** and independent of `ProductStock`. Variant-level
> warehouse stock is `[PLANNED]`. **Sheet 1** added `ProductVariant.Size` and the
> `ProductVariantAttributeValue` join table so a variant can carry arbitrary typed attributes beyond
> the fixed `Color`/`Finish`/`Material`/`Size` columns, reusing the same `ProductAttributeDefinition`
> catalog as product-level attributes.

### 3.2 Inventory / Warehousing

| Entity | Status | Notes |
|---|---|---|
| `Warehouse` | `[IMPL]` | `Name`, `Address`, `IsMainWarehouse`, `IsActive`, `Tier` (`WarehouseTier`), `ParentWarehouseId` (self-referencing FK). Seeded row **Id = 1 ("Main Warehouse")** — `Tier = MainCentral`, `ParentWarehouseId = null` — always safe to reference as the default. |
| `ProductStock` | `[IMPL]` | One row per `(ProductId, WarehouseId)` (unique composite index): `QuantityOnHand`, `ReorderLevel`, `LastRestockedAt`. |
| `InventoryTransaction` | `[IMPL]` | **Append-only ledger.** `TransactionType`, signed `QuantityChange`, loose polymorphic `ReferenceType`/`ReferenceId` ("Order", "PurchaseOrder", "StockTransfer"), optional `ProductBatchId` (which batch the movement was drawn from/into), `TransactionDate`, `Notes`. **Invariant: never mutate `ProductStock.QuantityOnHand` without writing a matching transaction row in the same EF transaction.** |
| `StockTransfer` / `StockTransferItem` | `[IMPL]` | Source/destination warehouse, `StockTransferStatus`, items. Transfers are validated against the 3-tier hierarchy (see below) — rejected unless source/destination are direct parent↔child, or both `MainCentral`. |
| **3-tier warehouse hierarchy** | `[IMPL]` | `Warehouse.Tier` enum `WarehouseTier { MainCentral = 1, RegionalBranch = 2, PosShelfLocation = 3 }` + self-referencing `ParentWarehouseId`, so stock flows `MainCentral → RegionalBranch → PosShelfLocation` via `StockTransfer`. Implemented as columns on the existing `Warehouse` (no new entity). `InventoryService.TransferStockAsync` calls `ValidateWarehouseHierarchy` before moving stock — direct parent↔child only (either direction, to allow returns), or `MainCentral ↔ MainCentral`. |
| `ProductBatch` | `[IMPL]` | `ProductId`, `BatchNumber`, `WarehouseId`, `SupplierId?`, `ProductionDate?`, `ExpiryDate`, `InitialQuantity`, `RemainingQuantity`, `CostPrice`. Consumed FEFO (first-expired-first-out) via `InventoryService.ConsumeFefoAsync` (oldest `ExpiryDate` first, ties broken by `CreatedAt`); `AdjustStockAsync` auto-routes negative adjustments through FEFO whenever batches exist for the product/warehouse. `InventoryTransaction.ProductBatchId` links each movement back to its batch. `InventoryService.GetExpiringBatchesAsync(withinDays, warehouseId)` powers the expiry-warning query; no expiry digest job exists in the current runtime (§5.2). |
| `UnitConversion` | `[IMPL]` | `ProductId`, `FromUnit`, `ToUnit`, `Factor` (decimal 18,4), `IsBaseUnit`. Sits alongside the flat `Product.QuantityPerCarton` (kept for backward compatibility with existing screens/reports) to model piece/box/carton selling. All stock (`ProductStock`, `ProductBatch`) is stored in the **base unit**; conversion happens at the POS/DTO boundary only. |

**Order checkout ledger behavior** `[IMPL]`: `OrderService.CreateAsync` decrements aggregate
`ProductStock.QuantityOnHand` for tracked items and appends `SaleDeducted` transactions in the order
transaction. **Current gap:** this path does not call `ConsumeFefoAsync`, does not decrement
`ProductBatch.RemainingQuantity`, and leaves `InventoryTransaction.ProductBatchId` null. FEFO is
implemented for the inventory service consume/negative-adjustment paths, but not yet for order checkout
or maintenance part usage; batch traceability for those movements is therefore incomplete.

### 3.3 Sales & POS

| Entity / capability | Status | Notes |
|---|---|---|
| `Customer` | `[IMPL]` | Includes `CreditLimit`, `CurrentBalance` (positive means customer owes the store), and `DefaultPricingTier` (default Retail). |
| `Order` / `OrderItem` | `[IMPL]` | Shared online + in-store order model. `Order.PricingTier` snapshots the selected tier. `OrderItem.UnitPrice` and `UnitCostPrice`, discounts, VAT, totals and line totals are captured at order time. |
| `Payment` / `CustomerLedgerEntry` | `[IMPL]` | Payment may settle an order or be a standalone debt collection. Customer debt ledger remains the source of truth for debt movements. |
| `Invoice` | `[PARTIAL]` | Entity and migration exist. One invoice source is either `OrderId` or `MaintenanceRequestId`; fields include `InvoiceNumber`, `CustomerId`, issue metadata, subtotal/tax/total, format, PDF path and share timestamps. Generation/PDF/share services and controller are present, but the actual startup project does not register their dependencies; API calls therefore are not operational until DI is wired. No fiscal/tax-authority certification is represented. |
| Tiered prices | `[IMPL]` | `ProductPrice` plus `PricingTierType { Retail=1, Wholesale=2, FirstWholesale=3, Distributor=4 }`. Resolve among active rows for tier and quantity in base units: variant-specific row first, then highest eligible `MinQuantity`; fallback to `ProductVariant.Price`, then `Product.SellingPrice`. `Customer.DefaultPricingTier` exists, but `OrderService` currently uses the DTO-selected tier or Retail fallback rather than resolving that customer default. |
| POS catalog / price preview | `[IMPL]` | Authenticated endpoints search POS products and return a server-resolved unit-price preview. Preview is convenience only; checkout re-resolves the price on the server. POS UI has tier choice and unit picker. |
| `CustomerDebtSchedule` | `[PARTIAL]` | Reminder cadence/snapshot only; **not** an installment plan. Entity/service/dashboard API code exists, but service/notification DI is missing and the frontend debt page is not mounted in `App.jsx`. |
| `POSShift` | `[PLANNED]` | No entity or shift lifecycle is present. |

**Checkout price integrity** `[IMPL]`: `OrderService.CreateAsync` uses `IPricingService.ResolveUnitPriceAsync` server-side; the client price/total is not authoritative. Tier is persisted on `Order`, and historical order item prices remain frozen. Discounts and 14% VAT are calculated server-side. Order number comes from SQL Server sequence `dbo.OrderNumberSeq`. The checkout stock-ledger and FEFO gap is documented in §3.2.

**Order creation authorization:** `POST /api/orders` and `POST /api/payments/checkout` require authentication. A guest-shaped walk-in order still requires an authenticated cashier/user; there is no anonymous checkout.

#### Enums (`Domain/Enums/Enums.cs`) `[IMPL]`

Persisted integer enum values must only be appended, never renumbered. Current relevant members include:

```csharp
OrderSource      { Online = 1, InStore = 2 }
OrderStatus      { PendingConfirmation=1, Confirmed=2, Processing=3, ReadyForPickup=4,
                   OutForDelivery=5, Delivered=6, Completed=7, Cancelled=8, Returned=9,
                   Shipped=10, CancellationRequested=11 }
PaymentMethod    { CashOnDelivery=1, Cash=2, CreditCard=3, InstaPay=4, Fawry=5,
                   CustomerCredit=6, Wallet=7 }
PaymentStatus    { Pending=1, Paid=2, Failed=3, CodPending=4 }
DiscountType     { None=0, Percentage=1, FixedAmount=2 }
LedgerTransactionType { SaleOnCredit=1, PaymentReceived=2, ManualAdjustment=3 }
PricingTierType  { Retail=1, Wholesale=2, FirstWholesale=3, Distributor=4 }
NotificationChannel { WhatsApp=1, Email=2, Sms=3 }
NotificationStatus { Pending=1, Sent=2, Failed=3 }
InventoryTransactionType: PurchaseReceived/PurchaseReceive/Purchase=1 aliases;
    SaleDeducted/SalesDeduct/Sale=2 aliases; TransferOut=3; TransferIn=4;
    AdjustmentIncrease/Adjustment=5 aliases; AdjustmentDecrease=6;
    CustomerReturn/Return/BatchReceived=7 aliases; SupplierReturn/BatchConsumed=8 aliases;
    MaintenanceUsed=9.
MaintenanceStatus { Pending=1, Priced=2, Approved=3, Rejected=4, Completed=5,
                    InDiagnostics=6, AwaitingParts=7, InRepair=8, Delivered=9, Cancelled=10 }
WarehouseTier { MainCentral=1, RegionalBranch=2, PosShelfLocation=3 }
```

Other existing enums remain in `Enums.cs` and the attendance entity. `ApplePay` is not defined.

### 3.4 Maintenance

`MaintenanceRequest` remains the existing entity (namespace `IbnAlZumar.Domain.Entities.Maintenance`); it has not been renamed to `MaintenanceTicket`. The online inquiry/pricing flow remains separate from the additive in-store repair workflow.

| Capability/entity | Status | Notes |
|---|---|---|
| `MaintenanceRequest` | `[IMPL]` | Customer/user, problem/images, delivery method/status, estimate/schedule, admin notes/report URL. |
| Workflow extensions | `[PARTIAL]` | `AssignedTechnicianUserId`, `LaborCost`, `ActualCost`, `DeliveredAt`; child `MaintenanceNote` and `MaintenancePartUsage`. Controller/service and frontend workflow views exist. Service enforces transition rules and writes status notes; actual cost is calculated/frozen when marked Completed. **Current defect:** `AllowedTransitions` dictionary initializer repeats keys for AwaitingParts, InRepair and Completed; type initialization can fail when the workflow service is first used. Resolve before classifying this workflow as operational. |
| Part usage | `[PARTIAL]` | Tracked products reduce `ProductStock` and create a compensating/append-only inventory transaction in a DB transaction. It does not use FEFO or alter batch balances. Removing a usage compensates stock with another movement, but the usage entity is removed through `DbSet.Remove`; see soft-delete finding in §10.1. |
| Inspection fee / `MaintenanceTicket` rename | `[PLANNED]` | No 50 EGP fee logic, payment-at-intake rule, fee credit, or entity rename is present. Build only to the precise planned requirements in the prior design; never hard-code the fee in multiple places. |

Workflow state values currently include Pending, Priced, Approved, Rejected, Completed, InDiagnostics, AwaitingParts, InRepair, Delivered, Cancelled. The service transition graph is in `MaintenanceWorkflowService.AllowedTransitions`; do not infer additional transitions from the enum alone.

### 3.5 Identity & RBAC `[IMPL]`

`User` (`FullName`, `Username`, `Email`, `PasswordHash`, `IsActive`, `LastLoginAt`, email/phone OTP
verification fields, `PendingEmail*`, `PendingPhone*`, `HourlyRate`, `VoiceEmbedding` JSON,
`VoiceEnrolledAtUtc`, `VoiceEnrolledByUserId`) — `*—* Role` via `UserRole`, `*—* Permission` via
`UserPermission` (with `IsGranted` for per-user overrides).
`Role` `*—* Permission` via `RolePermission`.
`Permission { Code, Name, Module, Description }` — codes are the constants in
`Persistence/Seed/DataSeeder.PermissionCodes`:

Products.View|Create|Edit|Delete · Categories.Manage
Inventory.View|Adjust|Transfer|ManageBatches
Purchasing.View|Create|Approve
Orders.View|Create|Edit|Cancel
Customers.View|Manage|ManageDebt
Users.Manage · Roles.Manage · Permissions.Manage · Reports.View


Frontend role names (`src/utils/roles.js`, with alias normalization):
`SUPER ADMIN > STORE_OWNER > ADMIN > ONLINE_MANAGER > MODERATOR > CASHIER > CUSTOMER`.

### 3.6 Purchasing & Supplier accounting `[IMPL]`

`Supplier` (`Name`, `ContactPerson`, `Phone`, `Email`, `Address`, `TaxId`, `CurrentBalance` = payable)
→ `PurchaseOrder` (`PurchaseOrderNumber`, `SupplierId`, `WarehouseId`, `Status`, `OrderDate`,
`ExpectedDeliveryDate`, `ReceivedDate`, `TotalCost`) → `PurchaseOrderItem` (`QuantityOrdered`,
`QuantityReceived`, `UnitCostPrice`, `LineTotal` — historical cost source for margin reporting).
`SupplierPayment` (`Amount`, `SupplierPaymentMethod`, optional `PurchaseOrderId`, `CreatedByUserId`) and
`SupplierLedgerEntry` (signed `Amount`, `RunningBalance`, related PO/payment) form the statement of
account. **Receiving a PO (`Status → Received`) must, in one transaction:** increase `ProductStock`,
write `InventoryTransaction` rows, update `Product.CurrentCostPrice`, and write a `PurchaseInvoice`
ledger entry.

**Opening Balance Supplier (مورد أول المدة)** `[IMPL]` — a reserved, fixed-Id `Supplier` row
(`Id = 999999`, exposed as `DataSeeder.OpeningBalanceSupplierId`), seeded idempotently by
`DataSeeder.SeedOpeningBalanceSupplierAsync` (inserted under `SET IDENTITY_INSERT`, wrapped in its own
transaction, so the Id stays stable across environments). Lets legacy/historical inventory be
registered — via `InventoryService.ReceiveBatchAsync` or a plain positive `AdjustStockAsync` — without
forcing a formal `PurchaseOrder` chain. `TaxId = "OPENING-BALANCE"` currently doubles as a lookup code
until a dedicated `Supplier.Code` column exists. **Never reuse Id 999999 for a real supplier.**

### 3.7 Finance & HR

| Entity | Status | Notes |
|---|---|---|
| `AttendanceLog` | `[IMPL]` | `UserId`, `CheckInTime`, `CheckOutTime?`, `AttendanceStatus`, `VerificationMethod` (**Voice** biometric or AdminManual), `WorkedMinutes`, `WorkedHours`, `Notes`. Index `(UserId, CheckInTime)`. |
| `PayrollRecord` | `[IMPL]` | `UserId`, `PeriodStart`, `PeriodEnd`, `TotalHours`, `TotalSalary` (= hours × `User.HourlyRate`), `IsPaid`, `PaymentDate`. |
| **QR attendance** | `[PLANNED]` | Current check-in is **voice-embedding based** (Hugging Face ECAPA, §5.1), *not* QR. Target: add `AttendanceVerificationMethod.Qr = 3`, a rotating signed QR token per branch (`WarehouseId` + HMAC + 30 s TTL) validated server-side, and `AttendanceLog.WarehouseId`. Keep voice as a fallback — do not remove it. |
| `CustomerDebtSchedule` | `[PARTIAL]` | Recurring debt-reminder cadence/snapshot only. It does not implement `Installment` or `InstallmentSchedule`; see §3.3 and §5.2. |
| `Expense` | `[PARTIAL]` | `ExpensesController` exists and correctly returns **`501 Not Implemented`** on both `POST` and `GET` rather than a fake success (see §12) — but there is still no `Expense` entity or DbSet. Implement `Expense { Amount, Category, Notes, IncurredAt, CreatedByUserId, POSShiftId? }` and wire it up before removing the 501 stub. |
| `PaymentVoucher` | `[PLANNED]` | Outgoing cash document: `VoucherNumber`, `Type { Receipt = 1, Payment = 2 }`, `Amount`, `PartyType { Customer, Supplier, Employee, Other }`, `PartyId`, `Method`, `IssuedAt`, `IssuedByUserId`, `Notes`, `RelatedChequeId?`. Prints on A5. |
| `Cheque` | `[PLANNED]` | `ChequeNumber`, `BankName`, `Amount`, `IssueDate`, `DueDate`, `Direction { Incoming, Outgoing }`, `Status { Pending, Deposited, Cleared, Bounced, Cancelled }`, `PartyType`/`PartyId`, `RelatedSupplierPaymentId?`. `SupplierPaymentMethod.Cheque` already exists and must link here. Feeds the Cheque Due Alert job. |
| `Installment` | `[PLANNED]` | `CustomerId`, `OrderId`, `PlanTotal`, `DownPayment`, `InstallmentCount`, then `InstallmentSchedule { InstallmentId, SequenceNo, DueDate, Amount, PaidAmount, PaidAt?, Status }`. Each payment writes a `Payment` + `CustomerLedgerEntry` — the ledger stays the single source of debt truth. |
| `CommissionConfig` | `[PLANNED]` | `Scope { Global, Role, User, Category, Product }`, `ScopeId?`, `CalculationType { PercentOfSale, PercentOfProfit, FixedPerUnit }`, `Rate`, `EffectiveFrom`, `EffectiveTo?`. Resolution order: Product → Category → User → Role → Global. |
| `EmployeeTarget` | `[PLANNED]` | `UserId`, `PeriodStart`, `PeriodEnd`, `TargetType { SalesAmount, Profit, UnitsSold }`, `TargetValue`, `AchievedValue` (recomputed by a Quartz job), `BonusRate`. |
| `EmployeeAdvance` | `[PLANNED]` | `UserId`, `Amount`, `RequestedAt`, `ApprovedByUserId?`, `Status { Pending, Approved, Rejected, Settled }`, `DeductionPlan` (installments against `PayrollRecord`), `SettledAt?`. |
| `LeaveRequest` | `[PLANNED]` | `UserId`, `LeaveType { Annual, Sick, Unpaid, Emergency }`, `FromDate`, `ToDate`, `DaysCount`, `Reason`, `Status { Pending, Approved, Rejected }`, `DecidedByUserId?`, `DecidedAt?`. Approved unpaid leave must reduce `PayrollRecord.TotalHours`. |

### 3.8 Cross-cutting entities `[IMPL]`

* `Reminder` — Quran ayah / dhikr shown in the global `ReminderBanner`; seeded from `Reminders.csv`.
* `AiAuditLog` — every AI assistant call: `UserId`, `UserEmail`, `Roles`, `Action`, `Prompt`,
  `ToolName`, `Succeeded`, `Error`, `MetadataJson`, `IpAddress`, `TimestampUtc` (indexed). `long Id`,
  does not inherit `BaseEntity`.
* `NotificationLog` `[IMPL schema]` — channel, recipient, template/payload, status, provider id/error,
  sent time, retry count and loose related entity reference. Notification sending code exists, but is
  not wired in the actual composition root (see §10.1).

### 3.9 Relationship map (text ERD)

Category ─┬─< Category (self) Supplier ─< PurchaseOrder ─< PurchaseOrderItem >─ Product
└─< Product >─ Brand Supplier ─< SupplierPayment ─< SupplierLedgerEntry
Product ─< ProductVariant ─< ProductPrice ; Product ─< ProductPrice (product-level)
Product ─< ProductImage ; Product ─< ProductAttributeValue >─ ProductAttributeDefinition
Product ─< UnitConversion ; Product ─< ProductBatch ; Product ─< ProductStock >─ Warehouse
Warehouse ─< InventoryTransaction ; Warehouse ─< StockTransfer(Source/Destination) ─< StockTransferItem >─ Product
Customer ─< Order ─< OrderItem >─ Product ; Order ─< Payment ; Order ─ Invoice
Customer ─< Payment ; Customer ─< CustomerLedgerEntry >─ Order/Payment
Customer ─< CustomerDebtSchedule ; Customer ─< MaintenanceRequest ─< MaintenanceNote
MaintenanceRequest ─< MaintenancePartUsage ; MaintenanceRequest ─ Invoice
User ─< AttendanceLog ; User ─< PayrollRecord ; User >─< Role >─< Permission ; User >─< Permission (override)
NotificationLog links loosely to related entity via RelatedEntityType/RelatedEntityId.


---

## 4. Coding & Design Standards

### 4.1 C# naming & style `[IMPL convention]`

* `PascalCase`: classes, records, methods, properties, enums & members, constants.
  `camelCase`: locals & parameters. `_camelCase`: private readonly fields.
* Interfaces prefixed `I` and placed **beside** the implementation (`Services/Sales/IOrderService.cs`).
* Namespaces are **file-scoped** and mirror folders, rooted at `IbnAlZumar.*`.
  ⚠ The codebase currently mixes `IbnAlZumar.API.*` and `IbnAlZumar.Api.*` casing. **New code must use
  `IbnAlZumar.API.*`**; do not mass-rename existing namespaces in a feature PR.
* Nullable reference types are on: use `?`, `= string.Empty`, and `= null!` for required navigations.
* All I/O is `async`/`await` with the `Async` suffix and a `CancellationToken` where practical.
* Arabic inline comments are accepted and common — keep them, and **keep identifiers English-only**.
* One enum file (`Domain/Enums/Enums.cs`) for domain enums; entity-local enums may live with the entity
  (e.g. `AttendanceStatus`).

### 4.2 Entity rules

* Inherit `BaseEntity`; never expose a public setter that bypasses the ledger invariants.
* Annotate with `[Required]`, `[MaxLength]`, `[Column(TypeName = "decimal(18,2)")]` where the global
  convention is not enough; anything more complex belongs in an `IEntityTypeConfiguration<T>` under
  `Persistence/Configurations/<Module>/` (auto-discovered by `ApplyConfigurationsFromAssembly`).
* Collections initialize to `new List<T>()`.
* Deletes are **soft** (`IsDeleted = true`); use `IgnoreQueryFilters()` deliberately and rarely.
* Money: `decimal` only — never `double`/`float`.

### 4.3 DTO rules

* Location `DTOs/<Module>/`, namespace `IbnAlZumar.API.DTOs.<Module>`.
* Naming: `Create<X>Dto`, `Update<X>Dto`, `<X>ResponseDto` (or `<X>Dto` for read models),
  `<X>FilterDto`, `PagedResultDto<T>`, `BulkImportResultDto`.
* **Entities never cross the HTTP boundary** — controllers accept and return DTOs only.
* Paged endpoints return `PagedResultDto<T>`; filters bind from query via `<X>FilterDto`.
* Errors always use `ApiErrorResponse { StatusCode, Message, TraceId, Errors, TimestampUtc }`
  (camelCase JSON) produced by `ExceptionHandlingMiddleware`.
* File uploads use `[FromForm]` + `IFormFile` with `[Consumes("multipart/form-data")]`.

### 4.4 Validation

* **Today `[PARTIAL]`:** DataAnnotations on DTOs + explicit guard clauses throwing
  `BadRequestException` / `NotFoundException` in services.
* **Target `[PLANNED]` FluentValidation pattern** (the packages are already referenced):
  one validator per write DTO, `Validators/<Module>/<Dto>Validator.cs`,
  `public sealed class CreateProductDtoValidator : AbstractValidator<CreateProductDto>`, registered by
  `builder.Services.AddValidatorsFromAssemblyContaining<Program>()`, messages **bilingual (Arabic
  primary)**, and cross-entity/uniqueness checks (e.g. duplicate SKU) stay in the **service**, not the
  validator.

### 4.5 Service / repository / CQRS rules

* **There is no repository layer and no MediatR/CQRS today.** Services depend directly on
  `ApplicationDbContext`. **Do not introduce a generic `IRepository<T>`** — EF `DbSet` already is one.
* Every module exposes `I<Module>Service` + `<Module>Service`, registered `AddScoped` in `Program.cs`.
* A service method: validate → load with explicit `Include`s → mutate → persist all related writes in a
  **single `SaveChangesAsync`** (use an explicit transaction when several aggregates move, e.g. order
  checkout: stock + ledger + payment — `OrderService.CreateAsync` is the reference implementation of
  this pattern, see §3.3).
* Read queries are `AsNoTracking()` and project to DTOs with `Select` (avoid loading full graphs).
* Throw `NotFoundException` / `BadRequestException` instead of returning `null` or `IActionResult` from
  services.
* **Controllers stay thin**: `[ApiController]`, `[Route("api/[controller]")]`,
  `[Authorize(Policy = PermissionCodes.X)]`, constructor-inject the service (not the DbContext),
  return `Ok(dto)` / `CreatedAtAction` / `NoContent`.
* If CQRS is ever adopted, do it **per module** behind the existing `I<Module>Service` interfaces —
  never a big-bang refactor.

### 4.6 React component rules

* Function components + hooks only; one component per file, `PascalCase.jsx`, default export.
* Hooks in `src/hooks` named `use*`; shared state via context providers in `src/context`.
* **All network calls go through `src/api/*` modules using the shared `axiosInstance`.** Never call
  `fetch`/`axios` from a component **or from a service module** — `src/services/syncService.js` was
  previously an exception (bare `axios.create({ baseURL: '/api' })`) and has been fixed to use
  `axiosInstance` too; see §5.3. Errors arrive pre-normalized as
  `{ statusCode, message, errors, traceId }`.
* Every list screen handles the four states explicitly: loading, error, empty (`<EmptyState />`), data.
* Reuse `src/components/ui` atoms (`Card`, `StatCard`, `EmptyState`, `Pagination`) before writing new
  markup.
* Validate user input with the Zod schemas in `src/validators` (they also sanitize XSS) before POSTing.
* Auth tokens are read/written **only** through `utils/secureStorage.js` (crypto-js encrypted) and
  `utils/auth.js` — never touch `localStorage` directly.
* Role gating: wrap routes in `AdminRoute` / `ModeratorRoute` / `CashierRoute` / `CustomerRoute` or
  `<ProtectedRoute allowRoles={[...]}>`; normalize role strings with `normalizeRole()`.
* New routes are declared in `src/App.jsx` inside the correct group; the router basename is
  `import.meta.env.BASE_URL` (`/IbnAlZumar-Frontend/`) — **always use relative router paths**, never
  hard-coded absolute URLs.

### 4.7 Tailwind & RTL styling rules

* Use the theme tokens from `tailwind.config.js`, not raw hex:
  `canvas #F4F5F7` (app bg), `surface #FFFFFF`, `border #E2E4E9`, `ink` / `ink-soft`,
  `graphite-950/900/800/700` (sidebar & dark surfaces), **`amber #F2A900` (signature accent)**,
  `amber-dark #C98900`, `success #1D9A6C`, `danger #D64545`, `info #2F6FED`, shadow `subtle`.
* Fonts: `font-display` (Space Grotesk), `font-body` (IBM Plex Sans / Cairo), `font-arabic` (Cairo),
  `font-mono` (JetBrains Mono).
* **RTL first.** `LanguageContext` sets `<html lang dir>`. Use logical utilities (`ms-*`, `me-*`,
  `ps-*`, `pe-*`, `text-start`, `text-end`) instead of `ml-*`/`mr-*`/`text-left`/`text-right`.
* Utility-first inline classes; no CSS modules or styled-components. Long class lists may be extracted
  to a local `const classes = {...}` map in the same file.
* Arabic numerals/dates: format with `toLocaleDateString('ar-EG', …)` for display; **send ISO UTC** to
  the API.

### 4.8 Print engine rules

**Frontend renderers `[IMPL]`:** `src/utils/print/` has shared print-window utilities and separate renderers for A4, A5, thermal 80 mm and thermal 58 mm. `printInvoice.js` remains for backward compatibility. POS UI offers the print-format selector; a maintenance receipt renderer is present. Keep HTML interpolations escaped and preserve Arabic RTL presentation.

**Server-generated PDFs `[PARTIAL]`:** Backend `InvoicePdfService` (QuestPDF) can render order/maintenance PDF content and `InvoiceService` stores files and creates signed short-lived download links. However, invoice/notification dependencies are not registered in the actual `Program.cs`, so controller endpoints are not usable yet. `ShareInvoiceButton.jsx` exists but is not mounted by current UI call sites.

Existing print format values in frontend include `thermal80`, `thermal58`, `a4`, `a5`. Do not describe the backend PDF path as live until its configuration/DI and public file URL are validated.

### 4.9 Git & PR conventions

* Branches: `feature/<slug>`, `fix/<slug>`, `chore/<slug>`. Small, focused PRs.
* Never commit secrets (see §9.3), `bin/`, `obj/`, `.vs/`, `node_modules/`, or `dist/`.
* Every schema change ships with an EF migration: `dotnet ef migrations add <Name>` from
  `Ibn al-Zumar.API/Ibn al-Zumar.API/`. Startup attempts migrations through `app.SeedDatabaseAsync()`,
  but errors are caught/logged and startup continues; verify deployed migration state explicitly (§9.2).

---

## 5. System Integrations & Background Services

### 5.1 External integrations that exist today `[IMPL]`

| Integration | Where | Config section | Notes |
|---|---|---|---|
| **Paymob** (card / wallet / InstaPay / Fawry) | `Services/Payments/PaymobService.cs`, `PaymobOptions`, `PaymentsController` (`/api/payments`) | `Paymob` (`BaseUrl`, `ApiKey`, `IframeId`, `CardIntegrationId`, `WalletIntegrationId`, `InstaPayIntegrationId`, `HmacSecret`) | Typed `HttpClient`, 30 s timeout. Callback **must** be HMAC-verified; `Order.PaymobOrderId` / `PaymobTransactionId` link back. `POST /api/payments/checkout` requires `[Authorize]` (see §3.3); the webhook callback intentionally stays unauthenticated (server-to-server, integrity via HMAC). |
| **Google Gemini AI assistant** | `Services/Ai/*`, `AiController` (`/api/ai`) | `Gemini` (`ApiKey`, `Model` = `gemini-1.5-flash`, `BaseUrl`, `MaxToolCallIterations` = 5) | Tool-calling agent. Tools implement `IAiTool` and are registered as singletons in `AiToolRegistry`: `GetPendingOrders`, `GetOrderDetails`, `GetLowStockProducts`, `GetSalesSummary`, `UpdateProductPrice`, `GetCategories`, `CreateCategory`, `CreateProduct`, `BulkImportProducts`, `GenerateProductsExcel`. Every call is written to `AiAuditLog`; tool access is role-checked via `AiRoles`. **New AI capability = new `IAiTool` class + DI registration — never widen an existing tool.** |
| **Hugging Face voice biometrics** | `Services/Attendance/VoiceVerificationService.cs`, `Services/Ai/VoiceCommandService.cs` | `HuggingFace` (`ApiKey`, `VoiceModelUrl` = `speechbrain/spkrec-ecapa-voxceleb`) | Enrollment stores a JSON embedding in `User.VoiceEmbedding`; check-in compares cosine similarity. Frontend records via `utils/audioToWav.js`. |
| **Brevo email** (MailKit) | `Services/Email/EmailService.cs` | `Brevo:ApiKey` | OTP: email verification, password reset, email/phone change. |
| **Google OAuth login** | `AuthController`, `Google.Apis.Auth`; frontend `@react-oauth/google` | `VITE_GOOGLE_CLIENT_ID` | |
| **Translation provider** | `Services/Catalog/TranslationService.cs`; `POST /api/v1/translation/translate`; create-time helper and product/category re-translate services | `Translation` | Typed HttpClient is registered. Frontend translation API has an external MyMemory fallback. Translation is best-effort. |
| **WhatsApp Cloud API (code present, inactive)** | `Services/Notifications/WhatsApp/WhatsAppCloudClient.cs`, `NotificationSender` | `WhatsApp` | Transport/composer/logging classes exist, but actual `Program.cs` does not bind/register them; do not claim sends are live. |
| **Azure SQL** | `ConnectionStrings:DefaultConnection`, or env `SQLAZURECONNSTR_DefaultConnection` / `DATABASE_URL` | | |

### 5.2 Background services and integrations — current state

Quartz 3.8 packages are referenced and `Infrastructure/Jobs/DebtReminderJob.cs` exists. `CustomerDebtService` contains schedule reconciliation and reminder-send logic, and `NotificationLog` plus WhatsApp/email transport/composer classes exist. **But** the actual compiled startup `Program.cs` contains no `AddQuartz`, job trigger/schedule, or DI registrations for `ICustomerDebtService`, `IInvoiceService`, `IInvoicePdfService`, `INotificationSender`, `INotificationComposer`, `IWhatsAppClient`, `InvoiceLinkSigner`, or their options. As a result:

- The debt-reminder job is not scheduled or running.
- `CustomerDebtController` and `InvoicesController` are discoverable but their service graphs cannot be resolved at request time.
- Notification transport and invoice-sharing code are present but inactive.
- No expiry, cheque, low-stock, employee-target, or other scheduled job is implemented in the actual runtime.

The debt reminder implementation differs from the original planned idle-triggered behavior: its `CustomerDebtSchedule` is a recurring cadence (configurable `IntervalDays`, default described in `DebtReminderOptions`) while balance truth remains `Customer.CurrentBalance`/`CustomerLedgerEntry`. `ReconcileSchedulesAsync` and `SendDueRemindersAsync` exist, but they need a registered scheduler/job before running. Manual `POST /api/customer-debt/{customerId}/send-reminder` code also depends on the currently missing DI graph.

WhatsApp client code is template-oriented and normalizes Egyptian phone numbers to E.164; notifications are logged before/after transport with retry attempts. Treat it as implementation code, not an active integration, until DI/configuration and provider delivery are verified. The original proposed WhatsApp template set remains aspirational except templates explicitly composed in current code (`debt_reminder`, `invoice_share`).

### 5.3 Offline / PWA sync `[IMPL]`

POS writes go to Dexie (`transactions` store, `syncStatus ∈ { pending, syncing, synced, failed }`) with
a client-generated `clientUuid`; `useAutoSync` + `syncService.syncPendingTransactions()` push them as
one batch to **`POST /api/orders/sync`** (`SyncBatchRequestDto`) through the **shared `axiosInstance`**
(not a bare `axios` client) — this ensures the JWT `Authorization` header and the real, environment-aware
API base URL are always attached. (A previous bug used `axios.create({ baseURL: '/api' })` directly,
which only ever worked behind the local Vite dev proxy and silently sent unauthenticated requests in
any other environment — fixed.) The server de-duplicates on `Order.ClientUuid` (unique filtered index)
and returns per-order `{ clientUuid, success, errorMessage }`.

**Status recovery (`db.js`):** `getPendingTransactions()` returns both `'pending'` rows and any
`'syncing'` rows left stuck by a crashed or closed tab from a previous session — a sync run always
retries both. On a per-order failure, or on the batch request itself failing (network/server error),
every affected row reverts to `'pending'` with `retryCount` incremented; after 5 failed attempts a row
becomes terminally `'failed'` (surfaced to the cashier) instead of being retried silently forever. A
successful sync marks the row `'synced'` with a `syncedAt` timestamp — **synced orders are never
hard-deleted from Dexie**, so local history survives for audit purposes.

**Any new offline-capable write must follow this same idempotency-key + recoverable-status pattern.**
API requests are `NetworkOnly` in the service worker — never cache API responses.

---

## 6. API Surface & Frontend Route Map

### 6.1 Controllers and current API surface

| Route | Controller | Current state / notable authorization |
|---|---|---|
| `/api/auth` | `AuthController` | login, register, Google login, OTP, password reset |
| `/api/users`, `/api/roles` | `UsersController`, `RolesController` | identity/RBAC |
| `/api/products`, `/api/categories`, `/api/catalog` | `ProductsController`, `CategoriesController`, `CatalogController` | catalog, import and export; selected endpoints use permission policies |
| `/api/productpricing` | `ProductPricingController` | `GET /{productId}`, `POST /`, `DELETE /{priceId}`; class-level `Products.Edit` policy |
| `/api/pos` | `PosController` | `GET /products`, `GET /products/{productId}/unit-price`; `[Authorize]` |
| `/api/inventory` | `InventoryController` | stock, batches, hierarchy, transfer; adjustment/transfer and batch writes use permission policies |
| `/api/orders`, `/api/orders/sync` | `OrdersController`, `SyncController` | order lifecycle and offline batch sync |
| `/api/customers` | `CustomersController` | customers |
| `/api/customer-debt` | `CustomerDebtController` | paged debt dashboard + send-reminder; `Customers.ManageDebt`; service graph currently not registered |
| `/api/payments` | `PaymentsController` | checkout requires authentication; Paymob webhook uses HMAC verification |
| `/api/invoices` | `InvoicesController` | `POST /from-order`, `POST /from-maintenance`, `GET /{id}`, `POST /{id}/share` require auth; `GET /{id}/pdf?token=...` is anonymous but must validate signed token. DI currently missing. |
| `/api/purchasing` | `PurchasingController` | suppliers, POs, supplier payments/ledger |
| `/api/maintenance` | `MaintenanceController` | existing maintenance request intake / legacy pricing |
| `/api/maintenance-workflow` | `MaintenanceWorkflowController` | list/detail, assign technician, status, notes, parts, labor cost, technicians, receipt; `Maintenance.View/Manage`; static transition-map defect noted §10.1 |
| `/api/attendance`, `/api/payroll` | `AttendanceController`, `PayrollController` | voice attendance and payroll |
| `/api/expenses` | `ExpensesController` | returns `501 Not Implemented` |
| `/api/reports`, `/api` dashboard | `ReportsController`, `DashboardController` | reports use `Reports.View`; dashboard also includes role checks |
| `/api/shippingzones`, `/api/reminders`, `/api/ai` | respective controllers | shipping/custom-zone, reminders, AI assistant/voice |
| `/api/v1/translation` | `TranslationController` | `POST /translate`; translation client registered |
| `/api/products/{id}/translate`, `/api/categories/{id}/translate` | `ProductTranslationController` | manual translation action, `Products.Edit` policy |

Permission-code authorization is in place on selected controllers/actions, not universally: several older APIs use generic `[Authorize]` or role checks. Do not infer a permission policy for an endpoint unless its controller/action actually declares it. Errors follow `ApiErrorResponse`; Swagger is at `/swagger`.

### 6.2 Frontend routes and feature integration

Routes are declared in `src/App.jsx` and use the router basename from `import.meta.env.BASE_URL`.

- **Storefront:** `/`, `/shop` redirect, `/products/:productId`, `/cart`, `/checkout`, `/payment/success`, `/payment/failed`, `/profile`, `/orders/:orderId`.
- **Auth:** `/login`, `/register`, `/verify-email`, `/forgot-password`, `/reset-password`, `/admin/login`, `/admin/forbidden`.
- **POS:** `/pos`; also `/admin/pos`.
- **Moderator:** `/moderator` and child routes `dashboard`, `operations`, `maintenance-workflow`, `products`, `products/import`, `categories`, `reminders`, `catalog`, `profile` (plus legacy profile redirects).
- **Admin:** `/admin/dashboard`, `/admin/profile`, `/admin/operations`, `/admin/maintenance-workflow`, `/admin/owner`, `/admin/catalog/categories`, `/admin/catalog/products`, `/admin/catalog/products/import`, `/admin/products/import`, `/admin/reminders`, `/admin/inventory/adjust`, `/admin/inventory/transfer`, `/admin/inventory/hierarchy`, `/admin/inventory/batches`, `/admin/customers`, `/admin/customers/:id`, `/admin/purchasing`, `/admin/pos`, `/admin/payroll`, `/admin/reports`.

Current front-end status: warehouse hierarchy/batch pages, unit conversion modal, maintenance workflow UI, product pricing manager (used by product pages), POS price-tier selection/unit picker, and multi-format print utilities exist. `DebtDashboardPage.jsx` and `ShareInvoiceButton.jsx` exist but are **not imported/mounted** by `App.jsx` or another current component; do not call them completed user-facing flows. Frontend route guards mostly use role helpers; API authorization remains server-side.

---

## 7. Cross-Cutting Invariants (do not break)

1. **Soft delete only** — set `IsDeleted`; the global filter hides the row.
2. **Stock never moves without a ledger row** — `ProductStock` change ⇒ matching `InventoryTransaction`
   in the same EF transaction. Order checkout writes this aggregate-stock ledger, but is not batch/FEFO-aware (§3.2).
3. **Debt never moves without a ledger row** — `Customer.CurrentBalance` change ⇒
   `CustomerLedgerEntry` (same for `Supplier` ⇒ `SupplierLedgerEntry`), with `RunningBalance` written.
4. **Historical prices are frozen, and never client-supplied** — `OrderItem.UnitPrice` is resolved
   server-side by pricing tier/quantity/variant through `IPricingService` (fallback to variant/base
   catalog price), never trusted from the request DTO; `PurchaseOrderItem.UnitCostPrice`, discounts,
   and `Order.TaxRate`/`Order.TaxAmount` are captured at transaction time and never recomputed.
5. **Idempotency for offline writes** — `ClientUuid` is the key; retries must be safe.
6. **Enum values are persisted ints** — append new members, never renumber or delete.
7. **UTC in the database**, Africa/Cairo only at the presentation layer.
8. **Permission policies are the target API guard; current coverage is selective.** Inventory write/batch actions, product update/pricing/translation, reports, customer-debt and maintenance-workflow use policies in code. Other controllers still use generic `[Authorize]` or role checks. Add the correct existing permission policy to new protected endpoints; do not claim global enforcement.
9. **Money is `decimal`**, `decimal(18,2)`, EGP.
10. **Warehouse Id = 1 is always a valid default** (seeded Main Warehouse, `Tier = MainCentral`).
11. **Offline POS writes must be recoverable** — a Dexie `transactions` record must never be permanently
    stuck in `'syncing'`; a failed or interrupted sync reverts it to `'pending'` with `retryCount`
    incremented, and a successful sync marks it `'synced'` with a `syncedAt` timestamp rather than
    hard-deleting it (kept for audit history). See §5.3.
12. **Warehouse transfers must respect the 3-tier hierarchy** — `StockTransfer` between two warehouses
    is only valid when one is the direct `ParentWarehouseId` of the other, or both are `MainCentral`.
    Enforced by `InventoryService.ValidateWarehouseHierarchy` before any stock moves. See §3.2.
13. **Batch quantities never move without a matching `InventoryTransaction` row** — every batch receipt or FEFO consumption writes a matching row with `ProductBatchId` in the same transaction. Current order checkout and maintenance part usage do not touch `ProductBatch` or carry `ProductBatchId`; see the traceability gap in §3.2 and §10.1.
14. **Tier price is resolved server-side and frozen per order.** Resolve ProductPrice by selected tier, quantity and variant precedence; store `Order.PricingTier` and `OrderItem.UnitPrice` at sale time. Never reprice historical orders.
15. **Maintenance part stock changes require ledger movement.** Add/remove operations use a DB transaction and a compensating inventory movement; keep the original inventory ledger append-only. Audit rows themselves must not be hard-deleted.

---

## 8. Definition of Done for any change

1. Domain change → entity + `IEntityTypeConfiguration` + **EF migration** + seed update if needed.
2. Service interface + implementation + DI registration in `Program.cs`.
3. DTOs (+ validators) and a thin controller with the right `[Authorize(Policy = …)]`.
4. `dotnet build` clean (no new warnings); backend runs and Swagger lists the endpoint.
5. Frontend: API module function → page/component → route in `App.jsx` → role guard → loading/error/empty
   states → Arabic + English labels → RTL-safe Tailwind classes.
6. `npm run build` passes.
7. No secrets added to source control; no new entity bypassing the invariants in §7.

---

## 9. Environments, Build & Operational Notes

### 9.1 Local development

```bash
# Backend
cd "Ibn al-Zumar.API/Ibn al-Zumar.API"
dotnet restore && dotnet run          # https://localhost:7223 — Swagger at /swagger
dotnet ef migrations add <n>       # schema change
# Frontend
npm ci && npm run dev                 # http://localhost:5173 — proxies /api and /uploads to the API
```

### 9.2 Deployment

* **API:** Docker (`mcr.microsoft.com/dotnet/sdk:9.0` → `aspnet:9.0`, `EXPOSE 8080`, honours `PORT`) to
  Azure App Service (`francecentral`). Startup calls `Database.MigrateAsync()` then seeding, but
  `SeedDatabaseAsync` catches/logs migration or seeding exceptions and continues startup. Therefore
  successful API boot does **not** prove migrations/seed completed; verify database migration state separately.
* **Frontend:** GitHub Actions → GitHub Pages, base path `/IbnAlZumar-Frontend/`,
  `VITE_API_URL` injected at build time.
* **CORS:** the API allows `https://kimo-25.github.io*` and `http://localhost*` with credentials.

### 9.3 ⚠ Secret hygiene (known issue — fix before any public release)

`Ibn al-Zumar.API/Ibn al-Zumar.API/appsettings.json` currently contains **real committed secrets**
(Azure SQL connection string with password, JWT signing key, Brevo, Hugging Face and Gemini API keys).
Any AI tool working in this repo must: **never echo these values**, never add new ones, and prefer
`dotnet user-secrets` / environment variables / Azure App Settings. Rotating these credentials and
stripping them from `appsettings.json` is an outstanding task. `DataSeeder.cs` also contains hard-coded
initial account credentials; do not reproduce them in docs/logs and treat them as compromised until replaced.

**Note:** `Program.cs` now throws `InvalidOperationException` at startup if `Jwt:Key` is missing or
shorter than 256 bits (32 bytes) — there is no hardcoded fallback key anymore. Whatever key replaces the
compromised one in `appsettings.json`/environment config must satisfy that length requirement or the
app will refuse to boot (by design).

---

## 10. Implementation Status Matrix (verified against repository heads above)

| Area | Status |
|---|---|
| Catalog, variants, attributes, images, bulk Excel import | `[IMPL]` |
| 3-tier warehouses, stock ledger, hierarchy-validated transfers | `[IMPL]` |
| ProductBatch/FEFO inventory-service operations, expiry query, UnitConversion, Opening Balance Supplier | `[IMPL]` for the described inventory service paths; order checkout and maintenance part usage are not FEFO/batch-aware (§3.2) |
| Sheet 1 frontend hierarchy/batches/unit conversion UI | `[IMPL]` |
| Product tier pricing (`ProductPrice`, Retail/Wholesale/FirstWholesale/Distributor), POS search/tier selection, server-side price resolution and order snapshot | `[IMPL]`; customer's `DefaultPricingTier` is not currently used as OrderService's fallback |
| Auto-translation on product/category create plus manual translation services/endpoints | `[IMPL]` code; provider is best-effort and frontend API has external fallback |
| Orders online/POS, discounts, tax, payments and customer debt ledger | `[IMPL]`; server-side prices/tax and atomic number sequence |
| Order checkout stock ledger | `[IMPL]` aggregate-stock movement + ledger; `[PARTIAL]` batch/FEFO traceability |
| Maintenance request intake/pricing | `[IMPL]` |
| Maintenance workflow, technician assignment, notes, parts, status UI | `[PARTIAL]` — transition static map duplicate keys; part consumption not batch-aware |
| Maintenance inspection fee / renamed MaintenanceTicket | `[PLANNED]` |
| Invoice entity, controller, service, PDF and share code | `[PARTIAL]` — schema/migration/code exist, but DI not registered; frontend share component unmounted |
| Customer debt schedule/service/controller and debt dashboard page | `[PARTIAL]` — reminder schedule is not an installment plan; DI/job wiring absent and page unmounted |
| WhatsApp notification transport/composer/logging | `[PARTIAL]` — code and schema exist; startup registration/config and delivery not active |
| Quartz | `[PARTIAL]` — packages and DebtReminderJob class exist; no configured scheduler/trigger in actual Program |
| POSShift, expense persistence, fiscal invoice certification, installment plans, voucher/cheque, other HR items | `[PLANNED]` except Expenses API's honest 501 stub `[PARTIAL]` |
| Expenses | `[PARTIAL]` — controller returns 501; no entity or DbSet |
| Frontend print engine A4/A5/thermal 80/58 | `[IMPL]`; backend PDF generation/share path remains `[PARTIAL]` |
| FluentValidation validators · Mapster mappings | `[PARTIAL]` (packages referenced, not used) |
| PostgreSQL support, automated tests, API CI | `[PLANNED]` |

### 10.1 Repository verification findings / blockers (2026-10-02)

These are observations from the checked-out source, not additional domain requirements:

1. **Phase 3 DI gap:** the compiled `Program.cs` does not register invoice, debt, notification, WhatsApp, signer/options services and does not configure Quartz. Corresponding controllers/classes are present but cannot currently execute their dependency graphs. A similarly named sibling `IbnAlZumar.API/Program.cs` is outside the `.csproj`; it is not the runtime entry point.
2. **Maintenance workflow state-map defect:** the static `AllowedTransitions` initializer repeats `AwaitingParts`, `InRepair` and `Completed` dictionary keys; first workflow use can throw during type initialization. Fix before treating Phase 2 as operational.
3. **FEFO gap:** order checkout and maintenance part use mutate aggregate `ProductStock` and write ledger movements without consuming a batch. `ProductBatch.RemainingQuantity` and per-batch provenance can diverge from aggregate stock.
4. **Soft-delete gaps:** `PricingService.DeletePriceAsync` and maintenance part removal call `DbSet.Remove`; `ApplicationDbContext` only installs query filters and does not convert deleted states to `IsDeleted`. These paths hard-delete BaseEntity rows and violate invariant §7.1; change them to soft-delete/retention-safe behavior before relying on audit retention.
5. **Phase 3 frontend wiring:** `DebtDashboardPage` and `ShareInvoiceButton` are present but currently have no route/consumer. Their files alone do not constitute exposed workflows.
6. **Startup migration visibility:** `SeedDatabaseAsync` catches and logs migration/seed errors and continues boot. Monitor/verify migration state explicitly; boot success is not migration success.
7. **Secret hygiene:** committed appsettings values and seeded initial account credentials are sensitive and must be rotated/removed; never copy their values into docs or logs.

Migrations currently in Backend source include InitialAzureCreate, AddOrderTaxAndOrderNumberSequence, Sheet 1 warehouse/batches, Phase 1 pricing/auto-translation, Phase 2 maintenance workflow, and Phase 3 debt/invoices. Their presence in Git does not prove they have been applied to any deployed database.

**Validation performed for this documentation sync:** `git diff --check` passed; Frontend `npm run build` passed (Vite emitted its existing large-chunk advisory); Backend build could not be run because the sandbox does not have the `dotnet` CLI installed. No source-code behavior was changed as part of this documentation update.

## 11. Prompting Rules for AI Coding Tools

1. Read §10 first. If the feature is `[PLANNED]`, you are creating it from scratch — follow the exact
   entity/field names given here so future prompts stay consistent.
2. Put files in the folders of §2. Never create a new top-level layer/project without being asked.
3. Respect the invariants in §7 — especially ledger writes, soft delete, and frozen historical prices.
4. Any schema change **must** include an EF Core migration.
5. Produce bilingual (Arabic-primary, RTL-safe) user-facing text on both API validation messages and UI.
6. Do not add dependencies that are already covered by the existing stack (no MediatR, AutoMapper,
   Redux, TypeScript migration, or a repository layer unless explicitly requested).
7. Do not print, log, or commit secrets; assume §9.3 credentials are compromised and being rotated.
8. When you are unsure whether something exists, **grep the repo** — do not assume.

---

## 12. Recent Changes Log

Read this before assuming something described elsewhere in this document as `[PARTIAL]` or generic is
still that way — this table tracks fixes that have landed since the base of this document was written.
Keep entries short; update the relevant section above (§3, §6, §7) as the primary source of truth, and
just log the change + date here.

| Date | Change | Status |
|---|---|---|
| 2026-09-24 | `OrderService.CreateAsync`: prices re-fetched server-side and client `UnitPrice`/`TotalAmount` never trusted; Phase 1 subsequently extended resolution to selected tier/quantity/variant through `IPricingService` (see §3.3). VAT computed server-side (`Order.TaxRate`/`TaxAmount`, `EgyptVatRate = 0.14m`); inventory decrement + aggregate `InventoryTransaction` write in the same transaction as the order; `OrderNumber` generated from an atomic `dbo.OrderNumberSeq` SQL sequence instead of `Count()+1`. `POST /api/orders` and `POST /api/payments/checkout` require `[Authorize]` (no more anonymous checkout). | `[IMPL]` — requires the `AddOrderTaxAndOrderNumberSequence` migration to be applied to each environment |
| 2026-09-24 | `InventoryController` (`Adjust`/`Transfer`), `ProductsController` (`Update`), and `ReportsController` (all endpoints) moved from generic `[Authorize]`/role checks to policy-based `[Authorize(Policy = DataSeeder.PermissionCodes.*)]` (`InventoryAdjust`, `ProductsEdit`, `ReportsView`). | `[IMPL]` |
| 2026-09-24 | `Program.cs`: removed the hardcoded JWT signing-key fallback; app now throws `InvalidOperationException` at startup if `Jwt:Key` is missing or under 256 bits. | `[IMPL]` |
| 2026-09-24 | `syncService.js` switched from a bare `axios.create({ baseURL: '/api' })` to the shared `axiosInstance` (JWT header + correct base URL in every environment); `db.js` recovers Dexie rows stuck in `'syncing'`, retries failed syncs with incremented `retryCount` (terminal `'failed'` after 5 attempts), and marks synced orders `'synced'` + `syncedAt` instead of hard-deleting them. | `[IMPL]` |
| 2026-09-24 | `ExpensesController` returns `501 Not Implemented` on both endpoints instead of a fake `200 OK`, since `Expense` still has no entity/DbSet; POS expense modal now shows an honest "not available yet" message instead of a false success alert. | `[PARTIAL]` (honest stub only — see §3.7) |
| 2026-09-24 | **Sheet 1 (Core ERP Requirements) completed.** `Warehouse` gained `Tier` (`WarehouseTier { MainCentral, RegionalBranch, PosShelfLocation }`) + self-referencing `ParentWarehouseId`; `InventoryService.TransferStockAsync` now rejects any transfer that isn't a direct parent↔child pair (or `MainCentral↔MainCentral`), via `ValidateWarehouseHierarchy`. New `ProductBatch` entity (`BatchNumber`, `WarehouseId`, `SupplierId?`, `ProductionDate?`, `ExpiryDate`, `InitialQuantity`/`RemainingQuantity`, `CostPrice`) consumed FEFO via `InventoryService.ConsumeFefoAsync`/received via `ReceiveBatchAsync`; `AdjustStockAsync` auto-routes negative adjustments through FEFO whenever batches exist for the product/warehouse; `GetExpiringBatchesAsync(withinDays, warehouseId)` added ahead of the `[PLANNED]` `ProductExpiryAlertJob` (§5.2). `DataSeeder.SeedOpeningBalanceSupplierAsync` seeds a fixed-Id (`999999`, `DataSeeder.OpeningBalanceSupplierId`) "Opening Balance Supplier" (مورد أول المدة) so legacy/historical stock can be registered without a formal `PurchaseOrder`. New `UnitConversion` entity (`FromUnit`/`ToUnit`/`Factor`/`IsBaseUnit`) alongside `Product.QuantityPerCarton`; `ProductVariant` gained `Size` plus a new `ProductVariantAttributeValue` join table for arbitrary attribute linking against the shared `ProductAttributeDefinition` catalog. New permission `Inventory.ManageBatches` gates the new endpoints: `POST /api/inventory/batches/receive`, `POST /api/inventory/batches/consume-fefo`, `GET /api/inventory/batches`, `GET /api/inventory/batches/expiring`, `GET /api/inventory/warehouses/hierarchy`. | `[IMPL]` — requires the `Sheet1_WarehouseHierarchy_ProductBatch_UnitConversion_OpeningBalanceSupplier` migration to be applied to each environment |
| 2026-09-25 | Complete Sheet 1 React Frontend UI & API Integration. Added `getWarehouseHierarchy`, `getBatches`, `getExpiringBatches`, `receiveBatch`, and `consumeBatchFefo` to `src/api/inventoryApi.js` using the shared `axiosInstance`. Created `WarehouseHierarchyView.jsx` in `src/pages/admin/Inventory/`, `BatchesManagementPage.jsx` in `src/pages/admin/Inventory/`, and `ProductUnitConversionModal.jsx` in `src/components/operations/`. Registered `/admin/inventory/hierarchy` and `/admin/inventory/batches` routes in `src/App.jsx`. | `[IMPL]` |
| 2026-09-27 | Phase 1 schema/code: `ProductPrice`, `PricingTierType` incl. Distributor, customer default/order price tier, POS product/price endpoints, price resolution service; translation settings/service, create-time auto-translation flags and manual product/category translation endpoints. Migration `20260927134919_Phase1_PricingTier_AutoTranslation`. | `[IMPL]` code; verify deployment migration separately |
| 2026-09-28 | Phase 2 maintenance workflow: additive fields on `MaintenanceRequest`, `MaintenanceNote`, `MaintenancePartUsage`, `MaintenanceUsed` inventory type, service/controller, technician/parts/status/labor/receipt frontend. Migration `20260928125419_Phase2_MaintenanceWorkflow`. Current static transition map has duplicate dictionary keys; see §10.1. | `[PARTIAL]` |
| 2026-09-30 | Phase 3 schema/code: `Invoice`, `CustomerDebtSchedule`, `NotificationLog`, debt and invoice controllers/services, QuestPDF and WhatsApp/email sender code, invoice sharing and debt dashboard frontend files. Migration `20260930220513_AddPhase3DebtAndInvoices`. Runtime DI/job configuration is missing and two frontend files are unmounted; see §10.1. | `[PARTIAL]` |
| 2026-10-02 | Synced this architecture against Backend `8686125` and Frontend `0709df2b`. Added verified implementation status, exact route surfaces, integration gaps, batch/FEFO and soft-delete caveats, startup migration failure behavior, secret hygiene note, and validation results. | `[DOC SYNC]` |

---
