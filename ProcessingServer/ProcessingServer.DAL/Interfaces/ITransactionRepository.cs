using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateTransaction(Transaction transaction);
        Task<List<Transaction>> GetTransactionsForUser(User user);
        Task<List<TransactionsGroup>> GroupTransactionsByCurrency(User user);
        Task<List<TransactionsGroup>> GroupTransactionsByType(User user);
        Task<Transaction> GetTransactionById(int transactionId);
    }
}
