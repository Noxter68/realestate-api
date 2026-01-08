using Api.Application.DTOs.InternalNote;
using Api.Domain.Enums;

namespace Api.Application.Services;

public interface IInternalNoteService
{
    Task<InternalNoteDto> Create(
        Guid organizationId, 
        Guid userId, 
        CreateInternalNoteRequest request
    );
    
    Task<IEnumerable<InternalNoteDto>> GetAll(
        Guid organizationId, 
        EntityType? entityType, 
        Guid? entityId
    );
}