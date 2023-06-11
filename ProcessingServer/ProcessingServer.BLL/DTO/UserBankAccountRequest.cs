using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.DTO
{
    public class UserBankAccountRequest
    {
        public string Currency { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
    }
}
