using System.Security.Claims;
using Api.Domain.Entities;
using Api.Domain.Enums;
using Api.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/test")]
[Authorize]
public class TestController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("leads")]
    public async Task<IActionResult> CreateLead([FromBody] CreateTestLeadRequest request)
    {
        var organizationId = GetOrganizationId();

        var lead = new Lead
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            PropertyId = request.PropertyId,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Message = request.Message,
            Status = LeadStatus.New,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Leads.Add(lead);
        await _context.SaveChangesAsync();

        return Ok(new { id = lead.Id, message = "Lead created successfully" });
    }

    private Guid GetOrganizationId()
    {
        var orgIdClaim = User.FindFirst("organizationId")?.Value;
        if (orgIdClaim == null || !Guid.TryParse(orgIdClaim, out var orgId))
            throw new UnauthorizedAccessException("Organization ID not found in token");

        return orgId;
    }
}

public class CreateTestLeadRequest
{
    public Guid? PropertyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}