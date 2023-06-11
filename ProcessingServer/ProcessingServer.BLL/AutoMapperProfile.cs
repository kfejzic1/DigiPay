using AutoMapper;
using ProcessingServer.BLL.DTO;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<Transaction, TransactionRequest>().ReverseMap();
            CreateMap<User, RecipientRequest>().ReverseMap();
            CreateMap<User, UserTransfer>().ReverseMap();
            CreateMap<Transaction, TransactionResponse>().ReverseMap();
            CreateMap<User, UserWithBankAccount>().ReverseMap();
            CreateMap<UserBankAccount, UserBankAccountRequest>().ReverseMap();
            CreateMap<NonPersonalBankAccountResponse, UserBankAccount>().ReverseMap();
            CreateMap<Voucher, VoucherRequest>().ReverseMap();
            CreateMap<VendorBankAccount, VendorBankAccountRequest>().ReverseMap();
            CreateMap<VendorBankAccountResponse, VendorBankAccount>().ReverseMap();
            CreateMap<VendorResponse, VendorTransfer>().ReverseMap();
        }
    }
}
