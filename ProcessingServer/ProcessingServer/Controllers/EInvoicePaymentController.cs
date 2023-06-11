using Microsoft.AspNetCore.Mvc;
using ProcessingServer.BLL.DTO;
using ProcessingServer.BLL.DTO.EInvoicePayment;
using ProcessingServer.BLL.Interfaces;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Other;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace ProcessingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EInvoicePaymentController : ControllerBase
    {
        private readonly IEInvoicePaymentHistoryService _eInvoicePaymentHistoryService;

        public EInvoicePaymentController(IEInvoicePaymentHistoryService eInvoicePaymentHistoryService)
        {
            _eInvoicePaymentHistoryService = eInvoicePaymentHistoryService;
        }

        [HttpPost("ExecuteInvoicePayment")]
        public async Task<ActionResult<Object>> CreateTransaction([FromBody] EInvoiceRequest eInvoiceRequest, [FromQuery, Required] string token)
        {
            try
            {
                var result = await _eInvoicePaymentHistoryService.ExecuteInvoicePayment(token, eInvoiceRequest);
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
