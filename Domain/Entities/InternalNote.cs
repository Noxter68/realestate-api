namespace Api.Domain.Entities;

public class InternalNote
{
  public Guid Id { get; set; }
  public Guid OrganizationId { get; set; }
  public Guid UserId { get; set; }
  public string EntityType { get; set; } = string.Empty; // Property, Lead, Visit
  public Guid EntityId { get; set; }
  public string Content { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  // Navigation
  public Organization Organization { get; set; } = null!;
  public User User { get; set; } = null!;
}