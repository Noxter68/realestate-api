

using Api.Application.DTOs.Property;
using Api.Domain.Entities;
using Api.Domain.Enums;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class PropertyService: IPropertyService
{
  private readonly AppDbContext _context;

  public PropertyService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<PropertyDto> Create(Guid organizationId, CreatePropertyRequest request)
  {
    var property = new Property
    {
      Id = Guid.NewGuid(),
      OrganizationId = organizationId,
      Title = request.Title,
      Description = request.Description,
      Type = request.Type,
      Price = request.Price,
      City = request.City,
      Address = request.Address,
      PostalCode = request.PostalCode,
      Bedrooms = request.Bedrooms,
      Bathrooms = request.Bathrooms,
      Surface = request.Surface,
      Status = PropertyStatus.Draft,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _context.Properties.Add(property);
    await _context.SaveChangesAsync();
    
    return MapToDto(property);
  }
  
  public async Task<PropertyDto?> GetById(Guid organizationId, Guid propertyId)
  {
    var property = await _context.Properties
      .FirstOrDefaultAsync(p => p.Id == propertyId && p.OrganizationId == organizationId);

    return property == null ? null : MapToDto(property);
  }

  public async Task<IEnumerable<PropertyDto>> GetAll(Guid organizationId)
  {
    var properties = await _context.Properties
      .Where(p => p.OrganizationId == organizationId)
      .OrderByDescending(o => o.CreatedAt)
      .ToListAsync();

    return properties.Select(MapToDto);
  }

  public async Task<PropertyDto?> Update(Guid organizationId, Guid propertyId, UpdatePropertyRequest request)
  {
    var property = await _context.Properties
      .FirstOrDefaultAsync(p => p.Id == propertyId && p.OrganizationId == organizationId);

    if (property == null) return null;

    if (request.Title != null) property.Title = request.Title;
    if (request.Description != null) property.Description = request.Description;
    if (request.Type != null) property.Type = request.Type.Value;
    if (request.Price.HasValue) property.Price = request.Price.Value;
    if (request.City != null) property.City = request.City;
    if (request.Address != null) property.Address = request.Address;
    if (request.PostalCode != null) property.PostalCode = request.PostalCode;
    if (request.Bedrooms.HasValue) property.Bedrooms = request.Bedrooms;
    if (request.Bathrooms.HasValue) property.Bathrooms = request.Bathrooms;
    if (request.Surface.HasValue) property.Surface = request.Surface;
    if (request.Status.HasValue) property.Status = request.Status.Value;

    property.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return MapToDto(property);
  }

  public async Task<bool> Delete(Guid organizationId, Guid propertyId)
  {
    var property = await _context.Properties
      .FirstOrDefaultAsync(p => p.Id == propertyId && p.OrganizationId == organizationId);

    if (property == null) return false;

    _context.Properties.Remove(property);
    await _context.SaveChangesAsync();
    return true;

  }
  
  private static PropertyDto MapToDto(Property property)
  {
    return new PropertyDto
    {
      Id = property.Id,
      Title = property.Title,
      Description = property.Description,
      Type = property.Type,
      Price = property.Price,
      City = property.City,
      Address = property.Address,
      PostalCode = property.PostalCode,
      Bedrooms = property.Bedrooms,
      Bathrooms = property.Bathrooms,
      Surface = property.Surface,
      Status = property.Status,
      CreatedAt = property.CreatedAt,
      UpdatedAt = property.UpdatedAt
    };
  }
}
