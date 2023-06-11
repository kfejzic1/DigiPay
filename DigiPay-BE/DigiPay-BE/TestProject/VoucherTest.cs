using AdministrationAPI.Contracts.Requests.Vouchers;
using AdministrationAPI.Contracts.Responses;
using AdministrationAPI.Controllers;
using AdministrationAPI.Data;
using AdministrationAPI.Models;
using AdministrationAPI.Models.Voucher;
using AdministrationAPI.Services;
using AdministrationAPI.Services.Interfaces;
using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Components.Routing;
using System.Diagnostics.CodeAnalysis;
using Castle.Components.DictionaryAdapter.Xml;

namespace TestProject
{
    public class VoucherTest
    {
        private List<User> users = new List<User>();
        private List<Currency> currencies = new List<Currency>();
        private List<Voucher> vouchers = new List<Voucher>();
        private readonly ITestOutputHelper output;
        private Mock<AppDbContext> _context = new Mock<AppDbContext>();


        public VoucherTest(ITestOutputHelper output) {
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
                    new Currency() { Id = Guid.NewGuid().ToString(), Country = "BIH", Name = "BAM" },
                    new Currency() { Id = Guid.NewGuid().ToString(), Country = "USA", Name = "USD" },
                    new Currency() { Id = Guid.NewGuid().ToString(), Country = "DEU", Name = "EUR" },
                    new Currency() { Id = Guid.NewGuid().ToString(), Country = "SWI", Name = "CHF" }
                };


            vouchers = new List<Voucher>()
            {
                new Voucher() { Id = 1, Amount = 50, CurrencyId = currencies[0].Id, Code = "12fg-4g2z-4gs2-gs35", VoucherStatusId = "1", CreatedBy = users[7].Id},
                new Voucher() { Id = 2, Amount = 20, CurrencyId = currencies[1].Id, Code = "FDg4-DG4A-HS5A-HA36", VoucherStatusId = "1", CreatedBy = users[7].Id},
                new Voucher() { Id = 3, Amount = 50, CurrencyId = currencies[2].Id, Code = "LLL4-GTA3-g4st-35h5", VoucherStatusId = "2", CreatedBy = users[7].Id},
                new Voucher() { Id = 4, Amount = 50, CurrencyId = currencies[3].Id, Code = "kg45-fkai-3k5f-ek1f", VoucherStatusId = "3", CreatedBy = users[7].Id, RedeemedBy = users[6].Id}
            };
            _context.Setup(x => x.Vouchers).ReturnsDbSet(vouchers);
        }

        [Fact]
        public async Task GetVoucherByIdTest1()
        {
            var service = new VoucherService(_context.Object);
            var voucher = await service.GetVoucherById(1);
            Assert.NotNull(voucher);
            Assert.Equal(50, voucher.Amount);
            Assert.Equal("12fg-4g2z-4gs2-gs35", voucher.Code);
        }

        [Fact]
        public async Task GetVoucherByIdTest2()
        {
            var service = new VoucherService(_context.Object);
            var voucher = await service.GetVoucherById(5);
            Assert.Null(voucher);
        }

        [Fact]
        public async Task CreateVoucherTest()
        {
            var service = new VoucherService(_context.Object);
            VoucherRequest voucherRequest = new VoucherRequest { NoVouchers = 5, Amount = 10, CurrencyId = currencies[3].Id };
            var voucher = await service.CreateVoucher(voucherRequest, users[0].Id);

            Assert.NotNull(voucher);
            Assert.Equal(10, voucher.Amount);

            _context.Verify(x => x.SaveChanges(), Times.Once);
            _context.Verify(x => x.Vouchers.Add(It.IsAny<Voucher>()), Times.Once);

            Assert.Equal(users[0].Id, voucher.CreatedBy);
        }

        [Fact]
        public async Task ActivateVoucherTest1()
        {
            var service = new VoucherService(_context.Object);
            var voucher = service.ActivateVoucher("12fg-4g2z-4gs2-gs35");
            output.WriteLine("str" + voucher);

            _context.Verify(x => x.SaveChanges(), Times.Once);
            Assert.NotNull(voucher);
            Assert.Equal("2", voucher.VoucherStatusId);
        }


        [Fact]
        public async Task RedeemVoucherTest_ActivatedVoucher()
        {
            var service = new VoucherService(_context.Object);
            var voucher = service.GetVoucherByCode("LLL4-GTA3-g4st-35h5");
     
            Assert.NotNull(voucher);
            var beforeRedeem = voucher.VoucherStatusId;

            voucher = await service.RedeemVoucher(users[1], "LLL4-GTA3-g4st-35h5");

            _context.Verify(x => x.SaveChanges(), Times.Once);
            
            Assert.Equal("2", beforeRedeem);
            Assert.Equal("3", voucher.VoucherStatusId);
        }


        [Fact]
        public async Task RedeemVoucherTest_NotActivatedVoucher()
        {
            var service = new VoucherService(_context.Object);
            var voucher = service.GetVoucherByCode("12fg-4g2z-4gs2-gs35");

            Assert.NotNull(voucher);
            Assert.ThrowsAsync<Exception>(async () => await service.RedeemVoucher(users[1], "12fg-4g2z-4gs2-gs35"));

        }

        [Fact]
        public async Task VoidVoucherTest_NotValidVoucher()
        {
            var service = new VoucherService(_context.Object);
            Assert.ThrowsAsync<Exception>(async () => await service.VoidVoucher("kg45-fkai-3k5f-ek1f"));

        }

        [Fact]
        public async Task VoidVoucherTest_ValidVoucher()
        {
            var service = new VoucherService(_context.Object);
            var voucher = service.GetVoucherByCode("12fg-4g2z-4gs2-gs35");

            Assert.NotNull(voucher);
            var beforeVoid = voucher.VoucherStatusId;

            voucher = await service.VoidVoucher("12fg-4g2z-4gs2-gs35");

            _context.Verify(x => x.SaveChanges(), Times.Once);

            Assert.Equal("1", beforeVoid);
            Assert.Equal("4", voucher.VoucherStatusId);
        }


        [Fact]
        public async Task GetVoucherByUserIdTest()
        {
            var service = new VoucherService(_context.Object);
            var voucher = service.GetVoucherByUserId(users[6].Id);
            Assert.NotNull(voucher);
       }


        [Fact]
        public async Task GetVouchersByAdminUsernameTest1()
        {
            var service = new VoucherService(_context.Object);
            var voucher = service.GetVouchers(users[7].UserName);
            Assert.NotNull(voucher);
        }


    }
}
