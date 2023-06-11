using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.DTO
{
    public class VoucherRequest
    {
        public double Amount { get; set; }
        public string AccountNumber { get; set; }
    }
}
