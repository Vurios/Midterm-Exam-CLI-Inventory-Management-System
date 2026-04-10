/*
 * Category.cs — Model representing a product category
 *
 * OOP concepts:
 *   • Encapsulation  — private fields, public properties
 *   • Constructors   — default and parameterized
 *   • Access modifiers — private fields, public class members
 */
namespace InventoryManagementSystem.Models
{
    // Represents a product category
    public class Category
    {
        private int _id;
        private string _name;
        private string _description;

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

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        // Default constructor
        public Category()
        {
            _id = 0;
            _name = string.Empty;
            _description = string.Empty;
        }

        // Parameterized constructor
        public Category(int id, string name, string description)
        {
            _id = id;
            _name = name;
            _description = description;
        }

        public override string ToString()
        {
            return $"[{_id}] {_name} - {_description}";
        }
    }
}
