using Microsoft.EntityFrameworkCore;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Repositories
{
    public class VendorBankAccountRepository : IVendorBankAccountRepository
    {
        private AppDbContext _dbContext;

        public VendorBankAccountRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task<VendorBankAccount> CreateVendorBankAccount(VendorBankAccount vendorBankAccount)
        {
            await _dbContext.VendorBankAccounts.AddAsync(vendorBankAccount);
            return vendorBankAccount;
        }

        public void DeleteVendorBankAccount(VendorBankAccount vendorBankAccount)
        {
            _dbContext.VendorBankAccounts.Remove(vendorBankAccount);
        }

        public async Task<List<VendorBankAccount>> GetAllVendorBankAccounts()
        {
            var accounts = await _dbContext.VendorBankAccounts.Include(account => account.Creator)
                                                              .ToListAsync();
            return accounts;
        }

        public async Task<List<VendorBankAccount>> GetVendorBankAccounts(int vendorId)
        {
            var accounts = await _dbContext.VendorBankAccounts.Include(vendorAccount => vendorAccount.Creator)
                                                              .Where(vendorAccount => vendorAccount.OwnerId == vendorId)
                                                              .ToListAsync();
            return accounts;
        }
    }
}
