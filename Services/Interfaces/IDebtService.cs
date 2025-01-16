using Pennywise.Model;

namespace Pennywise.Services.Interfaces
{
    
    /// Interface for managing debt-related operations in the Pennywise application
    /// Provides methods for CRUD operations and debt calculations
    
    public interface IDebtService
    {
        // Retrieves all debts from the database asynchronously
        Task<List<Debt>> GetAllDebtsAsync();

        // Gets a specific debt by its ID
        Task<Debt> GetDebtByIdAsync(int id);

        // Adds a new debt record to the database
        Task<Debt> AddDebtAsync(Debt debt);

        // Removes a debt record from the database
        Task DeleteDebtAsync(int id);

        // Marks a debt as cleared by recording the clearing amount
        Task<Debt> ClearDebtAsync(int debtId, decimal clearingAmount);

        /* 
         * Filters debts based on multiple criteria:
         * - Date range (start and end dates)
         * - Status of the debt
         * - Source of the debt
         */
        Task<List<Debt>> FilterDebtsAsync(DateTime? startDate, DateTime? endDate, string status, string source);

        // Searches for debts by their source/origin
        Task<List<Debt>> SearchDebtsBySourceAsync(string source);

        /* 
         * Calculates summary statistics for a list of debts:
         * - Total debt amount
         * - Amount of cleared debt
         * - Amount of pending debt
         * - Total count of debt records
         */
        (decimal totalDebt, decimal clearedDebt, decimal pendingDebt, int totalCount) CalculateDebtSummary(List<Debt> debts);
    }
}
