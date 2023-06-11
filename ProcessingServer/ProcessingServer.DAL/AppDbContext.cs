using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserBankAccount> UserBankAccounts { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<EInvoicePaymentHistory> EInvoicePaymentHistory { get; set; }
        public DbSet<VendorBankAccount> VendorBankAccounts { get; set; }
        public object Value { get; }
    }
}
