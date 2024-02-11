namespace RepairOperationService.Specifications;

internal class RepairOperationByEmployeeNameSpec : RepairOperationWithFaultReportSpecification
{
    public RepairOperationByEmployeeNameSpec(string employeeName)
    {
        AddCriteria(ro=>ro.Employee != null && ro.Employee.DisplayName == employeeName);
    }
}