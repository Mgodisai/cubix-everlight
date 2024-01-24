namespace RepairOperationService.Specifications
{
    internal class RepairOperationByOperationIdSpecification 
        : RepairOperationWithFaultReportSpecification
    {
        public RepairOperationByOperationIdSpecification(Guid operationId) : base()
        {
            AddCriteria(ro=>ro.Id == operationId);
        }
    }
}
