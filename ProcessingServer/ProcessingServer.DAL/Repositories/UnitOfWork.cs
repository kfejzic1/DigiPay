using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;

        public ITransactionRepository TransactionRepository { get; private set; }
        public IUserRepository UserRepository { get; private set; }
        public IUserBankAccountRepository UserBankAccountRepository { get; private set; }
        public IVoucherRepository VaucherRepository { get; private set; }
        public IVendorBankAccountRepository VendorBankAccountRepository { get; private set; }
        public IEInvoicePaymentHistoryRepository EInvoicePaymentHistoryRepository { get; private set; }

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            TransactionRepository = new TransactionRepository(appDbContext);
            UserRepository = new UserRepository(appDbContext);
            UserBankAccountRepository = new UserBankAccountRepository(appDbContext);
            VaucherRepository = new VoucherRepository(appDbContext);
            VendorBankAccountRepository = new VendorBankAccountRepository(appDbContext);
            EInvoicePaymentHistoryRepository = new EInvoicePaymentHistoryRepository(appDbContext);
        }

        public void Save()
        {
            _appDbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _appDbContext.SaveChangesAsync();
        }
    }
}
