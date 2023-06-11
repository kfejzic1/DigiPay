using AdministrationAPI.Contracts.Requests;
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
    public class RedeemVoucherTest
    {

        private List<User> users;

        private readonly ITestOutputHelper _output;
        public RedeemVoucherTest(ITestOutputHelper output)
        {
            users = new List<User>()
            {
                 new User() { FirstName = "Testing", LastName = "User", UserName = "testingUser", NormalizedUserName = "TESTINGUSER", ConcurrencyStamp = "1", Email = "kfejzic1@etf.unsa.ba", NormalizedEmail = "KFEJZIC1@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "062229993", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false },
                 new User() { FirstName = "Admin", LastName = "User", UserName = "adminUser", NormalizedUserName = "ADMINUSER", ConcurrencyStamp = "1", Email = "fejza2806@gmail.com", NormalizedEmail = "FEJZA2806@GMAIL.COM", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "062518214", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = false, LockoutEnabled = false }
            };
        }

        [Fact]
        public void MakeTransactionTest()
        {
            var service = new RedeemVoucherService();
            RedeemVoucherResponse res = new RedeemVoucherResponse { Amount = 20, AccountNumber = "1" };
            var transaction = service.MakeTransaction(res, "");
            Assert.NotNull(transaction);
        }
        [Fact]
        public void MakeTransactionTest1()
        {
            var service = new RedeemVoucherService();
            RedeemVoucherResponse res = new RedeemVoucherResponse { Amount = 20, AccountNumber = "1" };
            var transaction = service.MakeTransaction(res, "12345678");
            Assert.NotNull(transaction);
        }
    }
}
