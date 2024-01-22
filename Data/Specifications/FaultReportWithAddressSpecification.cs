using Data.Models;

namespace Data.Specifications
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
