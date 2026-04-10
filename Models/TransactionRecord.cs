/*
 * TransactionRecord.cs — Model representing a single stock-movement log entry
 *
 * OOP concepts:
 *   • Encapsulation  — private fields, public properties
 *   • Constructors   — default and parameterized; the parameterized constructor
 *                      automatically stamps Timestamp = DateTime.Now so callers
 *                      never forget to set it
 *   • Enum           — TransactionKind groups the three movement types (InitialStock,
 *                      Restock, Deduction) so magic strings are never used
 *   • Access modifiers — private fields, public class members
 */
using System;

namespace InventoryManagementSystem.Models
{
    // Classifies the type of stock movement recorded in a TransactionRecord
    public enum TransactionKind
    {
        InitialStock,   // Opening stock when a product is first added
        Restock,        // Stock added via the Restock operation
        Deduction       // Stock removed via the Deduct Stock operation
    }

    // Represents a single logged stock movement
    // OOP: encapsulation, constructors, properties
    public class TransactionRecord
    {
        private int _id;
        private int _productId;
        private string _productName;
        private TransactionKind _kind;
        private int _quantity;
        private DateTime _timestamp;
        private string _recordedBy;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int ProductId
        {
            get { return _productId; }
            set { _productId = value; }
        }

        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        public TransactionKind Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        public string RecordedBy
        {
            get { return _recordedBy; }
            set { _recordedBy = value; }
        }

        // Default constructor
        public TransactionRecord()
        {
            _id = 0;
            _productId = 0;
            _productName = string.Empty;
            _kind = TransactionKind.Restock;
            _quantity = 0;
            _timestamp = DateTime.Now;
            _recordedBy = string.Empty;
        }

        // Parameterized constructor — timestamp is set automatically
        public TransactionRecord(int id, int productId, string productName,
                                 TransactionKind kind, int quantity, string recordedBy)
        {
            _id = id;
            _productId = productId;
            _productName = productName;
            _kind = kind;
            _quantity = quantity;
            _timestamp = DateTime.Now;
            _recordedBy = recordedBy;
        }
    }
}
