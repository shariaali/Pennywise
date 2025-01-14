using System.ComponentModel.DataAnnotations;

namespace Pennywise.Model
{
    public class Debt
    {
        public int DebtId { get; set; }

        [Required(ErrorMessage = "Source of debt is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Source must be between 2 and 100 characters")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        public DateTime? ClearedDate { get; set; }

        public string Status { get; set; } = "PENDING"; // PENDING or CLEARED

        public string Notes { get; set; }

        // Reference to the associated transaction when debt is cleared
        public int? ClearingTransactionId { get; set; }
    }
}
