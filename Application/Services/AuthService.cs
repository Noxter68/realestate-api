using System.Security.Cryptography;
using Api.Application.DTOs.Auth;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class AuthService : IAuthService
{
  private readonly AppDbContext _context;
  private readonly IPasswordHasher _passwordHasher;
  private readonly IJwtGenerator _jwtGenerator;

  public AuthService(AppDbContext context, IPasswordHasher passwordHasher, IJwtGenerator jwtGenerator)
  {
    _context = context;
    _passwordHasher = passwordHasher;
    _jwtGenerator = jwtGenerator;
  }

  public async Task<AuthResponse> RegisterAgency(RegisterAgencyRequest request)
  {
    // check if email already exist
    if (await _context.Users.AnyAsync(u => u.Email == request.Email))
      throw new InvalidOperationException("Email already exists");

    // Check if the slug already exist
    if (await _context.Organizations.AnyAsync(o => o.Slug == request.Slug))
      throw new InvalidOperationException("Agency slug already exists");

    var organization = new Organization
    {
      Id = Guid.NewGuid(),
      Name = request.AgencyName,
      Slug = request.Slug,
      CreatedAt = DateTime.UtcNow,
      UpdateAt = DateTime.UtcNow,
    };

    _context.Organizations.Add(organization);

    var user = new User
    {
      Id = Guid.NewGuid(),
      OrganizationId = organization.Id,
      Email = request.Email,
      PasswordHash = _passwordHasher.Hash(request.Password),
      FirstName = request.FirstName,
      LastName = request.LastName,
      Role = "Owner",
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _context.Users.Add(user);

    // Genrate the token
    var accessToken = _jwtGenerator.GenerateAccessToken(user);
    var refreshTokenValue = _jwtGenerator.GenerateRefreshToken();

    // Stock the refreshed token
    var refreshToken = new RefreshToken
    {
      Id = Guid.NewGuid(),
      UserId = user.Id,
      TokenHash = HasRefreshToken(refreshTokenValue),
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      CreatedAt = DateTime.UtcNow,
      IsRevoked = false
    };

    _context.RefreshTokens.Add(refreshToken);
    await _context.SaveChangesAsync();

    return new AuthResponse
    {
      AccessToken = accessToken,
      RefreshToken = refreshTokenValue,
      User = MapToUserDto(user)
    };
  }

  public async Task<AuthResponse> Login(LoginRequest request)
  {


    // Trouver l'utilisateur avec son organisation
    var user = await _context.Users
        .Include(u => u.Organization)
        .FirstOrDefaultAsync(u => u.Email == request.Email);

    if (user == null)
    {
      throw new UnauthorizedAccessException("Invalid credentials");
    }

    // Vérifier le mot de passe
    if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
      throw new UnauthorizedAccessException("Invalid credentials");

    // Générer les tokens
    var accessToken = _jwtGenerator.GenerateAccessToken(user);
    var refreshTokenValue = _jwtGenerator.GenerateRefreshToken();

    // Stocker le refresh token
    var refreshToken = new RefreshToken
    {
      Id = Guid.NewGuid(),
      UserId = user.Id,
      TokenHash = HasRefreshToken(refreshTokenValue),
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      CreatedAt = DateTime.UtcNow,
      IsRevoked = false
    };

    _context.RefreshTokens.Add(refreshToken);
    await _context.SaveChangesAsync();

    return new AuthResponse
    {
      AccessToken = accessToken,
      RefreshToken = refreshTokenValue,
      User = MapToUserDto(user)
    };
  }
  public async Task<AuthResponse> RefreshToken(string refreshToken)
  {
    var tokenHash = HasRefreshToken(refreshToken);

    // Find the refresh token
    var storedToken = await _context.RefreshTokens.Include(rt => rt.User).ThenInclude(u => u.Organization).FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

    if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
      throw new UnauthorizedAccessException("Invalid refresh token");

    // Generate new access token
    var accessToken = _jwtGenerator.GenerateAccessToken(storedToken.User);

    return new AuthResponse
    {
      AccessToken = accessToken,
      RefreshToken = refreshToken, // On garde le même refresh token
      User = MapToUserDto(storedToken.User)
    };
  }

  public async Task Logout(string refreshToken)
  {
    var tokenHash = HasRefreshToken(refreshToken);

    var storedToken = await _context.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

    if (storedToken != null)
    {
      storedToken.IsRevoked = true;
      await _context.SaveChangesAsync();
    }
  }

  private UserDto MapToUserDto(User user)
  {
    return new UserDto
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
    };
  }

  private string HasRefreshToken(string token)
  {
    using var sha256 = SHA256.Create();
    var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
    return Convert.ToBase64String(bytes);
  }
}
