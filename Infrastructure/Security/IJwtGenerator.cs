using Api.Domain.Entities;

namespace Api.Infrastructure.Security;

public interface IJwtGenerator
{
  string GenerateAccessToken(User user);
  string GenerateRefreshToken();
}