using AutoMapper;
using ProcessingServer.BLL.DTO;
using ProcessingServer.BLL.Interfaces;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProcessingServer.BLL.Services
{
    public class AdminstrationService : IAdministrationService
    {
        const string APPLICATION_BACK_URL = "http://localhost:5051";
        public async Task<string> ValidateToken(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(APPLICATION_BACK_URL + "/api/User/validate-token?token=" + token);
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<TokenValidationResult>();
                return user.Username;
            }
            throw new AuthenticationException("Invalid token!");
        }

        public async Task<List<UserTransfer>> GetAllUsersFromAdministrationApi(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(APPLICATION_BACK_URL + "/api/User/all");
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<UserTransfer>>();
                foreach (var user in users)
                    user.Type = user.Type == null ? "PERSON" : user.Type.ToUpper();
                return users;
            }
            throw new AuthenticationException("Invalid token!");
        }

        // Gets user who activated HTTP request!
        public async Task<User> GetUserFromAdministrationApi(string token, IMapper _mapper)
        {
            var username = await ValidateToken(token);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(APPLICATION_BACK_URL + "/api/User/" + username);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var userTransfer = await response.Content.ReadFromJsonAsync<UserTransfer>();
                var user = _mapper.Map<User>(userTransfer);
                user.Name = userTransfer.FirstName + " " + userTransfer.LastName;
                user.UserId = userTransfer.Id;
                user.Type = user.Type == null ? "PERSON" : user.Type.ToUpper();
                return user;
            }
            throw new AuthenticationException("Unable to get sender details!");
        }

        // This recipient is User (not Vendor)!
        public async Task<User> GetRecipientFromAdministrationApi(string token, string name, IMapper _mapper)
        {
            var allUsers = await GetAllUsersFromAdministrationApi(token);
            try
            {
                var recipientTransfer = allUsers.Find(user => name.ToLower() == user.FirstName.ToLower() + " " + user.LastName.ToLower());
                var recipient = _mapper.Map<User>(recipientTransfer);
                recipient.Name = recipientTransfer.FirstName + " " + recipientTransfer.LastName;
                recipient.UserId = recipientTransfer.Id;
                recipient.Type = recipient.Type == null ? "PERSON" : recipient.Type.ToUpper();
                return recipient;
            }
            catch (Exception exception)
            {
                throw new Exception("Invalid recipient name or account number!");
            }
        }

        public async Task<List<ExchangeRate>> GetExchangeRatesFromAdministrationApi(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(APPLICATION_BACK_URL + "/api/ExchangeRate");
            if (response.IsSuccessStatusCode)
            {
                var exchangeRates = await response.Content.ReadFromJsonAsync<List<ExchangeRate>>();
                return exchangeRates;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                throw new NullReferenceException();
            throw new AuthenticationException("Invalid token!");
        }

        public async Task<List<Currency>> GetAvailableCurrenciesFromAdministrationApi(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(APPLICATION_BACK_URL + "/api/ExchangeRate/currency");
            if (response.IsSuccessStatusCode)
            {
                var currencies = await response.Content.ReadFromJsonAsync<List<Currency>>();
                return currencies;
            }
            throw new AuthenticationException("Invalid token!");
        }

        public async Task<List<VendorTransfer>> GetAllVendorsFromAdministrationApi(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(APPLICATION_BACK_URL + "/api/Vendor");
            if (response.IsSuccessStatusCode)
            {
                var vendors = await response.Content.ReadFromJsonAsync<List<VendorTransfer>>();
                return vendors;
            }
            throw new AuthenticationException("Invalid token!");
        }

        public async Task<VendorTransfer> GetVendorFromAdministrationApi(string token, VendorRequest vendor)
        {
            var vendors = await GetAllVendorsFromAdministrationApi(token);
            var result = vendors.Where(element => element.Id == vendor.VendorId).FirstOrDefault();
            if (result == null)
                throw new ArgumentNullException("Vendor with provided ID does not exist!");
            return result;
        }

        public async Task<User> GetUserFromAdministrationApiById(string token, string userId, IMapper _mapper)
        {
            var allUsers = await GetAllUsersFromAdministrationApi(token);
            try
            {
                var userTransfer = allUsers.Find(user => user.Id.Equals(userId));
                var user = _mapper.Map<User>(userTransfer);
                user.Name = userTransfer.FirstName + " " + userTransfer.LastName;
                user.UserId = userTransfer.Id;
                user.Type = user.Type == null ? "PERSON" : user.Type.ToUpper();
                return user;
            }
            catch (Exception exception)
            {
                throw new Exception("User with provided ID does not exist!");
            }
        }

        public async Task<bool> IsUserAdmin(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(APPLICATION_BACK_URL + "/api/User/validate-token?token=" + token);
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserAdminCheck>();
                foreach (var role in user.Roles)
                    if (role.ToLower().Equals("admin"))
                        return true;
                return false;
            }
            throw new AuthenticationException("Invalid token!");
        }
    }
}
