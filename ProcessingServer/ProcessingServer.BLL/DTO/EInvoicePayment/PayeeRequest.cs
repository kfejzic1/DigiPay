using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.DTO.EInvoicePayment
{
    public class PayeeRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string BankAccountNumber { get; set; }
    }
}
