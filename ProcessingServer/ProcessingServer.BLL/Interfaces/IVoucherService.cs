using ProcessingServer.BLL.DTO;
using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.Interfaces
{
    public interface IVoucherService
    {
        Task<Voucher> ExecuteVoucherRedemption(string token, VoucherRequest voucherRequest);
    }
}
