using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pennywise.Model;

namespace Pennywise.Services.Interfaces
{
    public interface IDebtService
    {
        /// <summary>
        /// Retrieves a list of pending debts.
        /// </summary>
        /// <returns>List of pending debt transactions.</returns>
        Task<List<Transaction>> GetPendingDebtsAsync();

        /// <summary>
        /// Clears a portion or the full amount of a debt transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the debt transaction to clear.</param>
        /// <param name="paymentAmount">The amount to pay towards the debt.</param>
        Task ClearDebtAsync(int transactionId, decimal paymentAmount);

        /// <summary>
        /// Gets the total amount of all pending debts.
        /// </summary>
        /// <returns>Total pending debt amount.</returns>
        Task<decimal> GetTotalPendingDebtAsync();
    }
}
