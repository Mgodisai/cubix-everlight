
using Data.Models;
using Data.Specifications;

namespace ServiceConsole.Specifications
{
    internal class FaultReportsNotCompletedWithAddressSpecification : BaseSpecification<FaultReport>
    {
        public FaultReportsNotCompletedWithAddressSpecification()
        {
            AddCriteria(fr => fr.Status != FaultReportStatus.Completed);
            AddInclude(fr => fr.Address);
        }
    }
}
