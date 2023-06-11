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
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdministrationService _administrationService;

        public VoucherService(IUnitOfWork unitOfWork, IMapper mapper, IAdministrationService administrationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _administrationService = administrationService;
        }

        public async Task<Voucher> ExecuteVoucherRedemption(string token,VoucherRequest voucherRequest)
        {
            var user = await _administrationService.GetUserFromAdministrationApi(token, _mapper);
            var account = await _unitOfWork.UserBankAccountRepository.GetAccountByAccountNumber(voucherRequest.AccountNumber);
            if (account == null)
                throw new Exception("Account with provided account number does not exist!");
            /*if (!account.OwnerId.Equals(user.UserId))
                throw new Exception("User does not own account with provided account number!");*/
            if (voucherRequest.Amount <= 0)
                throw new Exception("Amount must be a positive number!");
            account.Credit += voucherRequest.Amount;
            account.Total += voucherRequest.Amount;
            var voucher = _mapper.Map<Voucher>(voucherRequest);
            var userLocal = await _unitOfWork.UserRepository.GetUserById(user.UserId);
            voucher.UserBankAccountId = account.UserBankAccountId;
            voucher.Account = account;
            voucher.Activator = userLocal == null ? user : userLocal;
            voucher.ActivatorId = userLocal == null ? user.UserId : userLocal.UserId;
            await _unitOfWork.VaucherRepository.CreateVaucher(voucher);
            await _unitOfWork.SaveAsync();
            return voucher;
        }
    }
}
