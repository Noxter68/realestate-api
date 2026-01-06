namespace Api.Domain.Entities;

public class Visit
{
  public Guid Id { get; set; }
  public Guid PropertyId { get; set; }
  public Guid OrganizationId { get; set; }
  public string VisitorName { get; set; } = string.Empty;
  public string VisitorEmail { get; set; } = string.Empty;
  public string VisitorPhone { get; set; } = string.Empty;
  public DateTime ScheduledAt { get; set; }
  public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled
  public string? Notes { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  // Navigation
  public Property Property { get; set; } = null!;
  public Organization Organization { get; set; } = null!;
}