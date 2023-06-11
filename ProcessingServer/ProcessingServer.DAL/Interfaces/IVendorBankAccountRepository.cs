using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Interfaces
{
    public interface IVendorBankAccountRepository
    {
        Task<VendorBankAccount> CreateVendorBankAccount(VendorBankAccount vendorBankAccount);
        Task<List<VendorBankAccount>> GetAllVendorBankAccounts();
        Task<List<VendorBankAccount>> GetVendorBankAccounts(int vendorId);
        void DeleteVendorBankAccount(VendorBankAccount vendorBankAccount);
    }
}
