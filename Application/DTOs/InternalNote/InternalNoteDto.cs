using Api.Domain.Enums;

namespace Api.Application.DTOs.InternalNote;

public class InternalNoteDto
{
    public Guid Id { get; set; }
    public EntityType EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserSummaryDto User { get; set; } = null!;
}

public class UserSummaryDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}