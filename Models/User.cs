/*
 * User.cs — Model representing a system user with login credentials
 *
 * OOP concepts:
 *   • Encapsulation  — Password has a private setter so it cannot be changed
 *                      from outside the class after construction
 *   • Constructors   — default and parameterized
 *   • Methods        — Authenticate() encapsulates the login check so callers
 *                      never compare passwords directly
 *   • Access modifiers — private fields, private Password setter, public class
 */
namespace InventoryManagementSystem.Models
{
    // Represents a system user with login credentials
    public class User
    {
        private int _id;
        private string _username;
        private string _password;
        private string _role;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            private set { _password = value; }
        }

        public string Role
        {
            get { return _role; }
            set { _role = value; }
        }

        // Default constructor
        public User()
        {
            _id = 0;
            _username = string.Empty;
            _password = string.Empty;
            _role = "Staff";
        }

        // Parameterized constructor
        public User(int id, string username, string password, string role)
        {
            _id = id;
            _username = username;
            _password = password;
            _role = role;
        }

        // Checks if the given credentials match this user
        public bool Authenticate(string username, string password)
        {
            return _username == username && _password == password;
        }

        public override string ToString()
        {
            return $"{_username} ({_role})";
        }
    }
}
