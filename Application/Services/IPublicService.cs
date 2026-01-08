using Api.Application.DTOs.Public;

namespace Api.Application.Services;

public interface IPublicService
{
    Task<IEnumerable<PublicPropertyDto>> GetPropertiesByAgency(
        string slug,
        decimal? minPrice,
        decimal? maxPrice,
        string? city,
        string? type
    );
    
    Task<PublicPropertyDetailDto?> GetPropertyById(Guid propertyId);
    
    Task<Guid> CreateLeadForProperty(Guid propertyId, CreatePublicLeadRequest request);
}