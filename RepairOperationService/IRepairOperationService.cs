using DataContextLib.Models;

namespace RepairOperationService
{
    public interface IRepairOperationService
    {
        Task<IEnumerable<RepairOperation>> ListTakenReportsByEmployee(Employee? userAuthEmployee);
        Task<bool> AssignFaultReportToEmployee(Guid faultReportId, Employee employee);
        Task CompletedRepairOperationAsync(Guid parsedGuid, Employee? userAuthEmployee, string operation);

        Task<IEnumerable<RepairOperation>> GetAllOperations();
        Task<IEnumerable<RepairOperation>> GetOperationsByEmployee(string employeeName);
        Task<IEnumerable<RepairOperation>> GetOperationsByDate(DateTime date);
        Task<IEnumerable<RepairOperation>> GetOperationsByWorkType(string workType);
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<IEnumerable<RepairOperationType>> GetAllRepairOperationTypes();
        Task<IEnumerable<RepairOperation>> GetOperationsByStatus(FaultReportStatus status);
    }
}
