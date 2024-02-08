using DataContextLib.Models;

namespace RepairOperationService.Specifications;

internal class RepairOperationByStatusSpec : RepairOperationWithFaultReportSpecification
{
    public RepairOperationByStatusSpec(FaultReportStatus status)
    {
        AddCriteria(ro=>ro.FaultReport.Status == status);
    }
}