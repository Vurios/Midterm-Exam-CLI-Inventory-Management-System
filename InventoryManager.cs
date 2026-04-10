using System;
using System.Collections.Generic;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem
{
    // Manages all inventory data and operations using in-memory List<T> storage.
    // No database is used — all data lives while the program runs.
    // OOP: encapsulation (private lists), methods, access modifiers, exception handling
    public class InventoryManager
    {
        // Private lists — only accessible through the public methods below
        private List<Product> _products;
        private List<Category> _categories;
        private List<Supplier> _suppliers;
        private List<User> _users;
        private List<TransactionRecord> _transactions;

        // Auto-incrementing ID counters for each entity
        private int _nextProductId;
        private int _nextCategoryId;
        private int _nextSupplierId;
        private int _nextTransactionId;

        // Read-only accessors so Program.cs can iterate lists for display
        public List<Product> Products    { get { return _products; } }
        public List<Category> Categories { get { return _categories; } }
        public List<Supplier> Suppliers  { get { return _suppliers; } }
        public List<TransactionRecord> Transactions { get { return _transactions; } }

        // Constructor — initializes lists, seeds the default admin account,
        // and loads hardcoded sample data so the app is ready to demo on first run.
        public InventoryManager()
        {
            _products     = new List<Product>();
            _categories   = new List<Category>();
            _suppliers    = new List<Supplier>();
            _users        = new List<User>();
            _transactions = new List<TransactionRecord>();

            _nextProductId     = 1;
            _nextCategoryId    = 1;
            _nextSupplierId    = 1;
            _nextTransactionId = 1;

            // Default login credentials: admin / admin123
            _users.Add(new User(1, "admin", "admin123", "Admin"));

            // ── Seed Data ────────────────────────────────────────────────
            SeedCategories();
            SeedSuppliers();
            SeedProducts();
        }

        // ── Seed helpers ─────────────────────────────────────────────────

        private void SeedCategories()
        {
            AddCategory("Electronics",     "Gadgets, appliances, and electronic devices");
            AddCategory("Groceries",       "Everyday food and household consumables");
            AddCategory("School Supplies", "Notebooks, pens, and learning materials");
            AddCategory("Office Supplies", "Stationery and equipment for office use");
            AddCategory("Hardware",        "Tools, fasteners, and construction materials");
        }

        private void SeedSuppliers()
        {
            AddSupplier("TechZone Distributors",  "09171234567", "techzone@mail.ph");
            AddSupplier("Puregold Wholesale",     "09282345678", "wholesale@puregold.ph");
            AddSupplier("National Book Store HQ", "09393456789", "supply@nbs.ph");
            AddSupplier("Ace Hardware Supply",    "09504567890", "orders@acehardware.ph");
        }

        private void SeedProducts()
        {
            // Electronics  (Cat 1, Sup 1)
            AddProduct("USB-C Charging Cable",          149.00m,  80, 15, 1, 1, "admin");
            AddProduct("Wireless Earbuds",             1299.00m,   7, 10, 1, 1, "admin"); // LOW
            AddProduct("Portable Power Bank 10000mAh",  799.00m,  25,  8, 1, 1, "admin");
            AddProduct("LED Desk Lamp",                 549.00m,   4,  5, 1, 1, "admin"); // LOW

            // Groceries  (Cat 2, Sup 2)
            AddProduct("Lucky Me Noodles 12-pack",       89.00m, 150, 30, 2, 2, "admin");
            AddProduct("Sunflower Cooking Oil 1L",       98.00m,  60, 20, 2, 2, "admin");
            AddProduct("Coffee 3-in-1 30 sachets",       75.00m,  22, 25, 2, 2, "admin"); // LOW
            AddProduct("Canned Sardines 555 x6",         95.00m,  90, 20, 2, 2, "admin");

            // School Supplies  (Cat 3, Sup 3)
            AddProduct("Ballpen Black 12 pcs",           48.00m, 200, 40, 3, 3, "admin");
            AddProduct("Intermediate Pad Paper",         29.00m, 180, 40, 3, 3, "admin");
            AddProduct("Scientific Calculator",         349.00m,   3,  5, 3, 3, "admin"); // LOW
            AddProduct("Long Folder 10 pcs",             35.00m, 120, 30, 3, 3, "admin");

            // Office Supplies  (Cat 4, Sup 3)
            AddProduct("Stapler Standard",               89.00m,  35, 10, 4, 3, "admin");
            AddProduct("Bond Paper A4 500 sheets",      225.00m,  70, 15, 4, 3, "admin");

            // Hardware  (Cat 5, Sup 4)
            AddProduct("Flathead Screwdriver Set",      199.00m,  40, 10, 5, 4, "admin");
            AddProduct("Extension Cord 3m",             249.00m,   5,  5, 5, 4, "admin"); // LOW (at threshold)
        }

        // ── Authentication ───────────────────────────────────────────────

        // Returns the matching User or null if credentials are wrong
        public User Login(string username, string password)
        {
            foreach (User u in _users)
            {
                if (u.Authenticate(username, password))
                    return u;
            }
            return null;
        }

        // ── Category CRUD ────────────────────────────────────────────────

        public void AddCategory(string name, string description)
        {
            _categories.Add(new Category(_nextCategoryId++, name, description));
        }

        public Category FindCategory(int id)
        {
            foreach (Category c in _categories)
                if (c.Id == id) return c;
            return null;
        }

        // Returns null on success, or an error message on failure
        public string UpdateCategory(int id, string name, string description)
        {
            Category cat = FindCategory(id);
            if (cat == null) return "Category not found.";
            cat.Name = name;
            cat.Description = description;
            return null;
        }

        // Deletes the category then renumbers all remaining categories sequentially.
        // Products that referenced the old IDs are updated to the new mapped IDs.
        public string DeleteCategory(int id)
        {
            Category cat = FindCategory(id);
            if (cat == null) return "Category not found.";
            _categories.Remove(cat);
            RenumberCategories();
            return null;
        }

        // ── Supplier CRUD ────────────────────────────────────────────────

        public void AddSupplier(string name, string contact, string email)
        {
            _suppliers.Add(new Supplier(_nextSupplierId++, name, contact, email));
        }

        public Supplier FindSupplier(int id)
        {
            foreach (Supplier s in _suppliers)
                if (s.Id == id) return s;
            return null;
        }

        public string UpdateSupplier(int id, string name, string contact, string email)
        {
            Supplier sup = FindSupplier(id);
            if (sup == null) return "Supplier not found.";
            sup.Name = name;
            sup.ContactNumber = contact;
            sup.Email = email;
            return null;
        }

        // Deletes the supplier then renumbers all remaining suppliers sequentially.
        // Products that referenced the old IDs are updated to the new mapped IDs.
        public string DeleteSupplier(int id)
        {
            Supplier sup = FindSupplier(id);
            if (sup == null) return "Supplier not found.";
            _suppliers.Remove(sup);
            RenumberSuppliers();
            return null;
        }

        // ── Product CRUD ─────────────────────────────────────────────────

        // Returns null on success, or an error message on failure
        public string AddProduct(string name, decimal price, int stockQty,
                                 int minStockLevel, int categoryId,
                                 int supplierId, string addedBy)
        {
            if (FindCategory(categoryId) == null)
                return "Category ID does not exist.";
            if (FindSupplier(supplierId) == null)
                return "Supplier ID does not exist.";

            Product p = new Product(_nextProductId++, name, price,
                                    stockQty, minStockLevel, categoryId, supplierId);
            _products.Add(p);

            // Log as InitialStock transaction if an opening quantity was provided
            if (stockQty > 0)
                RecordTransaction(p.Id, p.Name, TransactionKind.InitialStock, stockQty, addedBy);

            return null;
        }

        public Product FindProduct(int id)
        {
            foreach (Product p in _products)
                if (p.Id == id) return p;
            return null;
        }

        // Case-insensitive keyword search across product names
        public List<Product> SearchByName(string keyword)
        {
            List<Product> results = new List<Product>();
            foreach (Product p in _products)
            {
                if (p.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    results.Add(p);
            }
            return results;
        }

        public string UpdateProduct(int id, string name, decimal price,
                                    int minStockLevel, int categoryId, int supplierId)
        {
            Product p = FindProduct(id);
            if (p == null) return "Product not found.";
            if (FindCategory(categoryId) == null) return "Category ID does not exist.";
            if (FindSupplier(supplierId) == null) return "Supplier ID does not exist.";

            p.Name = name;
            p.Price = price;
            p.LowStockThreshold = minStockLevel;
            p.CategoryId = categoryId;
            p.SupplierId = supplierId;
            return null;
        }

        // Deletes the product then renumbers all remaining products sequentially.
        // Transaction records that referenced the old product ID are updated.
        public string DeleteProduct(int id)
        {
            Product p = FindProduct(id);
            if (p == null) return "Product not found.";
            _products.Remove(p);
            RenumberProducts();
            return null;
        }

        // ── Stock Operations ─────────────────────────────────────────────

        public string Restock(int productId, int qty, string performedBy)
        {
            if (qty <= 0) return "Quantity must be greater than zero.";
            Product p = FindProduct(productId);
            if (p == null) return "Product not found.";

            p.StockQty += qty;
            RecordTransaction(productId, p.Name, TransactionKind.Restock, qty, performedBy);
            return null;
        }

        public string DeductStock(int productId, int qty, string performedBy)
        {
            if (qty <= 0) return "Quantity must be greater than zero.";
            Product p = FindProduct(productId);
            if (p == null) return "Product not found.";
            if (p.StockQty < qty) return "Insufficient stock. Current stock: " + p.StockQty;

            p.StockQty -= qty;
            RecordTransaction(productId, p.Name, TransactionKind.Deduction, qty, performedBy);
            return null;
        }

        // ── Reports ──────────────────────────────────────────────────────

        public List<Product> GetLowStockProducts()
        {
            List<Product> low = new List<Product>();
            foreach (Product p in _products)
                if (p.IsLowStock()) low.Add(p);
            return low;
        }

        public decimal GetTotalInventoryValue()
        {
            decimal total = 0m;
            foreach (Product p in _products)
                total += p.ComputeLineValue();
            return total;
        }

        // ── ID Renumbering ───────────────────────────────────────────────
        // After a deletion, IDs are reassigned from 1 upward so there are
        // never any gaps. Related foreign-key references are updated too.

        // Renumbers products 1..N and updates any transaction records
        // that pointed to the old product IDs.
        private void RenumberProducts()
        {
            // Build old-ID → new-ID map before changing anything
            Dictionary<int, int> idMap = new Dictionary<int, int>();
            for (int i = 0; i < _products.Count; i++)
            {
                int oldId = _products[i].Id;
                int newId = i + 1;
                idMap[oldId] = newId;
                _products[i].Id = newId;
            }
            _nextProductId = _products.Count + 1;

            // Update transaction records so history stays consistent
            foreach (TransactionRecord t in _transactions)
            {
                if (idMap.ContainsKey(t.ProductId))
                    t.ProductId = idMap[t.ProductId];
            }
        }

        // Renumbers categories 1..N and updates the CategoryId on every product.
        private void RenumberCategories()
        {
            Dictionary<int, int> idMap = new Dictionary<int, int>();
            for (int i = 0; i < _categories.Count; i++)
            {
                int oldId = _categories[i].Id;
                int newId = i + 1;
                idMap[oldId] = newId;
                _categories[i].Id = newId;
            }
            _nextCategoryId = _categories.Count + 1;

            // Update foreign key on all products
            foreach (Product p in _products)
            {
                if (idMap.ContainsKey(p.CategoryId))
                    p.CategoryId = idMap[p.CategoryId];
            }
        }

        // Renumbers suppliers 1..N and updates the SupplierId on every product.
        private void RenumberSuppliers()
        {
            Dictionary<int, int> idMap = new Dictionary<int, int>();
            for (int i = 0; i < _suppliers.Count; i++)
            {
                int oldId = _suppliers[i].Id;
                int newId = i + 1;
                idMap[oldId] = newId;
                _suppliers[i].Id = newId;
            }
            _nextSupplierId = _suppliers.Count + 1;

            // Update foreign key on all products
            foreach (Product p in _products)
            {
                if (idMap.ContainsKey(p.SupplierId))
                    p.SupplierId = idMap[p.SupplierId];
            }
        }

        // ── Private helper ───────────────────────────────────────────────

        private void RecordTransaction(int productId, string productName,
                                       TransactionKind kind, int qty, string user)
        {
            _transactions.Add(new TransactionRecord(
                _nextTransactionId++, productId, productName, kind, qty, user));
        }
    }
}
