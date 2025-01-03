using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pennywise.Model
{
    public class Transaction
    {
        public int TransactionId { get; set; }  // Unique identifier for the transaction
        public string Title { get; set; }  // Title or description of the transaction
        public decimal Amount { get; set; }  // Amount of the transaction
        public string Type { get; set; }  // Type of transaction: Inflow, Outflow, Debt
        public DateTime? Date { get; set; }  // Date of the transaction
        public string Tags { get; set; }  // Comma separated list of tags
        public string Note { get; set; }  // Additional information or description about the transaction
    }
}
