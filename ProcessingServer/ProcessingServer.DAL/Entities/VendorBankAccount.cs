using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Entities
{
    public class VendorBankAccount
    {
        public int VendorBankAccountId { get; set; }
        public string AccountNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Currency { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public double Total { get; set; }
        public int OwnerId { get; set; } // This is ID of vendor (can be found in Administration database)!

        [ForeignKey("User")]
        public string CreatorId { get; set; }
        public User Creator { get; set; }
    }
}
