using Api.Application.DTOs.InternalNote;
using Api.Domain.Entities;
using Api.Domain.Enums;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class InternalNoteService: IInternalNoteService
{
    private readonly AppDbContext _context;

    public InternalNoteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InternalNoteDto> Create(
        Guid organizationId,
        Guid userId,
        CreateInternalNoteRequest request)
    {
        var note = new InternalNote
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            UserId = userId,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.InternalNotes.Add(note);
        await _context.SaveChangesAsync();
        
        var noteWithUser = await _context.InternalNotes
            .Include(n => n.User)
            .FirstAsync(n => n.Id == note.Id);
        
        return MapToDto(noteWithUser);
    }
    
    public async Task<IEnumerable<InternalNoteDto>> GetAll(
        Guid organizationId, 
        EntityType? entityType, // ✅ Enum nullable
        Guid? entityId)
    {
        var query = _context.InternalNotes
            .Include(n => n.User)
            .Where(n => n.OrganizationId == organizationId);

        if (entityType.HasValue)
        {
            query = query.Where(n => n.EntityType == entityType.Value);
        }

        if (entityId.HasValue)
        {
            query = query.Where(n => n.EntityId == entityId.Value);
        }

        var notes = await query
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new InternalNoteDto
            {
                Id = n.Id,
                EntityType = n.EntityType, // ✅ Enum dans le DTO
                EntityId = n.EntityId,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                User = new UserSummaryDto
                {
                    Id = n.User.Id,
                    FirstName = n.User.FirstName,
                    LastName = n.User.LastName,
                    Email = n.User.Email
                }
            })
            .ToListAsync();

        return notes;
    }

    private static InternalNoteDto MapToDto(InternalNote note)
    {
        return new InternalNoteDto
        {
            Id = note.Id,
            EntityType = note.EntityType,
            EntityId = note.EntityId,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            User = new UserSummaryDto
            {
                Id = note.User.Id,
                FirstName = note.User.FirstName,
                LastName = note.User.LastName,
                Email = note.User.Email
            }
        };
    }
}