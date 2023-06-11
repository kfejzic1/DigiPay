using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.DTO
{
    public class UserWithBankAccount
    {
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string PhoneNumber { get; set; }
        public string Type { get; set; }
    }
}
