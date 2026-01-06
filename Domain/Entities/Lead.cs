namespace Api.Domain.Entities;

public class Lead
{
  public Guid Id { get; set; }
  public Guid OrganizationId { get; set; }
  public Guid? PropertyId { get; set; } // Nullable (lead général ou pour un bien)
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Phone { get; set; } = string.Empty;
  public string Message { get; set; } = string.Empty;
  public string Status { get; set; } = "New"; // New, Contacted, Qualified, Lost
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  // Navigation
  public Organization Organization { get; set; } = null!;
  public Property? Property { get; set; }
}