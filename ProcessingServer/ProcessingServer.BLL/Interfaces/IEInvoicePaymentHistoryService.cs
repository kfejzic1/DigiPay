using ProcessingServer.BLL.DTO.EInvoicePayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.Interfaces
{
    public interface IEInvoicePaymentHistoryService
    {
        Task<Object> ExecuteInvoicePayment(string token, EInvoiceRequest eInvoiceRequest);
    }
}
