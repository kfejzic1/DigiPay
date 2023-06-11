using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Entities
{
    public class UserBankAccount
    {
        public int UserBankAccountId { get; set; }
        public string AccountNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Currency { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public double Total { get; set; }

        [ForeignKey("User")]
        public string OwnerId { get; set; }
        public User Owner { get; set; }
    }
}
