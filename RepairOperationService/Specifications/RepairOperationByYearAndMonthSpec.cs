namespace RepairOperationService.Specifications;

internal class RepairOperationByYearAndMonthSpec : RepairOperationWithFaultReportSpecification
{
    public RepairOperationByYearAndMonthSpec(int year, int month)
    {
        AddCriteria(ro=>ro.EndDate.HasValue && ro.EndDate.Value.Year == year && ro.EndDate.Value.Month == month);
    }
}