using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pennywise.Model;
namespace Pennywise.Services.Interfaces;

public interface ITransactionService
{
    Task SaveTransactionAsync(Transaction transaction);
    Task<List<Transaction>> LoadTransactionsAsync();
    Task DeleteTransactionAsync(int transactionId);
    (decimal totalInflows, decimal totalOutflows, decimal totalDebt, decimal totalClearedDebt, decimal remainingDebt, decimal balance, bool isSufficientBalance) CalculateTransactionSums(List<Transaction> transactions);

    Task<List<Transaction>> SearchTransactionsByTitleAsync(string title);
    Task<List<Transaction>> FilterTransactionsAsync(DateTime? startDate, DateTime? endDate, string type, string tags);
    Task<List<Transaction>> SortTransactionsAsync(string sortBy, bool isAscending);
    Task<List<Transaction>> SortTransactionsAscending(string sortBy);
    Task<List<Transaction>> SortTransactionsDescending(string sortBy);
}