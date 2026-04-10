/*
 * Supplier.cs — Model representing a product supplier
 *
 * OOP concepts:
 *   • Encapsulation  — private fields, public properties
 *   • Constructors   — default and parameterized
 *   • Access modifiers — private fields, public class members
 */
namespace InventoryManagementSystem.Models
{
    // Represents a product supplier
    public class Supplier
    {
        private int _id;
        private string _name;
        private string _contactNumber;
        private string _email;

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

        public string ContactNumber
        {
            get { return _contactNumber; }
            set { _contactNumber = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        // Default constructor
        public Supplier()
        {
            _id = 0;
            _name = string.Empty;
            _contactNumber = string.Empty;
            _email = string.Empty;
        }

        // Parameterized constructor
        public Supplier(int id, string name, string contactNumber, string email)
        {
            _id = id;
            _name = name;
            _contactNumber = contactNumber;
            _email = email;
        }

        public override string ToString()
        {
            return $"[{_id}] {_name} | {_contactNumber} | {_email}";
        }
    }
}
