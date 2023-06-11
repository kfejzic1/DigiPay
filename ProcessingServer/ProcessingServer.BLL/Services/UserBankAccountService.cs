using AutoMapper;
using Mysqlx.Crud;
using ProcessingServer.BLL.DTO;
using ProcessingServer.BLL.Interfaces;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.Services
{
    public class UserBankAccountService : IUserBankAccountService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdministrationService _administrationService;

        public UserBankAccountService(IUnitOfWork unitOfWork, IMapper mapper, IAdministrationService administrationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _administrationService = administrationService;
        }

        private string GenerateRandomString()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public UserBankAccount MapToAccount(UserBankAccountRequest request, List<UserBankAccount> allAccounts)
        {
            int uniqueNumber = 1;
            if (allAccounts.Count != 0)
                uniqueNumber = allAccounts.MaxBy(account => account.UserBankAccountId).UserBankAccountId + 1;
            var account = _mapper.Map<UserBankAccount>(request);
            account.AccountNumber = GenerateRandomString().ToUpper() + uniqueNumber; // Generating unique account number!
            account.CreatedAt = DateTime.Now;
            account.Credit = 0;
            account.Debit = 0;
            account.Total = 0;
            account.Currency = account.Currency.ToUpper();
            return account;
        } 

        public async Task<bool> ValidateCurrency(string token, string currencyName)
        {
            var availableCurrencies = await _administrationService.GetAvailableCurrenciesFromAdministrationApi(token);
            foreach (var currency in availableCurrencies)
                if (currency.Name.ToLower().Equals(currencyName.ToLower()))
                    return true;
            throw new Exception("Unavailable currency!");
        }

        private async Task CheckAccountExistance(User user, string currency)
        {
            var allAccounts = await _unitOfWork.UserBankAccountRepository.GetAllAccountsForUser(user.UserId);
            var currencyAccount = allAccounts.Where(account => account.Currency.ToLower().Equals(currency.ToLower()))
                                             .ToList();
            if (currencyAccount.Count != 0)
                throw new Exception("User already has " + currency.ToUpper() + " account!");
        }

        public async Task<UserBankAccount> CreateAccount(string token, UserBankAccountRequest accountRequest)
        {
            var owner = await _administrationService.GetUserFromAdministrationApi(token, _mapper); // Returns type User!
            await CheckAccountExistance(owner, accountRequest.Currency);
            var ownerLocal = await _unitOfWork.UserRepository.GetUserById(owner.UserId);
            await ValidateCurrency(token, accountRequest.Currency);
            var allAccounts = await _unitOfWork.UserBankAccountRepository.GetAllAccounts();
            var account = MapToAccount(accountRequest, allAccounts);
            account.Currency = account.Currency.ToUpper();
            account.Owner = ownerLocal == null ? owner : ownerLocal;
            account.OwnerId = ownerLocal == null ? owner.UserId : ownerLocal.UserId;
            var createdAccount = await _unitOfWork.UserBankAccountRepository.CreateAccount(account);
            await _unitOfWork.SaveAsync();
            return createdAccount;
        }

        public async Task<List<UserBankAccount>> GetAllAccountsForUser(string token)
        {
            var owner = await _administrationService.GetUserFromAdministrationApi(token, _mapper);
            var accounts = await _unitOfWork.UserBankAccountRepository.GetAllAccountsForUser(owner.UserId);
            if (accounts.Count == 0)
                throw new Exception("User does not have any account!");
            return accounts;
        }

        public async Task<List<NonPersonalBankAccountResponse>> GetAllAccounts(string token)
        {
            await _administrationService.ValidateToken(token);
            var allAccounts = await _unitOfWork.UserBankAccountRepository.GetAllAccounts();
            var result = _mapper.Map<List<NonPersonalBankAccountResponse>>(allAccounts);
            return result;
        }

        public async Task<UserBankAccount> CreateAccount(string token, UserBankAccountRequest accountRequest, string userId)
        {
            var isAdmin = await _administrationService.IsUserAdmin(token);
            if (!isAdmin)
                throw new Exception("Only ADMIN can create bank account for someone else!");
            var owner = await _administrationService.GetUserFromAdministrationApiById(token, userId, _mapper);
            await CheckAccountExistance(owner, accountRequest.Currency);
            var ownerLocal = await _unitOfWork.UserRepository.GetUserById(owner.UserId);
            await ValidateCurrency(token, accountRequest.Currency);
            var allAccounts = await _unitOfWork.UserBankAccountRepository.GetAllAccounts();
            var account = MapToAccount(accountRequest, allAccounts);
            account.Currency = account.Currency.ToUpper();
            account.Owner = ownerLocal == null ? owner : ownerLocal;
            account.OwnerId = ownerLocal == null ? owner.UserId : ownerLocal.UserId;
            var createdAccount = await _unitOfWork.UserBankAccountRepository.CreateAccount(account);
            await _unitOfWork.SaveAsync();
            return createdAccount;
        }

        public async Task DeleteAllAccountsForUser(string token)
        {
            var user = await _administrationService.GetUserFromAdministrationApi(token, _mapper);
            var allAccountsForUser = await _unitOfWork.UserBankAccountRepository.GetAllAccountsForUser(user.UserId);
            foreach (var account in allAccountsForUser)
            {
                _unitOfWork.UserBankAccountRepository.DeleteUserBankAccount(account);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
