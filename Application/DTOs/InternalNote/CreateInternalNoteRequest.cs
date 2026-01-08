using Api.Domain.Enums;

namespace Api.Application.DTOs.InternalNote;

public class CreateInternalNoteRequest
{
    public EntityType EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string Content { get; set; } = string.Empty;
}