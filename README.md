# Inventory Management System

A CLI-based Inventory Management System built with C# (.NET 8) as a console application. Developed as a course project demonstrating Object-Oriented Programming principles including encapsulation, classes, constructors, properties, access modifiers, and exception handling.

---

## Features

**Product Management**
- Add, view, search, update, and delete products
- Products are linked to categories and suppliers
- Automatic low-stock detection

**Category & Supplier Management**
- Add, view, update, and delete categories
- Add, view, update, and delete suppliers

**Stock Operations**
- Restock products with quantity and reason tracking
- Deduct stock with reason tracking
- All stock changes are recorded as transactions

**Reports**
- View full transaction history
- View low-stock items (products below minimum stock level)
- Compute total inventory value

---

## Project Structure

```
InventoryManagementSystem/
├── Models/
│   ├── Product.cs
│   ├── Category.cs
│   ├── Supplier.cs
│   ├── User.cs
│   └── TransactionRecord.cs
├── InventoryManager.cs
├── Program.cs
├── InventoryManagementSystem.csproj
└── InventoryManagementSystem.sln
```

---

## OOP Concepts Used

| Concept | Where Applied |
|---|---|
| Classes & Objects | All 5 models, `InventoryManager`, `Program` |
| Constructors | All model classes |
| Properties | Private fields with public getters/setters |
| Encapsulation | Private backing fields, controlled access |
| Access Modifiers | `public`, `private` throughout |
| Methods | Business logic in models and manager |
| Exception Handling | `try-catch` on all user inputs, `GoBackException` for navigation |

---

## Models

- **Product** — Stores product details: name, price, stock quantity, minimum stock level, category, and supplier
- **Category** — Stores category ID and name
- **Supplier** — Stores supplier ID, name, and contact info
- **User** — Stores user credentials and role
- **TransactionRecord** — Logs every stock change with timestamp, type, quantity, and reason

---

## How to Run

**Requirements:** .NET 8 SDK — download at [dotnet.microsoft.com](https://dotnet.microsoft.com/download)

```bash
# Clone the repository
git clone https://github.com/yourusername/InventoryManagementSystem.git

# Navigate into the project folder
cd InventoryManagementSystem

# Run the program
dotnet run
```

---

## Storage

This project uses `List<T>` in-memory storage only. No database or external files are used, as required by the project specifications.

---

## Course Info

- **Subject:** Object-Oriented Programming
- **Language:** C# (.NET 8)
- **Type:** Console Application
