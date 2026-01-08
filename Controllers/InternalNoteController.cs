using System.Security.Claims;
using Api.Application.DTOs.InternalNote;
using Api.Application.Services;
using Api.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/agency/notes")]
[Authorize]
public class InternalNoteController : ControllerBase
{
    private readonly IInternalNoteService _noteService;

    public InternalNoteController(IInternalNoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInternalNoteRequest request)
    {
        try
        {
            var organizationId = GetOrganizationId();
            var userId = GetUserId();

            var note = await _noteService.Create(organizationId, userId, request);
            return Ok(note);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] EntityType? entity_type,
        [FromQuery] Guid? entity_id)
    {
        var organizationId = GetOrganizationId();
        var notes = await _noteService.GetAll(organizationId, entity_type, entity_id);
        return Ok(notes);
    }

    private Guid GetOrganizationId()
    {
        var orgIdClaim = User.FindFirst("organizationId")?.Value;
        if (orgIdClaim == null || !Guid.TryParse(orgIdClaim, out var orgId))
            throw new UnauthorizedAccessException("Organization ID not found in token");

        return orgId;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User ID not found in token");

        return userId;
    }
}