using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Entities
{
    public class EInvoicePaymentHistory
    {
        public int EInvoicePaymentHistoryId { get; set; }

        [ForeignKey("UserBankAccount")]
        public int PayerBankAccountId { get; set; }
        public UserBankAccount PayerBankAccount { get; set; }

        [ForeignKey("VendorBankAccount")]
        public int PayeeBankAccountId { get; set; }
        public VendorBankAccount PayeeBankAccount { get; set; }

        public string PayerAddress { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }

    }
}
