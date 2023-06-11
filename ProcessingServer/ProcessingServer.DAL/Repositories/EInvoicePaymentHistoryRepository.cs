using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Repositories
{
    public class EInvoicePaymentHistoryRepository : IEInvoicePaymentHistoryRepository
    {
        private AppDbContext _dbContext;

        public EInvoicePaymentHistoryRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task<EInvoicePaymentHistory> CreateEInvoicePaymentHistory(EInvoicePaymentHistory eInvoicePaymentHistory)
        {
            await _dbContext.EInvoicePaymentHistory.AddAsync(eInvoicePaymentHistory);
            return eInvoicePaymentHistory;
        }
    }
}
