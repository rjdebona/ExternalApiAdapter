using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DebtCollectionPortal.Application;
using DebtCollectionPortal.Domain;

namespace DebtCollectionPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly IAccountService _accountService;

    public ClientController(IClientService clientService, IAccountService accountService)
    {
        _clientService = clientService;
        _accountService = accountService;
    }

    [HttpPut("{accountId}/address")]
    public async Task<IActionResult> UpdateAddress(string accountId, [FromBody] AddressDto dto)
    {
        try
        {
            var accId = (AccountId)accountId;
            var account = await _accountService.GetAccountAsync(accId);
            if (account == null) return NotFound(new { Message = "Account not found" });

            var ok = await _clientService.UpdateAddressAsync(accId, dto);
            return ok ? Ok(new { Message = "Address updated" }) : BadRequest(new { Message = "Could not update address" });
        }
        catch (DomainException dex)
        {
            return BadRequest(new { Message = dex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpPut("{accountId}/email")]
    public async Task<IActionResult> UpdateEmail(string accountId, [FromBody] EmailUpdateDto dto)
    {
        try
        {
            var accId = (AccountId)accountId;
            var account = await _accountService.GetAccountAsync(accId);
            if (account == null) return NotFound(new { Message = "Account not found" });

            var ok = await _clientService.UpdateEmailAsync(accId, dto.Email, dto.Password);
            return ok ? Ok(new { Message = "Email updated" }) : BadRequest(new { Message = "Could not update email" });
        }
        catch (DomainException dex)
        {
            return BadRequest(new { Message = dex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }
}
