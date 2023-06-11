using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Interfaces
{
    public interface IEInvoicePaymentHistoryRepository
    {
        Task<EInvoicePaymentHistory> CreateEInvoicePaymentHistory(EInvoicePaymentHistory eInvoicePaymentHistory);
    }
}
