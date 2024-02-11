namespace RepairOperationService.Specifications;

internal class RepairOperationByOperationTypeNameSpec : RepairOperationWithFaultReportSpecification
{
    public RepairOperationByOperationTypeNameSpec(string operationTypeName)
    {
        AddCriteria(ro=>ro.OperationType != null && ro.OperationType.Name == operationTypeName);
    }
}