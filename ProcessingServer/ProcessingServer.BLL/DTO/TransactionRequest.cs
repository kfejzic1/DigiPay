using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.DTO
{
    public class TransactionRequest
    {
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string TransactionType { get; set; }
        public string TransactionPurpose { get; set; }
        public string Category { get; set; }
        public SenderRequest Sender { get; set; }
        public RecipientRequest Recipient { get; set; }
    }
}