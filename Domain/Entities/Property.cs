namespace Api.Domain.Entities;

public class Property
{
  public Guid Id { get; set; }
  public Guid OrganizationId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty; // Apartment, House, Commercial
  public decimal Price { get; set; }
  public string City { get; set; } = string.Empty;
  public string Address { get; set; } = string.Empty;
  public string PostalCode { get; set; } = string.Empty;
  public int? Bedrooms { get; set; }
  public int? Bathrooms { get; set; }
  public decimal? Surface { get; set; } // m²
  public string Status { get; set; } = "Draft"; // Draft, Published, Sold, Rented
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  // Navigation
  public Organization Organization { get; set; } = null!;
  public ICollection<Lead> Leads { get; set; } = new List<Lead>();
  public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}