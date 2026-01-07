using Api.Domain.Enums;

namespace Api.Application.DTOs.Lead;

public class UpdateLeadStatusRequest
{
    public LeadStatus Status { get; set; }
}