﻿using System;
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
            var transactions = await LoadTransactionsAsync();
            
            if (transaction.TransactionId == 0)
            {
                // New transaction
                transaction.TransactionId = transactions.Count > 0 ? 
                    transactions.Max(t => t.TransactionId) + 1 : 1;
                transactions.Add(transaction);
            }
            else
            {
                // Update existing transaction
                var existingTransaction = transactions.FirstOrDefault(t => t.TransactionId == transaction.TransactionId);
                if (existingTransaction != null)
                {
                    existingTransaction.Title = transaction.Title;
                    existingTransaction.Amount = transaction.Amount;
                    existingTransaction.Type = transaction.Type;
                    existingTransaction.Date = transaction.Date;
                    existingTransaction.Note = transaction.Note;
                    existingTransaction.Tags = transaction.Tags;
                }
                else
                {
                    transactions.Add(transaction);
                }
            }

            await SaveTransactionsToFile(transactions);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error saving transaction: {ex.Message}");
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
                        Tags = fields[5]?.Replace("||", ","),
                        Note = fields[6]?.Replace("||", ",")
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

        foreach (var transaction in transactions)
        {
            switch (transaction.Type)
            {
                case "Inflow":
                    totalInflows += transaction.Amount;
                    break;
                case "Outflow":
                    totalOutflows += transaction.Amount;
                    break;
                case "DEBT" when transaction.Title?.StartsWith("Debt from") == true:
                    totalDebt += transaction.Amount;  // Add to total debt and inflows when debt is received
                    totalInflows += transaction.Amount;
                    break;
                case "DEBT":  // This is for debt clearing transactions
                    totalOutflows += transaction.Amount;
                    totalClearedDebt += transaction.Amount;
                    break;
            }
        }

        decimal remainingDebt = totalDebt - totalClearedDebt;
        decimal balance = totalInflows - totalOutflows;
        bool isSufficientBalance = balance >= 0;

        return (totalInflows, totalOutflows, totalDebt, totalClearedDebt, remainingDebt, balance, isSufficientBalance);
    }
    public (Transaction highestInflow, Transaction lowestInflow, Transaction highestOutflow, Transaction lowestOutflow, Transaction highestDebt, Transaction lowestDebt) GetTransactionExtremes(List<Transaction> transactions)
    {
        var highestInflow = transactions.Where(t => t.Type == "Inflow").OrderByDescending(t => t.Amount).FirstOrDefault();
        var lowestInflow = transactions.Where(t => t.Type == "Inflow").OrderBy(t => t.Amount).FirstOrDefault();
        var highestOutflow = transactions.Where(t => t.Type == "Outflow").OrderByDescending(t => t.Amount).FirstOrDefault();
        var lowestOutflow = transactions.Where(t => t.Type == "Outflow").OrderBy(t => t.Amount).FirstOrDefault();
        var highestDebt = transactions.Where(t => t.Type == "Debt").OrderByDescending(t => t.Amount).FirstOrDefault();
        var lowestDebt = transactions.Where(t => t.Type == "Debt").OrderBy(t => t.Amount).FirstOrDefault();

        return (highestInflow, lowestInflow, highestOutflow, lowestOutflow, highestDebt, lowestDebt);
    }

    public async Task DeleteTransactionAsync(int transactionId)
    {
        try
        {
            var transactions = await LoadTransactionsAsync();
            var transaction = transactions.FirstOrDefault(t => t.TransactionId == transactionId);
            
            if (transaction == null)
                throw new Exception($"Transaction with ID {transactionId} not found");

            // If this is a debt-related transaction, update the corresponding debt
            if (transaction.Type == "DEBT")
            {
                // Implementation will depend on your debt service implementation
                // You'll need to find and update the corresponding debt
            }

            transactions.Remove(transaction);
            await SaveTransactionsToFile(transactions);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting transaction: {ex.Message}");
        }
    }

    public async Task<List<Transaction>> SearchTransactionsByTitleAsync(string title)
    {
        var transactions = await LoadTransactionsAsync();
        return transactions.Where(t => t.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    public async Task<List<Transaction>> FilterTransactionsAsync(DateTime? startDate, DateTime? endDate, string type, string tags)
    {
        var transactions = await LoadTransactionsAsync();
        return transactions.Where(t =>
            (!startDate.HasValue || t.Date >= startDate.Value) &&
            (!endDate.HasValue || t.Date <= endDate.Value) &&
            (string.IsNullOrEmpty(type) || t.Type.Equals(type, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(tags) || t.Tags.Contains(tags, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }
    public async Task<List<Transaction>> SortTransactionsAsync(string sortBy, bool isAscending)
    {
        var transactions = await LoadTransactionsAsync();
        return sortBy.ToLower() switch
        {
            "date" => isAscending ? transactions.OrderBy(t => t.Date).ToList() : transactions.OrderByDescending(t => t.Date).ToList(),
            _ => transactions // Default: no sorting
        };
    }
    public async Task<List<Transaction>> SortTransactionsAscending(string sortBy)
    {
        var transactions = await LoadTransactionsAsync();

        // Sort transactions in ascending order by date
        return transactions.OrderBy(t => t.Date).ToList();
    }
    public async Task<List<Transaction>> SortTransactionsDescending(string sortBy)
    {
        var transactions = await LoadTransactionsAsync();
        // Sort transactions in descending order by date
        return transactions.OrderByDescending(t => t.Date).ToList();
    }

    public async Task<Transaction> GetTransactionAsync(int id)
    {
        var transactions = await LoadTransactionsAsync();
        return transactions.FirstOrDefault(t => t.TransactionId == id) 
               ?? throw new Exception($"Transaction with ID {id} not found");
    }

    private async Task SaveTransactionsToFile(List<Transaction> transactions)
    {
        try
        {
            var directory = Path.GetDirectoryName(_transactionsFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var writer = new StreamWriter(_transactionsFilePath, false))
            {
                await writer.WriteLineAsync("TransactionId,Title,Amount,Type,Date,Tags,Note");
                foreach (var t in transactions)
                {
                    var tags = t.Tags?.Replace(",", "||");
                    var note = t.Note?.Replace(",", "||");
                    
                    string csvRow = $"{t.TransactionId},{t.Title},{t.Amount},{t.Type}," +
                                  $"{t.Date:yyyy-MM-dd},{tags},{note}";
                    await writer.WriteLineAsync(csvRow);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error saving transactions to file: {ex.Message}");
        }
    }

}