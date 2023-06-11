using AdministrationAPI.Contracts.Requests;
using AdministrationAPI.Contracts.Requests.Exchange;
using AdministrationAPI.Contracts.Requests.Vendors;
using AdministrationAPI.Contracts.Responses;
using AdministrationAPI.Data;
using AdministrationAPI.Models;
using AdministrationAPI.Models.Vendor;
using AdministrationAPI.Models.Voucher;
using AdministrationAPI.Services;
using AdministrationAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace TestProject
{
    public class ExchangeTests
    {
        private List<User> users;

        private Mock<AppDbContext> _context = new Mock<AppDbContext>();

        private readonly ITestOutputHelper _output;

        public ExchangeTests(ITestOutputHelper output)
        {
            _output = output;
            users = new List<User>()
            {
                 new User() { FirstName = "Testing", LastName = "User", UserName = "testingUser", NormalizedUserName = "TESTINGUSER", ConcurrencyStamp = "1", Email = "kfejzic1@etf.unsa.ba", NormalizedEmail = "KFEJZIC1@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "062229993", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false },
                 new User() { FirstName = "Admin", LastName = "User", UserName = "adminUser", NormalizedUserName = "ADMINUSER", ConcurrencyStamp = "1", Email = "fejza2806@gmail.com", NormalizedEmail = "FEJZA2806@GMAIL.COM", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "062518214", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = false, LockoutEnabled = false }
            };


        }

        [Fact]
        public void CreateAccountTest()
        {
            var service = new ExchangeService(_context.Object);
            ExchangeAccountRequest req = new ExchangeAccountRequest { Currency = "BAM", BankName = "BBI", Description = "Test exchange" };
            var account = service.CreateAccount(req,"");

            Assert.NotNull(account);
        }
        [Fact]
        public void CreateAccountTest2()
        {
            var service = new ExchangeService(_context.Object);
            ExchangeAccountRequest req = new ExchangeAccountRequest { Currency = "BAM", BankName = "BBI", Description = "Test exchange" };
            var account = service.CreateAccount(req, "12345678");

            Assert.NotNull(account);
        }

        [Fact]
        public void GetUserAccountsTest()
        {
            var service = new ExchangeService(_context.Object);
            var accounts = service.GetUserAccounts("");

            Assert.NotNull(accounts);
        }
        [Fact]
        public void GetUserAccountsTest1()
        {
            var service = new ExchangeService(_context.Object);
            var accounts = service.GetUserAccounts("12345678");

            Assert.NotNull(accounts);
        }

        [Fact]
        public void GetAllAccountsTest()
        {
            var service = new ExchangeService(_context.Object);
            var accounts = service.GetAllAccounts("");

            Assert.NotNull(accounts);
        }
        [Fact]
        public void GetAllAccountsTest1()
        {
            var service = new ExchangeService(_context.Object);
            var accounts = service.GetAllAccounts("12345678");

            Assert.NotNull(accounts);
        }
        [Fact]
        public void MakeTransaction()
        {
            var service = new ExchangeService(_context.Object);
            var req = new TransactionRequest();
            var accounts = service.MakeTransaction(req,"");

            Assert.NotNull(accounts);
        }
        [Fact]
        public void MakeTransaction1()
        {
            var req = new TransactionRequest();
            var service = new ExchangeService(_context.Object);
            var accounts = service.MakeTransaction(req,"12345678");

            Assert.NotNull(accounts);
        }
    }

}