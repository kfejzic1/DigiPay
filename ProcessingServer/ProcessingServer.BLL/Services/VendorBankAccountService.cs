using AutoMapper;
using Mysqlx.Crud;
using ProcessingServer.BLL.DTO;
using ProcessingServer.BLL.Interfaces;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Interfaces;
using ProcessingServer.DAL.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.Services
{
    public class VendorBankAccountService : IVendorBankAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdministrationService _administrationService;

        public VendorBankAccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private bool IsUserAssignedToVendor(VendorTransfer vendor, User user)
        {
            bool isAssigned = false;
            vendor.AssignedUsers.ForEach(assignedUser =>
            {
                if (assignedUser.Id.Equals(user.UserId))
                    isAssigned = true;
            });
            return isAssigned;
        }

        private async Task<bool> ValidateCurrency(string token, string currencyName)
        {
            var availableCurrencies = await _administrationService.GetAvailableCurrenciesFromAdministrationApi(token);
            foreach (var currency in availableCurrencies)
                if (currency.Name.ToLower().Equals(currencyName.ToLower()))
                    return true;
            throw new Exception("Unavailable currency!");
        }

        private string GenerateRandomString()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async Task<int> GetUniqueIdentifier()
        {
            var allAcounts = await _unitOfWork.VendorBankAccountRepository.GetAllVendorBankAccounts();
            return allAcounts.Count == 0 ? 1 : allAcounts.Count + 1;
        }

        private async Task<string> GenerateUniqueVendorBankAccountNumber()
        {
            var uniqueNumber = await GetUniqueIdentifier();
            return GenerateRandomString().ToLower() + uniqueNumber;
        }

        private async Task<VendorBankAccount> MapToVendorBankAccount(VendorBankAccountRequest vendorBankAccountRequest)
        {
            var vendorBankAccount = _mapper.Map<VendorBankAccount>(vendorBankAccountRequest);
            vendorBankAccount.AccountNumber = await GenerateUniqueVendorBankAccountNumber();
            vendorBankAccount.CreatedAt = DateTime.Now;
            vendorBankAccount.Debit = 0;
            vendorBankAccount.Total = 0;
            vendorBankAccount.Credit = 0;
            vendorBankAccount.OwnerId = vendorBankAccountRequest.Vendor.VendorId;
            return vendorBankAccount;
        }

        private async Task CheckAccountExistance(int vendorId, string currency)
        {
            var vendorBankAccounts = await _unitOfWork.VendorBankAccountRepository.GetVendorBankAccounts(vendorId);
            vendorBankAccounts.ForEach(account =>
            {
                if (account.Currency.ToLower().Equals(currency.ToLower()))
                    throw new Exception($"Vendor already has {currency.ToUpper()} bank account!");
            });
        }

        private async Task<VendorBankAccountResponse> MapToVendorBankAccountResponse(string token, VendorBankAccount vendorBankAccount)
        {
            var vendorRequest = new VendorRequest()
            {
                VendorId = vendorBankAccount.OwnerId
            };
            var vendor = await _administrationService.GetVendorFromAdministrationApi(token, vendorRequest); 
            var vendorBankAccountResponse = _mapper.Map<VendorBankAccountResponse>(vendorBankAccount);
            vendorBankAccountResponse.Owner = _mapper.Map<VendorResponse>(vendor);
            return vendorBankAccountResponse;
        }

        private async Task<List<VendorBankAccountResponse>> MapToVendorBankAccountResponse(string token, List<VendorBankAccount> vendorBankAccounts)
        {
            List<VendorBankAccountResponse> vendorBankAccountResponses = new List<VendorBankAccountResponse>();
            foreach (var account in vendorBankAccounts)
                vendorBankAccountResponses.Add(await MapToVendorBankAccountResponse(token, account));
            return vendorBankAccountResponses;
        }

        public async Task<VendorBankAccountResponse> CreateVendorBankAccount(string token, VendorBankAccountRequest vendorBankAccountRequest)
        {
            var creator = await _administrationService.GetUserFromAdministrationApi(token, _mapper);
            var vendorTransfer = await _administrationService.GetVendorFromAdministrationApi(token, vendorBankAccountRequest.Vendor);
            if (!IsUserAssignedToVendor(vendorTransfer, creator))
                throw new Exception($"User {creator.Name} is not authorized to create bank account for this vendor!");
            await ValidateCurrency(token, vendorBankAccountRequest.Currency);
            await CheckAccountExistance(vendorTransfer.Id, vendorBankAccountRequest.Currency);
            var creatorLocal = await _unitOfWork.UserRepository.GetUserById(creator.UserId);
            var vendorBankAccount = MapToVendorBankAccount(vendorBankAccountRequest).Result;
            vendorBankAccount.Creator = creatorLocal == null ? creator : creatorLocal;
            vendorBankAccount.CreatorId = creatorLocal == null ? creator.UserId : creatorLocal.UserId;
            await _unitOfWork.VendorBankAccountRepository.CreateVendorBankAccount(vendorBankAccount);
            await _unitOfWork.SaveAsync();
            var result = await MapToVendorBankAccountResponse(token, vendorBankAccount);
            return result;
        }

        public async Task<List<VendorBankAccountResponse>> GetBankAccountsForVendor(string token, string vendorId)
        {
            var user = await _administrationService.GetUserFromAdministrationApi(token, _mapper);
            var vendorTransfer = await _administrationService.GetVendorFromAdministrationApi(token, new VendorRequest()
            {
                VendorId = int.Parse(vendorId)
            });
            var vendorBankAccounts = await _unitOfWork.VendorBankAccountRepository.GetVendorBankAccounts(vendorTransfer.Id);
            return await MapToVendorBankAccountResponse(token, vendorBankAccounts);
        }

        public async Task DeleteVendorBankAccounts(string token, string vendorId)
        {
            await _administrationService.ValidateToken(token);
            var vendorBankAccounts = await _unitOfWork.VendorBankAccountRepository.GetVendorBankAccounts(int.Parse(vendorId));
            vendorBankAccounts.ForEach(account =>
            {
                _unitOfWork.VendorBankAccountRepository.DeleteVendorBankAccount(account);
            });
            await _unitOfWork.SaveAsync();
            return;
        }
    }
}
