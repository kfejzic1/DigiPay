using ProcessingServer.BLL.DTO;
using ProcessingServer.BLL.Services;
using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.Interfaces
{
    public interface IUserBankAccountService
    {
        Task<UserBankAccount> CreateAccount(string token, UserBankAccountRequest accountRequest);
        Task<UserBankAccount> CreateAccount(string token, UserBankAccountRequest accountRequest, string userId);
        Task<List<UserBankAccount>> GetAllAccountsForUser(string token);
        Task<List<NonPersonalBankAccountResponse>> GetAllAccounts(string token);
        Task DeleteAllAccountsForUser(string token);
    }
}
