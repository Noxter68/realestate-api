using Api.Domain.Enums;

namespace Api.Application.DTOs.Public;

public class PublicPropertyDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PropertyType Type { get; set; } // Enum
    public decimal Price { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public decimal? Surface { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PublicPropertyDetailDto : PublicPropertyDto
{
    public AgencySummaryDto Agency { get; set; } = null!;
}

public class AgencySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}