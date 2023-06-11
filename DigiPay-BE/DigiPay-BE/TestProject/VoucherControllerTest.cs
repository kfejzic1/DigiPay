using System;
using AdministrationAPI.Contracts.Requests.Vouchers;
using AdministrationAPI.Contracts.Responses;
using AdministrationAPI.Controllers;
using AdministrationAPI.Services.Interfaces;
using AdministrationAPI.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using AdministrationAPI.Models.Voucher;
using AdministrationAPI.Models;
using AdministrationAPI.Extensions;
using System.Data;

namespace TestProject
{
	public class VoucherControllerTest : BaseControllerTest
	{

        private VoucherController voucherController;
        private Mock<IVoucherService> voucherService = new Mock<IVoucherService>();
        private Mock<IUserService> userService = new Mock<IUserService>();
        private Mock<TokenUtilities> tokenUtilities = new Mock<TokenUtilities>();


        public VoucherControllerTest()
        {
            voucherController = new VoucherController(
                voucherService.Object,
                userService.Object,
                tokenUtilities.Object
                );
            voucherController.ControllerContext.HttpContext = GetAuthorizedHttpContext();
        }

        [Fact]
        public async Task testCreateVoucher()
        {
            VoucherRequest voucherRequest = new VoucherRequest
            {
                NoVouchers = 5

            };

            var token = new TokenVerificationResult
            {
                Username = "admin"

            };

            userService.Setup(x => x.IsTokenValid(It.IsAny<string>())).Returns(true);
            tokenUtilities.Setup(x => x.VerifyJwtToken(It.IsAny<string>())).Returns(token);
            userService.Setup(u => u.GetUserByName(It.IsAny<string>())).Returns(new User());
            voucherService.Setup(x => x.CreateVoucher(It.IsAny<VoucherRequest>(), It.IsAny<string>()))
                .ReturnsAsync(new Voucher { Id = 1, Amount = 10, Code = "ABC123" });

            var result = await voucherController.CreateVoucher(voucherRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<List<VoucherDataResponse>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task testCreateVoucherBadRequest()
        {
            var voucherRequest = new VoucherRequest();

            userService.Setup(x => x.IsTokenValid(It.IsAny<string>())).Throws<DataException>();
            userService.Setup(x => x.GetUserByName(It.IsAny<string>()));
            voucherService.Setup(x => x.CreateVoucher(It.IsAny<VoucherRequest>(), It.IsAny<string>()));

            var response = await voucherController.CreateVoucher(voucherRequest);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Data Exception.", badRequestResult.Value);
        }

        [Fact]
        public async Task testCreateVoucherInternalServerError()
        {
            var voucherRequest = new VoucherRequest();

            userService.Setup(x => x.IsTokenValid(It.IsAny<string>())).Throws<Exception>();
            userService.Setup(x => x.GetUserByName(It.IsAny<string>()));
            voucherService.Setup(x => x.CreateVoucher(It.IsAny<VoucherRequest>(), It.IsAny<string>()));

            var response = await voucherController.CreateVoucher(voucherRequest);

            var statusCodeResult = Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }



        [Fact]
        public async Task ChangeVoucherStatus_WithValidCodeAndStatusId_ReturnsOkResult()
        {
            var changeVoucherStatusRequest = new ChangeVoucherStatusRequest
            {
                Code = "12fg-4g2z-4gs2-gs35",
                StatusId = "2" 
            };

            var voucher = new Voucher
            {
                Id = 1,
                Amount = 50,
                Code = "12fg-4g2z-4gs2-gs35",
                CreatedBy = "testingUser",
                CurrencyId = "2",
                RedeemedBy = null,
                VoucherStatusId = "2"
            };

            voucherService.Setup(service => service.GetVoucherByCode(changeVoucherStatusRequest.Code))
                .Returns(voucher);

            voucherService.Setup(service => service.ActivateVoucher(changeVoucherStatusRequest.Code))
                .Returns(voucher);

            var result = await voucherController.ChangeVoucherStatus(changeVoucherStatusRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<VoucherDataResponse>(okResult.Value);

            Assert.Equal(voucher.Id, response.Id);
            Assert.Equal(voucher.Amount, response.Amount);
            Assert.Equal(voucher.Code, response.Code);
            Assert.Equal(voucher.CreatedBy, response.CreatedBy);
            Assert.Equal(voucher.CurrencyId, response.CurrencyId);
            Assert.Equal(voucher.RedeemedBy, response.RedeemedBy);
            Assert.Equal(voucher.VoucherStatusId, response.VoucherStatusId);
        }

        [Fact]
        public async Task ChangeVoucherStatus_WithStatusId4_ReturnsOkResult()
        {
            var changeVoucherStatusRequest = new ChangeVoucherStatusRequest
            {
                Code = "12fg-4g2z-4gs2-gs35",
                StatusId = "4"
            };

            var tokenUsername = "testingUser";
            var user = new User() { FirstName = "Testing", LastName = "User", UserName = "testingUser", NormalizedUserName = "TESTINGUSER", ConcurrencyStamp = "1", Email = "kfejzic1@etf.unsa.ba", NormalizedEmail = "KFEJZIC1@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "062229993", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false };


            var voucher = new Voucher
            {
                Id = 1,
                Amount = 50,
                Code = "12fg-4g2z-4gs2-gs35",
                CreatedBy = "abrulic1",
                CurrencyId = "1",
                RedeemedBy = "testingUser",
                VoucherStatusId = "4"
            };


            voucherService.Setup(service => service.GetVoucherByCode(changeVoucherStatusRequest.Code))
                .Returns(voucher);

            voucherService.Setup(service => service.VoidVoucher(changeVoucherStatusRequest.Code))
                .ReturnsAsync(voucher);

            var result = await voucherController.ChangeVoucherStatus(changeVoucherStatusRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<VoucherDataResponse>(okResult.Value);

            Assert.Equal(voucher.Id, response.Id);
            Assert.Equal(voucher.Amount, response.Amount);
            Assert.Equal(voucher.Code, response.Code);
            Assert.Equal(voucher.CreatedBy, response.CreatedBy);
            Assert.Equal(voucher.CurrencyId, response.CurrencyId);
            Assert.Equal(voucher.RedeemedBy, response.RedeemedBy);
            Assert.Equal(voucher.VoucherStatusId, response.VoucherStatusId);
        }

        [Fact]
        public async Task ChangeVoucherStatus_WithVoucherNull_ReturnsExceptionMessage()
        {
            var changeVoucherStatusRequest = new ChangeVoucherStatusRequest
            {
                Code = "12fg-4g2z-4gs2-gs35",
                StatusId = "5" 
            };

            var result = await voucherController.ChangeVoucherStatus(changeVoucherStatusRequest);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var exceptionMessage = Assert.IsType<string>(statusCodeResult.Value);
            Assert.Equal("Voucher with this code doesn't exist!", exceptionMessage);
        }

        [Fact]
        public async Task ChangeVoucherStatus_WithInvalidStatusId_ReturnsExceptionMessage()
        {
            var changeVoucherStatusRequest = new ChangeVoucherStatusRequest
            {
                Code = "12fg-4g2z-4gs2-gs35",
                StatusId = "5"
            };

            var voucher = new Voucher
            {
                Id = 1,
                Amount = 50,
                Code = "12fg-4g2z-4gs2-gs35",
                CreatedBy = "abrulic1",
                CurrencyId = "1",
                RedeemedBy = "testingUser",
                VoucherStatusId = "5"
            };

            voucherService.Setup(service => service.GetVoucherByCode(changeVoucherStatusRequest.Code))
            .Returns(voucher);

            var result = await voucherController.ChangeVoucherStatus(changeVoucherStatusRequest);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var exceptionMessage = Assert.IsType<string>(statusCodeResult.Value);
            Assert.Equal("Invalid voucher status provided!", exceptionMessage);
        }



        [Fact]
        public async Task GetVouchers_WithValidToken_ReturnsOkResult()
        {
            List<User> users = new List<User>()
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

            List<Currency> currencies = new List<Currency>()
                {
                    new Currency() { Id = "1", Country = "BIH", Name = "BAM" },
                    new Currency() { Id = "2", Country = "USA", Name = "USD" },
                    new Currency() { Id = "3", Country = "DEU", Name = "EUR" },
                    new Currency() { Id = "4", Country = "SWI", Name = "CHF" }
                };

            List<VoucherDataResponse> expectedVouchers = new List<VoucherDataResponse>()
            {
                new VoucherDataResponse() { Id = 1, Amount = 50, CurrencyId = currencies[0].Id, Code = "12fg-4g2z-4gs2-gs35", VoucherStatusId = "1", CreatedBy = users[7].Id},
                new VoucherDataResponse() { Id = 2, Amount = 20, CurrencyId = currencies[1].Id, Code = "FDg4-DG4A-HS5A-HA36", VoucherStatusId = "1", CreatedBy = users[7].Id},
                new VoucherDataResponse() { Id = 3, Amount = 50, CurrencyId = currencies[2].Id, Code = "LLL4-GTA3-g4st-35h5", VoucherStatusId = "2", CreatedBy = users[7].Id},
                new VoucherDataResponse() { Id = 4, Amount = 50, CurrencyId = currencies[3].Id, Code = "kg45-fkai-3k5f-ek1f", VoucherStatusId = "3", CreatedBy = users[7].Id, RedeemedBy = users[6].Id}
            };

            var username = "abrulic1";
            var token = new TokenVerificationResult { Username = username };
            tokenUtilities.Setup(tokenUtil => tokenUtil.VerifyJwtToken(It.IsAny<string>())).Returns(token);
            voucherService.Setup(service => service.GetVouchers(username))
                .ReturnsAsync(expectedVouchers);

            var result = await voucherController.GetVouchers();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetVoucher_WithExistingVoucherId_ReturnsOkResult()
        {
            List<User> users = new List<User>()
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

            List<Currency> currencies = new List<Currency>()
                {
                    new Currency() { Id = Guid.NewGuid().ToString(), Country = "BIH", Name = "BAM" },
                    new Currency() { Id = Guid.NewGuid().ToString(), Country = "USA", Name = "USD" },
                    new Currency() { Id = Guid.NewGuid().ToString(), Country = "DEU", Name = "EUR" },
                    new Currency() { Id = Guid.NewGuid().ToString(), Country = "SWI", Name = "CHF" }
                };

            int voucherId = 123;
            var expectedVoucher = new Voucher { Id = 123, Amount = 50, CurrencyId = currencies[0].Id, Code = "12fg-4g2z-4gs2-gs35", VoucherStatusId = "1", CreatedBy = users[7].Id};
            voucherService.Setup(service => service.GetVoucherById(voucherId))
                .ReturnsAsync(expectedVoucher);

            var result = await voucherController.GetVoucher(voucherId);
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(expectedVoucher, okResult.Value);
        }

        [Fact]
        public async Task GetVoucher_WithNonExistingVoucherId_ReturnsNotFoundResult()
        {

            int voucherId = 123;
            Voucher voucher = null;
            voucherService.Setup(service => service.GetVoucherById(voucherId))
                .ReturnsAsync(voucher);

            var result = await voucherController.GetVoucher(voucherId);

            Assert.IsType<NotFoundObjectResult>(result);

        }

    }
}

