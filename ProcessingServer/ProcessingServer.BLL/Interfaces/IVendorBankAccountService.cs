using ProcessingServer.BLL.DTO;
using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.Interfaces
{
    public interface IVendorBankAccountService
    {
        Task<VendorBankAccountResponse> CreateVendorBankAccount(string token, VendorBankAccountRequest vendorBankAccountRequest);
        Task<List<VendorBankAccountResponse>> GetBankAccountsForVendor(string token, string vendorId);
        Task DeleteVendorBankAccounts(string token, string vendorId);
    }
}
