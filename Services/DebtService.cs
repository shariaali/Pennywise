using Pennywise.Model;
using Pennywise.Services.Interfaces;

namespace Pennywise.Services
{
    // Service class responsible for managing debt-related operations
    public class DebtService : IDebtService
    {
        // File path for storing debts in CSV format
        private readonly string _debtsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "debts.csv");
        private readonly ITransactionService _transactionService;

        public DebtService(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /*
         * Retrieves all debts from the CSV file
         * Returns an empty list if file doesn't exist or on error
         */
        public async Task<List<Debt>> GetAllDebtsAsync()
        {
            try
            {
                if (!File.Exists(_debtsFilePath))
                {
                    return new List<Debt>();
                }

                var debts = new List<Debt>();
                var lines = await File.ReadAllLinesAsync(_debtsFilePath);

                foreach (var line in lines.Skip(1)) // Skip header
                {
                    var fields = line.Split(',');
                    var debt = new Debt
                    {
                        DebtId = int.Parse(fields[0]),
                        Source = fields[1],
                        Amount = decimal.Parse(fields[2]),
                        DueDate = DateTime.Parse(fields[3]),
                        Status = fields[4],
                        Notes = fields[5],
                        ClearedDate = !string.IsNullOrEmpty(fields[6]) ? DateTime.Parse(fields[6]) : null,
                        ClearingTransactionId = !string.IsNullOrEmpty(fields[7]) ? int.Parse(fields[7]) : null
                    };
                    debts.Add(debt);
                }

                return debts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading debts: {ex.Message}");
                return new List<Debt>();
            }
        }

        // Creates a new debt and corresponding transaction
        public async Task<Debt> AddDebtAsync(Debt debt)
        {
            var debts = await GetAllDebtsAsync();
            debt.DebtId = debts.Count > 0 ? debts.Max(d => d.DebtId) + 1 : 1;
            debt.Status = "PENDING";
            debts.Add(debt);

            // Create a corresponding transaction for the debt
            var transaction = new Transaction
            {
                Title = $"Debt from {debt.Source}",
                Amount = debt.Amount,
                Type = "DEBT",
                Date = DateTime.Now,
                Tags = "Debt",
                Note = debt.Notes
            };

            await _transactionService.SaveTransactionAsync(transaction);
            await SaveDebtsToFile(debts);

            return debt;
        }

        /*
         * Clears a debt by creating a clearing transaction
         * Throws exception if:
         * - Debt not found
         * - Already cleared
         * - Insufficient balance
         */
        public async Task<Debt> ClearDebtAsync(int debtId, decimal clearingAmount)
        {
            var debts = await GetAllDebtsAsync();
            var debt = debts.FirstOrDefault(d => d.DebtId == debtId);

            if (debt == null)
                throw new Exception("Debt not found");

            if (debt.Status == "CLEARED")
                throw new Exception("Debt is already cleared");

            // Check if there's sufficient balance
            var transactions = await _transactionService.LoadTransactionsAsync();
            var sums = _transactionService.CalculateTransactionSums(transactions);
            var availableBalance = sums.totalInflows - sums.totalOutflows;

            if (availableBalance < clearingAmount)
                throw new Exception($"Insufficient balance. Available: ${availableBalance:F2}, Required: ${clearingAmount:F2}");

            // Create a clearing transaction
            var transaction = new Transaction
            {
                Title = $"Cleared debt to {debt.Source}",
                Amount = clearingAmount,
                Type = "DEBT",
                Date = DateTime.Now,
                Tags = "Debt",
                Note = $"Clearing debt #{debt.DebtId}"
            };

            await _transactionService.SaveTransactionAsync(transaction);

            debt.Status = "CLEARED";
            debt.ClearedDate = DateTime.Now;
            debt.ClearingTransactionId = transaction.TransactionId;

            await SaveDebtsToFile(debts);
            return debt;
        }

        // Internal method to save debts to CSV file
        private async Task SaveDebtsToFile(List<Debt> debts)
        {
            try
            {
                // First, ensure we have all existing debts
                var existingDebts = await GetAllDebtsAsync();
                
                // For each new debt, check if it exists and update or add accordingly
                foreach (var debt in debts)
                {
                    var existingDebt = existingDebts.FirstOrDefault(d => d.DebtId == debt.DebtId);
                    if (existingDebt != null)
                    {
                        // Update existing debt
                        existingDebt.Source = debt.Source;
                        existingDebt.Amount = debt.Amount;
                        existingDebt.DueDate = debt.DueDate;
                        existingDebt.Status = debt.Status;
                        existingDebt.Notes = debt.Notes;
                        existingDebt.ClearedDate = debt.ClearedDate;
                        existingDebt.ClearingTransactionId = debt.ClearingTransactionId;
                    }
                    else
                    {
                        // Add new debt
                        existingDebts.Add(debt);
                    }
                }

                // Write all debts to file
                var lines = new List<string>
                {
                    "DebtId,Source,Amount,DueDate,Status,Notes,ClearedDate,ClearingTransactionId"
                };

                foreach (var debt in existingDebts)
                {
                    lines.Add($"{debt.DebtId},{debt.Source},{debt.Amount}," +
                             $"{debt.DueDate:yyyy-MM-dd},{debt.Status},{debt.Notes}," +
                             $"{debt.ClearedDate?.ToString("yyyy-MM-dd")},{debt.ClearingTransactionId}");
                }

                await File.WriteAllLinesAsync(_debtsFilePath, lines);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving debts to file: {ex.Message}");
            }
        }

        // Retrieves a single debt by ID
        public async Task<Debt> GetDebtByIdAsync(int id)
        {
            var debts = await GetAllDebtsAsync();
            var debt = debts.FirstOrDefault(d => d.DebtId == id);
            
            if (debt == null)
                throw new Exception($"Debt with ID {id} not found");
        
            return debt;
        }

        /*
         * Deletes a debt and its associated transactions:
         * - Debt creation transaction
         * - Debt clearing transaction (if exists)
         */
        public async Task DeleteDebtAsync(int id)
        {
            try
            {
                var debts = await GetAllDebtsAsync();
                var debt = debts.FirstOrDefault(d => d.DebtId == id);
                
                if (debt == null)
                    throw new Exception($"Debt with ID {id} not found");

                // Delete associated transactions
                var transactions = await _transactionService.LoadTransactionsAsync();
                
                // Find and delete debt creation transaction
                var debtCreationTransaction = transactions.FirstOrDefault(t => 
                    t.Type == "DEBT" && 
                    t.Title == $"Debt from {debt.Source}" &&
                    Math.Abs(t.Amount - debt.Amount) < 0.01m);

                if (debtCreationTransaction != null)
                {
                    await _transactionService.DeleteTransactionAsync(debtCreationTransaction.TransactionId);
                }

                // Find and delete debt clearing transaction if it exists
                if (debt.ClearingTransactionId.HasValue)
                {
                    var debtClearingTransaction = transactions.FirstOrDefault(t => 
                        t.TransactionId == debt.ClearingTransactionId.Value);
                        
                    if (debtClearingTransaction != null)
                    {
                        await _transactionService.DeleteTransactionAsync(debtClearingTransaction.TransactionId);
                    }
                }
            
                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(_debtsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Remove the debt and write directly to file
                debts.Remove(debt);
                var lines = new List<string>
                {
                    "DebtId,Source,Amount,DueDate,Status,Notes,ClearedDate,ClearingTransactionId"
                };

                foreach (var d in debts)
                {
                    lines.Add($"{d.DebtId},{d.Source},{d.Amount}," +
                             $"{d.DueDate:yyyy-MM-dd},{d.Status},{d.Notes}," +
                             $"{d.ClearedDate?.ToString("yyyy-MM-dd")},{d.ClearingTransactionId}");
                }

                await File.WriteAllLinesAsync(_debtsFilePath, lines);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting debt: {ex.Message}");
            }
        }

        // Filters debts based on date range, status, and source
        public async Task<List<Debt>> FilterDebtsAsync(DateTime? startDate, DateTime? endDate, string status, string source)
        {
            var debts = await GetAllDebtsAsync();
            
            return debts.Where(d => 
                (!startDate.HasValue || d.DueDate >= startDate) &&
                (!endDate.HasValue || d.DueDate <= endDate) &&
                (string.IsNullOrEmpty(status) || d.Status.Equals(status, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(source) || d.Source.Contains(source, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        // Searches debts by source name (case-insensitive)
        public async Task<List<Debt>> SearchDebtsBySourceAsync(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return await GetAllDebtsAsync();
        
            var debts = await GetAllDebtsAsync();
            return debts.Where(d => d.Source.Contains(source, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /*
         * Calculates debt summary statistics:
         * - Total debt amount
         * - Cleared debt amount
         * - Pending debt amount
         * - Total number of debts
         */
        public (decimal totalDebt, decimal clearedDebt, decimal pendingDebt, int totalCount) CalculateDebtSummary(List<Debt> debts)
        {
            var totalDebt = debts.Sum(d => d.Amount);
            var clearedDebt = debts.Where(d => d.Status == "CLEARED").Sum(d => d.Amount);
            var pendingDebt = debts.Where(d => d.Status == "PENDING").Sum(d => d.Amount);
            var totalCount = debts.Count;
            
            return (totalDebt, clearedDebt, pendingDebt, totalCount);
        }

    }
}
