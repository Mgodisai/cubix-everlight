namespace Data.Models
{
    public class FaultReport : BaseEntity
    {
        public Guid AddressId { get; set; }
        public Address? Address { get; set; }
        public DateTime ReportedAt { get; }
        public string? Description { get; set; }
        public string? Email { get; set; }

        public FaultReportStatus Status { get; set; }

        public FaultReport() : base()
        {
            ReportedAt = DateTime.UtcNow;
            Status = FaultReportStatus.New;
        }
    }
}
