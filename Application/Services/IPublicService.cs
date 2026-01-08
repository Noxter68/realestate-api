using Api.Application.DTOs.Public;
using Api.Domain.Enums;

namespace Api.Application.Services;

public interface IPublicService
{
    Task<IEnumerable<PublicPropertyDto>> GetPropertiesByAgency(
        string slug,
        decimal? minPrice,
        decimal? maxPrice,
        string? city,
        PropertyType? type
    );
    
    Task<PublicPropertyDetailDto?> GetPropertyById(Guid propertyId);
    
    Task<Guid> CreateLeadForProperty(Guid propertyId, CreatePublicLeadRequest request);
}