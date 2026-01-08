using System.Security.Claims;
using Api.Application.DTOs.Visit;
using Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/agency/properties/{propertyId}/visits")]
[Authorize]
public class VisitController : ControllerBase
{
    private readonly IVisitService _visitService;

    public VisitController(IVisitService visitService)
    {
        _visitService = visitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByProperty(Guid propertyId)
    {
        var organizationId = GetOrganizationId();
        var visits = await _visitService.GetByProperty(organizationId, propertyId);
        return Ok(visits);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid propertyId, [FromBody] CreateVisitRequest request)
    {
        try
        {
            var organizationId = GetOrganizationId();
            var visit = await _visitService.Create(organizationId, propertyId, request);
            return CreatedAtAction(nameof(GetByProperty), new { propertyId }, visit);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid GetOrganizationId()
    {
        var orgIdClaim = User.FindFirst("organizationId")?.Value;
        if (orgIdClaim == null || !Guid.TryParse(orgIdClaim, out var orgId))
            throw new UnauthorizedAccessException("Organization ID not found in token");

        return orgId;
    }
}