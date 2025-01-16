using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pennywise.Model;
namespace Pennywise.Services.Interfaces;

/* 
 * Interface for managing financial transactions including CRUD operations,
 * calculations, searching, filtering, and sorting capabilities
 */
public interface ITransactionService
{
    // Saves a new transaction to the data store
    Task SaveTransactionAsync(Transaction transaction);

    // Retrieves all transactions from the data store
    Task<List<Transaction>> LoadTransactionsAsync();

    // Removes a transaction by its ID
    Task DeleteTransactionAsync(int transactionId);

    /* 
     * Calculates various financial metrics from a list of transactions
     * Returns a tuple containing:
     * - Total money coming in (inflows)
     * - Total money going out (outflows)
     * - Total debt
     * - Total cleared debt
     * - Remaining debt to be paid
     * - Current balance
     * - Flag indicating if balance is sufficient
     */
    (decimal totalInflows, decimal totalOutflows, decimal totalDebt, decimal totalClearedDebt, decimal remainingDebt, decimal balance, bool isSufficientBalance) CalculateTransactionSums(List<Transaction> transactions);

    // Searches transactions by their title
    Task<List<Transaction>> SearchTransactionsByTitleAsync(string title);

    // Filters transactions based on date range, type, and tags
    Task<List<Transaction>> FilterTransactionsAsync(DateTime? startDate, DateTime? endDate, string type, string tags);

    // Generic sorting method with ascending/descending option
    Task<List<Transaction>> SortTransactionsAsync(string sortBy, bool isAscending);

    // Sorts transactions in ascending order by specified field
    Task<List<Transaction>> SortTransactionsAscending(string sortBy);

    // Sorts transactions in descending order by specified field
    Task<List<Transaction>> SortTransactionsDescending(string sortBy);

    // Retrieves a single transaction by its ID
    Task<Transaction> GetTransactionAsync(int id);
}