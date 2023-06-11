using Microsoft.AspNetCore.Mvc;
using ProcessingServer.BLL.DTO;
using ProcessingServer.BLL.Interfaces;
using ProcessingServer.DAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace ProcessingServer.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
                _voucherService = voucherService;
        }

        [HttpPost("ExecuteVoucherRedemption")]
        public async Task<ActionResult<Voucher>> ExecuteVoucherRedemption([FromQuery] [Required] string token, [FromBody] VoucherRequest voucherRequest)
        {
            try
            {
                var result = await _voucherService.ExecuteVoucherRedemption(token, voucherRequest);
                return Ok(result);
            }
            catch (AuthenticationException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
            catch (Exception exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }
    }
}
