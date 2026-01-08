using Api.Domain.Enums;

namespace Api.Application.DTOs.Property;

public class UpdatePropertyRequest
{
  public string? Title { get; set; }
  public string? Description { get; set; }
  public PropertyType? Type { get; set; } //  Enum nullable
  public decimal? Price { get; set; }
  public string? City { get; set; }
  public string? Address { get; set; }
  public string? PostalCode { get; set; }
  public int? Bedrooms { get; set; }
  public int? Bathrooms { get; set; }
  public decimal? Surface { get; set; }
  public PropertyStatus? Status { get; set; }
}