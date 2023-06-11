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
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("CreateTransaction")]
        public async Task<ActionResult<TransactionResponse>> CreateTransaction([FromBody] TransactionRequest transactionDto, [FromQuery, Required] string token)
        {
            try
            {
                var result = await _transactionService.CreateTransaction(token, transactionDto);
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

        [HttpGet("GetTransactionsForUser")]
        public async Task<ActionResult<List<TransactionResponse>>> GetTransactionsForUser([FromQuery, Required] string token, [FromQuery, Required] string pageNumber, [FromQuery, Required] string pageSize, [FromQuery] TransactionsFilter filter, [FromQuery] string? sortingOrder)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsForUser(token, pageNumber, pageSize, filter, sortingOrder == null ? "default" : sortingOrder);
                return transactions;
            }
            catch (ArgumentOutOfRangeException exception) // PageNumber/pageSize out of range
            {
                return BadRequest(new { message = exception.ParamName });
            }
            catch (NullReferenceException exception) // No match between user and transactions
            {
                return NotFound(new { message = exception.Message });
            }
            catch (AuthenticationException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
            catch (ArgumentException exception) // Invalid amount range filter
            {
                return BadRequest(new { message = exception.Message });
            }
            catch (Exception exception)
            {
                return BadRequest(new { message = "Invalid parameter format!" });
            }
        }

        [HttpGet("GroupTransactionsByCurrency")]
        public async Task<ActionResult<List<TransactionsGroupResponse>>> GroupTransactionsByCurrency([FromQuery, Required] string token)
        {
            try
            {
                var transactions = await _transactionService.GroupTransactionsByCurrency(token);
                return Ok(transactions);
            }
            catch (AuthenticationException exception) 
            {
                return Unauthorized(new { message = exception.Message });
            }
            catch (NullReferenceException exception)
            {
                return NotFound(new { message = exception.Message });
            }
        }

        [HttpGet("GroupTransactionsByType")]
        public async Task<ActionResult<List<TransactionsGroupResponse>>> GroupTransactionsByType([FromQuery, Required] string token)
        {
            try
            {
                var transactions = await _transactionService.GroupTransactionsByType(token);
                return Ok(transactions);
            }
            catch (AuthenticationException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
            catch (NullReferenceException exception)
            {
                return NotFound(new { message = exception.Message });
            }
        }

        [HttpGet("GetTransactionById")]
        public async Task<ActionResult<TransactionResponse>> GetTransactionById([FromQuery, Required] string token, [FromQuery, Required] string transactionId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionById(token, transactionId);
                return Ok(transactions);
            }
            catch (AuthenticationException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
            catch (NullReferenceException exception)
            {
                return NotFound(new { message = exception.Message });
            }
            catch (Exception exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }
    }
}
