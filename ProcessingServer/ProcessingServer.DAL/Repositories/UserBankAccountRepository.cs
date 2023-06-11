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
    public class UserBankAccountRepository : IUserBankAccountRepository
    {
        private AppDbContext _dbContext;

        public UserBankAccountRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task<UserBankAccount> CreateAccount(UserBankAccount account)
        {
            await _dbContext.UserBankAccounts.AddAsync(account);
            return account;
        }

        public void DeleteUserBankAccount(UserBankAccount userBankAccount)
        {
            _dbContext.UserBankAccounts.Remove(userBankAccount);
        }

        public async Task<UserBankAccount> GetAccountByAccountNumber(string accountNumber)
        {
            var result = await _dbContext.UserBankAccounts.Include(account => account.Owner)
                                                  .FirstOrDefaultAsync(account => account.AccountNumber.Equals(accountNumber));
            return result;
        }

        public async Task<List<UserBankAccount>> GetAllAccounts()
        {
            var accounts = await _dbContext.UserBankAccounts.Include(account => account.Owner)
                                                            .ToListAsync();
            return accounts;
        }

        public async Task<List<UserBankAccount>> GetAllAccountsForUser(string userId)
        {
            var accounts = await _dbContext.UserBankAccounts.Include(account => account.Owner)
                                                            .Where(account => account.OwnerId.Equals(userId))
                                                            .ToListAsync();
            return accounts;
        }
    }
}
