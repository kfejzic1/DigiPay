using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Interfaces
{
    public interface IUserBankAccountRepository
    {
        Task<UserBankAccount> GetAccountByAccountNumber(string accountNumber);
        Task<UserBankAccount> CreateAccount(UserBankAccount account);
        Task<List<UserBankAccount>> GetAllAccounts();
        Task<List<UserBankAccount>> GetAllAccountsForUser(string userId);
        void DeleteUserBankAccount(UserBankAccount userBankAccount);
    }
}
