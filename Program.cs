using System;
using System.Collections.Generic;
using InventoryManagementSystem.Models;

/*
 * Program.cs — Entry point and all console UI logic
 *
 * OOP concepts demonstrated here:
 *   • Objects          — InventoryManager, User, Product, Category, Supplier instances
 *   • Methods          — each menu action is its own static method (single responsibility)
 *   • Encapsulation    — all Console I/O goes through Ask / ShowError / ShowSuccess helpers
 *   • Access modifiers — helpers are private static; only Main is the public entry point
 *   • Exception handling — every data-entry screen uses try-catch; GoBackException signals cancel
 */

namespace InventoryManagementSystem
{
    // Thrown by Ask* helpers when the user enters "0" to cancel and go back.
    // Caught by each menu method's catch block instead of checking return values.
    class GoBackException : Exception { }

    class Program
    {
        static InventoryManager manager = new InventoryManager();
        static User currentUser = null;

        const int LINE_WIDTH = 76;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            if (LoginScreen())
                MainMenu();
            else
            {
                ShowError("Too many failed attempts. Exiting.");
                Console.ReadLine();
            }
        }

        // ── Login ────────────────────────────────────────────────────────

        static bool LoginScreen()
        {
            int attempts = 0;
            const int maxAttempts = 3;

            while (attempts < maxAttempts)
            {
                Console.Clear();
                PrintHeader("INVENTORY MANAGEMENT SYSTEM");
                Console.WriteLine("  Default credentials: admin / admin123");
                Console.WriteLine("  Attempt " + (attempts + 1) + " of " + maxAttempts);
                PrintLine();

                try
                {
                    string username = Ask("Username");
                    string password = Ask("Password");

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        ShowError("Username and password cannot be empty.");
                        Pause();
                        attempts++;
                        continue;
                    }

                    User user = manager.Login(username, password);
                    if (user != null)
                    {
                        currentUser = user;
                        ShowSuccess("Login successful! Welcome, " + user.Username + ".");
                        Pause();
                        return true;
                    }

                    ShowError("Invalid username or password.");
                    Pause();
                    attempts++;
                }
                catch (Exception ex)
                {
                    ShowError("Unexpected error: " + ex.Message);
                    Pause();
                    attempts++;
                }
            }

            return false;
        }

        // ── Main Menu ─────────────────────────────────────────────────────

        static void MainMenu()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                PrintHeader("MAIN MENU");
                Console.WriteLine("  Logged in as: " + currentUser +
                                  "   |   " + DateTime.Now.ToString("MMM dd, yyyy  hh:mm tt"));
                PrintLine();
                Console.WriteLine("  [1]  Product Management");
                Console.WriteLine("  [2]  Category Management");
                Console.WriteLine("  [3]  Supplier Management");
                Console.WriteLine("  [4]  Stock Operations");
                Console.WriteLine("  [5]  Reports");
                PrintLine();
                Console.WriteLine("  [0]  Logout");
                Console.WriteLine();

                string choice = Ask("Select an option");
                switch (choice)
                {
                    case "1": ProductMenu();  break;
                    case "2": CategoryMenu(); break;
                    case "3": SupplierMenu(); break;
                    case "4": StockMenu();    break;
                    case "5": ReportsMenu();  break;
                    case "0":
                        ShowSuccess("Logged out successfully. Goodbye!");
                        running = false;
                        break;
                    default:
                        ShowError("Invalid choice. Please enter a number from the menu.");
                        Pause();
                        break;
                }
            }
        }

        // ── Product Menu ──────────────────────────────────────────────────

        static void ProductMenu()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                PrintHeader("PRODUCT MANAGEMENT");
                Console.WriteLine("  [1]  Add Product");
                Console.WriteLine("  [2]  View All Products");
                Console.WriteLine("  [3]  Search Product");
                Console.WriteLine("  [4]  Update Product");
                Console.WriteLine("  [5]  Delete Product");
                PrintLine();
                Console.WriteLine("  [0]  Back to Main Menu");
                Console.WriteLine();

                string choice = Ask("Select an option");
                switch (choice)
                {
                    case "1": AddProduct();    break;
                    case "2": ViewProducts();  break;
                    case "3": SearchProduct(); break;
                    case "4": UpdateProduct(); break;
                    case "5": DeleteProduct(); break;
                    case "0": running = false; break;
                    default:
                        ShowError("Invalid choice.");
                        Pause();
                        break;
                }
            }
        }

        static void AddProduct()
        {
            Console.Clear();
            PrintHeader("ADD PRODUCT");
            Console.WriteLine("  [0] at any prompt to go back to Product Management");
            PrintLine();

            if (manager.Categories.Count == 0 || manager.Suppliers.Count == 0)
            {
                ShowError("You need at least one category and one supplier before adding a product.");
                Pause();
                return;
            }

            PrintCategoryList();
            PrintSupplierList();

            try
            {
                string name    = AskName("Product name");
                decimal price  = AskPositiveDecimal("Price (P)");
                int stock      = AskNonNegativeInt("Initial stock quantity");
                int minLevel   = AskMinStockLevel("Min. stock level", stock);
                int catId      = AskExistingCategoryId("Category ID");
                int supId      = AskExistingSupplierID("Supplier ID");

                string err = manager.AddProduct(name, price, stock, minLevel, catId, supId,
                                                currentUser.Username);
                if (err != null)
                    ShowError(err);
                else
                    ShowSuccess("Product \"" + name + "\" added successfully.");

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Product Management...");
                Pause();
            }
            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                Pause();
            }
        }

        static void ViewProducts()
        {
            Console.Clear();
            PrintHeader("ALL PRODUCTS");

            if (manager.Products.Count == 0)
            {
                Console.WriteLine("  No products found.");
                Pause();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("  {0,-4} {1,-26} {2,10} {3,6} {4,9} {5,4} {6,4}",
                "ID", "Name", "Price (P)", "Stock", "Min.Lvl", "Cat", "Sup");
            PrintLine();

            foreach (Product p in manager.Products)
            {
                string flag = p.IsLowStock() ? "  [LOW]" : "";
                Console.WriteLine("  {0,-4} {1,-26} {2,10:F2} {3,6} {4,9} {5,4} {6,4}{7}",
                    p.Id, TruncateName(p.Name, 26), p.Price, p.StockQty,
                    p.LowStockThreshold, p.CategoryId, p.SupplierId, flag);
            }

            PrintLine();
            Console.WriteLine("  Total: " + manager.Products.Count +
                              " product(s)     [LOW] = at or below min. stock level");
            Pause();
        }

        static void SearchProduct()
        {
            Console.Clear();
            PrintHeader("SEARCH PRODUCT");
            Console.WriteLine("  [0] at any prompt to go back to Product Management");
            PrintLine();

            try
            {
                string keyword = AskName("Enter keyword");

                List<Product> results = manager.SearchByName(keyword);

                Console.WriteLine();
                if (results.Count == 0)
                {
                    Console.WriteLine("  No products matched \"" + keyword + "\".");
                }
                else
                {
                    Console.WriteLine("  {0,-4} {1,-26} {2,10} {3,6}", "ID", "Name", "Price (P)", "Stock");
                    PrintLine();
                    foreach (Product p in results)
                        Console.WriteLine("  {0,-4} {1,-26} {2,10:F2} {3,6}",
                            p.Id, TruncateName(p.Name, 26), p.Price, p.StockQty);
                    PrintLine();
                    Console.WriteLine("  " + results.Count + " result(s) found.");
                }

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Product Management...");
                Pause();
            }
        }

        static void UpdateProduct()
        {
            Console.Clear();
            PrintHeader("UPDATE PRODUCT");
            Console.WriteLine("  [0] at any prompt to go back to Product Management");
            PrintLine();

            if (manager.Products.Count == 0)
            {
                Console.WriteLine("  No products to update.");
                Pause();
                return;
            }

            PrintProductSummary();
            PrintCategoryList();
            PrintSupplierList();

            try
            {
                int id     = AskExistingProductId("Product ID to update");
                Product p  = manager.FindProduct(id);

                Console.WriteLine();
                Console.WriteLine("  Editing: [" + p.Id + "] " + p.Name);
                Console.WriteLine("  (Press Enter to keep the current value)");
                Console.WriteLine();

                string name  = AskOptionalName("Name [" + p.Name + "]");
                if (string.IsNullOrWhiteSpace(name)) name = p.Name;

                decimal price   = AskOptionalPositiveDecimal("Price (P) [" + p.Price.ToString("F2") + "]", p.Price);
                int minLevel    = AskOptionalMinStockLevel("Min. stock level [" + p.LowStockThreshold + "]", p.LowStockThreshold, p.StockQty);
                int catId       = AskOptionalExistingCategoryId("Category ID [" + p.CategoryId + "]", p.CategoryId);
                int supId       = AskOptionalExistingSupplierID("Supplier ID [" + p.SupplierId + "]", p.SupplierId);

                string err = manager.UpdateProduct(id, name, price, minLevel, catId, supId);
                if (err != null) ShowError(err);
                else ShowSuccess("Product updated successfully.");

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Product Management...");
                Pause();
            }
            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                Pause();
            }
        }

        static void DeleteProduct()
        {
            Console.Clear();
            PrintHeader("DELETE PRODUCT");
            Console.WriteLine("  [0] at any prompt to go back to Product Management");
            PrintLine();

            if (manager.Products.Count == 0)
            {
                Console.WriteLine("  No products to delete.");
                Pause();
                return;
            }

            PrintProductSummary();

            try
            {
                int id    = AskExistingProductId("Product ID to delete");
                Product p = manager.FindProduct(id);

                Console.WriteLine();
                Console.WriteLine("  About to delete: [" + p.Id + "] " + p.Name);
                string confirm = AskYesNo("Confirm deletion? (y/n)");
                if (confirm != "y")
                {
                    Console.WriteLine("  Deletion cancelled.");
                    Pause();
                    return;
                }

                string err = manager.DeleteProduct(id);
                if (err != null) ShowError(err);
                else ShowSuccess("Product \"" + p.Name + "\" deleted.");

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Product Management...");
                Pause();
            }
            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                Pause();
            }
        }

        // ── Category Menu ─────────────────────────────────────────────────

        static void CategoryMenu()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                PrintHeader("CATEGORY MANAGEMENT");
                Console.WriteLine("  [1]  Add Category");
                Console.WriteLine("  [2]  View All Categories");
                Console.WriteLine("  [3]  Update Category");
                Console.WriteLine("  [4]  Delete Category");
                PrintLine();
                Console.WriteLine("  [0]  Back to Main Menu");
                Console.WriteLine();

                string choice = Ask("Select an option");
                switch (choice)
                {
                    case "1": AddCategory();    break;
                    case "2": ViewCategories(); break;
                    case "3": UpdateCategory(); break;
                    case "4": DeleteCategory(); break;
                    case "0": running = false;  break;
                    default: ShowError("Invalid choice."); Pause(); break;
                }
            }
        }

        static void AddCategory()
        {
            Console.Clear();
            PrintHeader("ADD CATEGORY");
            Console.WriteLine("  [0] at any prompt to go back to Category Management");
            PrintLine();

            try
            {
                string name = AskName("Category name");
                string desc = AskName("Description");

                manager.AddCategory(name, desc);
                ShowSuccess("Category \"" + name + "\" added.");

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Category Management...");
                Pause();
            }
            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                Pause();
            }
        }

        static void ViewCategories()
        {
            Console.Clear();
            PrintHeader("ALL CATEGORIES");

            if (manager.Categories.Count == 0)
            {
                Console.WriteLine("  No categories found.");
                Pause();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("  {0,-4} {1,-22} {2,-35}", "ID", "Name", "Description");
            PrintLine();
            foreach (Category c in manager.Categories)
                Console.WriteLine("  {0,-4} {1,-22} {2,-35}", c.Id, c.Name, c.Description);
            PrintLine();
            Console.WriteLine("  Total: " + manager.Categories.Count + " category(ies)");
            Pause();
        }

        static void UpdateCategory()
        {
            Console.Clear();
            PrintHeader("UPDATE CATEGORY");
            Console.WriteLine("  [0] at any prompt to go back to Category Management");
            PrintLine();

            if (manager.Categories.Count == 0)
            {
                Console.WriteLine("  No categories to update.");
                Pause();
                return;
            }

            PrintCategoryList();

            try
            {
                int id       = AskExistingCategoryId("Category ID to update");
                Category cat = manager.FindCategory(id);

                Console.WriteLine();
                Console.WriteLine("  (Press Enter to keep the current value)");
                Console.WriteLine();

                string name = AskOptionalName("Name [" + cat.Name + "]");
                if (string.IsNullOrWhiteSpace(name)) name = cat.Name;

                string desc = AskOptionalName("Description [" + cat.Description + "]");
                if (string.IsNullOrWhiteSpace(desc)) desc = cat.Description;

                string err = manager.UpdateCategory(id, name, desc);
                if (err != null) ShowError(err);
                else ShowSuccess("Category updated.");

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Category Management...");
                Pause();
            }
            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                Pause();
            }
        }

        static void DeleteCategory()
        {
            Console.Clear();
            PrintHeader("DELETE CATEGORY");
            Console.WriteLine("  [0] at any prompt to go back to Category Management");
            PrintLine();

            if (manager.Categories.Count == 0)
            {
                Console.WriteLine("  No categories to delete.");
                Pause();
                return;
            }

            PrintCategoryList();

            try
            {
                int id       = AskExistingCategoryId("Category ID to delete");
                Category cat = manager.FindCategory(id);

                Console.WriteLine();
                Console.WriteLine("  About to delete: " + cat);
                string confirm = AskYesNo("Confirm? (y/n)");
                if (confirm != "y") { Console.WriteLine("  Cancelled."); Pause(); return; }

                string err = manager.DeleteCategory(id);
                if (err != null) ShowError(err);
                else ShowSuccess("Category deleted.");

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Category Management...");
                Pause();
            }
            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                Pause();
            }
        }

        // ── Supplier Menu ─────────────────────────────────────────────────

        static void SupplierMenu()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                PrintHeader("SUPPLIER MANAGEMENT");
                Console.WriteLine("  [1]  Add Supplier");
                Console.WriteLine("  [2]  View All Suppliers");
                Console.WriteLine("  [3]  Update Supplier");
                Console.WriteLine("  [4]  Delete Supplier");
                PrintLine();
                Console.WriteLine("  [0]  Back to Main Menu");
                Console.WriteLine();

                string choice = Ask("Select an option");
                switch (choice)
                {
                    case "1": AddSupplier();    break;
                    case "2": ViewSuppliers();  break;
                    case "3": UpdateSupplier(); break;
                    case "4": DeleteSupplier(); break;
                    case "0": running = false;  break;
                    default: ShowError("Invalid choice."); Pause(); break;
                }
            }
        }

        static void AddSupplier()
        {
            Console.Clear();
            PrintHeader("ADD SUPPLIER");
            Console.WriteLine("  [0] at any prompt to go back to Supplier Management");
            PrintLine();

            try
            {
                string name    = AskName("Supplier name");
                string contact = AskPhone("Contact number (e.g. 09171234567)");
                string email   = AskEmail("Email address");

                manager.AddSupplier(name, contact, email);
                ShowSuccess("Supplier \"" + name + "\" added.");

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Supplier Management...");
                Pause();
            }
            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                Pause();
            }
        }

        static void ViewSuppliers()
        {
            Console.Clear();
            PrintHeader("ALL SUPPLIERS");

            if (manager.Suppliers.Count == 0)
            {
                Console.WriteLine("  No suppliers found.");
                Pause();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("  {0,-4} {1,-24} {2,-14} {3,-28}", "ID", "Name", "Contact", "Email");
            PrintLine();
            foreach (Supplier s in manager.Suppliers)
                Console.WriteLine("  {0,-4} {1,-24} {2,-14} {3,-28}",
                    s.Id, s.Name, s.ContactNumber, s.Email);
            PrintLine();
            Console.WriteLine("  Total: " + manager.Suppliers.Count + " supplier(s)");
            Pause();
        }

        static void UpdateSupplier()
        {
            Console.Clear();
            PrintHeader("UPDATE SUPPLIER");
            Console.WriteLine("  [0] at any prompt to go back to Supplier Management");
            PrintLine();

            if (manager.Suppliers.Count == 0)
            {
                Console.WriteLine("  No suppliers to update.");
                Pause();
                return;
            }

            PrintSupplierList();

            try
            {
                int id       = AskExistingSupplierID("Supplier ID to update");
                Supplier sup = manager.FindSupplier(id);

                Console.WriteLine();
                Console.WriteLine("  (Press Enter to keep the current value)");
                Console.WriteLine();

                string name    = AskOptionalName("Name [" + sup.Name + "]");
                if (string.IsNullOrWhiteSpace(name)) name = sup.Name;

                string contact = AskOptionalPhone("Contact [" + sup.ContactNumber + "]", sup.ContactNumber);
                string email   = AskOptionalEmail("Email [" + sup.Email + "]", sup.Email);

                string err = manager.UpdateSupplier(id, name, contact, email);
                if (err != null) ShowError(err);
                else ShowSuccess("Supplier updated.");

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Supplier Management...");
                Pause();
            }
            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                Pause();
            }
        }

        static void DeleteSupplier()
        {
            Console.Clear();
            PrintHeader("DELETE SUPPLIER");
            Console.WriteLine("  [0] at any prompt to go back to Supplier Management");
            PrintLine();

            if (manager.Suppliers.Count == 0)
            {
                Console.WriteLine("  No suppliers to delete.");
                Pause();
                return;
            }

            PrintSupplierList();

            try
            {
                int id       = AskExistingSupplierID("Supplier ID to delete");
                Supplier sup = manager.FindSupplier(id);

                Console.WriteLine();
                Console.WriteLine("  About to delete: " + sup);
                string confirm = AskYesNo("Confirm? (y/n)");
                if (confirm != "y") { Console.WriteLine("  Cancelled."); Pause(); return; }

                string err = manager.DeleteSupplier(id);
                if (err != null) ShowError(err);
                else ShowSuccess("Supplier deleted.");

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Supplier Management...");
                Pause();
            }
            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                Pause();
            }
        }

        // ── Stock Operations Menu ─────────────────────────────────────────

        static void StockMenu()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                PrintHeader("STOCK OPERATIONS");
                Console.WriteLine("  [1]  Restock Product");
                Console.WriteLine("  [2]  Deduct Stock");
                PrintLine();
                Console.WriteLine("  [0]  Back to Main Menu");
                Console.WriteLine();

                string choice = Ask("Select an option");
                switch (choice)
                {
                    case "1": RestockProduct(); break;
                    case "2": DeductProduct();  break;
                    case "0": running = false;  break;
                    default: ShowError("Invalid choice."); Pause(); break;
                }
            }
        }

        static void RestockProduct()
        {
            Console.Clear();
            PrintHeader("RESTOCK PRODUCT");
            Console.WriteLine("  [0] at any prompt to go back to Stock Operations");
            PrintLine();

            if (manager.Products.Count == 0)
            {
                Console.WriteLine("  No products available.");
                Pause();
                return;
            }

            PrintProductSummary();

            try
            {
                int id    = AskExistingProductId("Product ID to restock");
                Product p = manager.FindProduct(id);

                Console.WriteLine("  Current stock for \"" + p.Name + "\": " + p.StockQty + " unit(s)");
                int qty = AskPositiveInt("Quantity to add");

                string err = manager.Restock(id, qty, currentUser.Username);
                if (err != null)
                    ShowError(err);
                else
                    ShowSuccess("Restocked " + qty + " unit(s). New stock: " + manager.FindProduct(id).StockQty);

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Stock Operations...");
                Pause();
            }
            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                Pause();
            }
        }

        static void DeductProduct()
        {
            Console.Clear();
            PrintHeader("DEDUCT STOCK");
            Console.WriteLine("  [0] at any prompt to go back to Stock Operations");
            PrintLine();

            if (manager.Products.Count == 0)
            {
                Console.WriteLine("  No products available.");
                Pause();
                return;
            }

            PrintProductSummary();

            try
            {
                int id    = AskExistingProductId("Product ID to deduct from");
                Product p = manager.FindProduct(id);

                Console.WriteLine("  Current stock for \"" + p.Name + "\": " + p.StockQty + " unit(s)");
                int qty = AskPositiveInt("Quantity to deduct");

                string err = manager.DeductStock(id, qty, currentUser.Username);
                if (err != null)
                {
                    ShowError(err);
                }
                else
                {
                    Product updated = manager.FindProduct(id);
                    ShowSuccess("Deducted " + qty + " unit(s). New stock: " + updated.StockQty);
                    if (updated.IsLowStock())
                        Console.WriteLine("\n  WARNING: Stock is now at or below the min. stock level (" +
                                          updated.LowStockThreshold + "). Consider restocking soon.");
                }

                Pause();
            }
            catch (GoBackException)
            {
                Console.WriteLine("  Going back to Stock Operations...");
                Pause();
            }
            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                Pause();
            }
        }

        // ── Reports Menu ──────────────────────────────────────────────────

        static void ReportsMenu()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                PrintHeader("REPORTS");
                Console.WriteLine("  [1]  Transaction History");
                Console.WriteLine("  [2]  Low-Stock Items");
                Console.WriteLine("  [3]  Total Inventory Value");
                PrintLine();
                Console.WriteLine("  [0]  Back to Main Menu");
                Console.WriteLine();

                string choice = Ask("Select an option");
                switch (choice)
                {
                    case "1": ViewTransactions();     break;
                    case "2": LowStockReport();       break;
                    case "3": InventoryValueReport(); break;
                    case "0": running = false;        break;
                    default: ShowError("Invalid choice."); Pause(); break;
                }
            }
        }

        static void ViewTransactions()
        {
            Console.Clear();
            PrintHeader("TRANSACTION HISTORY");

            if (manager.Transactions.Count == 0)
            {
                Console.WriteLine("  No transactions recorded yet.");
                Pause();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("  {0,-5} {1,-22} {2,-14} {3,5}  {4,-17} {5,-10}",
                "ID", "Product", "Type", "Qty", "Date & Time", "User");
            PrintLine();

            foreach (TransactionRecord t in manager.Transactions)
                Console.WriteLine("  {0,-5} {1,-22} {2,-14} {3,5}  {4,-17} {5,-10}",
                    t.Id, TruncateName(t.ProductName, 22), t.Kind.ToString(), t.Quantity,
                    t.Timestamp.ToString("MM/dd/yyyy HH:mm"), t.RecordedBy);

            PrintLine();
            Console.WriteLine("  Total transactions: " + manager.Transactions.Count);
            Pause();
        }

        static void LowStockReport()
        {
            Console.Clear();
            PrintHeader("LOW-STOCK ITEMS");

            List<Product> lowStock = manager.GetLowStockProducts();

            if (lowStock.Count == 0)
            {
                ShowSuccess("All products are adequately stocked. No action needed.");
                Pause();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("  {0,-4} {1,-40} {2,10} {3,17}", "ID", "Name", "Curr. Stock", "Min. Stock");
            PrintLine();

            foreach (Product p in lowStock)
                Console.WriteLine("  {0,-4} {1,-40} {2,10} {3,17}",
                    p.Id, TruncateName(p.Name, 40), p.StockQty, p.LowStockThreshold);

            PrintLine();
            Console.WriteLine("  " + lowStock.Count + " product(s) need restocking.");
            Pause();
        }

        static void InventoryValueReport()
        {
            Console.Clear();
            PrintHeader("TOTAL INVENTORY VALUE");
            Console.WriteLine();

            if (manager.Products.Count == 0)
            {
                Console.WriteLine("  No products in inventory.");
                Pause();
                return;
            }

            Console.WriteLine("  {0,-4} {1,-26} {2,10} {3,6} {4,14}",
                "ID", "Name", "Price (P)", "Stock", "Line Value (P)");
            PrintLine();

            foreach (Product p in manager.Products)
                Console.WriteLine("  {0,-4} {1,-26} {2,10:F2} {3,6} {4,14:F2}",
                    p.Id, TruncateName(p.Name, 26), p.Price, p.StockQty, p.ComputeLineValue());

            PrintLine();
            Console.WriteLine("  TOTAL INVENTORY VALUE: P " +
                              manager.GetTotalInventoryValue().ToString("N2"));
            Pause();
        }

        // ── Validated Ask Helpers ─────────────────────────────────────────
        // All helpers throw GoBackException when the user enters "0"

        static string AskName(string prompt)
        {
            while (true)
            {
                string val = Ask(prompt);
                if (val == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(val))
                { ShowError("This field cannot be empty."); continue; }
                if (!IsValidName(val))
                { ShowError("Invalid characters. Use letters, digits, spaces, or common punctuation ( - . , ' ( ) & / # )."); continue; }
                return val;
            }
        }

        static string AskOptionalName(string prompt)
        {
            while (true)
            {
                string val = Ask(prompt);
                if (val == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(val)) return val;
                if (!IsValidName(val))
                { ShowError("Invalid characters. Use letters, digits, spaces, or common punctuation ( - . , ' ( ) & / # )."); continue; }
                return val;
            }
        }

        static decimal AskPositiveDecimal(string prompt)
        {
            while (true)
            {
                string raw = Ask(prompt);
                if (raw == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(raw))
                { ShowError("This field cannot be empty."); continue; }
                if (!IsValidDecimalInput(raw))
                { ShowError("Enter a valid amount using digits only (e.g. 149 or 149.99)."); continue; }
                decimal val = decimal.Parse(raw);
                if (val <= 0)
                { ShowError("Amount must be greater than zero."); continue; }
                return val;
            }
        }

        static decimal AskOptionalPositiveDecimal(string prompt, decimal fallback)
        {
            while (true)
            {
                string raw = Ask(prompt);
                if (raw == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(raw)) return fallback;
                if (!IsValidDecimalInput(raw))
                { ShowError("Enter a valid amount using digits only (e.g. 149 or 149.99)."); continue; }
                decimal val = decimal.Parse(raw);
                if (val <= 0)
                { ShowError("Amount must be greater than zero."); continue; }
                return val;
            }
        }

        static int AskNonNegativeInt(string prompt)
        {
            while (true)
            {
                string raw = Ask(prompt);
                if (raw == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(raw))
                { ShowError("This field cannot be empty."); continue; }
                if (!IsDigitsOnly(raw))
                { ShowError("Enter a whole number using digits only (e.g. 50)."); continue; }
                return int.Parse(raw);
            }
        }

        static int AskPositiveInt(string prompt)
        {
            while (true)
            {
                string raw = Ask(prompt);
                if (raw == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(raw))
                { ShowError("This field cannot be empty."); continue; }
                if (!IsDigitsOnly(raw))
                { ShowError("Enter a whole number using digits only (e.g. 10)."); continue; }
                int val = int.Parse(raw);
                if (val <= 0)
                { ShowError("Quantity must be greater than zero."); continue; }
                return val;
            }
        }

        static int AskMinStockLevel(string prompt, int stockQty)
        {
            while (true)
            {
                string raw = Ask(prompt);
                if (raw == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(raw))
                { ShowError("This field cannot be empty."); continue; }
                if (!IsDigitsOnly(raw))
                { ShowError("Enter a whole number using digits only."); continue; }
                int val = int.Parse(raw);
                if (stockQty > 0 && val >= stockQty)
                { ShowError("Min. stock level (" + val + ") must be less than the initial stock (" + stockQty + ")."); continue; }
                return val;
            }
        }

        static int AskOptionalMinStockLevel(string prompt, int fallback, int stockQty)
        {
            while (true)
            {
                string raw = Ask(prompt);
                if (raw == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(raw)) return fallback;
                if (!IsDigitsOnly(raw))
                { ShowError("Enter a whole number using digits only."); continue; }
                int val = int.Parse(raw);
                if (stockQty > 0 && val >= stockQty)
                { ShowError("Min. stock level (" + val + ") must be less than current stock (" + stockQty + ")."); continue; }
                return val;
            }
        }

        static string AskPhone(string prompt)
        {
            while (true)
            {
                string val = Ask(prompt);
                if (val == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(val))
                { ShowError("Contact number cannot be empty."); continue; }
                if (!IsValidPhoneNumber(val))
                { ShowError("Invalid contact number. Use 11 digits starting with 09 (e.g. 09171234567)."); continue; }
                return val;
            }
        }

        static string AskOptionalPhone(string prompt, string fallback)
        {
            while (true)
            {
                string val = Ask(prompt);
                if (val == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(val)) return fallback;
                if (!IsValidPhoneNumber(val))
                { ShowError("Invalid contact number. Use 11 digits starting with 09 (e.g. 09171234567)."); continue; }
                return val;
            }
        }

        static string AskEmail(string prompt)
        {
            while (true)
            {
                string val = Ask(prompt);
                if (val == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(val))
                { ShowError("Email address cannot be empty."); continue; }
                if (!IsValidEmail(val))
                { ShowError("Invalid email. Must contain '@' and a domain (e.g. name@mail.com)."); continue; }
                return val;
            }
        }

        static string AskOptionalEmail(string prompt, string fallback)
        {
            while (true)
            {
                string val = Ask(prompt);
                if (val == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(val)) return fallback;
                if (!IsValidEmail(val))
                { ShowError("Invalid email. Must contain '@' and a domain (e.g. name@mail.com)."); continue; }
                return val;
            }
        }

        static int AskExistingCategoryId(string prompt)
        {
            while (true)
            {
                string raw = Ask(prompt);
                if (raw == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(raw))
                { ShowError("Category ID cannot be empty."); continue; }
                if (!IsDigitsOnly(raw))
                { ShowError("Category ID must be digits only."); continue; }
                int id = int.Parse(raw);
                if (manager.FindCategory(id) == null)
                { ShowError("No category found with ID " + id + ". Check the list above."); continue; }
                return id;
            }
        }

        static int AskOptionalExistingCategoryId(string prompt, int fallback)
        {
            while (true)
            {
                string raw = Ask(prompt);
                if (raw == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(raw)) return fallback;
                if (!IsDigitsOnly(raw))
                { ShowError("Category ID must be digits only."); continue; }
                int id = int.Parse(raw);
                if (manager.FindCategory(id) == null)
                { ShowError("No category found with ID " + id + ". Check the list above."); continue; }
                return id;
            }
        }

        static int AskExistingSupplierID(string prompt)
        {
            while (true)
            {
                string raw = Ask(prompt);
                if (raw == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(raw))
                { ShowError("Supplier ID cannot be empty."); continue; }
                if (!IsDigitsOnly(raw))
                { ShowError("Supplier ID must be digits only."); continue; }
                int id = int.Parse(raw);
                if (manager.FindSupplier(id) == null)
                { ShowError("No supplier found with ID " + id + ". Check the list above."); continue; }
                return id;
            }
        }

        static int AskOptionalExistingSupplierID(string prompt, int fallback)
        {
            while (true)
            {
                string raw = Ask(prompt);
                if (raw == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(raw)) return fallback;
                if (!IsDigitsOnly(raw))
                { ShowError("Supplier ID must be digits only."); continue; }
                int id = int.Parse(raw);
                if (manager.FindSupplier(id) == null)
                { ShowError("No supplier found with ID " + id + ". Check the list above."); continue; }
                return id;
            }
        }

        static int AskExistingProductId(string prompt)
        {
            while (true)
            {
                string raw = Ask(prompt);
                if (raw == "0") throw new GoBackException();
                if (string.IsNullOrWhiteSpace(raw))
                { ShowError("Product ID cannot be empty."); continue; }
                if (!IsDigitsOnly(raw))
                { ShowError("Product ID must be digits only."); continue; }
                int id = int.Parse(raw);
                if (manager.FindProduct(id) == null)
                { ShowError("No product found with ID " + id + ". Check the list above."); continue; }
                return id;
            }
        }

        static string AskYesNo(string prompt)
        {
            while (true)
            {
                string val = Ask(prompt).ToLower();
                if (val == "0") throw new GoBackException();
                if (val == "y" || val == "n") return val;
                ShowError("Please enter 'y' for yes or 'n' for no.");
            }
        }

        // ── Format Validation Helpers ─────────────────────────────────────

        // Allows letters, digits, spaces, and common punctuation: - . , ' ( ) & / #
        static bool IsValidName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            foreach (char c in s)
            {
                if (char.IsLetterOrDigit(c)) continue;
                if (c == ' ' || c == '-' || c == '.' || c == ',' ||
                    c == '\'' || c == '(' || c == ')' || c == '&' ||
                    c == '/'  || c == '#') continue;
                return false;
            }
            return s.Trim().Length > 0;
        }

        // Returns true only if every character is an ASCII digit (0-9).
        static bool IsDigitsOnly(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            foreach (char c in s)
                if (!char.IsDigit(c)) return false;
            return true;
        }

        // Accepts unsigned decimals (e.g. "149" or "149.99") — digits and at most one dot.
        static bool IsValidDecimalInput(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            int dotCount = 0;
            foreach (char c in s)
            {
                if (c == '.')
                {
                    dotCount++;
                    if (dotCount > 1) return false; // more than one decimal point
                }
                else if (!char.IsDigit(c)) return false;
            }
            return s != "."; // bare "." is not a valid number
        }

        // Accepts PH mobile numbers: 11-digit "09..." or 10-digit "9...", with optional +63 prefix.
        static bool IsValidPhoneNumber(string number)
        {
            string digits = number.Trim();
            if (digits.StartsWith("+63")) // normalize +63 → 0
                digits = "0" + digits.Substring(3);
            if (!IsDigitsOnly(digits)) return false;
            if (digits.Length == 11 && digits.StartsWith("09")) return true;
            if (digits.Length == 10 && digits.StartsWith("0"))  return true;
            return false;
        }

        // Checks for exactly one '@' (not at start) and a domain with a dot.
        static bool IsValidEmail(string email)
        {
            string e = email.Trim();
            int atIdx = e.IndexOf('@');
            if (atIdx <= 0) return false;                        // '@' missing or at position 0
            if (atIdx != e.LastIndexOf('@')) return false;       // more than one '@'
            string domain = e.Substring(atIdx + 1);
            if (domain.Length < 3) return false;                 // domain too short
            if (!domain.Contains('.')) return false;             // no dot in domain
            if (domain.StartsWith(".") || domain.EndsWith(".")) return false;
            return true;
        }

        // ── UI Helpers ────────────────────────────────────────────────────

        static void PrintHeader(string title)
        {
            Console.WriteLine();
            Console.WriteLine("  " + new string('=', LINE_WIDTH));
            Console.WriteLine("  " + title);
            Console.WriteLine("  " + new string('=', LINE_WIDTH));
        }

        static void PrintLine()
        {
            Console.WriteLine("  " + new string('-', LINE_WIDTH));
        }

        static void ShowSuccess(string msg)
        {
            Console.WriteLine();
            Console.WriteLine("  " + msg);
        }

        static void ShowError(string msg)
        {
            Console.WriteLine("  ERROR: " + msg);
        }

        static string Ask(string prompt)
        {
            Console.Write("  " + prompt + ": ");
            return (Console.ReadLine() ?? "").Trim();
        }

        static void Pause()
        {
            Console.WriteLine();
            Console.Write("  Press Enter to continue...");
            Console.ReadLine();
        }

        static string TruncateName(string s, int max)
        {
            if (s.Length <= max) return s;
            return s.Substring(0, max - 1) + "~";
        }

        static void PrintCategoryList()
        {
            if (manager.Categories.Count == 0) return;
            Console.WriteLine("  -- Categories --");
            foreach (Category c in manager.Categories)
                Console.WriteLine("  [" + c.Id + "] " + c.Name);
            Console.WriteLine();
        }

        static void PrintSupplierList()
        {
            if (manager.Suppliers.Count == 0) return;
            Console.WriteLine("  -- Suppliers --");
            foreach (Supplier s in manager.Suppliers)
                Console.WriteLine("  [" + s.Id + "] " + s.Name);
            Console.WriteLine();
        }

        static void PrintProductSummary()
        {
            Console.WriteLine();
            Console.WriteLine("  {0,-4} {1,-26} {2,6}", "ID", "Name", "Stock");
            PrintLine();
            foreach (Product p in manager.Products)
                Console.WriteLine("  {0,-4} {1,-26} {2,6}",
                    p.Id, TruncateName(p.Name, 26), p.StockQty);
            Console.WriteLine();
        }
    }
}
