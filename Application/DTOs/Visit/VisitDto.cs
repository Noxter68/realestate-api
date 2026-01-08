using Api.Domain.Enums;

namespace Api.Application.DTOs.Visit;

public class VisitDto
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string VisitorEmail { get; set; } = string.Empty;
    public string VisitorPhone { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public VisitStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public PropertySummaryDto Property { get; set; } = null!;
}

public class PropertySummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}