namespace IbnAlZumar.API.Ai.Tools
{
    /// <summary>
    /// Role name constants used to gate AI tools. These must match the exact role names
    /// your DataSeeder assigns and that show up in the JWT "role" claim — adjust the
    /// string values here (not the tool wiring) if your seeded role names differ.
    /// </summary>
    public static class AiRoles
    {
        public const string Admin = "Admin";
        public const string Moderator = "Moderator";
        public const string Cashier = "Cashier";
        public const string StoreOwner = "STORE_OWNER";
        public const string OnlineManager = "ONLINE_MANAGER";
        public const string Customer = "Customer";

        /// <summary>Everyone allowed to open the AI assistant at all.</summary>
        public static readonly string[] AllStaff = { Admin, Moderator, Cashier, StoreOwner, OnlineManager };

        /// <summary>Roles trusted with operational (non-financial) read tools.</summary>
        public static readonly string[] OperationalRead = { Admin, Moderator, Cashier, StoreOwner, OnlineManager };

        /// <summary>Roles trusted with confidential financial data.</summary>
        public static readonly string[] FinancialRead = { Admin, StoreOwner };

        /// <summary>Roles trusted with write/mutating actions via the assistant.</summary>
        public static readonly string[] SensitiveWrite = { Admin };

        /// <summary>
        /// NEW — roles trusted to create/manage catalog data (products & categories) via the
        /// assistant, including from uploaded invoices/documents. Mirrors the "Products.Create"
        /// permission policy used by ProductsController (per its bulk-import comment, that
        /// policy is granted to Admin and Moderator in the seeder). Adjust if your seeder differs.
        /// </summary>
        public static readonly string[] CatalogWrite = { Admin, Moderator };

        /// <summary>Roles allowed to attach files (invoices/documents/images) to an AI chat turn.</summary>
        public static readonly string[] FileUpload = { Admin, Moderator, Customer };
    }
}