using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DebtCollectionPortal.Application;
using DebtCollectionPortal.Domain;

namespace DebtCollectionPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IClientService _clientService;

    public AccountController(IAccountService accountService, IClientService clientService)
    {
        _accountService = accountService;
        _clientService = clientService;
    }

    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetAccount(string accountId)
    {
        try
        {
            var accId = (AccountId)accountId;
            var account = await _accountService.GetAccountAsync(accId);
            return account != null ? Ok(account) : NotFound(new { Message = "Account not found" });
        }
        catch (DomainException)
        {
            return BadRequest(new { Message = "Invalid request" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    
}