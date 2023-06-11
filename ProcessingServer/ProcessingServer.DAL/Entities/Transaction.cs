using Org.BouncyCastle.Cms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Entities
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string TransactionType { get; set; } // Possible values: C2C, B2B, C2B
        public string TransactionPurpose { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserBankAccount")]
        public int RecipientAccountId { get; set; }
        public UserBankAccount RecipientAccount { get; set; }

        [ForeignKey("UserBankAccount")]
        public int SenderAccountId { get; set; }
        public UserBankAccount SenderAccount { get; set; }
    }
}