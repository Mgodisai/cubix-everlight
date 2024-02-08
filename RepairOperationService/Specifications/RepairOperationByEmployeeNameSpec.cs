using DataContextLib.Models;

namespace RepairOperationService.Specifications;

internal class RepairOperationByEmployeeSpec : RepairOperationWithFaultReportSpecification
{
    public RepairOperationByEmployeeSpec(Guid employeeGuid)
    {
        AddCriteria(ro=>ro.EmployeeId==employeeGuid );
        AddCriteria(ro=>ro.FaultReport.Status == FaultReportStatus.InProgress);
    }
}