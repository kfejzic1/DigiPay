using ProcessingServer.BLL.DTO;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponse> CreateTransaction(string token, TransactionRequest transactionDto);
        Task<List<TransactionResponse>> GetTransactionsForUser(string token, string pageNumber, string pageSize, TransactionsFilter filter, string sortingOrder);
        Task<List<TransactionsGroupResponse>> GroupTransactionsByCurrency(string token);
        Task<List<TransactionsGroupResponse>> GroupTransactionsByType(string token);
        Task<TransactionResponse> GetTransactionById(string token, string transactionId);
    }
}