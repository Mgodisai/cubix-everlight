using Data.Models;

namespace DataContextLib.Models
{
    public class RepairOperation : BaseEntity
    {
        public Guid FaultReportId { get; set; }
        public FaultReport FaultReport { get; set; }
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public Guid OperationTypeId { get; set; }
        public RepairOperationType OperationType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndTime { get; set; }
    }
}
