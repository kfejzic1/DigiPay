using Microsoft.AspNetCore.Authorization;
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
    public class UserBankAccountController : ControllerBase
    {
        private readonly IUserBankAccountService _accountService;

        public UserBankAccountController(IUserBankAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("CreateAccount/{userId}")]
        public async Task<ActionResult<UserBankAccount>> CreateAccount([FromBody] UserBankAccountRequest accountRequest, [FromQuery][Required] string token, string userId)
        {
            try
            {
                var result = await _accountService.CreateAccount(token, accountRequest, userId);
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

        [HttpPost("CreateAccount")]
        public async Task<ActionResult<UserBankAccount>> CreateAccount([FromBody] UserBankAccountRequest accountRequest, [FromQuery][Required] string token)
        {
            try
            {
                var result = await _accountService.CreateAccount(token, accountRequest);
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

        [HttpGet("GetAllAccountsForUser")]
        public async Task<ActionResult<List<UserBankAccount>>> GetAllAccountsForUser([FromQuery][Required] string token)
        {
            try
            {
                var result = await _accountService.GetAllAccountsForUser(token);
                return Ok(result);
            }
            catch (AuthenticationException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
            catch (Exception exception)
            {
                return NotFound(new { message = exception.Message });
            }
        }

        [HttpGet("GetAllAccounts")]
        public async Task<ActionResult<List<NonPersonalBankAccountResponse>>> GetAllAccounts([FromQuery][Required] string token)
        {
            try
            {
                var result = await _accountService.GetAllAccounts(token);
                return Ok(result);
            }
            catch (AuthenticationException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
        }

        [HttpDelete("DeleteAllAccountsForUser")]
        public async Task<ActionResult> DeleteAllAccountsForUser([FromQuery][Required] string token)
        {
            try
            {
                await _accountService.DeleteAllAccountsForUser(token);
                return Ok();
            }
            catch (AuthenticationException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
        }

    }
}
