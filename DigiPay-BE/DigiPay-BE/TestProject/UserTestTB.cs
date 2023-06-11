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
using Microsoft.AspNetCore.Identity;
using AdministrationAPI.Contracts.Requests.Users;
using Microsoft.AspNetCore.Http;
using Google;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq.Expressions;

namespace TestProject

{
    public class UserTestTB
    {

        private List<User> users = new List<User>();
        private readonly ITestOutputHelper output;
        private Mock<AppDbContext> _context = new Mock<AppDbContext>();

        private Mock<UserManager<User>> _userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        private Mock<SignInManager<User>> _signInManager = new Mock<SignInManager<User>>();
        private Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
        private Mock<IMapper> _mapper = new Mock<IMapper>();
        private Mock<RoleManager<IdentityRole>> _roleManager = new Mock<RoleManager<IdentityRole>>();
        private Mock<IVendorService> _vendorService = new Mock<IVendorService>();
        private Mock<IHttpContextAccessor> _httpContext = new Mock<IHttpContextAccessor>();


        public UserTestTB(ITestOutputHelper output)
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

            _context.Setup(x => x.Users).ReturnsDbSet(users);
        }

        /*
        [Fact]
        public void GetUserByIdTest()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<User>>(MockBehavior.Default, null, null, null, null, null, null, null, null);
            var signInManagerMock = new Mock<SignInManager<User>>(userManagerMock.Object, null, null, null, null, null, null);
            var userService = new UserService(userManagerMock.Object, signInManagerMock.Object, _configuration.Object, _mapper.Object, _roleManager.Object, _context.Object, _vendorService.Object, _httpContext.Object);
 
            var userId = "ID";
            var userMock = new Mock<User>();
            userMock.Object.Id = userId;
 
            userManagerMock.Setup(m => m.Users).Returns(new List<User> { userMock.Object }.AsQueryable());
 
            // Act
            var user = userService.GetUserById(userId);
 
            // Assert
            Assert.NotNull(user);
            Assert.Equal(userId, user.Id);
            Assert.Equal("Test", user.FirstName);
        }
 
        [Fact]
        public void GetUserByEmailTest()
        {
            // Arrange
            var userMock = new User
            {
                Id = "ID",
                FirstName = "Test",
                Email = "test@gmail.com"
            };
            _userManager.Setup(u => u.Users.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(userMock);
 
            var service = new UserService(_userManager.Object, _signInManager.Object, _configuration.Object, _mapper.Object, _roleManager.Object, _context.Object, _vendorService.Object, _httpContext.Object);
 
            // Act
            var user = service.GetUserByEmail("test@gmail.com");
 
            // Assert
            Assert.NotNull(user);
            Assert.Equal("ID", user.Id);
            Assert.Equal("Test", user.FirstName);
        }
        */

        [Fact]
        public async Task GetAllUsersTest()
        {
            // Arrange
            var usersMock = new List<User>
    {
        new User { Id = "1", FirstName = "Test1" },
        new User { Id = "2", FirstName = "Test2" },
        new User { Id = "3", FirstName = "Test3" }
    };
            _userManager.Setup(u => u.Users).Returns(usersMock.AsQueryable());

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var contextMock = new Mock<HttpContext>();
            httpContextAccessorMock.Setup(a => a.HttpContext).Returns(contextMock.Object);
            var claimsPrincipal = new ClaimsPrincipal();
            contextMock.Setup(c => c.User).Returns(claimsPrincipal);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            var roleManager = new RoleManager<IdentityRole>(roleStoreMock.Object, null, null, null, null);

            var userService = new UserService(_userManager.Object, new SignInManager<User>(_userManager.Object, httpContextAccessorMock.Object, new Mock<IUserClaimsPrincipalFactory<User>>().Object, null, null, null, null), _configuration.Object, _mapper.Object, roleManager, _context.Object, _vendorService.Object, _httpContext.Object);

            // Act
            var users = userService.GetAllUsers();

            // Assert
            Assert.NotNull(users);
            Assert.Equal(3, users.Count);
            Assert.Contains(usersMock[0], users);
            Assert.Contains(usersMock[1], users);
            Assert.Contains(usersMock[2], users);
        }

        /*
        [Fact]
        public async Task EditUserTest()
        {
            // Arrange
            var userMock = new User
            {
                Id = "ID",
                FirstName = "Test",
                LastName = "LastName",
                Address = "Address",
                Email = "test@example.com",
                PhoneNumber = "123456789"
            };
 
            _userManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(userMock);
 
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            var roleManager = new RoleManager<IdentityRole>(roleStoreMock.Object, null, null, null, null);
 
            var service = new UserService(_userManager.Object, null, _configuration.Object, _mapper.Object, roleManager, _context.Object, _vendorService.Object, _httpContext.Object);
 
            // Act
            var user = service.GetUserById("ID"); // Retrieve the user object
            Assert.NotNull(user);
            Assert.Equal("ID", user.Id);
            user.FirstName = "TestEdit";
 
            var request = new EditRequest()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = "User"
            };
 
            await service.EditUser(request);
 
            // Assert
            _userManager.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Once);
            _context.Verify(x => x.SaveChanges(), Times.Once);
            Assert.Equal("TestEdit", user.FirstName);
        }
        */


        [Fact]
        public async Task GetUser_ValidId_ReturnsUserDT()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(m => m.Users).Returns(new List<User>
            {
                new User { Id = "1", UserName = "testuser", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" }
            }.AsQueryable());

            var userService = new UserService(userManagerMock.Object, null, null, null, null, null, null, null);

            // Act
            var result = await userService.GetUser("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.UserName);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john.doe@example.com", result.Email);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var user = new User { UserName = "testuser" };

            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(m => m.FindByNameAsync("testuser")).ReturnsAsync(user);
            userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(userManagerMock.Object, null, null, null, null, null, null, null);

            // Act
            var result = await userService.DeleteUserAsync("testuser");

            // Assert
            Assert.True(result);
        }


    }
}