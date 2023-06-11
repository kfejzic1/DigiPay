using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        AppDbContext _DbContext;

        public VoucherRepository(AppDbContext appDbContext)
        {
            _DbContext = appDbContext;
        }

        public async Task<Voucher> CreateVaucher(Voucher voucher)
        {
            await _DbContext.Vouchers.AddAsync(voucher);
            return voucher;
        }
    }
}
