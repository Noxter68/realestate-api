using Api.Application.DTOs.Auth;

namespace Api.Application.Services;

public interface IAuthService
{
  Task<AuthResponse> RegisterAgency(RegisterAgencyRequest request);
  Task<AuthResponse> Login(LoginRequest request);
  Task<AuthResponse> RefreshToken(string refreshToken);
  Task Logout(string refreshToken);
}