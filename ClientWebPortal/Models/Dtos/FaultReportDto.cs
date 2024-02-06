using System.Text.Json.Serialization;

namespace ClientWebPortal.Models.Dtos;

public class FaultReportDto
{
    public Guid? Id { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Email { get; set; }
    public string ReportedAt { get; set; }
    public AddressDto AddressDto { get; set; }
}