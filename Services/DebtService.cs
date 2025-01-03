using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pennywise.Model;
using Pennywise.Services.Interfaces;

namespace Pennywise.Services
{
    public class DebtService : IDebtService
    {
        private readonly ITransactionService _transactionService;

        public DebtService(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<List<Transaction>> GetPendingDebtsAsync()
        {
            var transactions = await _transactionService.LoadTransactionsAsync();

            return transactions
                .Where(t => t.Type == "Debt" && t.Amount > 0)
                .OrderBy(t => t.Date)
                .ToList();
        }

        public async Task ClearDebtAsync(int transactionId, decimal paymentAmount)
        {
            var transactions = await _transactionService.LoadTransactionsAsync();
            var debtTransaction = transactions.FirstOrDefault(t => t.TransactionId == transactionId && t.Type == "Debt");

            if (debtTransaction == null)
            {
                throw new Exception("Debt transaction not found.");
            }

            if (paymentAmount > debtTransaction.Amount)
            {
                throw new Exception("Payment amount exceeds the debt amount.");
            }

            // Update the debt transaction
            debtTransaction.Amount -= paymentAmount;

            if (debtTransaction.Amount == 0)
            {
                debtTransaction.Type = "Cleared";
            }

            // Create a new transaction to record the payment
            var paymentTransaction = new Transaction
            {
                TransactionId = transactions.Max(t => t.TransactionId) + 1,
                Title = $"Cleared Debt - {debtTransaction.Title}",
                Amount = paymentAmount,
                Type = "Debit",
                Date = DateTime.Now,
                Tags = debtTransaction.Tags,
                Note = $"Debt payment for {debtTransaction.Title}"
            };

            // Save updated transactions
            await _transactionService.SaveTransactionAsync(debtTransaction);
            await _transactionService.SaveTransactionAsync(paymentTransaction);
        }

        public async Task<decimal> GetTotalPendingDebtAsync()
        {
            var pendingDebts = await GetPendingDebtsAsync();
            return pendingDebts.Sum(d => d.Amount);
        }
    }
}
