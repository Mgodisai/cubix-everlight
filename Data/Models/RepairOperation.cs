using System.ComponentModel;

namespace DataContextLib.Models;

public class RepairOperation : BaseEntity
{
    [Browsable(false)]
    public Guid FaultReportId { get; set; }
    public FaultReport FaultReport { get; set; }
    [Browsable(false)]
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; }
    [Browsable(false)]
    public Guid OperationTypeId { get; set; }
    public RepairOperationType OperationType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}