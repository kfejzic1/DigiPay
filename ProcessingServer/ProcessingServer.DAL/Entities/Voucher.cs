using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Entities
{
    public class Voucher
    {
        public int VoucherId { get; set; }
        public double Amount { get; set; }

        [ForeignKey("UserBankAccount")]
        public int UserBankAccountId { get; set; }
        public UserBankAccount Account { get; set; }

        [ForeignKey("User")]
        public string ActivatorId { get; set; }
        public User Activator { get; set; }
    }
}
