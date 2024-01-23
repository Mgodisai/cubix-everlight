using Data.Models;
using Data.Specifications;

namespace ClientWebPortal.Service.Specifications
{
    public class FaultReportWithAddressSpecification : BaseSpecification<FaultReport>
    {
        public FaultReportWithAddressSpecification(Guid faultReportId)
        {
            AddCriteria(fr => fr.Id == faultReportId);
            AddInclude(fr => fr.Address);
        }
    }

}
