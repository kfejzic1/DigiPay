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
using AdministrationAPI.Models.Transaction;
using AdministrationAPI.Contracts.Requests.Transactions;

namespace TestProject
{
    public class TransactionClaimTest
    {
        private List<User> users = new List<User>();
        private List<TransactionClaim> transactionClaims = new List<TransactionClaim>();
        private readonly ITestOutputHelper output;
        private Mock<AppDbContext> _context = new Mock<AppDbContext>();
        //private Mock<DBContext> _dbContext = new Mock<DBContext>();
        private DBContext _dbContext;
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ITransactionService> _transaction = new Mock<ITransactionService>();
        private List<TransactionClaimUser> transactionClaimUsers = new List<TransactionClaimUser>();
        private List<TransactionClaimDocument> transactionClaimDocuments = new List<TransactionClaimDocument>();
        private List<TransactionClaimMessage> transactionClaimMessages = new List<TransactionClaimMessage>();
        private List<Document> documents = new List<Document>();
        private List<ClaimsMessagesDocuments> claimMessageDocuments= new List<ClaimsMessagesDocuments>();
        private List<ClaimAcceptRequest> claimAcceptRequests = new List<ClaimAcceptRequest>();

        public TransactionClaimTest(ITestOutputHelper output)
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

            transactionClaims = new List<TransactionClaim>()
            {
                new TransactionClaim() {Id=1, TransactionId=1, Subject="claim", Description="desc", CreatedBy="ID"},
                new TransactionClaim() {Id=2, TransactionId=1, Subject="subject", Description="opis", CreatedBy="ID"},
                new TransactionClaim() {Id=3, TransactionId=1, Subject="123", Description="3211", CreatedBy="ID"},
                new TransactionClaim() {Id=4, TransactionId=1, Subject="123", Description="3211", CreatedBy="ID", Status=(TransactionClaimStatus)1},
                new TransactionClaim() {Id=5, TransactionId=2, Subject="1234", Description="32111", CreatedBy="ID", Status=(TransactionClaimStatus)1},
                new TransactionClaim() {Id=6, TransactionId=2, Subject="61234", Description="362111", CreatedBy="ID", Status=(TransactionClaimStatus)3}


            };
            _context.Setup(x => x.TransactionClaims).ReturnsDbSet(transactionClaims);

            transactionClaimUsers = new List<TransactionClaimUser>()
            {
                new TransactionClaimUser() { Id=1, TransactionClaimId=1, UserId="ID", AdminId="admin1"},
                new TransactionClaimUser() { Id=2, TransactionClaimId=2, UserId="ID", AdminId="admin2"},
                new TransactionClaimUser() { Id=3, TransactionClaimId=2, UserId="ID", AdminId=""},
                new TransactionClaimUser() { Id=4, TransactionClaimId=2, UserId="ID", AdminId="ID"},
                new TransactionClaimUser() { Id=5, TransactionClaimId=5, UserId="ID", AdminId="ID"},
                new TransactionClaimUser() { Id=6, TransactionClaimId=6, UserId="ID", AdminId="ID"},


            };
            _context.Setup(x => x.TransactionClaimUsers).ReturnsDbSet(transactionClaimUsers);


            transactionClaimDocuments = new List<TransactionClaimDocument>()
            {
                new TransactionClaimDocument() { Id=1, ClaimId=1, DocumentId=1},
                new TransactionClaimDocument() { Id=2, ClaimId=2, DocumentId=2},
                new TransactionClaimDocument() { Id=3, ClaimId=3, DocumentId=3},
            };
            _context.Setup(x => x.TransactionClaimDocuments).ReturnsDbSet(transactionClaimDocuments);

            transactionClaimMessages = new List<TransactionClaimMessage>()
            {
                new TransactionClaimMessage() {Id = 1, TransactionClaimId=1, Message="abc", UserId="ID"},
                new TransactionClaimMessage() {Id = 2, TransactionClaimId=2, Message="def", UserId="ID"},
                new TransactionClaimMessage() {Id = 3, TransactionClaimId=3, Message="ghi", UserId="ID"},

            };
            _context.Setup(x => x.TransactionClaimMessages).ReturnsDbSet(transactionClaimMessages);

            documents = new List<Document>()
            {
                new Document() { Id = 1, FileName="file1", Description="desc1", Folder="folder1", UNC="unc1", Extension="extension1", CreatedBy="ID", ModifiedBy="ID"},
                new Document() { Id = 2, FileName="file2", Description="desc2", Folder="folder2", UNC="unc2", Extension="extension2", CreatedBy="ID", ModifiedBy="ID"},
                new Document() { Id = 3, FileName="file3", Description="desc3", Folder="folder3", UNC="unc3", Extension="extension3", CreatedBy="ID", ModifiedBy="ID"}
            };
            _context.Setup(x => x.Documents).ReturnsDbSet(documents);

            claimMessageDocuments = new List<ClaimsMessagesDocuments>()
            {
                new ClaimsMessagesDocuments() { Id = 1, MessageId=1, DocumentId=1},
                new ClaimsMessagesDocuments() { Id = 2, MessageId=2, DocumentId=2},
                new ClaimsMessagesDocuments() { Id = 3, MessageId=3, DocumentId=3}
            };
            _context.Setup(x => x.ClaimsMessagesDocuments).ReturnsDbSet(claimMessageDocuments);
        }

        [Fact]
        public void GetTransactionClaimsTestNotNull()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            var claims =  service.GetTransactionClaims("ID");
            Assert.NotNull(claims);
        }


        [Fact]
        public async Task GetTransactionClaimsTest()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            var claims = service.GetTransactionClaims("ID");
            Assert.Equal(4, claims.Count());
        }

        [Fact]
        public async Task GetTransactionClaimByIdTest()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            var claim = service.GetTransactionClaim(1);
            Assert.NotNull(claim);
        }


        [Fact]
        public async Task AcceptTransactionClaimTransactionClaimNullTest()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            var request = new ClaimAcceptRequest() { TransactionClaimId = 7 };
             Assert.Throws<Exception>( () => service.AcceptTransactionClaim(request, "ID"));
        }

        [Fact]
        public async Task AcceptTransactionClaimClaimNotOpenTest()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            var request = new ClaimAcceptRequest() { TransactionClaimId = 2 };
            Assert.Throws<Exception>(() => service.AcceptTransactionClaim(request, "string3"));
        }

        [Fact]
        public async Task AcceptTransactionClaimExceptionTest()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            var request = new ClaimAcceptRequest() { TransactionClaimId = 3 };
            Assert.Throws<Exception>(() => service.AcceptTransactionClaim(request, "ID"));

        }


        [Fact]
        public async Task UpdateTransactionClaimTransactionClaimNullTest()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            var request = new ClaimUpdateRequest() { TransactionClaimId = 7 };
            Assert.Throws<Exception>(() => service.UpdateTransactionClaim(request, "ID"));
        }


        [Fact]
        public async Task UpdateTransactionClaimTestException()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            var request = new ClaimUpdateRequest() { TransactionClaimId = 2 };
            Assert.Throws<Exception>(() => service.UpdateTransactionClaim(request, "ID"));
        }

        [Fact]
        public async Task UpdateTransactionClaimTestIllegalStatusValue()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            var request = new ClaimUpdateRequest() { TransactionClaimId = 5 };
            Assert.Throws<Exception>(() => service.UpdateTransactionClaim(request, "ID"));
        }

        [Fact]
        public async Task UpdateTransactionClaimTest()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            var request = new ClaimUpdateRequest() { TransactionClaimId = 6 };
            Assert.NotNull(service.UpdateTransactionClaim(request, "ID"));
        }


        [Fact]
        public async Task GetTransactionClaimsOpenTest()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            Assert.Equal(5, service.GetTransactionClaimsOpen().Count);
        }


        [Fact]
        public async Task GetTransactionClaimsForAdminTest()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            Assert.NotNull(service.GetTransactionClaimsForAdmin("ID"));
        }

        [Fact]
        public async Task GetDocumentTest()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            Assert.NotNull(service.GetDocument(2));
        }

        [Fact]
        public async Task CreateTransactionExceptionTest()
        {
            var service = new TransactionService(_mapperMock.Object, _dbContext, _context.Object);
            var request = new TransactionCreateRequest();
            Assert.ThrowsAsync<Exception>(() => service.CreateTransaction(request));
        }
    }
}