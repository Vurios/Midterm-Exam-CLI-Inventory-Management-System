/*
 * Product.cs — Model representing a single item in the inventory
 *
 * OOP concepts:
 *   • Encapsulation  — all fields are private; outside code reads/writes them
 *                      only through the public properties below
 *   • Constructors   — a default (no-arg) constructor for flexibility, and a
 *                      parameterized constructor for normal use
 *   • Properties     — full get/set pairs expose each field; the backing field
 *                      names use the _camelCase convention to distinguish them
 *                      from the PascalCase property names
 *   • Methods        — IsLowStock() and ComputeLineValue() encapsulate business
 *                      rules so callers never duplicate the logic
 *   • Access modifiers — fields are private; class and members are public
 */
namespace InventoryManagementSystem.Models
{
    // Represents a product in the inventory
    public class Product
    {
        private int _id;
        private string _name;
        private decimal _price;
        private int _stockQty;
        private int _lowStockThreshold;
        private int _categoryId;
        private int _supplierId;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public int StockQty
        {
            get { return _stockQty; }
            set { _stockQty = value; }
        }

        // Stock level at which the product is flagged as low
        public int LowStockThreshold
        {
            get { return _lowStockThreshold; }
            set { _lowStockThreshold = value; }
        }

        public int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }

        public int SupplierId
        {
            get { return _supplierId; }
            set { _supplierId = value; }
        }

        // Default constructor
        public Product()
        {
            _id = 0;
            _name = string.Empty;
            _price = 0m;
            _stockQty = 0;
            _lowStockThreshold = 5;
            _categoryId = 0;
            _supplierId = 0;
        }

        // Parameterized constructor
        public Product(int id, string name, decimal price, int stockQty,
                       int lowStockThreshold, int categoryId, int supplierId)
        {
            _id = id;
            _name = name;
            _price = price;
            _stockQty = stockQty;
            _lowStockThreshold = lowStockThreshold;
            _categoryId = categoryId;
            _supplierId = supplierId;
        }

        // Returns true if stock is at or below the low-stock threshold
        public bool IsLowStock()
        {
            return _stockQty < _lowStockThreshold;
        }

        // Calculates the total inventory value for this product line
        public decimal ComputeLineValue()
        {
            return _price * _stockQty;
        }

        public override string ToString()
        {
            return $"[{_id}] {_name} | Php {_price:F2} | Stock: {_stockQty}";
        }
    }
}
