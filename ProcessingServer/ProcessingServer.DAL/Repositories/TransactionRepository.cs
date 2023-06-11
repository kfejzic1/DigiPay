using Microsoft.EntityFrameworkCore;
using ProcessingServer.DAL.Interfaces;
using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessingServer.DAL.Other;
using Microsoft.Extensions.Options;

namespace ProcessingServer.DAL.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private AppDbContext _dbContext;

        public TransactionRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            await _dbContext.Transactions.AddAsync(transaction);
            return transaction;
        }

        public async Task<Transaction> GetTransactionById(int transactionId)
        {
            var result = await _dbContext.Transactions.Include(transaction => transaction.SenderAccount)
                                                      .Include(transaction => transaction.RecipientAccount)
                                                      .Where(transaction => transaction.TransactionId == transactionId)
                                                      .FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<Transaction>> GetTransactionsForUser(User user)
        {
            var transactions = await _dbContext.Transactions.Include(transaction => transaction.RecipientAccount)
                                                            .Include(transaction => transaction.SenderAccount)
                                                            .Where(transaction => transaction.RecipientAccount.OwnerId.Equals(user.UserId) || transaction.SenderAccount.OwnerId.Equals(user.UserId))
                                                            .ToListAsync();
            return transactions;
        }

        public async Task<List<TransactionsGroup>> GroupTransactionsByCurrency(User user)
        {
            var transactions = await _dbContext.Transactions.Include(transaction => transaction.RecipientAccount)
                                                            .Include(transaction => transaction.SenderAccount)
                                                            .Where(transaction => transaction.RecipientAccount.OwnerId.Equals(user.UserId) || transaction.SenderAccount.OwnerId.Equals(user.UserId))
                                                            .GroupBy(transaction => transaction.Currency)
                                                            .Select(transactions => new TransactionsGroup(transactions.Key, transactions.ToList()))
                                                            .ToListAsync();
            return transactions;  
        }

        public async Task<List<TransactionsGroup>> GroupTransactionsByType(User user)
        {
            var transactions = await _dbContext.Transactions.Include(transaction => transaction.RecipientAccount)
                                                            .Include(transaction => transaction.SenderAccount)
                                                            .Where(transaction => transaction.RecipientAccount.OwnerId.Equals(user.UserId) || transaction.SenderAccount.OwnerId.Equals(user.UserId))
                                                            .GroupBy(transaction => transaction.TransactionType)
                                                            .Select(transactions => new TransactionsGroup(transactions.Key, transactions.ToList()))
                                                            .ToListAsync();
            return transactions;
        }
    }
}
