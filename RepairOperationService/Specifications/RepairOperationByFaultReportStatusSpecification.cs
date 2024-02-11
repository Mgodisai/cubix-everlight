using DataContextLib.Models;

namespace RepairOperationService.Specifications
{
    internal class RepairOperationByFaultReportStatusSpecification 
        : RepairOperationWithFaultReportSpecification
    {
        public RepairOperationByFaultReportStatusSpecification(FaultReportStatus faultReportStatus) : base()
        {
            AddCriteria(ro=>ro.FaultReport != null && ro.FaultReport.Status == faultReportStatus);
        }
    }
}
