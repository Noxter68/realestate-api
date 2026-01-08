using Api.Application.DTOs.Visit;

namespace Api.Application.Services;

public interface IVisitService
{
    Task<VisitDto> Create(Guid organizationId, Guid propertyId, CreateVisitRequest request);
    Task<IEnumerable<VisitDto>> GetByProperty(Guid organizationId, Guid propertyId);
}