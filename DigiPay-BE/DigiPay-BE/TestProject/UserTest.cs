using AdministrationAPI.Contracts.Requests.Users;
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
using AdministrationAPI.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace TestProject
{
    public class UserTest
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _userController;
        private readonly Mock<IMapper> _IMapperMock;
        private readonly Mock<IActivationCodeService> _IActivationCodeMock;
        private readonly Mock<IConfiguration> _IConfirugrationMock;

        public UserTest()
        {
            _userServiceMock = new Mock<IUserService>();

            _IMapperMock = new Mock<IMapper>();
            _IConfirugrationMock = new Mock<IConfiguration>();
            _IActivationCodeMock = new Mock<IActivationCodeService>();

            _userController = new UserController(_userServiceMock.Object, _IMapperMock.Object, _IActivationCodeMock.Object, _IConfirugrationMock.Object);

        }

        [Fact]
        public async Task Login_ValidLoginRequest_ReturnsOkResult()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "esmajic2@etf.unsa.ba",
                Password = "String1!"
            };

            var authenticationResult = new AuthenticationResult
            {
                Success = true,
                Token = "token"
            };

            _userServiceMock.Setup(x => x.Login(loginRequest)).ReturnsAsync(authenticationResult);

            var controller = new UserController(_userServiceMock.Object, _IMapperMock.Object, _IActivationCodeMock.Object, _IConfirugrationMock.Object);

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task Login_TwoFactorEnabled_ReturnsOkResult()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "esmajic2@etf.unsa.ba",
                Password = "String1!"
            };

            var authenticationResult = new AuthenticationResult
            {
                TwoFactorEnabled = true,
                Mail = "esmajic2@etf.unsa.ba"
            };

            _userServiceMock.Setup(x => x.Login(loginRequest)).ReturnsAsync(authenticationResult);

            // Act
            var result = await _userController.Login(loginRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var authenticationResultValue = Assert.IsType<AuthenticationResult>(okResult.Value);
            Assert.Equal(authenticationResult.TwoFactorEnabled, authenticationResultValue.TwoFactorEnabled);
            Assert.Equal(authenticationResult.Mail, authenticationResultValue.Mail);
        }

        [Fact]
        public async Task Login_InvalidLoginRequest_ReturnsBadRequestResult()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "esmajic2@etf.unsa.ba",
                Password = "String1!"
            };

            var authenticationResult = new AuthenticationResult
            {
                Success = false,
                Errors = new[] { "Invalid credentials" }
            };

            _userServiceMock.Setup(x => x.Login(loginRequest)).ReturnsAsync(authenticationResult);

            var controller = new UserController(_userServiceMock.Object, _IMapperMock.Object, _IActivationCodeMock.Object, _IConfirugrationMock.Object);

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        /*
        [Fact]
        public void GetUserByFirstName_UserExists_ReturnsUser()
        {
            // Arrange
            var users = new List<User>()
    {
        new User() { FirstName = "Testing", LastName = "User", UserName = "testingUser" },
        new User() { FirstName = "Admin", LastName = "User", UserName = "adminUser" },
        new User() { FirstName = "Elvedin", LastName = "Smajic", UserName = "esmajic2" }
    };
 
            var firstName = "Admin";
 
            _userServiceMock.Setup(x => x.GetUserByName(firstName)).Returns(users.FirstOrDefault(u => u.FirstName == firstName));
 
            var controller = new UserController(_userServiceMock.Object, _IMapperMock.Object, _IActivationCodeMock.Object, _IConfirugrationMock.Object);
 
            // Act
            var actionResult = controller.GetUserByName(firstName);
            var result = actionResult.Value as User;
 
            // Assert
            Assert.NotNull(result);
            Assert.Equal(firstName, result.FirstName);
        }
 
        [Fact]
        public void GetUserByFirstName_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var users = new List<User>()
    {
        new User() { FirstName = "Testing", LastName = "User", UserName = "testingUser" },
        new User() { FirstName = "Admin", LastName = "User", UserName = "adminUser" },
        new User() { FirstName = "Elvedin", LastName = "Smajic", UserName = "esmajic2" }
    };
 
            var firstName = "John";
 
            // Act
            var result = _userController.GetUserByName(firstName);
 
            // Assert
            Assert.Null(result);
        }
 
        [Fact]
        public async Task CreateUser_UserWithEmailExists_ReturnsConflict()
        {
            // Arrange
            var createRequest = new CreateRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                PhoneNumber = "123456789",
                Address = "123 Main St",
                Role = "User"
            };
 
            _userServiceMock.Setup(x => x.CreateUser(createRequest))
                            .Throws(new Exception("User with email already exists."));
 
            // Act
            var result = await _userController.CreateUser(createRequest);
 
            // Assert
            Assert.IsType<ConflictObjectResult>(result);
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var errorMessage = Assert.IsType<string>(conflictResult.Value);
            Assert.Equal("User with email already exists.", errorMessage);
            Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
        }
 
 
        [Fact]
        public async Task CreateUser_ValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new CreateRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                PhoneNumber = "123456789",
                Address = "123 Main St",
                Role = "User"
            };
 
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(u => u.GetUserByEmail(request.Email)).Returns((User)null);
            mockUserService.Setup(u => u.CreateUser(request)).ReturnsAsync(IdentityResult.Success);
            mockUserService.Setup(u => u.GetUserByEmail(request.Email)).Returns(new User { Email = request.Email });
 
            var controller = _userController;
 
            // Act
            var result = await controller.CreateUser(request);
 
            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
            Assert.Equal($"User created and confirmation email has been sent to {request.Email} successfully", objectResult.Value);
        }
        */

        [Fact]
        public async Task SetPassword_ValidToken_ReturnsSuccessResult()
        {
            // Arrange
            var request = new SetPasswordRequest
            {
                Id = "userId",
                Token = "emailConfirmationToken",
                Password = "newPassword"
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(u => u.GetUserById(request.Id)).Returns(new User());

            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(u => u.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(u => u.AddPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var controller = _userController;

            // Act
            var result = await controller.SetUserPassword(request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
            var errorMessage = (string)notFoundResult.Value;
            Assert.Equal("User not found.", errorMessage);
        }


        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserNotCreated()
        {
            // Arrange
            var model = new RegisterRequest
            {
                Email = "user@example.com",
                Username = "username",
                PhoneNumber = "1234567890",
                Password = "password"
            };

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(u => u.Register(model)).ReturnsAsync((User)null);

            var controller = _userController;

            // Act
            var result = await controller.Register(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal("User not created. Password must contain at least one uppercase letter, a digit and a non-alphanumeric character. Password must be at least six characters long.", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsOkResult_WhenRegistrationSuccessful()
        {
            // Arrange
            var model = new RegisterRequest
            {
                Email = "user@example.com",
                Username = "username",
                PhoneNumber = "1234567890",
                Password = "Password123!"
            };

            var user = new User
            {
                Email = model.Email,
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber
            };

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(u => u.Register(model))
                .ReturnsAsync(user);

            var controller = new UserController(userServiceMock.Object, _IMapperMock.Object, _IActivationCodeMock.Object, _IConfirugrationMock.Object);

            // Act
            var result = await controller.Register(model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal("Registration successful", okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null));
        }

        /*
        [Fact]
        public void GetAllUsers_ReturnsAllUsers()
        {
            // Arrange
 
            var mockUserService = new Mock<IUserService>();
            var users = new List<User>
    {
        new User { FirstName = "Elvedin", LastName = "Smajic", UserName = "esmajic2", NormalizedUserName = "ESMAJIC2", ConcurrencyStamp = "1", Email = "esmajic2@etf.unsa.ba", NormalizedEmail = "ESMAJIC2@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "11111", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false },
        new User { FirstName = "Admir", LastName = "Mehmedagic", UserName = "amehmedagi1", NormalizedUserName = "AMEHMEDAGI1", ConcurrencyStamp = "1", Email = "amehmedagi1@etf.unsa.ba", NormalizedEmail = "AMEHMEDAGI1@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "11111", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false }
    };
            mockUserService.Setup(u => u.GetAllUsers()).Returns(users);
 
            var controller = _userController;
 
            // Act
            var result = controller.GetAllUsers();
 
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<List<User>>(okResult.Value);
            Assert.Equal(users.Count, returnedUsers.Count);
            Assert.Equal(users[0].FirstName, returnedUsers[0].FirstName);
            Assert.Equal(users[0].LastName, returnedUsers[0].LastName);
            Assert.Equal(users[1].FirstName, returnedUsers[1].FirstName);
            Assert.Equal(users[1].LastName, returnedUsers[1].LastName);
        }
        */

        [Fact]
        public async Task DeleteUser_ValidUsername_ReturnsOk()
        {
            // Arrange
            string username = "dmuhic1";
            _userServiceMock.Setup(x => x.DeleteUserAsync(username)).ReturnsAsync(true);

            // Act
            var result = await _userController.DeleteUser(username);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal("User deleted", okResult.Value);
        }

        [Fact]
        public async Task DeleteUser_InvalidUsername_ReturnsNotFound()
        {
            // Arrange
            string username = "abrulic1";
            _userServiceMock.Setup(x => x.DeleteUserAsync(username)).ReturnsAsync(false);

            // Act
            var result = await _userController.DeleteUser(username);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.Equal("User not found", notFoundResult.Value);
        }



    }
}