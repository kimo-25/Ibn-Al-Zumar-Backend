# Ibn Al-Zumar Backend — Technical Context Blueprint

> **Purpose:** Give future engineers and AI assistants a source-grounded map of the backend: what is active, how requests and data flow, how to make safe changes, and which repository inconsistencies require extra care.
>
> **Repository:** `kimo-25/Ibn-Al-Zumar-Backend`  
> **Inspected snapshot:** `main` at the cloned `origin/main` state during this task.  
> **Active solution/project:** `Ibn al-Zumar.API/Ibn al-Zumar.API.sln` → `Ibn al-Zumar.API/Ibn al-Zumar.API/Ibn al-Zumar.API.csproj`.

## 1. Project overview and technology stack

### 1.1 What the system does

This is the API/backend for a hardware-retail operation. Its business capabilities include catalog and product variants/pricing, inventory and warehouses, point-of-sale and orders, purchasing/suppliers, customer balances/debt, invoices, maintenance workflow, staff attendance/payroll, reminders, reports, role/permission management, and AI-assisted catalog/sales tasks. API messages include both English and Arabic; do not assume English-only content or locale-independent display strings.

The README describes an ASP.NET Core and EF Core backend with inventory, POS, and an administrative dashboard. It does not give more setup detail; this file derives the operational description from the source.

### 1.2 Runtime and language

| Concern | Observed implementation |
|---|---|
| Web framework | ASP.NET Core Web SDK, `net9.0` target framework |
| Language | C#; nullable reference types and implicit usings enabled |
| Hosting model | ASP.NET Core minimal hosting in `Program.cs`, with MVC controllers |
| Database | SQL Server via EF Core SQL Server provider |
| ORM | Entity Framework Core; context `ApplicationDbContext` |
| API docs | Swagger/Swashbuckle, exposed at `/swagger` |
| Authentication | JWT bearer, HMAC-SHA256 signing and custom user/role tables; ASP.NET Core Identity is **not** configured as the identity store |
| Hosting/deployment | Docker multi-stage build on .NET 9 SDK/ASP.NET images; IIS `web.config`; Azure/App Service-oriented deployment profiles are present |
| Test projects | None found in the tracked solution tree |

### 1.3 NuGet packages and their role

Versions below are from the active `.csproj`, not inferred from source usage.

| Package | Version | Role / usage notes |
|---|---:|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.0 | SQL Server provider used by `UseSqlServer` |
| `Microsoft.EntityFrameworkCore.Design` | 9.0.15 | Design-time EF tooling support; private asset |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.10 | EF tooling package; **version does not match the EF Core 9 runtime packages**—check tool compatibility before changing/upgrading |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 9.0.0 | JWT bearer authentication middleware |
| `Microsoft.Extensions.Identity.Core` | 9.0.0 | Password hasher abstraction/implementation used by the custom `User` entity; not full Identity UI/store configuration |
| `Microsoft.AspNetCore.OpenApi` | 9.0.8 | OpenAPI-related ASP.NET package; Swagger generation is also provided by Swashbuckle |
| `Swashbuckle.AspNetCore` | 6.8.1 | Swagger document and UI |
| `Mapster`, `Mapster.DependencyInjection` | 10.0.11 | Referenced, but no active C# usage or registration was found. DTO conversion is currently hand-written/projection-based. |
| `FluentValidation.AspNetCore` | 11.3.1 | Referenced, but no validators or MVC integration were found. |
| `FluentValidation.DependencyInjectionExtensions` | 12.1.1 | Referenced, but no validators/registration found; note the major-version mismatch against the ASP.NET package. |
| `ClosedXML` | 0.105.1 | Excel import/export, including product import and invoice-to-Excel functionality |
| `DocumentFormat.OpenXml` | 3.5.1 | Open XML spreadsheet/document support, potentially alongside ClosedXML |
| `Google.Apis.Auth` | 1.75.0 | Google identity token validation/login path |
| `MailKit` | 4.17.0 | Email transport/library support; `EmailService` is configured through the `Brevo` section |
| `Quartz`, `Quartz.Extensions.Hosting` | 3.8.0 | Scheduling library dependencies; no Quartz scheduler/job registration was found in active `Program.cs` |
| `QuestPDF` | 2023.12.1 | Invoice PDF generation |
| `RestSharp` | 114.0.0 | HTTP integration dependency; most visible integrations use `HttpClient` directly |

### 1.4 Build / verification status

A build was attempted with `dotnet build ... --no-restore`, but the sandbox did not have the `dotnet` CLI/SDK installed (`dotnet: command not found`). Thus **build success has not been verified**. Static review also finds version/namespace/duplicate-source concerns detailed below; install/use a .NET 9 SDK and run a full restore/build before treating the repository as build-clean.

## 2. Architecture and design

### 2.1 Actual architecture (not a strict Clean Architecture solution)

The active project is a **single ASP.NET Core project/assembly** organized into feature and technical folders. It has Clean-Architecture-like concepts but not separate Domain/Application/Infrastructure/API projects, project references, or a strict dependency boundary.

```text
HTTP client
  → Controllers (routing, authorization, model binding, response shaping)
  → Services (business rules, DTO/entity shaping, integrations)
  → ApplicationDbContext / EF configurations (persistence)
  → SQL Server

Cross-cutting: JWT authentication, role/permission authorization, exception middleware,
settings/options, external APIs, file/PDF/XLSX processing.
```

Controllers/services often inject `ApplicationDbContext` directly, so persistence is not fully isolated behind repositories. Some actions delegate through an `I...Service`, while some controllers implement database queries/response projection themselves. Follow the local feature precedent rather than assuming every endpoint has the same layering.

### 2.2 Layers and responsibilities

- **Domain** (`Domain/Common`, `Domain/Entities`, `Domain/Enums`): persistence-oriented business entities, enum definitions, and `BaseEntity`. These are in the same project as the API.
- **Application/business logic** (`Services/<feature>`): service interfaces and implementations. Most interface-backed services are request-scoped. Feature logic includes auth, catalog, orders, inventory, purchasing, customer debt, maintenance, attendance, payments and integrations.
- **Infrastructure/persistence** (`Persistence`, `Migrations`, `Infrastructure/Jobs`): EF `DbContext`, entity configurations, migrations, seeding, and the debt-reminder job class. The `Infrastructure/Jobs` class is present but is not scheduled in active startup code.
- **Presentation** (`Controllers`, `DTOs`, `Middleware`, `Authorization`): MVC API endpoints, request/response types, cross-cutting error middleware, custom authorization handlers/providers.

### 2.3 Dependency injection and lifetimes

`Program.cs` is the single DI composition root. EF `AddDbContext<ApplicationDbContext>` uses the standard scoped context lifetime. The source explicitly registers:

- **Scoped:** `IAuthService/AuthService`, `ICategoryService/CategoryService`, `IProductService/ProductService`, `IAiAuditLogService/AiAuditLogService`, `IInvoiceToExcelService/InvoiceToExcelService`, `ICustomerService/CustomerService`, `IInventoryService/InventoryService`, `IPurchasingService/PurchasingService`, `IUserManagementService/UserManagementService`, `IOrderService/OrderService`, `IMaintenanceWorkflowService/MaintenanceWorkflowService`, `IPricingService/PricingService`, `IPosCatalogService/PosCatalogService`, `IProductTranslationService/ProductTranslationService`, `IReminderService/ReminderService`, `IEmailService/EmailService`, `IAiFileProcessingService/AiFileProcessingService`, `IVoiceVerificationService/VoiceVerificationService`, `IVoiceCommandService/VoiceCommandService`, `IAttendanceService/AttendanceService`, and the authorization handler.
- **Typed `HttpClient` registrations:** `ITranslationService/TranslationService`, `IPaymobService/PaymobService`, and `IAiAssistantService/AiAssistantService` (configured timeouts/base URL in startup).
- **Singleton:** `IPasswordHasher<User>` (`PasswordHasher<User>`), each registered `IAiTool` implementation, `AiToolRegistry`, and `IAuthorizationPolicyProvider` (`PermissionPolicyProvider`). AI tools are registered singleton; they should not capture scoped services/DbContexts in their constructors. Tool code that needs scoped DB work should resolve it from the request/service scope as existing implementations do.
- **No explicit transient registrations** found.

**DI wiring to verify when adding functionality:** active `Program.cs` does not show registrations for invoice service/PDF, notification composer/sender, WhatsApp client, or Quartz/debt reminder job. `IInvoiceToExcelService` is registered, but it is not the same thing as invoice PDF service. Some classes/options may be used via direct construction or not yet wired; check the target controller constructor and service graph before assuming availability.

## 3. Database and ORM

### 3.1 Provider and startup behavior

`ApplicationDbContext` is registered with `options.UseSqlServer(connectionString)`. It reads the connection string in this order:

1. `ConnectionStrings:DefaultConnection`
2. environment variable `SQLAZURECONNSTR_DefaultConnection`
3. environment variable `DATABASE_URL`

A missing/empty connection string throws during host construction. `DATABASE_URL` is passed as a SQL Server connection string without provider translation; it must contain an appropriate SQL Server-style value.

After `builder.Build()`, startup calls `SeedDatabaseAsync()` before middleware setup/run. That method creates a DI scope, executes `Database.MigrateAsync()` and then `DataSeeder.SeedAsync(...)`. A broad `catch` logs and **swallows migration/seeding errors**, so the process may start while the DB schema/data was not updated; check logs and schema if the API starts but database-backed actions fail.

### 3.2 Context conventions and shared persistence rules

`Persistence/ApplicationDbContext.cs`:

- Exposes DbSets for catalog, inventory, purchasing, sales, shipping, notifications, dynamic RBAC, reminders, maintenance, attendance/payroll and AI audit logging.
- Calls `ApplyConfigurationsFromAssembly(...)` to find `IEntityTypeConfiguration<T>` mappings.
- Applies a global decimal convention of `decimal(18,2)`.
- Configures `AiAuditLog` table, lengths and indexes in `OnModelCreating`.
- Adds a unique filtered index on `Order.ClientUuid` (`[ClientUuid] IS NOT NULL`) for offline order idempotency.
- Defines explicit relationships/delete rules and a number of additional indexes inline.
- Applies a dynamic global soft-delete filter to every entity type derived from `BaseEntity` (`IsDeleted == false`). Entities **not** deriving from `BaseEntity` do not get this filter automatically. `RolePermission`, `UserPermission`, and `UserRole` also filter records through deleted linked permission/role entities.
- Seeds a `Warehouse` row with fixed ID 1 through model `HasData` (`Main Warehouse`, main/active, central tier). This is model data and therefore must stay aligned with migrations.
- Overrides `SaveChanges` and `SaveChangesAsync`: on inserts it writes UTC `CreatedAt`; on updates it writes UTC `UpdatedAt` for tracked `BaseEntity` entities.

The context does not call `UseIdentity` or derive from `IdentityDbContext`. Users/roles/permissions are custom application tables and a custom `IPasswordHasher<User>` is registered.

### 3.3 Main entity groups and relationships

| Area | Entities / important relationships |
|---|---|
| Catalog | `Category` has self-referential parent/subcategories and a unique slug; `Brand`; `Product` associated with category/brand, stock, order lines, batches, images/attributes/variants/conversions; `ProductVariant`; attribute definitions/values; `ProductImage`; `UnitConversion`; `ProductPrice`. Product prices are tiered and can apply to product or variant plus minimum quantity; configuration enforces a unique key across product/variant/tier/min-quantity. |
| Inventory | `Warehouse` has a parent/child hierarchy, tier and central/main indicator; `ProductStock` is unique per product+warehouse; `InventoryTransaction` records product/warehouse (and optional batch) movements; `StockTransfer` has source/destination warehouse and child transfer items; `ProductBatch` links product+warehouse and carries batch/expiry data. Main transaction/parent references use restrictive deletion, while true child collections (such as transfer items) cascade. |
| Sales | `Customer` holds contact, credit limit/current balance, pricing tier, and collections of orders/payments/ledger entries. `Order` stores customer or guest data, source/status/payment state, pricing tier, warehouse/cashier, shipping, discount/tax totals and line/payment collections. `OrderItem` references order/product and stores sale/cost/discount snapshots. `Payment` may reference order/customer/receiver. `CustomerLedgerEntry` records balance movements and related order/payment. Order number is unique; order date indexed; ClientUuid has unique filtered index. |
| Debt/invoices | `CustomerDebtSchedule` tracks reminder schedule and last known balance. `Invoice` may originate from an order or maintenance request and records invoice number, customer, issuer, totals, format, PDF storage path, and delivery timestamps. |
| Purchasing | `Supplier`, `PurchaseOrder`, `PurchaseOrderItem`, `SupplierPayment`, `SupplierLedgerEntry`; supplier accounting payments and ledger entries are linked with restrictive deletes. `DataSeeder.OpeningBalanceSupplierId` reserves a fixed supplier ID for opening inventory balance; do not reuse it. |
| Staff/identity | Custom `User`, `Role`, `Permission`, join tables `UserRole`, `RolePermission`, and `UserPermission` (including grant/deny override). User also stores verification/reset codes, phone state, hourly rate, voice-enrollment embedding and attendance/payroll navigations. Composite keys are used for joins. |
| Attendance/payroll | `AttendanceLog` links to a user; `PayrollRecord` links to a user. Attendance-user and payroll-user deletion is restrictive. Attendance query index is `(UserId, CheckInTime)`. |
| Maintenance | `MaintenanceRequest`, `MaintenanceNote`, `MaintenancePartUsage`; notes/parts belong to a request, use tracked status/workflow, technician, labor/parts, warehouse/product and inventory transaction references. Request-child notes cascade; author/user/parts/inventory references generally restrict deletion. |
| Other | `Reminder`, `ShippingZone`, `NotificationLog`, `AiAuditLog`. AI audit rows index timestamp, `(UserId, TimestampUtc)` and tool name. |

Enums are declared in `Domain/Enums/Enums.cs` (e.g. order source/status, payment method/status, discounts, purchase order state, inventory transaction/transfer status, delivery, reminder, warehouse tier, pricing tier and custom shipping-zone state). Several configurations persist enums as **strings**, not integers; inspect each entity config before changing an enum because an enum edit can affect existing stored values and migrations.

### 3.4 Migrations strategy/history

Migrations are stored in `Persistence` project folder `Migrations` in timestamped EF Core migration pairs (`*.cs` + `*.Designer.cs`) plus `ApplicationDbContextModelSnapshot.cs`. Current tracked migration sequence:

1. `20260921180731_InitialAzureCreate` — initial schema and initial seeded schema data.
2. `20260923214146_AddOrderTaxAndOrderNumberSequence` — order tax-related fields and SQL Server order-number sequence (`dbo.OrderNumberSeq`).
3. `20260924160129_AddSheet1WarehouseHierarchyAndBatches` — warehouse hierarchy and batch tracking; includes data updates and foreign keys/indexes.
4. `20260927134919_Phase1_PricingTier_AutoTranslation` — pricing tier and product pricing/automatic translation-related schema.
5. `20260928125419_Phase2_MaintenanceWorkflow` — maintenance workflow tables/fields.
6. `20260930220513_AddPhase3DebtAndInvoices` — debt schedules and invoice additions.

The exact authoritative model is the current context/configuration plus `ApplicationDbContextModelSnapshot`. Always generate and review a new migration against the snapshot; do not hand-edit only a migration designer/snapshot or change the startup migration policy casually. Some migrations contain provider-specific SQL (notably SQL Server sequence syntax), reinforcing that SQL Server is the intended provider.

### 3.5 Seed data

`Persistence/Seed/DataSeeder.cs` is called at each startup and uses existence checks for most rows. It seeds permission definitions, roles (`Owner`, `Admin`, `Moderator`, `Cashier`), role-permission links, bootstrap user accounts, brands/categories, CSV products/reminders, and an opening-balance supplier. It creates a fixed bootstrap administrator/moderator login credential pair in source code; **do not copy or disclose those values**. These are security-sensitive and should be rotated/removed or provisioned from secure configuration. The seeder sets password hashes through `IPasswordHasher<User>`; check user verification flags in the entity and login path before assuming a seeded account can authenticate.

Seed input files are `Persistence/Seed/Products.csv`, `Reminders.csv`, and `SeedData.sql`. The two CSV files are configured to copy to output. The seed implementation tries paths relative to the app base directory and current working directory.

## 4. Directory structure map

### 4.1 Repository root and active solution

```text
.
├── README.md                         # Short product summary; little developer setup guidance.
├── LICENSE                           # Repository license text.
├── .gitignore                        # Root ignore file; inspect before adding generated files.
└── Ibn al-Zumar.API/
    ├── Ibn al-Zumar.API.sln           # Sole solution; contains one project.
    ├── Dockerfile                     # .NET 9 SDK build + ASP.NET runtime container.
    ├── Controllers/                   # Legacy/duplicate controllers OUTSIDE active csproj.
    ├── Ibn al-Zumar.API/              # Active Web SDK project root; default compile scope.
    ├── Ibn al-Zumar.Domain/           # Stray/legacy domain fragment outside active project.
    ├── IbnAlZumar.API/                # Stray/legacy API entry-point fragment outside active project.
    └── Persistence/                   # Stray/legacy seeding/initializer fragments outside active project.
```

**Important:** The solution points only to `Ibn al-Zumar.API/Ibn al-Zumar.API/Ibn al-Zumar.API.csproj`. SDK default compile items are under that project directory. The sibling folders `Ibn al-Zumar.API/Controllers`, `Ibn al-Zumar.API/Ibn al-Zumar.Domain`, `Ibn al-Zumar.API/IbnAlZumar.API`, and `Ibn al-Zumar.API/Persistence` are not additional projects and are not compiled by that `.csproj` as configured. Do not infer active runtime behavior from those duplicates; make changes in the nested project unless project membership is deliberately changed.

### 4.2 Active project map

```text
Ibn al-Zumar.API/Ibn al-Zumar.API/
├── Authorization/                    # Permission requirement, dynamic policy provider, claim handler.
├── Common/
│   ├── Exceptions/                   # App-level exception definitions (coexists with another exception file).
│   ├── Helpers/                      # Invoice link signing, slug, and translation helpers.
│   └── Settings/                     # Typed options for JWT, email, AI, translation, PDF, WhatsApp, reminders.
├── Controllers/                      # MVC controllers, one or more feature endpoints each.
├── DTOs/                             # Feature-grouped request/response contracts; not entities.
├── Domain/
│   ├── Common/                       # BaseEntity/audit/soft-delete fields.
│   ├── Entities/                     # Custom relational model grouped by feature.
│   └── Enums/                        # Persisted business-state enums.
├── Infrastructure/Jobs/              # Debt reminder job class (not scheduled in Program.cs).
├── Middleware/                       # Global exception-to-JSON middleware.
├── Migrations/                       # EF migration classes and model snapshot.
├── Persistence/
│   ├── ApplicationDbContext.cs       # DbSets, conventions, filters, relationships and timestamps.
│   ├── Configurations/               # Per-entity EF Core fluent mappings.
│   └── Seed/                         # Startup seeder and seed data files.
├── Properties/                       # Local launch settings, publish/service deployment metadata.
├── Services/                         # Feature business services and external-system clients.
├── wwwroot/uploads/products/         # Static sample/default product images.
├── Program.cs                        # Startup, DI, auth, middleware, Swagger, CORS, static assets.
├── appsettings*.json                 # Configuration shape and environment-specific logging/AI values.
├── Ibn al-Zumar.API.csproj           # Project target framework and NuGet packages.
├── Ibn al-Zumar.API.http             # HTTP scratch/request file for local testing.
├── web.config                        # IIS hosting, anonymous access, SPA rewrite exclusions.
└── .config/dotnet-tools.json         # Local dotnet-ef tool manifest (declared EF tool 10.0.10).
```

### 4.3 Feature file groups

- **Controllers (29 files):** `AiController`, `AiVoiceController`, `AttendanceController`, `AuthController`, `CatalogController`, `CategoriesController`, `CustomerDebtController`, `CustomersController`, `DashboardController`, `ExpensesController`, `InventoryController`, `InvoicesController`, `MaintenanceController`, `MaintenanceWorkflowController`, `OrdersController`, `PaymentsController`, `PayrollController`, `PosController`, `ProductPricingController`, `ProductTranslationController`, `ProductsController`, `PurchasingController`, `RemindersController`, `ReportsController`, `RolesController`, `ShippingZonesController`, `SyncController`, `TranslationController`, `UsersController`.
- **DTOs:** `AI/` for chat turns/attachments; `Attendance/`; `Auth/`; `Catalog/`; `Common/`; `Customers/`; `Identity/`; `Inventory/`; `Invoices/`; `Maintenance/`; `Payments/`; `Purchasing/`; `Reminders/`; `Sales/`; `Sync/`; `Translation/`. Watch for duplicate generic pagination DTOs: `DTOs/Catalog/PagedResultDto<T>` and `DTOs/Common/PagedResultDto<T>` live in different namespaces and have slightly different computed metadata.
- **Domain entities:** `Ai/`, `Attendance/`, `Catalog/`, `Identity/`, `Inventory/`, `Maintenance/`, `Notifications/`, `Purchasing/`, `Reminders/`, and `Sales/`.
- **Persistence configurations:** `Catalog/`, `Identity/`, `Inventory/`, `Maintenance/`, `Notifications/`, `Purchasing/`, and `Sales/`; loaded by assembly scanning.
- **Services:**
  - `Ai/` (assistant, roles, audit, voice, tool registry and tool actions; nested `Files/`, `Tools/`)
  - `Attendance/`, `Auth/`, `Catalog/`, `Customers/`, `Email/`, `Identity/`, `Inventory/`, `Invoices/`, `Maintenance/`, `Models/`, `Notifications/` (with `WhatsApp/`), `Payments/`, `Purchasing/`, `Reminders/`, `Sales/`.
- **Configuration assets:** `appsettings.json`, `appsettings.Development.json`, `appsettings.Production.json`, `Properties/launchSettings.json`, `Properties/PublishProfiles/*`, `Properties/ServiceDependencies/*`, `Dockerfile`, and `web.config`. Publish profiles can contain sensitive deployment metadata; do not paste their contents in generated context or logs.

## 5. API endpoints and routing

### 5.1 General conventions

- Controllers use attribute routes and are mapped through `app.MapControllers()`; there is no global `/api/v1` prefix. Most use `api/[controller]`, which produces paths such as `/api/Products`, `/api/Orders`, and `/api/Customers` (controller casing follows the class name but URLs are normally case-insensitive). Several feature controllers use explicit lowercase/hyphenated routes.
- No single response envelope is enforced. Actions return a mix of `Ok(...)`, `CreatedAtAction(...)`, `NoContent()`, direct `BadRequest`/`NotFound` objects, file downloads, and ad-hoc anonymous `{ message }` responses. Respect the existing endpoint's concrete DTO and JSON shape.
- Query filtering/paging is action/DTO-specific. Product/catalog list endpoints use feature-specific paging DTOs; sync uses per-item success/failure results; pagination is not globally standardized.
- Check each action's authorization attribute. A controller-level `[Authorize]` can be overridden by action `[AllowAnonymous]`; route existence does not imply authentication behavior.

### 5.2 Route inventory (active controllers)

| Controller / route base | Main actions (verbs and suffixes) | Access highlights |
|---|---|---|
| `AuthController` — `/api/Auth` | `POST register`, `login`, `verify-email`, `resend-verification-code`; `POST change-email`, `resend-new-email-code`, `verify-new-email`, `send-phone-otp`, `verify-phone`, `change-password` (authenticated); `POST forgot-password`, `verify-reset-code`, `reset-password`; `GET profile`; `PUT update-profile`; `POST google`. | Registration/login/verification/reset/Google are public; profile/email/phone/password changes require a bearer token. |
| `AiController` — `/api/Ai` | `POST chat`; `POST chat/stream` (SSE response). | Controller-level authenticated. Chat input includes prompt/context and optional attachments. |
| `AiVoiceController` — `/api/ai` | `POST voice-command`. | Explicit role allow-list: admin/superadmin/moderator/cashier/store owner/online manager/owner. |
| `AttendanceController` — `/api/attendance` | `POST enroll-voice` (multipart); `POST voice-check` (multipart); `GET logs?from=&to=`. | Authenticated generally; logs restricted to admin/superadmin/store owner. |
| `CatalogController` — `/api/Catalog` | `POST convert-invoice-to-excel` (multipart file). | Admin/superadmin/store owner. |
| `CategoriesController` — `/api/Categories` | `GET`; `GET {id:int}`; `POST`; `PUT {id:int}`; `DELETE {id:int}`. | Reads explicitly anonymous; writes require `Categories.Manage`. |
| `CustomerDebtController` — `/api/customer-debt` | `GET` paginated debt dashboard; `POST {customerId:int}/send-reminder`. | Controller-level `Customers.ManageDebt` permission. |
| `CustomersController` — `/api/Customers` | `GET`; `GET {id:int}`; `POST`; `PUT {id:int}`; `DELETE {id:int}`; `POST {id:int}/adjust-debt`. | Authenticated base; per-action roles narrow read/create/update/delete/debt adjustment. |
| `DashboardController` — `/api` | `GET owner/summary`; `GET operations/summary`. | Authenticated; Owner-only owner summary; Owner/Admin operations summary. |
| `ExpensesController` — `/api/Expenses` | `POST`; `GET`. | Authenticated; **currently returns 501 Not Implemented** (stub, not functional expense persistence). |
| `InventoryController` — `/api/Inventory` | `POST adjust`, `transfer`, `batches/receive`, `batches/consume-fefo`; `GET batches`, `batches/expiring`, `warehouses/hierarchy`, `warehouses`, `stock-levels`, `transactions`, `low-stock`. | Controller-level authenticated; batch/stock/warehouse query and mutation use permission attributes or role restriction as defined on each action. |
| `InvoicesController` — `/api/invoices` | `POST from-order`, `from-maintenance`; `GET {id:int}`; `POST {id:int}/share`; `GET {id:int}/pdf?token=`. | Most require authentication. PDF action is intentionally anonymous; it validates a signed query token rather than `[Authorize]`. |
| `MaintenanceController` — `/api/Maintenance` | `POST` multipart and JSON request creation; `GET my-requests`; `GET`; `PUT {id}/respond`. | Create/my requests require authentication; list/respond allow specific management roles. |
| `MaintenanceWorkflowController` — `/api/maintenance-workflow` | `GET`; `GET {id:int}`; `POST {id}/assign-technician`, `{id}/status`, `{id}/notes`, `{id}/parts`; `DELETE {id}/parts/{partUsageId:int}`; `POST {id}/labor-cost`; `GET technicians`; `GET {id}/receipt`. | Base authenticated; read uses `Maintenance.View`; workflow mutations use `Maintenance.Manage`. |
| `OrdersController` — `/api/Orders` | `GET`; `GET {id}`; `POST`; `GET my-orders`; `PUT {id}/advance-status`; `PUT {id}/status?status=`; `POST {id}/request-cancel`; `POST {id}/approve-cancel`. | Authenticated for individual/create/my-order/cancel request; admin/moderator for list/status/approval. Order create is explicitly intended never to be anonymous. |
| `PaymentsController` — `/api/payments` | `POST checkout`; `POST webhook`. | Checkout authenticated; webhook authenticates callback using Paymob HMAC. |
| `PayrollController` — `/api/payroll` | `GET summary?startDate=&endDate=`. | Admin/superadmin/store owner. |
| `PosController` — `/api/Pos` | `GET products` (search/catalog); `GET products/{productId:int}/unit-price` with tier/quantity/variant query. | Any authenticated staff account. |
| `ProductPricingController` — `/api/ProductPricing` | `GET {productId:int}`; `POST` upsert; `DELETE {priceId:int}`. | `Products.Edit` permission. |
| `ProductTranslationController` — `/api` | `POST products/{productId:int}/translate?overwrite=`; `POST categories/{categoryId:int}/translate?overwrite=`. | `Products.Edit` permission. |
| `ProductsController` — `/api/Products` | `GET`; `GET {id:int}`; `POST` multipart; `PUT {id:int}` multipart; `DELETE {id:int}`; `POST bulk-import` multipart. | Public reads; product create/edit/delete/bulk import use permission requirements. |
| `PurchasingController` — `/api/Purchasing` | `GET suppliers`; `GET suppliers/{id:int}`; `POST suppliers`; `PUT suppliers/{id:int}`; `DELETE suppliers/{id:int}`; `GET orders`; `GET orders/{id:int}`; `POST orders`; `POST orders/receive`; `POST suppliers/{id:int}/payments`; `GET suppliers/{id:int}/ledger`; `GET suppliers/{id:int}/details`. | Controller-level Owner/Admin. Some actions locally catch `KeyNotFoundException`. |
| `RemindersController` — `/api/Reminders` | `GET random`; admin CRUD `GET admin/all`, `POST admin`, `PUT admin/{id:int}`, `PATCH admin/{id:int}/toggle-status`, `DELETE admin/{id:int}`. | Random read anonymous; admin actions restricted to management roles. |
| `ReportsController` — `/api/Reports` | `GET sales?startDate=&endDate=`; `GET inventory-status`; `GET financial?startDate=&endDate=`. | `Reports.View` permission. |
| `RolesController` — `/api/Roles` | `GET`; `POST`; `GET permissions`. | Authenticated base; role listing/creation needs `Roles.Manage`; permissions list needs `Permissions.Manage`. |
| `ShippingZonesController` — `/api/ShippingZones` | Public `GET`, `GET {id}`; admin `POST`, `PUT {id}`, `DELETE {id}`; `GET pending-requests` (admin/moderator); `POST requests/{orderId}/accept` and `/reject` (admin). | Reads public; management endpoints use role-based authorization. |
| `SyncController` — `/api/orders` | `POST sync` (batch offline orders). | Authenticated employee/cashier; explicitly never anonymous. Uses `ClientUuid` idempotency and server-side calculation/validation. |
| `TranslationController` — `/api/v1/translation` | `POST translate`. | No controller-level authorization attribute found; confirm business exposure before expanding it. |
| `UsersController` — `/api/Users` | `GET`; `GET {id}`; `POST`; `PUT {id}/roles`; `PATCH {id}/toggle-status`; `GET {userId}/profile-summary?from=&to=`; `PATCH {userId}/hourly-rate`. | Authenticated base; user administration permission for core CRUD/roles/status; profile/pay-rate endpoints restricted to admin/superadmin/store owner. |

**Routing review note:** The surface mixes `/api/[controller]`, explicit lowercase routes, and one versioned translation prefix. Do not rename route segments or change HTTP verbs casually; clients are likely tied to these literal paths.

### 5.3 Request, response, binding and mapping patterns

- JSON body DTOs typically bind through `[FromBody]`; query strings use `[FromQuery]`; product/maintenance/attendance uploads use `[FromForm]` and/or `IFormFile`.
- DTO folders are feature-specific (`DTOs/Auth`, `Catalog`, `Sales`, etc.). Prefer adding/editing a DTO over binding a persistence entity directly. Some endpoints, notably shipping-zone management, accept entity objects directly, which is an existing exception rather than a general recommendation.
- Response mapping is mostly manual projection or service-built DTOs. No Mapster configuration/use was found despite package references. Do not introduce `Adapt<T>()` assumptions without wiring and tests.
- Pagination has multiple shapes/namespaces; catalog and common `PagedResultDto<T>` differ (one computes `TotalPages` and previous/next flags). Check the consumer and service before reusing one.
- `POST /api/orders/sync` accepts a batch (1–200 records) and returns aggregate counts plus per-order results with `ClientUuid`, success, server ID, error code/message. The client sends business inputs; server computes its own total/discount/stock effects as implemented—never trust client-supplied computed totals in a new sync flow.
- Several API response messages and validation messages are Arabic. Preserve stable text when a client may display it; prefer structured error code/DTO additions for contract changes.

## 6. Error handling and validation

### 6.1 Exception handling

`ExceptionHandlingMiddleware` is registered immediately after `Build()` and wraps downstream middleware. It emits camelCase JSON using `ApiErrorResponse` fields:

```json
{
  "statusCode": 400,
  "message": "...",
  "traceId": "...",
  "errors": { "field": ["..."] },
  "timestampUtc": "..."
}
```

The active error types in `DTOs/Common/Exceptions/Appexceptions.cs` live under `IbnAlZumar.Api.Common.Exceptions`: `AppException` carries an HTTP status, with `ValidationAppException` (400 + optional errors), `UnauthorizedAppException` (401), `ForbiddenAppException` (403), `NotFoundAppException` (404) and `ConflictAppException` (409). Middleware logs handled app exceptions at warning and unknown exceptions at error. Unknown failures return 500; message is exception text in Development and generic in non-Development.

**Do not assume all exceptions reach this mapping.** `Common/Exceptions/AppExceptions.cs` is a second file with a different namespace (`IbnAlZumar.API.Common.Exceptions`) and separate `NotFoundException`/`BadRequestException` types. These are not the same base hierarchy as `AppException`; many controllers catch them and return ad-hoc 400/404, while other paths may fall through to the 500 handler. `KeyNotFoundException` is also locally caught in some controllers. Preserve/normalize this intentionally and avoid creating yet another exception taxonomy. Middleware currently does not visibly special-case framework model-state errors or database exceptions.

### 6.2 Validation

- Request DTOs use `System.ComponentModel.DataAnnotations` (`Required`, `EmailAddress`, `MaxLength`, `MinLength`, `Range`, etc.). A subset of actions explicitly checks `ModelState.IsValid` and returns `BadRequest(ModelState)`.
- `Program.cs` calls `AddControllers()` but does not visibly add `AddFluentValidation`/validator scanning or configure a global validation response factory. No `AbstractValidator<T>`, `IValidator<T>`, or custom FluentValidation validator classes were found. Thus FluentValidation is an unused dependency at this snapshot; do not claim it validates requests.
- `[ApiController]` presence is not consistently established as a repository-wide convention; inspect each controller class before assuming automatic model-state 400 behavior. Existing manual checks differ by action.
- Business validation is also performed inside services/controllers (e.g. duplicate SKU, quantity/stock, invalid product/variant associations, date ranges, price tier uniqueness). If adding a rule, place it at the domain/business boundary that all relevant callers use, and preserve DTO-level input constraints as needed.

## 7. Security and authentication

### 7.1 JWT flow

- Default authenticate/challenge scheme is `JwtBearerDefaults.AuthenticationScheme` (Bearer).
- JWT settings are materialized in startup from `Jwt:Key`, with `Jwt__Key` fallback; key must be at least 32 UTF-8 bytes or startup throws. Issuer and audience default if absent. Token validation checks issuer, audience, signing key and lifetime; clock skew is one minute. HTTPS metadata is required outside Development.
- `AuthService` verifies password hashes with `IPasswordHasher<User>`. Login accepts normalized username or email, rejects inactive accounts, checks email verification, resolves roles/effective permissions, emits an HMAC-SHA256 JWT with `sub`, `jti`, name identifier/name/email/fullName, one role claim per role and one custom `permission` claim per permission. Expiry is derived from `JwtSettings.ExpiryMinutes` (class default 120; startup configuration shown does not explicitly bind expiry).
- JWT permission set is assembled from role grants plus user-level `IsGranted`/deny overrides. The token is a snapshot: role/permission changes do not affect already issued tokens until renewed/expired.
- Google login uses `Google.Apis.Auth`; email and phone verification/password reset are custom service flows with codes/expiry persisted on the user entity.

### 7.2 Role and permission authorization

Two mechanisms coexist:

1. `[Authorize(Roles = "...")]` for static role allow-lists.
2. `[Authorize(Policy = "Products.Edit")]` / policy constants for permission claims.

`PermissionPolicyProvider` first asks the default provider for a named policy; if no policy exists, it treats the policy name as a permission code and creates a `PermissionRequirement`. `PermissionAuthorizationHandler` succeeds if the current user has a custom `permission` claim matching the code case-insensitively. Seeder-owned codes include product view/create/edit/delete, category management, inventory view/adjust/transfer/batch management, purchasing view/create/approve, order view/create/edit/cancel, customer view/manage/debt, maintenance view/manage, user/role/permission management and reports view.

Roles seeded by the active seeder are Owner/Admin/Moderator/Cashier. Controller role strings also include aliases/casing such as `SuperAdmin`, `STORE_OWNER`, `admin`, `Store POS`, and `ONLINE_MANAGER`; do not remove them without checking deployed tokens/frontends/seed state.

### 7.3 Security findings / non-negotiable precautions

- `appsettings.json` contains a **committed temporary JWT signing key**, marked as migration-only in its value/comment context. Do not repeat its value, use it in real deployment, or treat it as a secure secret. Replace it with a generated secret of at least 32 bytes from a secret store/environment and rotate any environment that used it.
- Active `DataSeeder.cs` contains hard-coded bootstrap credentials for at least two users. Do not reproduce them in docs, logs, or chat; rotate/remove/provision safely. Login requires email verification, while seeded entities should be checked for `IsEmailVerified` because its entity default is false.
- No DB connection string or integration API keys are populated in tracked standard appsettings at this snapshot; startup still requires a DB connection and integrations require their own secret values. Use User Secrets for local development and a production secret manager/environment, never commit credentials.
- CORS policy `PosFrontend` allows any origin with string prefix matching `https://kimo-25.github.io` or `http://localhost`, plus empty origin, and allows credentials/any method/any header. Prefix tests can match hostnames that merely start with the allowed text. If hardening CORS, parse/compare exact origins and test the deployed frontend carefully.
- Invoice PDF route is intentionally anonymous and uses a signed query token. Preserve expiry/signature checks in `InvoiceLinkSigner`; do not make the route public by dropping token validation.
- Payment webhook must verify Paymob HMAC before state changes. Do not trust callback payload status before HMAC verification, and keep updates idempotent.
- Static file serving enables `ServeUnknownFileTypes = true` for both configured providers. Upload validation/content-type handling and static exposure should be considered when modifying file upload behavior.
- `web.config` explicitly enables IIS anonymous access and a wildcard allow rule; API authorization is application-level. Do not mistake IIS auth config for endpoint protection.
- Authentication/authorization middleware order is routing → CORS → authentication → authorization → endpoint mapping (after static files). Maintain the required ordering.

## 8. Important conventions and guidance for AI assistants

### 8.1 Runtime configuration surface

Configuration files should be treated as **schema documentation only**—never copy raw secret values into generated docs. Active startup binds:

| Section / environment key | Consumer / notes |
|---|---|
| `ConnectionStrings:DefaultConnection` or `SQLAZURECONNSTR_DefaultConnection` / `DATABASE_URL` | Required SQL Server connection string; checked at startup. |
| `Jwt:Key` / `Jwt__Key`, `Jwt:Issuer`, `Jwt:Audience` / double-underscore equivalents | Token signing/validation. Key required/minimum 256 bits; issuer/audience have defaults. |
| `Brevo` | Bound to `EmailSettings`; `ApiKey` is expected by email service. |
| `Gemini` | Bound to `GeminiSettings` for generative AI and invoice file processing. |
| `Paymob` | Bound to `PaymobOptions`; checkout/webhook processor. Typed client timeout is 30 seconds. |
| `Translation` | Bound to `TranslationSettings`; typed client base URL defaults to LibreTranslate and uses configured timeout/settings. |
| `PORT` | Optional hosting env var; startup binds `http://*:<PORT>`. |
| `ASPNETCORE_ENVIRONMENT` | Standard environment-specific settings; local launch profiles use Development. |

Options types exist for `DebtReminder`, `InvoicePdf`, and `WhatsApp`, but `Program.cs` does not visibly bind them in its current registrations. Verify before relying on those settings. `appsettings.Production.json` contains Gemini defaults/keys schema (secret currently empty) and logging overrides; Development mainly changes logging.

### 8.2 External integrations and helpers

- **Email:** `Services/Email/EmailService.cs`, `IEmailService`, options bound from `Brevo`; auth/maintenance paths can send verification or status messages.
- **Translation:** `TranslationService` via `HttpClient`; product/category translation services wrap it. Defaults to public LibreTranslate URL if no URL is set; timeout and source-language options exist.
- **Gemini/AI:** `AiAssistantService`, AI tool registry/actions, audit logging, and file processor. `AiController` supports normal and SSE streaming chat. Tool availability is restricted based on role allow-lists and audit logging is persisted.
- **Google identity:** Google token verification in auth service.
- **Paymob:** `PaymobService` and `PaymentsController` implement checkout and HMAC-verified webhook. It uses typed HttpClient.
- **Voice/attendance:** `VoiceVerificationService` uses the configured HuggingFace voice-model endpoint/API settings; voice enrollment and voice check use multipart audio.
- **Invoice documents:** QuestPDF-based invoice PDF service and signed-link helper exist; invoice workflows include WhatsApp/email timestamps and link generation. Confirm DI/options wiring before changes.
- **Excel/document:** ClosedXML/OpenXML utilities support bulk product import and invoice-to-Excel conversion.
- **WhatsApp notifications:** WhatsApp client/composer/sender types exist, with options model, but visible active startup does not register/bind them.
- **Debt reminders:** `DebtReminderJob` exists with Quartz dependencies, but no active scheduler registration/start was found in `Program.cs`; do not assume recurring reminders run automatically.

### 8.3 Local/deployment entrypoints

- Local launch profiles: `http://localhost:5211`, and HTTPS `https://localhost:7223` plus HTTP `5211`, with `ASPNETCORE_ENVIRONMENT=Development`.
- Swagger UI: `/swagger` and OpenAPI JSON at `/swagger/v1/swagger.json`; Bearer JWT security scheme is enabled.
- Dockerfile builds with `mcr.microsoft.com/dotnet/sdk:9.0`, publishes Release, then runs on `mcr.microsoft.com/dotnet/aspnet:9.0`; port 8080 is exposed. Startup also respects `PORT`.
- Middleware includes forwarded headers (clears known networks/proxies), Swagger, HTTPS redirect outside Development, static files and an additional `/uploads` mapping, routing, CORS and JWT auth.
- IIS `web.config` includes SPA fallback rewrite exclusions for `/api` and `/swagger`, IIS in-process hosting, and stdout logging enabled; check deployment environment/log permissions before modifying.

### 8.4 Source-of-truth and edit discipline

#### 8.4.1 Active code and architecture boundaries

1. **Active code is under `Ibn al-Zumar.API/Ibn al-Zumar.API/`.** The sibling directories at `Ibn al-Zumar.API/` contain duplicate/older controllers, a second Program, another DataSeeder/initializer and an older Product entity; the single solution does not include them. Before editing, confirm the target path is inside the active project.
2. `Program.cs` is the composition root. New service registrations/options, middleware, routes, scheduled jobs and clients must be wired there or they are not active.
3. The repository appears mid-refactor and namespace casing is mixed (`IbnAlZumar.Api` vs `IbnAlZumar.API`, and historical `IbnAlZumar.Persistence` namespaces). Do not “normalize” namespaces piecemeal. Check exact declaration/imports and compile after changes.
4. Make database changes in entity/config/context **and** add a migration/snapshot update. EF migrations auto-run on startup in current code (migration errors are swallowed), which has production-impact implications.
5. Avoid exposing entity objects directly from new endpoints. Use typed request/response DTOs and explicit mapping in the local feature style.
6. Respect async APIs and cancellation tokens when the existing service/action exposes them. Use `AsNoTracking` for read-only EF query paths where consistent with neighboring code.
7. Preserve idempotency and transaction semantics for POS/offline sync/payment flows. `ClientUuid` is a database uniqueness boundary, not merely a client field.
8. Preserve soft-delete conventions: query filters hide `BaseEntity.IsDeleted`; use `IgnoreQueryFilters()` only intentionally and explain why. Distinguish hard delete from soft delete when using direct EF removal.
9. Preserve UTC timestamps (`DateTime.UtcNow`) and money precision `(18,2)`. Avoid float/double arithmetic for monetary business calculations.
10. Enums may be string-converted into SQL columns. Keep migration compatibility and frontend JSON expectations in mind when adding/changing enum members.

#### 8.4.2 Naming and code style

- C# types/methods/properties use PascalCase; locals/parameters camelCase; interfaces begin with `I`.
- Feature folders are named by domain (`Services/Sales`, `DTOs/Inventory`, etc.); controllers generally use plural resource names and `[controller]` route tokens, with custom kebab-case routes for new workflow-oriented modules.
- DTO names typically communicate operation (`Create...Dto`, `Update...Dto`, `...ResponseDto`, request DTO). Avoid ambiguous reuse when input and output contracts diverge.
- Entity configuration is predominantly fluent API in `Persistence/Configurations/<feature>` using `IEntityTypeConfiguration<T>`. Keep constraints/indexes/delete behavior there, not scattered across startup.
- Comments/messages may be Arabic, English, or mixed. Keep localized strings intact unless a contract change is intended.
- `BaseEntity` carries common IDs/audit/deletion attributes for many entities; verify whether an entity inherits it before assuming timestamp/filter behavior.

#### 8.4.3 Known repository mismatches / verification checklist

- One solution and one active `.csproj`; no independent test project was found.
- `Mapster` and FluentValidation packages exist but no mapper/validator usage or registration was found. DataAnnotations and manual business validation are the effective patterns.
- EF runtime is 9.x while EF tools/dotnet tool manifest say 10.0.10; FluentValidation packages mix 11.x and 12.x. Reconcile only with a tested dependency update, not as a side-effect of feature work.
- Two pagination DTO definitions and two custom exception families exist.
- Some controllers/services catch custom `NotFoundException`/`BadRequestException` locally; middleware recognizes a different `AppException` hierarchy. Be explicit about the returned status/body path.
- No active Quartz registration despite the job and package; no evident active registration for notification/WhatsApp or invoice PDF services/options. Verify actual constructors if editing these features.
- Expenses controller is a stub returning 501.
- Default CORS uses prefix matching and allows credentials; committed JWT key and fixed bootstrap credentials require remediation before production exposure.
- `Program.cs` mixes namespace casing/import conventions; build was not runnable here because .NET SDK/CLI is absent. **Run `dotnet restore` and `dotnet build Ibn al-Zumar.API/Ibn al-Zumar.API.sln` in a .NET 9 SDK environment before relying on a change.**
- Root README is brief and does not document build/secrets/migration procedures; this file is the richer code-context reference, not a substitute for runtime secrets or deployment docs.

#### 8.4.4 Safe modification workflow

Before implementing a backend feature:

1. Find the active controller/action and confirm its actual route/auth attributes.
2. Trace DTO → service/interface → entity/config/context and identify whether a duplicate exists outside the project.
3. Inspect matching seeder values, enum mapping, global query filters, delete behavior and migration snapshot.
4. Check DI/options registrations and secret/env-variable paths in `Program.cs`; never embed secrets.
5. Preserve response shape, query contract and frontend-facing enum/string casing unless versioning/migration is deliberate.
6. Add/update migration for schema changes; review generated SQL/provider assumptions.
7. Build and run focused API/database checks on a configured SQL Server environment. In the current inspected environment the `dotnet` CLI is unavailable, so no build result should be assumed.
8. Review `git diff` for unintended changes and ensure this context file remains a documentation-only addition unless source changes were requested.
