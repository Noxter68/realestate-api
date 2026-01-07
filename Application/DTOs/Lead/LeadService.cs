using Api.Application.DTOs.Lead;
using Api.Domain.Enums;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class LeadService : ILeadService
{
  private readonly AppDbContext _context;

  public LeadService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<PagedResult<LeadDto>> GetAll(
    Guid organizationId, 
    LeadStatus? status, 
    string? q, 
    int page = 1, 
    int pageSize = 10)
  {
    var query = _context.Leads
      .Include(l => l.Property)
      .Where(l => l.OrganizationId == organizationId);

    // Filtre par status
    if (status.HasValue)
    {
      query = query.Where(l => l.Status == status.Value);
    }

    // Recherche sur name/email/message (case-insensitive)
    if (!string.IsNullOrWhiteSpace(q))
    {
      var searchTerm = q.ToLower();
      query = query.Where(l => 
        l.Name.ToLower().Contains(searchTerm) ||
        l.Email.ToLower().Contains(searchTerm) ||
        l.Message.ToLower().Contains(searchTerm)
      );
    }

    // Total count avant pagination
    var totalCount = await query.CountAsync();

    // Order by createdAt desc + pagination
    var leads = await query
      .OrderByDescending(l => l.CreatedAt)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .Select(l => new LeadDto
      {
        Id = l.Id,
        PropertyId = l.PropertyId,
        Name = l.Name,
        Email = l.Email,
        Phone = l.Phone,
        Message = l.Message,
        Status = l.Status,
        CreatedAt = l.CreatedAt,
        Property = l.Property != null ? new PropertySummaryDto
        {
          Id = l.Property.Id,
          Title = l.Property.Title,
          City = l.Property.City
        } : null
      })
      .ToListAsync();

    return new PagedResult<LeadDto>
    {
      Items = leads,
      TotalCount = totalCount,
      Page = page,
      PageSize = pageSize
    };
  }

  public async Task<LeadDto?> UpdateStatus(Guid organizationId, Guid leadId, LeadStatus status)
  {
    var lead = await _context.Leads
      .Include(l => l.Property)
      .FirstOrDefaultAsync(l => l.Id == leadId && l.OrganizationId == organizationId);

    if (lead == null) return null;

    lead.Status = status;
    lead.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return new LeadDto
    {
      Id = lead.Id,
      PropertyId = lead.PropertyId,
      Name = lead.Name,
      Email = lead.Email,
      Phone = lead.Phone,
      Message = lead.Message,
      Status = lead.Status,
      CreatedAt = lead.CreatedAt,
      Property = lead.Property != null ? new PropertySummaryDto
      {
        Id = lead.Property.Id,
        Title = lead.Property.Title,
        City = lead.Property.City
      } : null
    };
  }
}