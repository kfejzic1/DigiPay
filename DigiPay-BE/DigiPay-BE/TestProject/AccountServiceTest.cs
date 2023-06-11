using System;
using AdministrationAPI.Contracts.Requests.Users;
using AdministrationAPI.Data;
using AdministrationAPI.Models;
using AdministrationAPI.Services;
using Moq;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AdministrationAPI.Contracts.Requests;
using AdministrationAPI.Contracts.Responses;
using AdministrationAPI.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Routing;
using Moq.EntityFrameworkCore;
using AdministrationAPI.Services.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using AdministrationAPI.Models.Vendor;
using AdministrationAPI.Models.Voucher;


namespace TestProject
{
	public class AccountServiceTest
	{
        private readonly ITestOutputHelper output;
        private Mock<AppDbContext> _context = new Mock<AppDbContext>();
        private List<User> users = new List<User>();
        private List<Currency> currencies = new List<Currency>();
        private List<Account> accounts = new List<Account>();
        private List<AccountCreationRequest> requests = new List<AccountCreationRequest>();


        public AccountServiceTest(ITestOutputHelper output)
        {
            this.output = output;

            users = new List<User>()
                {
                    new User() { FirstName = "Testing", LastName = "User", UserName = "testingUser", NormalizedUserName = "TESTINGUSER", ConcurrencyStamp = "1", Email = "kfejzic1@etf.unsa.ba", NormalizedEmail = "KFEJZIC1@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "062229993", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false },
                    new User() { FirstName = "Admin", LastName = "User", UserName = "adminUser", NormalizedUserName = "ADMINUSER", ConcurrencyStamp = "1", Email = "fejza2806@gmail.com", NormalizedEmail = "FEJZA2806@GMAIL.COM", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "062518214", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = false, LockoutEnabled = false },
                    new User() { FirstName = "Elvedin", LastName = "Smajic", UserName = "esmajic2", NormalizedUserName = "ESMAJIC2", ConcurrencyStamp = "1", Email = "esmajic2@etf.unsa.ba", NormalizedEmail = "ESMAJIC2@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "11111", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false },
                    new User() { FirstName = "Admir", LastName = "Mehmedagic", UserName = "amehmedagi1", NormalizedUserName = "AMEHMEDAGI1", ConcurrencyStamp = "1", Email = "amehmedagi1@etf.unsa.ba", NormalizedEmail = "AMEHMEDAGI1@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "11111", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false },
                    new User() { FirstName = "Merjem", LastName = "Becirovic", UserName = "mbecirovic3", NormalizedUserName = "MBECIROVIC3", ConcurrencyStamp = "1", Email = "mbecirovic3@etf.unsa.ba", NormalizedEmail = "MBECIROVIC3@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "11111", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false },
                    new User() { FirstName = "Dzenis", LastName = "Muhic", UserName = "dmuhic1", NormalizedUserName = "DMUHIC1", ConcurrencyStamp = "1", Email = "dmuhic1@etf.unsa.ba", NormalizedEmail = "DMUHIC1@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "11111", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false },
                    new User() { FirstName = "Ema", LastName = "Mekic", UserName = "emekic2", NormalizedUserName = "EMEKIC2", ConcurrencyStamp = "1", Email = "emekic2@etf.unsa.ba", NormalizedEmail = "EMEKIC2@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "11111", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false },
                    new User() { FirstName = "Almina", LastName = "Brulic", UserName = "abrulic1", NormalizedUserName = "ABRULIC1", ConcurrencyStamp = "1", Email = "abrulic1@etf.unsa.ba", NormalizedEmail = "ABRULIC1@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "11111", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false },
                    new User() { FirstName = "Elvir", LastName = "Vlahovljak", UserName = "evlahovlja1", NormalizedUserName = "EVLAHOVLJA1", ConcurrencyStamp = "1", Email = "evlahovlja1@etf.unsa.ba", NormalizedEmail = "EVLAHOVLJA1@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAEL+9sxZQaY0F4wxS0N24IGTB+z6oIeFEX8wQgqdzsskd4XC/oE+2YWgxc/LwTsx+dw==", PhoneNumber = "061904086", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = false, LockoutEnabled = false },
                    new User() { Id = "ID", FirstName = "Test", LastName = "Test", UserName = "test", NormalizedUserName = "TEST", ConcurrencyStamp = "1", Email = "test@gmail.com", NormalizedEmail = "TEST@GMAIL.COM", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAEL+9sxZQaY0F4wxS0N24IGTB+z6oIeFEX8wQgqdzsskd4XC/oE+2YWgxc/LwTsx+dw==", PhoneNumber = "12345", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = false, LockoutEnabled = false }
                };

            currencies = new List<Currency>()
                {
                    new Currency() { Id = "1", Country = "BIH", Name = "BAM" },
                    new Currency() { Id = "2", Country = "USA", Name = "USD" },
                    new Currency() { Id = "3", Country = "DEU", Name = "EUR" },
                    new Currency() { Id = "4", Country = "SWI", Name = "CHF" }
                };

            requests = new List<AccountCreationRequest>()
                {
                    new AccountCreationRequest(){Id = 1, UserId = users[0].Id, CurrencyId = currencies[0].Id, Approved = false},
                    new AccountCreationRequest(){Id = 2, UserId = users[1].Id, CurrencyId = currencies[1].Id, Approved = false},
                    new AccountCreationRequest(){Id = 3, UserId = users[2].Id, CurrencyId = currencies[2].Id, Approved = false}
                };


            accounts = new List<Account>()
                {
                    new Account(){Id = 1, User = users[0] ,UserId = users[0].Id, AccountNumber = "1", Currency = currencies[0],  CurrencyId = currencies[0].Id, Description = "Acc1", RequestId = 1},
                    new Account(){Id = 2, User = users[1] ,UserId = users[1].Id, AccountNumber = "2", Currency = currencies[1],  CurrencyId = currencies[1].Id, Description = "Acc2", RequestId = 2},
                    new Account(){Id = 3, User = users[2] ,UserId = users[2].Id, AccountNumber = "3", Currency = currencies[2],  CurrencyId = currencies[2].Id, Description = "Acc3", RequestId = 3}
                };


            _context.Setup(x => x.Accounts).ReturnsDbSet(accounts);
            _context.Setup(x => x.Currencies).ReturnsDbSet(currencies);
            _context.Setup(x => x.Users).ReturnsDbSet(users);
            _context.Setup(x => x.AccountCreationRequests).ReturnsDbSet(requests);
        }



        [Fact]
        public void GetAllAccountsTest()
        {
            var service = new AccountService(_context.Object);
            List<Account> req = service.GetAllAccounts();
            Assert.Equal(3, req.Count);

        }


        [Fact]
        public void GetAllRequestsTest()
        {
            var service2 = new AccountService(_context.Object);
            List<AccountCreationRequest> req2 = service2.GetAllRequests();
            Assert.Equal(3, req2.Count);
        }


        [Fact]
        public async Task CreateUserAccountCreationRequestTest()
        {
            var service = new AccountService(_context.Object);
            AccountCreationRequestCreateRequest novi = new AccountCreationRequestCreateRequest() { CurrencyId = currencies[3].Id, Description = "NestoDugo2", RequestDocumentPath = "Neki", UserId = users[0].Id };
            var kreirani = service.CreateUserAccountCreationRequest(novi);
            Assert.NotNull(kreirani);
            Assert.Equal(novi.UserId, kreirani.Result.UserId);
            Assert.ThrowsAsync<Exception>(async () => await service.CreateUserAccountCreationRequest(novi));

            AccountCreationRequestCreateRequest nepostoji = new AccountCreationRequestCreateRequest() { CurrencyId = currencies[3].Id, Description = "NestoDugo2", RequestDocumentPath = "Neki", UserId = users[1].Id };
            Assert.ThrowsAsync<Exception>(async () => await service.CreateUserAccountCreationRequest(nepostoji));


            _context.Verify(x => x.AccountCreationRequests.Add(It.IsAny<AccountCreationRequest>()), Times.Exactly(3));
        }



        [Fact]
        public void GetUserAccountCreationRequestsTest()
        {
            var service = new AccountService(_context.Object);
            var kreirani = service.GetUserAccountCreationRequests(users[0].Id);
            Assert.NotNull(kreirani);
            Assert.Single(kreirani);
        }
    }
}

