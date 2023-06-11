using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.DTO
{
    public class VendorBankAccountResponse
    {
        public string AccountNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Currency { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public double Total { get; set; }
        public VendorResponse Owner { get; set; }
        public User Creator { get; set; }
    }
}