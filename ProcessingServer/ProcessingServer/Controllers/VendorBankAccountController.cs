using Microsoft.AspNetCore.Mvc;
using ProcessingServer.BLL.DTO;
using ProcessingServer.BLL.Interfaces;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Other;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace ProcessingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorBankAccountController : ControllerBase
    {
        private readonly IVendorBankAccountService _vendorBankAccountService;

        public VendorBankAccountController(IVendorBankAccountService vendorBankAccountService)
        {
            _vendorBankAccountService = vendorBankAccountService;
        }

        [HttpPost("CreateVendorBankAccount")]
        public async Task<ActionResult<VendorBankAccountResponse>> CreateVendorBankAcount([FromBody] VendorBankAccountRequest vendorBankAccountRequest, [FromQuery, Required] string token)
        {
            try
            {
                var result = await _vendorBankAccountService.CreateVendorBankAccount(token, vendorBankAccountRequest);
                return Ok(result);
            }
            catch (AuthenticationException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
            catch (ArgumentNullException exception)
            {
               return NotFound(new { message = exception.ParamName });
            }
            catch (Exception exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [HttpGet("GetBankAccountsForVendor")]
        public async Task<ActionResult<List<VendorBankAccountResponse>>> GetBankAccountsForVendor([FromQuery, Required] string token, [FromQuery, Required] string vendorId)
        {
            try
            {
                var result = await _vendorBankAccountService.GetBankAccountsForVendor(token, vendorId);
                return Ok(result);
            }
            catch (AuthenticationException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
            catch (ArgumentNullException exception)
            {
                return NotFound(new { message = exception.ParamName });
            }
            catch (Exception exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [HttpDelete("DeleteVendorBankAccounts")]
        public async Task<ActionResult> DeleteVendorBankAccounts([FromQuery, Required] string token, [FromQuery, Required] string vendorId)
        {
            try
            {
                await _vendorBankAccountService.DeleteVendorBankAccounts(token, vendorId);
                return Ok(new { message = "Bank accounts successfully deleted!"});
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
