using Api.Application.DTOs.Visit;
using Api.Domain.Entities;
using Api.Domain.Enums;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class VisitService : IVisitService
{
  private readonly AppDbContext _context;

  public VisitService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<VisitDto> Create(Guid organizationId, Guid propertyId, CreateVisitRequest request)
  {
    // Vérifier que la property existe et appartient à l'organisation
    var property = await _context.Properties
      .FirstOrDefaultAsync(p => p.Id == propertyId && p.OrganizationId == organizationId);

    if (property == null)
      throw new InvalidOperationException("Property not found");

    var visit = new Visit
    {
      Id = Guid.NewGuid(),
      PropertyId = propertyId,
      OrganizationId = organizationId,
      VisitorName = request.VisitorName,
      VisitorEmail = request.VisitorEmail,
      VisitorPhone = request.VisitorPhone,
      ScheduledAt = request.ScheduledAt.ToUniversalTime(),
      Status = VisitStatus.Scheduled,
      Notes = request.Notes,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _context.Visits.Add(visit);
    await _context.SaveChangesAsync();

    // Recharger avec la property pour le DTO
    await _context.Entry(visit).Reference(v => v.Property).LoadAsync();

    return MapToDto(visit);
  }

  public async Task<IEnumerable<VisitDto>> GetByProperty(Guid organizationId, Guid propertyId)
  {
    var visits = await _context.Visits
      .Include(v => v.Property)
      .Where(v => v.PropertyId == propertyId && v.OrganizationId == organizationId)
      .OrderBy(v => v.ScheduledAt)
      .ToListAsync();

    return visits.Select(MapToDto);
  }

  private static VisitDto MapToDto(Visit visit)
  {
    return new VisitDto
    {
      Id = visit.Id,
      PropertyId = visit.PropertyId,
      VisitorName = visit.VisitorName,
      VisitorEmail = visit.VisitorEmail,
      VisitorPhone = visit.VisitorPhone,
      ScheduledAt = visit.ScheduledAt,
      Status = visit.Status,
      Notes = visit.Notes,
      CreatedAt = visit.CreatedAt,
      Property = new PropertySummaryDto
      {
        Id = visit.Property.Id,
        Title = visit.Property.Title,
        City = visit.Property.City,
        Address = visit.Property.Address
      }
    };
  }
}