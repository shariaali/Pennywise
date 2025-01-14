using Pennywise.Model;

namespace Pennywise.Services.Interfaces
{
    public interface IDebtService
    {
        Task<List<Debt>> GetAllDebtsAsync();
        Task<Debt> GetDebtByIdAsync(int id);
        Task<Debt> AddDebtAsync(Debt debt);
        Task DeleteDebtAsync(int id);
        Task<Debt> ClearDebtAsync(int debtId, decimal clearingAmount);
        Task<List<Debt>> FilterDebtsAsync(DateTime? startDate, DateTime? endDate, string status, string source);
        Task<List<Debt>> SearchDebtsBySourceAsync(string source);
        (decimal totalDebt, decimal clearedDebt, decimal pendingDebt, int totalCount) CalculateDebtSummary(List<Debt> debts);
    }
}
