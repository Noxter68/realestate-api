using Api.Domain.Enums;

namespace Api.Application.DTOs.Property;

public class PropertyDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public decimal Price { get; set; }
  public string City { get; set; } = string.Empty;
  public string Address { get; set; } = string.Empty;
  public string PostalCode { get; set; } = string.Empty;
  public int? Bedrooms { get; set; }
  public int? Bathrooms { get; set; }
  public decimal? Surface { get; set; }
  public PropertyStatus Status { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}