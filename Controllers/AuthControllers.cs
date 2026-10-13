using Microsoft.AspNetCore.Mvc;
using HealthcarePortalApi.Models;
using HealthcarePortalApi.Services;

namespace HealthcarePortalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService _authService): ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        
        if (result == null)
            return BadRequest("User with this email already exists.");

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result == null)
            return Unauthorized("Invalid email or password.");
        return Ok(result);
    }
}