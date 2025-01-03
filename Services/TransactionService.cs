using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pennywise.Services;
using Pennywise.Model;
using Pennywise.Services.Interfaces;


public class TransactionService : ITransactionService
{
    private readonly string _transactionsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "transactions.csv");

    public async Task SaveTransactionAsync(Transaction transaction)
    {
        try
        {
            // Check if the file exists
            bool fileExists = File.Exists(_transactionsFilePath);

            // Open the file for appending
            using (var writer = new StreamWriter(_transactionsFilePath, append: true))
            {
                if (!fileExists)
                {
                    // Write the header if the file is newly created
                    await writer.WriteLineAsync("TransactionId, Title, Amount, Type, Date, Tags, Note");
                }
                if (string.IsNullOrEmpty(transaction.Type))
                {
                    transaction.Type = "Credit"; // Default to "Credit" if not set
                }
                // Format the Date to yyyy-MM-dd
                string formattedDate = transaction.Date.ToString("yyyy-MM-dd");
                // Write the transaction as a CSV row
                string csvRow = $"{transaction.TransactionId},{transaction.Title}," +
                                $"{transaction.Amount},{transaction.Type},{formattedDate}, {transaction.Tags}, {transaction.Note}";
                await writer.WriteLineAsync(csvRow);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving transaction: {ex.Message}");
            throw;
        }
    }

    public async Task<List<Transaction>> LoadTransactionsAsync()
    {
        try
        {
            if (!File.Exists(_transactionsFilePath))
            {
                return new List<Transaction>();
            }

            var transactions = new List<Transaction>();

            using (var reader = new StreamReader(_transactionsFilePath))
            {
                string headerLine = await reader.ReadLineAsync(); // Skip header line

                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var fields = line.Split(',');

                    var transaction = new Transaction
                    {
                        TransactionId = int.Parse(fields[0]),
                        Title = fields[1],
                        Amount = decimal.Parse(fields[2]),
                        Type = fields[3],
                        Date = DateTime.Parse(fields[4]),
                        Tags = fields[5],
                        Note = fields[6]
                    };

                    transactions.Add(transaction);
                }
            }

            return transactions;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading transactions: {ex.Message}");
            return new List<Transaction>();
        }
    }
    public (decimal totalInflows, decimal totalOutflows, decimal totalDebt, decimal totalClearedDebt, decimal remainingDebt, decimal balance, bool isSufficientBalance) CalculateTransactionSums(List<Transaction> transactions)
    {
        decimal totalInflows = 0;
        decimal totalOutflows = 0;
        decimal totalDebt = 0;
        decimal totalClearedDebt = 0;
        bool isSufficientBalance = true;  // Assume sufficient balance unless proven otherwise

        foreach (var transaction in transactions)
        {
            if (transaction.Type == "Credit")
            {
                totalInflows += transaction.Amount;
                totalClearedDebt += transaction.Amount;
            }
            else if (transaction.Type == "Debit" || transaction.Type == "Debt")
            {
                totalOutflows += transaction.Amount;
                totalDebt += transaction.Amount;

                // Check if there is sufficient balance before processing the outflow
                if (totalOutflows > totalInflows)
                {
                    isSufficientBalance = false; // Not enough balance to cover outflows
                }
            }
        }

        decimal remainingDebt = totalDebt - totalClearedDebt;
        decimal balance = totalInflows - totalOutflows; // Calculate balance as the difference between inflows and outflows

        return (totalInflows, totalOutflows, totalDebt, totalClearedDebt, remainingDebt, balance, isSufficientBalance);
    }
    public (Transaction highestInflow, Transaction lowestInflow, Transaction highestOutflow, Transaction lowestOutflow, Transaction highestDebt, Transaction lowestDebt) GetTransactionExtremes(List<Transaction> transactions)
    {
        var highestInflow = transactions.Where(t => t.Type == "Credit").OrderByDescending(t => t.Amount).FirstOrDefault();
        var lowestInflow = transactions.Where(t => t.Type == "Credit").OrderBy(t => t.Amount).FirstOrDefault();
        var highestOutflow = transactions.Where(t => t.Type == "Debit").OrderByDescending(t => t.Amount).FirstOrDefault();
        var lowestOutflow = transactions.Where(t => t.Type == "Debit").OrderBy(t => t.Amount).FirstOrDefault();
        var highestDebt = transactions.Where(t => t.Type == "Debt").OrderByDescending(t => t.Amount).FirstOrDefault();
        var lowestDebt = transactions.Where(t => t.Type == "Debt").OrderBy(t => t.Amount).FirstOrDefault();

        return (highestInflow, lowestInflow, highestOutflow, lowestOutflow, highestDebt, lowestDebt);
    }
}
