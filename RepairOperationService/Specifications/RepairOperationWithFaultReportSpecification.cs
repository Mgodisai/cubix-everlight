using DataContextLib.Models;
using DataContextLib.Specifications;

namespace RepairOperationService.Specifications;

internal class RepairOperationWithFaultReportSpecification : BaseSpecification<RepairOperation>
{
    public RepairOperationWithFaultReportSpecification()
    {
        AddInclude(ro => ro.FaultReport);
        AddInclude(ro=>ro.OperationType);
        AddInclude(ro=>ro.Employee);
        AddInclude(ro => ro.Employee!.Position);
        AddInclude(ro => ro.FaultReport!.Address);
    }
}