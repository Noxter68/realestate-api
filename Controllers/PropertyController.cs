using Api.Application.DTOs.Property;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/agency/properties")]
[Authorize]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        var organizationId = GetOrganizationId();
        var property = await _propertyService.Create(organizationId, request);
        return Ok(property);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizationId = GetOrganizationId();
        var properties = await _propertyService.GetAll(organizationId);
        return Ok(properties);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyRequest request)
    {
        var organizationId = GetOrganizationId();
        var property = await _propertyService.Update(organizationId, id, request);

        if (property == null)
            return NotFound();
        
        return Ok(property);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var organizationId = GetOrganizationId();
        var deleted = await _propertyService.Delete(organizationId, id);
    
        if (!deleted)
            return NotFound();
    
        return NoContent();
    }

    private Guid GetOrganizationId()
    {
        var orgIdClaim = User.FindFirst("organizationId")?.Value;
        if (orgIdClaim == null || !Guid.TryParse(orgIdClaim, out var orgId))
            throw new UnauthorizedAccessException("Organization ID not found in token");

        return orgId;
    }
}