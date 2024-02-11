namespace DataContextLib.Models;

public class FaultReport : BaseEntity
{
    public Guid AddressId { get; set; }
    public Address? Address { get; set; }
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public string? Email { get; set; }

    public FaultReportStatus Status { get; set; } = FaultReportStatus.New;

    public override string? ToString()
    {
        return $"FaultReport ({Status}): {Address} - {Description}, reported: {ReportedAt}";
    }
}