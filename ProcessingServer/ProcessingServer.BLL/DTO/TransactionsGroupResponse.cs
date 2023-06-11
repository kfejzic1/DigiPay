using ProcessingServer.BLL.DTO;
using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Other
{
    public class TransactionsGroupResponse
    {
        public string KeyValue { get; set; }
        public List<TransactionResponse> Transactions { get; set; }
        public double TotalAmount { get; set; }
        public int NumberOfTransactions { get; set; }

        public TransactionsGroupResponse(string keyValue, List<TransactionResponse> transactions, double totalAmount, int numberOfTransactions)
        {
            KeyValue = keyValue;
            Transactions = transactions;
            TotalAmount = totalAmount;
            NumberOfTransactions = numberOfTransactions;
        }
    }
}