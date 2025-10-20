using System.Threading.Tasks;
using DebtCollectionPortal.Application;
using DebtCollectionPortal.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DebtCollectionPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebUserController : ControllerBase
{ 

    private readonly IUserService _userService;

    public WebUserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var result = await _userService.RegisterAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (DomainException)
        {
            return BadRequest(new { Success = false, Message = "Invalid request" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] LoginDto dto)
    {
        try
        {
            var result = await _userService.LoginAsync(dto);
            return result.Success ? Ok(result) : Unauthorized(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Success = false, Message = "Internal server error" });
        }
    }
}