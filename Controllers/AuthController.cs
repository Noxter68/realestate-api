using System.Security.Claims;
using Api.Application.DTOs.Auth;
using Api.Application.Services;
using Api.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;
  private readonly AppDbContext _context;

  public AuthController(IAuthService authService, AppDbContext context)
  {
    _authService = authService;
    _context = context;
  }

  [HttpPost("register-agence")]
  public async Task<IActionResult> RegisterAgency([FromBody] RegisterAgencyRequest request)
  {
    try
    {
      var result = await _authService.RegisterAgency(request);
      return Ok(result);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest request)
  {
    try
    {
      var result = await _authService.Login(request);
      return Ok(result);
    }
    catch (UnauthorizedAccessException ex)
    {
      return Unauthorized(new { message = ex.Message });
    }
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
  {
    try
    {
      var result = await _authService.RefreshToken(request.RefreshToken);
      return Ok(result);
    }
    catch (UnauthorizedAccessException ex)
    {
      return Unauthorized(new { message = ex.Message });
    }
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
  {
    await _authService.Logout(request.RefreshToken);
    return Ok(new { message = "Logged out successfully" });
  }

  [HttpGet("me")]
  [Authorize]  // ← Protected by JWT
  public async Task<IActionResult> GetCurrentUser()
  {
    // Get the id of the user from jwt
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
      return Unauthorized();

    // Charge the user from db
    var user = await _context.Users
    .Include(u => u.Organization)
    .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null)
      return NotFound();

    return Ok(new UserDto
    {
      Id = user.Id,
      Email = user.Email,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Role = user.Role,
      Organization = new OrganizationDto
      {
        Id = user.Organization.Id,
        Name = user.Organization.Name,
        Slug = user.Organization.Slug
      }
    });
  }

}

public class RefreshTokenRequest
{
  public string RefreshToken { get; set; } = string.Empty;
}