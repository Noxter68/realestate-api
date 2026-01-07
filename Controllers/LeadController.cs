using Api.Application.DTOs.Lead;
using Api.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/agency/leads")]
[Authorize]
public class LeadController : ControllerBase
{
    private readonly ILeadService _leadService;

    public LeadController(ILeadService leadService)
    {
        _leadService = leadService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] LeadStatus? status,
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var organizationId = GetOrganizationId();
        var result = await _leadService.GetAll(organizationId, status, q, page, pageSize);
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateLeadStatusRequest request)
    {
        var organizationId = GetOrganizationId();
        var lead = await _leadService.UpdateStatus(organizationId, id, request.Status);

        if (lead == null)
            return NotFound();

        return Ok(lead);
    }

    private Guid GetOrganizationId()
    {
        var orgIdClaim = User.FindFirst("organizationId")?.Value;
        if (orgIdClaim == null || !Guid.TryParse(orgIdClaim, out var orgId))
            throw new UnauthorizedAccessException("Organization ID not found in token");

        return orgId;
    }
}