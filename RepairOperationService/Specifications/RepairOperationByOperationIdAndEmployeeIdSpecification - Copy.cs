namespace RepairOperationService.Specifications
{
    internal class RepairOperationByOperationIdAndEmployeeIdSpecification 
        : RepairOperationByOperationIdSpecification
    {
        public RepairOperationByOperationIdAndEmployeeIdSpecification(Guid operationId, Guid employeeId) : base(operationId)
        {
            AddCriteria(ro=>ro.EmployeeId == employeeId);
        }
    }
}
