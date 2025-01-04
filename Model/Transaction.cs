using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Pennywise.Model
{
    public class Transaction
    {
        public int TransactionId { get; set; }  // Unique identifier for the transaction

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters")]
        public string Title { get; set; }  // Title or description of the transaction

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }  // Amount of the transaction

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; }  // Type of transaction: Inflow, Outflow, Debt

        [Required(ErrorMessage = "Date is required")]
        public DateTime? Date { get; set; }  // Date of the transaction

        public string Tags { get; set; }  // Comma separated list of tags
        public string Note { get; set; }  // Additional information or description about the transaction
    }
}
