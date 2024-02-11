namespace RepairOperationService.Specifications
{
    internal class RepairOperationByFaultReportIdSpecification
        : RepairOperationWithFaultReportSpecification
    {
        public RepairOperationByFaultReportIdSpecification(Guid faultReportGuid) : base()
        {
            AddCriteria(ro=>ro.FaultReportId==faultReportGuid);
        }
    }
}
