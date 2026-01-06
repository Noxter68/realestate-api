namespace Api.Application.DTOs.Auth;

public class UserDto
{
  public Guid Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
  public OrganizationDto Organization { get; set; } = null!;
}

public class OrganizationDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Slug { get; set; } = string.Empty;
}