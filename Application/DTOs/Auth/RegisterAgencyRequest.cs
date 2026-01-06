namespace Api.Application.DTOs.Auth;

public class RegisterAgencyRequest
{
  public string AgencyName { get; set; } = string.Empty;
  public string Slug { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
}