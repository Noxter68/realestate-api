using Api.Application.DTOs.Public;
using Api.Domain.Entities;
using Api.Domain.Enums;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class PublicService : IPublicService
{
    private readonly AppDbContext _context;

    public PublicService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PublicPropertyDto>> GetPropertiesByAgency(
        string slug,
        decimal? minPrice,
        decimal? maxPrice,
        string? city,
        PropertyType? type)
    {
        // Trouver l'organisation par slug
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Slug == slug);

        if (organization == null)
            return Enumerable.Empty<PublicPropertyDto>();

        // Query de base : uniquement Published
        var query = _context.Properties
            .Where(p => p.OrganizationId == organization.Id && p.Status == PropertyStatus.Published);

        // Filtres optionnels
        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(p => p.City.ToLower() == city.ToLower());

        if (type.HasValue)
            query = query.Where(p => p.Type == type.Value);

        // Tri par date de création (plus récentes en premier)
        var properties = await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PublicPropertyDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Type = p.Type,
                Price = p.Price,
                City = p.City,
                Address = p.Address,
                PostalCode = p.PostalCode,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Surface = p.Surface,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return properties;
    }

    public async Task<PublicPropertyDetailDto?> GetPropertyById(Guid propertyId)
    {
        var property = await _context.Properties
            .Include(p => p.Organization)
            .Where(p => p.Id == propertyId && p.Status == PropertyStatus.Published)
            .Select(p => new PublicPropertyDetailDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Type = p.Type,
                Price = p.Price,
                City = p.City,
                Address = p.Address,
                PostalCode = p.PostalCode,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Surface = p.Surface,
                CreatedAt = p.CreatedAt,
                Agency = new AgencySummaryDto
                {
                    Id = p.Organization.Id,
                    Name = p.Organization.Name,
                    Slug = p.Organization.Slug
                }
            })
            .FirstOrDefaultAsync();

        return property;
    }

    public async Task<Guid> CreateLeadForProperty(Guid propertyId, CreatePublicLeadRequest request)
    {
        // Vérifier que la property existe et est Published
        var property = await _context.Properties
            .Where(p => p.Id == propertyId && p.Status == PropertyStatus.Published)
            .FirstOrDefaultAsync();

        if (property == null)
            throw new InvalidOperationException("Property not found or not available");

        // Créer le lead
        var lead = new Lead
        {
            Id = Guid.NewGuid(),
            OrganizationId = property.OrganizationId, // ✅ Récupère l'OrganizationId de la property
            PropertyId = propertyId,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Message = request.Message,
            Status = LeadStatus.New, // ✅ Toujours "New" pour les leads publics
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Leads.Add(lead);
        await _context.SaveChangesAsync();

        return lead.Id;
    }
}