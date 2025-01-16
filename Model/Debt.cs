using System.ComponentModel.DataAnnotations;

namespace Pennywise.Model
{
    // Represents a debt entity that tracks both pending and cleared debts
    public class Debt
    {
        // Unique identifier for the debt
        public int DebtId { get; set; }

        // Name of the person or organization to whom the debt is owed
        [Required(ErrorMessage = "Source of debt is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Source must be between 2 and 100 characters")]
        public string Source { get; set; }

        // The monetary value of the debt
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        // The date by which the debt needs to be paid
        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        // Date when the debt was paid off (null if not yet cleared)
        public DateTime? ClearedDate { get; set; }

        /* Status of the debt
         * Can be either:
         * - PENDING: debt is still outstanding
         * - CLEARED: debt has been paid off
         */
        public string Status { get; set; } = "PENDING"; // PENDING or CLEARED

        // Additional information or remarks about the debt
        public string Notes { get; set; }

        // Links to the transaction that was used to clear this debt
        // Null when debt is still pending
        public int? ClearingTransactionId { get; set; }
    }
}
