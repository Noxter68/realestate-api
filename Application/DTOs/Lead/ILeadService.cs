using Api.Domain.Enums;

namespace Api.Application.DTOs.Lead;

public interface ILeadService
{
    Task<PagedResult<LeadDto>> GetAll(
        Guid organizationId, 
        LeadStatus? status, 
        string? q, 
        int page, 
        int pageSize
    );
  
    Task<LeadDto?> UpdateStatus(Guid organizationId, Guid leadId, LeadStatus status);
}