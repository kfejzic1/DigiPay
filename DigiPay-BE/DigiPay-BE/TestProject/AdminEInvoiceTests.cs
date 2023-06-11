using AdministrationAPI.Contracts.Requests.Vouchers;
using AdministrationAPI.Contracts.Responses;
using AdministrationAPI.Contracts.Requests.EInvoices;
using AdministrationAPI.Contracts.Requests.EInvoiceRegistration;
using AdministrationAPI.Controllers;
using AdministrationAPI.Data;
using AdministrationAPI.Models;
using AdministrationAPI.Models.Vendor;
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
using AdministrationAPI.Models.Voucher;
using AdministrationAPI.Models.EInvoiceForms;
using AdministrationAPI.Contracts.Requests;

namespace TestProject
{
    public class AdminEInvoiceTests
    {
        private List<User> users = new List<User>();
        private List<Vendors> vendors = new List<Vendors>();
        private List<EInvoiceRequest> requests = new List<EInvoiceRequest>();
        private readonly ITestOutputHelper output;
        private Mock<AppDbContext> _context = new Mock<AppDbContext>();
        private readonly Mock<IVendorService> _vendor = new Mock<IVendorService>();
        private readonly Mock<IUserService> _user = new Mock<IUserService>();
        private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();




        public AdminEInvoiceTests(ITestOutputHelper output)
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


            vendors = new List<Vendors>()
            {
                new Vendors() { Id = 1, Name = "Vendor1", Address = "Address1", CompanyDetails = "", Phone = "1", CreatedBy = users[0].Id},
                new Vendors() { Id = 2, Name = "Vendor2", Address = "Address2", CompanyDetails = "", Phone = "2", CreatedBy = users[1].Id},
                new Vendors() { Id = 3, Name = "Vendor3", Address = "Address3", CompanyDetails = "", Phone = "3", CreatedBy = users[2].Id},
                new Vendors() { Id = 4, Name = "Vendor4", Address = "Address4", CompanyDetails = "", Phone = "4", CreatedBy = users[3].Id},
                new Vendors() { Id = 5, Name = "Merkator", Address = "Address4", CompanyDetails = "", Phone = "4", CreatedBy = users[4].Id},
                new Vendors() { Id = 6, Name = "Vendor7", Address = "Address7", CompanyDetails = "", Phone = "4", CreatedBy = users[4].Id, Param1="a", Param2="b", Param3="c", Param4="d"}

            };

            // 1 je pending, 0 je denied i 2 je approved
            requests = new List<EInvoiceRequest>()
            {
                new EInvoiceRequest() {EInvoiceRequestId=1, UserId="ID", VendorId=1, Status=1, Param1="bilosta", Param2="stabilo", Param3="", Param4=""},
                new EInvoiceRequest() {EInvoiceRequestId=2, UserId="ID", VendorId=2, Status=1, Param1="bilosta", Param2="stabilo", Param3="", Param4=""},
                new EInvoiceRequest() {EInvoiceRequestId=3, UserId="ID", VendorId=3, Status=1, Param1="", Param2="stabilo", Param3="adresa", Param4=""},
                new EInvoiceRequest() {EInvoiceRequestId=4, UserId="ID", VendorId=4, Status=1, Param1="bilosta", Param2="stabilo", Param3="", Param4=""},
            };
            _context.Setup(x => x.EInvoiceRequests).ReturnsDbSet(requests);
            _context.Setup(x => x.Vendors).ReturnsDbSet(vendors);
            _context.Setup(x => x.Users).ReturnsDbSet(users);


        }


        [Fact]
        public async Task GetInvoiceRequestsByIDTest()
        {
            var service = new AdminEInvoiceService(_configuration.Object, _context.Object, _vendor.Object, _user.Object);
            var requests = await service.GetInvoiceRequestsByID(1);

            Assert.NotNull(requests);
            Assert.Equal(1, requests.Count);
            Assert.Equal("bilosta", requests[0].Param1);

        }


        [Fact]
        public async Task GetAllInvoiceRequestsTest()
        {
            var service = new AdminEInvoiceService(_configuration.Object, _context.Object, _vendor.Object, _user.Object);
            var requests = await service.GetAllInvoiceRequests();

            Assert.NotNull(requests);
            Assert.Equal(4, requests.Count);

        }

        [Fact]
        public async Task HandleRequestStatusTestApprove()
        {
            var service = new AdminEInvoiceService(_configuration.Object, _context.Object, _vendor.Object, _user.Object);
            var request = await service.HandleRequestStatus(true, 2);
            Assert.Equal(2, request.Status);
            

        }

        [Fact]
        public async Task HandleRequestStatusTestDeny()
        {
            var service = new AdminEInvoiceService(_configuration.Object, _context.Object, _vendor.Object, _user.Object);
            var request = await service.HandleRequestStatus(false, 4);
            Assert.Equal(0, request.Status);


        }
        [Fact]
        public async Task HandleRequestStatusTestAlreadyHandled()
        {
            var service = new AdminEInvoiceService(_configuration.Object, _context.Object, _vendor.Object, _user.Object);
            var request = await service.HandleRequestStatus(false, 3);
            Assert.ThrowsAsync<Exception>(async () => await service.HandleRequestStatus(false, 3));

        }

        [Fact]
        public async Task HandleRequestStatusRequestNotFoundTest()
        {
            var service = new AdminEInvoiceService(_configuration.Object, _context.Object, _vendor.Object, _user.Object);
            Assert.ThrowsAsync<Exception>(async () => await service.HandleRequestStatus(true, 10));
        }


        [Fact]
        public async Task DefineRequiredDataForVendorTestVendorNotFound()
        {
            var service = new AdminEInvoiceService(_configuration.Object, _context.Object, _vendor.Object, _user.Object);
            var data = new RequiredData() { Param1 = "", Param2 = "", Param3 = "", Param4 = "" };
            Assert.ThrowsAsync<Exception>(async () => await service.DefineRequiredDataForVendor(8, data));
        }


        [Fact]
        public async Task DefineRequiredDataForVendorTestException()
        {
            var service = new AdminEInvoiceService(_configuration.Object, _context.Object, _vendor.Object, _user.Object);
            var data = new RequiredData() { Param1 = "Number", Param2 = "Address", Param3 = "abc", Param4 = "def" };
            Assert.ThrowsAsync<Exception>(async () => await service.DefineRequiredDataForVendor(1, data));
        }

        [Fact]
        public async Task DefineRequiredDataForVendorTest()
        {
            var service = new AdminEInvoiceService(_configuration.Object, _context.Object, _vendor.Object, _user.Object);
            var data = new RequiredData() { Param1 = "Number", Param2 = "Address", Param3 = "abc", Param4 = "def" };
            var vendor = await service.DefineRequiredDataForVendor(1, data);
            Assert.Equal("Number", vendor.Param1);
            Assert.Equal("Address", vendor.Param2);
        }


        [Fact]
        public async Task AddEInvoiceRequestExceptionTest()
        {
            var service = new AdminEInvoiceService(_configuration.Object, _context.Object, _vendor.Object, _user.Object);
            var data = new EInvoiceRegistrationData() { B2BName = "Vendor7", Field1 = "", Field2 = "", Field3 = "", Field4 = "" };
            Assert.ThrowsAsync<Exception>(async () => await service.AddEInvoiceRequest(data, "ID"));

        }


        [Fact]
        public async Task AddEInvoiceRequestTest()
        {
            var service = new AdminEInvoiceService(_configuration.Object, _context.Object, _vendor.Object, _user.Object);
            var data = new EInvoiceRegistrationData() { B2BName = "Vendor7", Field1 = "13", Field2 = "32", Field3 = "ab", Field4 = "3c" };
            // Assert.ThrowsAsync<Exception>(async () => await service.AddEInvoiceRequest(data, "ID"));
            Assert.NotNull(service.AddEInvoiceRequest(data, "ID"));

        }


    }


}
