using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.DTO.EInvoicePayment
{
    public class EInvoiceRequest
    {
        public PayerRequest Payer { get; set; }
        public PayeeRequest Payee { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
    }
}
