using Data.Specifications;
using DataContextLib.Models;

namespace RepairOperationService.Specifications
{
    internal class RepairOperationWithFaultReportSpecification : BaseSpecification<RepairOperation>
    {
        public RepairOperationWithFaultReportSpecification()
        {
            AddInclude(ro => ro.FaultReport);
        }
    }
}
