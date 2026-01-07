using Api.Domain.Enums;

namespace Api.Application.DTOs.Lead;

public class LeadDto
{
    public Guid Id { get; set; }
    public Guid? PropertyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public LeadStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public PropertySummaryDto? Property { get; set; }
}

public class PropertySummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}