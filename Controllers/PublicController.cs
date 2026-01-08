using Api.Application.DTOs.Public;
using Api.Application.Services;
using Api.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/public")]
public class PublicController : ControllerBase
{
    private readonly IPublicService _publicService;

    public PublicController(IPublicService publicService)
    {
        _publicService = publicService;
    }

    /// <summary>
    /// Liste des properties publiées d'une agence (accessible sans auth)
    /// </summary>
    [HttpGet("agencies/{slug}/properties")]
    public async Task<IActionResult> GetPropertiesByAgency(
        string slug,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? city,
        [FromQuery] PropertyType? type)
    {
        var properties = await _publicService.GetPropertiesByAgency(slug, minPrice, maxPrice, city, type);
        return Ok(properties);
    }

    /// <summary>
    /// Détail d'une property (accessible sans auth)
    /// </summary>
    [HttpGet("properties/{id}")]
    public async Task<IActionResult> GetPropertyById(Guid id)
    {
        var property = await _publicService.GetPropertyById(id);

        if (property == null)
            return NotFound(new { message = "Property not found or not published" });

        return Ok(property);
    }

    /// <summary>
    /// Créer un lead depuis le formulaire de contact public
    /// </summary>
    [HttpPost("properties/{id}/lead")]
    public async Task<IActionResult> CreateLead(Guid id, [FromBody] CreatePublicLeadRequest request)
    {
        try
        {
            var leadId = await _publicService.CreateLeadForProperty(id, request);
            return Ok(new 
            { 
                message = "Your request has been sent successfully. We will contact you soon.",
                leadId = leadId 
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}