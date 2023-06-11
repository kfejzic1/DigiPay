using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        ITransactionRepository TransactionRepository { get; }
        IUserRepository UserRepository { get; }
        IUserBankAccountRepository UserBankAccountRepository { get; }
        IVoucherRepository VaucherRepository { get; }
        IVendorBankAccountRepository VendorBankAccountRepository { get; }
        IEInvoicePaymentHistoryRepository EInvoicePaymentHistoryRepository { get; }

        void Save();
        Task SaveAsync();
    }
}
