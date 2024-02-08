using DataContextLib.Models;

namespace RepairOperationService.Specifications;

internal class RepairOperationByEmployeeNameSpec : RepairOperationWithFaultReportSpecification
{
    public RepairOperationByEmployeeNameSpec(string employeeName)
    {
        AddCriteria(ro=>ro.Employee.DisplayName == employeeName);
    }
}