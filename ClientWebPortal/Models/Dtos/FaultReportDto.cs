namespace ClientWebPortal.Models.Dtos;

public class FaultReportDto
{
    public Guid? Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ReportedAt { get; set; } = string.Empty;
    public AddressDto? AddressDto { get; set; }
}