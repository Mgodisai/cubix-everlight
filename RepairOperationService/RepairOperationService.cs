using DataContextLib.Models;
using DataContextLib.Repository;
using RepairOperationService.Specifications;

namespace RepairOperationService
{
    public class RepairService(IRepository<RepairOperation> repairOperationRepository,
                         IRepository<Employee> employeeRepository,
                         IRepository<RepairOperationType> operationTypeRepository,
                         IRepository<FaultReport> faultReportRepository)
    {
        private readonly IRepository<RepairOperation> _repairOperationRepository = repairOperationRepository;
        private readonly IRepository<Employee> _employeeRepository = employeeRepository;
        private readonly IRepository<RepairOperationType> _operationTypeRepository = operationTypeRepository;
        private readonly IRepository<FaultReport> _faultReportRepository = faultReportRepository;

        public async Task CreateRepairOperation(RepairOperation repairOperation)
        {
            await _repairOperationRepository.InsertAsync(repairOperation);
        }

        public async Task CreateRepairOperation(Guid faultReportId, Guid employeeId, RepairOperationType operationType, DateTime startDate)
        {
            var faultReport = await _faultReportRepository.GetByIdAsync(faultReportId);
            if (faultReport == null)
                throw new InvalidOperationException("Fault report not found.");
            

            var technician = await _employeeRepository.GetByIdAsync(employeeId);
            if (technician == null)
                throw new InvalidOperationException("Technician not found.");

            var repairOperation = new RepairOperation
            {
                FaultReportId = faultReportId,
                FaultReport = faultReport,
                EmployeeId = employeeId,
                OperationType = operationType,
                StartDate = startDate
            };
            repairOperation.FaultReport.Status = FaultReportStatus.InProgress;

            await _repairOperationRepository.InsertAsync(repairOperation);
        }

        public async Task AssignOperationToEmployee(Guid operationId, Guid employeeId)
        {
            var operation = await _repairOperationRepository.GetByIdAsync(operationId);
            if (operation is null)
                throw new InvalidOperationException("Repair operation not found.");

            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (employee is null)
                throw new InvalidOperationException("Employee not found.");

            operation.EmployeeId = employeeId;
            operation.FaultReport.Status = FaultReportStatus.InProgress;
            await _repairOperationRepository.UpdateAsync(operation);
        }

        public async Task CompleteRepairOperation(Guid operationId)
        {
            var operation = await _repairOperationRepository.GetByIdAsync(operationId);
            if (operation == null || operation.FaultReport.Status != FaultReportStatus.InProgress)
                throw new InvalidOperationException("Repair operation not found or not in progress.");

            operation.FaultReport.Status = FaultReportStatus.Completed;
            await _repairOperationRepository.UpdateAsync(operation);
        }

        public async Task<IEnumerable<RepairOperation>> GetRepairOperation(Guid operationId)
        {
            return await _repairOperationRepository.FindWithSpecificationAsync(new RepairOperationByOperationIdSpecification(operationId));
        }

        public async Task<IEnumerable<RepairOperation>> GetAllRepairOperations()
        {
            return await _repairOperationRepository.FindWithSpecificationAsync(new RepairOperationWithFaultReportSpecification());
        }

        public async Task<IEnumerable<RepairOperation>> GetRepairOperationsByStatus(FaultReportStatus status)
        {
            return await _repairOperationRepository.FindWithSpecificationAsync(new RepairOperationByFaultReportStatusSpecification(status));
        }
    }
}