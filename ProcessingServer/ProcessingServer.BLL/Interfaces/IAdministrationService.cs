using AutoMapper;
using ProcessingServer.BLL.DTO;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.Interfaces
{
    public interface IAdministrationService
    {
        public Task<string> ValidateToken(string token);
        public Task<List<UserTransfer>> GetAllUsersFromAdministrationApi(string token);

        public Task<User> GetUserFromAdministrationApi(string token, IMapper _mapper);

        // This recipient is User (not Vendor)!
        public Task<User> GetRecipientFromAdministrationApi(string token, string name, IMapper _mapper);

        public Task<List<ExchangeRate>> GetExchangeRatesFromAdministrationApi(string token);

        public Task<List<Currency>> GetAvailableCurrenciesFromAdministrationApi(string token);

        public Task<List<VendorTransfer>> GetAllVendorsFromAdministrationApi(string token);

        public Task<VendorTransfer> GetVendorFromAdministrationApi(string token, VendorRequest vendor);

        public Task<User> GetUserFromAdministrationApiById(string token, string userId, IMapper _mapper);

        public Task<bool> IsUserAdmin(string token);
    }
}
