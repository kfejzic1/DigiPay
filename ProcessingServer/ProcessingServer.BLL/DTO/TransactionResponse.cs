using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.DTO
{
    // This is user's transaction view.
    public class TransactionResponse
    {
        public int TransactionId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string TransactionType { get; set; } // Possible values: C2C, B2B, C2B
        public string TransactionPurpose { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserWithBankAccount Sender { get; set; }
        public UserWithBankAccount Recipient { get; set; }
    }
}