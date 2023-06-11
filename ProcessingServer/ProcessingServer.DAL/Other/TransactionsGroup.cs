using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Other
{
    public class TransactionsGroup
    {
        public string KeyValue { get; set; }
        public List<Transaction> Transactions { get; set; }
        public double TotalAmount { get; set; }
        public int NumberOfTransactions { get; set; }

        public TransactionsGroup(string keyValue, List<Transaction> transactions)
        {
            KeyValue = keyValue;
            Transactions = transactions;
            TotalAmount = transactions.Sum(transaction => transaction.Amount);
            NumberOfTransactions = transactions.Count;
        }
    }
}
