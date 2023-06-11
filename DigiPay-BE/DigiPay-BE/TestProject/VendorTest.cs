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
    public class VendorTest
    {
        private User user;
        private List<Vendors> vendors = new List<Vendors>();
        private List<VendorLocation> vendorLocations = new List<VendorLocation>();
        private List<VendorPOS> vendorPOS = new List<VendorPOS>();
        private List<VendorPaymentTerm> vendorPaymentTerms = new List<VendorPaymentTerm>();
        private List<VendorPaymentTermContract> vendorPaymentTermContracts = new List<VendorPaymentTermContract>();
        private List<InvoiceFrequency> invoiceFrequencies = new List<InvoiceFrequency>();
        private List<Document> documents = new List<Document>();

        private readonly ITestOutputHelper _output;
        private Mock<AppDbContext> _context = new Mock<AppDbContext>();
        private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
        private readonly Mock<IDocumentService> _documentService = new Mock<IDocumentService>();

        public VendorTest(ITestOutputHelper output)
        {
            _output = output;
            user = new User() { FirstName = "Testing", LastName = "User", UserName = "testingUser", NormalizedUserName = "TESTINGUSER", ConcurrencyStamp = "1", Email = "kfejzic1@etf.unsa.ba", NormalizedEmail = "KFEJZIC1@ETF.UNSA.BA", EmailConfirmed = true, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "062229993", PhoneNumberConfirmed = true, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false };
            vendors = new List<Vendors>()
            {
                new Vendors() { Id = 1, Name = "Vendor1", Address = "Address1", CompanyDetails = "", Phone = "1", CreatedBy = user.Id},
                new Vendors() { Id = 2, Name = "Vendor2", Address = "Address2", CompanyDetails = "", Phone = "2", CreatedBy = user.Id},
                new Vendors() { Id = 3, Name = "Vendor3", Address = "Address3", CompanyDetails = "", Phone = "3", CreatedBy = user.Id},
                new Vendors() { Id = 4, Name = "Vendor4", Address = "Address4", CompanyDetails = "", Phone = "4", CreatedBy = user.Id},
                new Vendors() { Id = 5, Name = "Merkator", Address = "Address4", CompanyDetails = "", Phone = "4", CreatedBy = user.Id}
            };

            vendorLocations = new List<VendorLocation>()
            {
                new VendorLocation() { Id = 1, Address = "ADDRESS", CreatedBy = user.Id, VendorId = 1}
            };

            vendorPOS = new List<VendorPOS>()
            {
                new VendorPOS() { Id = 1, Name = "VENDOR POS", CreatedBy = user.Id, LocationId = 1}
            };

            vendorPaymentTerms = new List<VendorPaymentTerm>()
            { 
                new VendorPaymentTerm { Id = 1, Name = "PAYMENT TERM TEST", VendorId = 1, StartDate = DateTime.Now, DueDate = DateTime.Now, ExpiryDate = DateTime.Now, InvoiceFrequencyTypeId = 1, Contracts = new List<Document>() { } }
            };

            vendorPaymentTermContracts = new List<VendorPaymentTermContract>() { };

            invoiceFrequencies = new List<InvoiceFrequency>() 
            {
                new InvoiceFrequency() { Id = 1, Name = "Monthly", FrequencyDays = 30 },
                new InvoiceFrequency() { Id = 2, Name = "Weekly", FrequencyDays = 7 },
                new InvoiceFrequency() { Id = 3, Name = "Biweekly", FrequencyDays = 14 },
            };

            documents = new List<Document>() { };

            _context.Setup(x => x.Documents).ReturnsDbSet(documents);
            _context.Setup(x => x.InvoiceFrequency).ReturnsDbSet(invoiceFrequencies);
            _context.Setup(x => x.VendorPaymentTerm).ReturnsDbSet(vendorPaymentTerms);
            _context.Setup(x => x.VendorPaymentTermContract).ReturnsDbSet(vendorPaymentTermContracts);
            _context.Setup(x => x.VendorLocations).ReturnsDbSet(vendorLocations);
            _context.Setup(x => x.VendorPOS).ReturnsDbSet(vendorPOS);
            _context.Setup(x => x.Vendors).ReturnsDbSet(vendors);

        }

        #region Vendor
        [Fact]
        public void CreateVendorTest()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            VendorCreateRequest req = new VendorCreateRequest{ Id=1, Name="Merkator", Address="Lozionicka", CompanyDetails="Detalji", Phone="033/222-333", Created=null, CreatedBy=user.Id, Modified=null, ModifiedBy=null, AssignedUserIds=new List<string>()};
            var created = service.Create(req);

            Assert.NotNull(created);
            Assert.True(created);
            _context.Verify(x => x.SaveChanges(), Times.AtLeastOnce);
            _context.Verify(x => x.Vendors.Add(It.IsAny<Vendors>()), Times.Once);            
        }

        [Fact]
        public void GetVendorByNameTest()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            var vendor = service.GetByName("Merkator");

            Assert.NotNull(vendor);
        }

        [Fact]
        public void GetVendorByIdTest()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            var vendor = service.Get(1);

            Assert.NotNull(vendor);
        }

        [Fact]
        public void GetAllVendorTest()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            List<VendorsResponse> vendors = service.GetAll();

            Assert.NotNull(vendors);
            Assert.NotEmpty(vendors);
        }

        [Fact]
        public void DeleteVendor()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            var response = service.Delete(1);

            Assert.Equal(true, response);
        }
        #endregion

        #region VendorPOS
        [Fact]
        public void CreateVendorPOS()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            POSCreateRequest req = new POSCreateRequest { Id = 1, Name = "POS TEST", LocationId = 1 };
            var created = service.CreatePOS(req);

            Assert.NotNull(created);
            Assert.True(created);
            _context.Verify(x => x.SaveChanges(), Times.AtLeastOnce);
            _context.Verify(x => x.VendorPOS.Add(It.IsAny<VendorPOS>()), Times.Once);
        }

        [Fact]
        public void GetAllPOSTest()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            List<POSResponse> pos = service.GetAllPOS(1);

            Assert.NotNull(pos);
            Assert.NotEmpty(pos);
        }

        [Fact]
        public void GetPOS()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            VendorPOS pos = service.GetPOS(1);

            Assert.NotNull(pos);
            Assert.Equal("VENDOR POS", pos.Name);
        }

        [Fact]
        public void UpdatePOS()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            var res = service.UpdatePOS(1, new POSUpdateRequest() { Name = "UPDATED POS", LocationId = 1, ModifiedBy = user.Id });

            Assert.True(res);

            VendorPOS pos = service.GetPOS(1);
            Assert.Equal("UPDATED POS", pos.Name);
        }
        #endregion

        #region VendorLocation
        [Fact]
        public void CreateVendorLocation()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            VendorLocationCreateRequest req = new VendorLocationCreateRequest {  Name = "POS TEST", Address = "ADDRESS TEST", VendorId = 1 };
            var created = service.CreateLocation(req);

            Assert.NotNull(created);
            Assert.True(created);
            _context.Verify(x => x.SaveChanges(), Times.AtLeastOnce);
            _context.Verify(x => x.VendorLocations.Add(It.IsAny<VendorLocation>()), Times.Once);
        }
        #endregion

        #region VendorPaymentTerms
        [Fact]
        public void CreatePaymentTermTest()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            PaymentTermRequest req = new PaymentTermRequest { Id = 1, Name = "PAYMENT TERM TEST", VendorId = 1, StartDate = DateTime.Now, DueDate = DateTime.Now, ExpiryDate = DateTime.Now, InvoiceFrequencyTypeId = 1,  DocumentIds = new List<int>() { 1, 2, 3 } };
            var created = service.CreatePaymentTerm(req);

            Assert.NotNull(created);
            _context.Verify(x => x.SaveChanges(), Times.AtLeastOnce);
            _context.Verify(x => x.VendorPaymentTerm.Add(It.IsAny<VendorPaymentTerm>()), Times.Once);

            _context.Verify(x => x.VendorPaymentTermContract.Add(It.IsAny<VendorPaymentTermContract>()), Times.Exactly(3));

        }
        [Fact]
        public void GetAllPaymentTermTest()
        {
            var service = new VendorService(_configuration.Object, _documentService.Object, _context.Object);
            List<PaymentTermResponse> paymentTerms = service.GetAllPaymentTerms(1);

            Assert.NotNull(paymentTerms);
            Assert.NotEmpty(paymentTerms);
        }
        #endregion
    }
}