using AdministrationAPI.Contracts.Responses;
using AdministrationAPI.Data;
using AdministrationAPI.Models;
using AdministrationAPI.Utilities;
using AdministrationAPI.Utilities.TokenUtility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using AdministrationAPI.Services.Interfaces;
using AdministrationAPI.Controllers;
using AutoMapper;

namespace TestProject
{
    public class TokenUtilitiesTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _userController;
        private readonly Mock<IMapper> _IMapperMock;
        private readonly Mock<IActivationCodeService> _IActivationCodeMock;
        private readonly Mock<IConfiguration> _IConfirugrationMock;
        public TokenUtilitiesTests()
        {
            _userServiceMock = new Mock<IUserService>();

            _IMapperMock = new Mock<IMapper>();
            _IConfirugrationMock = new Mock<IConfiguration>();
            _IActivationCodeMock = new Mock<IActivationCodeService>();

            _userController = new UserController(_userServiceMock.Object, _IMapperMock.Object, _IActivationCodeMock.Object, _IConfirugrationMock.Object);

        }

        [Fact]
        public void VerifyToken_ValidToken_ReturnsVerificationResult()
        {
            // Arrange
            var jwt = "valid_jwt_token";
            var handler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                issuer: "your_issuer",
                audience: "your_audience",
                claims: new List<Claim>
                {
                new Claim("UserName", "john.doe"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Role, "User")
                });

            var tokenString = handler.WriteToken(token);

            // Act
            var result = TokenUtilities.VerifyToken(tokenString);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("john.doe", result.Username);
            Assert.Contains("Admin", result.Roles);
            Assert.Contains("User", result.Roles);
        }
        [Fact]
        public void TokenValidity_PropertiesAreSet()
        {
            // Arrange
            var id = new Guid("01234567-89AB-CDEF-0123-456789ABCDEF");
            var token = "test-token";
            var isValid = true;

            // Act
            var tokenValidity = new TokenValidity
            {
                Id = id,
                Token = token,
                IsValid = isValid
            };

            // Assert
            Assert.Equal(id, tokenValidity.Id);
            Assert.Equal(token, tokenValidity.Token);
            Assert.Equal(isValid, tokenValidity.IsValid);
        }



        [Fact]
        public async Task Invoke_ShouldRefreshTokenIfAboutToExpire()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer <valid_token>";

            var userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddSeconds(30) // Token is about to expire in 30 seconds
            );
            var accessToken = tokenHandler.WriteToken(token);

            context.Request.Headers["Authorization"] = $"Bearer {accessToken}";

            var nextHandlerInvoked = false;
            RequestDelegate next = (ctx) =>
            {
                nextHandlerInvoked = true;
                return Task.CompletedTask;
            };

            // Act
            var handler = new TokenExpirationHandler(next, config);
            await handler.Invoke(context, userManager.Object);

            // Assert
            Assert.True(nextHandlerInvoked);
            // Additional assertions for token refresh logic, response headers, etc.
        }
        [Fact]
        public async Task GetAuthClaimsAsync_ReturnsAuthClaimsIncludingUserIdAndRoles()
        {
            // Arrange
            var user = new User
            {
                FirstName = "Elvedin",
                LastName = "Smajic",
                UserName = "esmajic2",
                NormalizedUserName = "ESMAJIC2",
                ConcurrencyStamp = "1",
                Email = "esmajic2@etf.unsa.ba",
                NormalizedEmail = "ESMAJIC2@ETF.UNSA.BA",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==",
                PhoneNumber = "11111",
                PhoneNumberConfirmed = true,
                Address = "Tamo negdje 1",
                TwoFactorEnabled = true,
                LockoutEnabled = false
            };



            var mockUserService = new Mock<IUserService>();

            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var userRoles = new List<string>
    {

        "User",
        "Admin"
    };

            userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(userRoles);

            var sut = new TokenUtilities();

            // Act
            var authClaims = await TokenUtilities.GetAuthClaimsAsync(user, userManagerMock.Object);
            Console.WriteLine(authClaims);

            // Assert
            Assert.NotNull(authClaims);
            Assert.Contains(authClaims, c => c.Type == "UserId" && c.Value == user.Id);
            Assert.Contains(authClaims, c => c.Type == "UserName" && c.Value == user.UserName);
            Assert.Contains(authClaims, c => c.Type == JwtRegisteredClaimNames.Jti && !string.IsNullOrEmpty(c.Value));

            foreach (var role in userRoles)
            {
                Assert.Contains(authClaims, c => c.Type == ClaimTypes.Role && c.Value == role);
            }

        }





    }
}
