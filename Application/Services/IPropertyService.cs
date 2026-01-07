using Api.Application.DTOs.Property;

public interface IPropertyService
{
  Task<PropertyDto> Create(Guid organizationId, CreatePropertyRequest request);
  Task<PropertyDto?> GetById(Guid organizationId, Guid propertyId);
  Task<IEnumerable<PropertyDto>> GetAll(Guid organizationId);
  Task<PropertyDto?> Update(Guid organizationId, Guid propertyId, UpdatePropertyRequest request);
  Task<bool> Delete(Guid organizationId, Guid propertyId);
}
