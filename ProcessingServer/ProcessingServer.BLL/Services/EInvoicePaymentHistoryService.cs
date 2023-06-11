using AutoMapper;
using ProcessingServer.BLL.DTO.EInvoicePayment;
using ProcessingServer.BLL.Interfaces;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.Services
{
    public class EInvoicePaymentHistoryService : IEInvoicePaymentHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdministrationService _administrationService;

        public EInvoicePaymentHistoryService(IUnitOfWork unitOfWork, IMapper mapper, IAdministrationService administrationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _administrationService = administrationService;
        }

        private async Task<UserBankAccount> CheckPayer(User tokenOwner, PayerRequest payer, string currency)
        {
            if (!tokenOwner.Name.ToLower().Equals(payer.Name.ToLower()))
                throw new Exception("Unable to pay someone else's invoice!");
            var allUserAccounts = await _unitOfWork.UserBankAccountRepository.GetAllAccountsForUser(tokenOwner.UserId);
            foreach (var account in allUserAccounts)
                if (account.Currency.ToLower().Equals(currency.ToLower()))
                    return account;
            throw new Exception($"Payer does not own {currency.ToUpper()} currency account!");
        }

        private async Task<VendorBankAccount> CheckPayee(PayeeRequest payee, string currency, string token)
        {
            var allVendorBankAccounts = await _unitOfWork.VendorBankAccountRepository.GetAllVendorBankAccounts();
            var allVendors = await _administrationService.GetAllVendorsFromAdministrationApi(token);
            var vendorId = -1;
            foreach (var vendor in allVendors)
                if (vendor.Name.ToLower().Equals(payee.Name.ToLower()) && vendor.Address.Equals(payee.Address))
                {
                    vendorId = vendor.Id;
                    break;
                }
            if (vendorId == -1)
                throw new Exception("Vendor with provided name or address does not exist!");
            foreach (var vendorBankAccount in allVendorBankAccounts)
                if (vendorBankAccount.Currency.ToLower().Equals(currency.ToLower()) && vendorBankAccount.AccountNumber.Equals(payee.BankAccountNumber))
                    return vendorBankAccount;
            throw new Exception("Vendor does not own corresponding bank account!");
        }

        private EInvoicePaymentHistory CreateInvoicePaymentHistory(UserBankAccount payerBankAccount, VendorBankAccount payeeBankAccount, EInvoiceRequest eInvoice)
        {
            var history = new EInvoicePaymentHistory()
            {
                PayerBankAccountId = payerBankAccount.UserBankAccountId,
                PayerBankAccount = payerBankAccount,
                PayeeBankAccountId = payeeBankAccount.VendorBankAccountId,
                PayeeBankAccount = payeeBankAccount,
                PayerAddress = eInvoice.Payer.Address,
                Description = eInvoice.Description,
                Reference = eInvoice.Reference,
                Amount = eInvoice.Amount,
                Currency = eInvoice.Currency
            };
            return history;
        }

        private void Pay(UserBankAccount payerBankAccount, VendorBankAccount payeeBankAccount, double amount)
        {
            payerBankAccount.Total -= amount;
            payerBankAccount.Debit -= amount;
            payeeBankAccount.Total += amount;
            payeeBankAccount.Credit += amount;
        }

        public async Task<object> ExecuteInvoicePayment(string token, EInvoiceRequest eInvoiceRequest)
        {
            var payer = await _administrationService.GetUserFromAdministrationApi(token, _mapper);
            var payerBankAccount = await CheckPayer(payer, eInvoiceRequest.Payer, eInvoiceRequest.Currency);
            var payeeBankAccount = await CheckPayee(eInvoiceRequest.Payee, eInvoiceRequest.Currency, token);
            if (eInvoiceRequest.Amount < 0)
                throw new Exception("Amount must be positive number!");
            Pay(payerBankAccount, payeeBankAccount, eInvoiceRequest.Amount);
            var eInvoiceHistory = CreateInvoicePaymentHistory(payerBankAccount, payeeBankAccount, eInvoiceRequest);
            var result = await _unitOfWork.EInvoicePaymentHistoryRepository.CreateEInvoicePaymentHistory(eInvoiceHistory);
            await _unitOfWork.SaveAsync();
            return result;
        }
    }
}
