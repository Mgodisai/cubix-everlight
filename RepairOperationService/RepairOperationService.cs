using Data.Models;
using Data.Repository;
using DataContextLib.Models;
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

        public void CreateRepairOperation(RepairOperation repairOperation)
        {
            _repairOperationRepository.Add(repairOperation);
        }

        public void CreateRepairOperation(Guid faultReportId, Guid employeeId, RepairOperationType operationType, DateTime startDate)
        {
            var faultReport = _faultReportRepository.GetById(faultReportId);
            if (faultReport == null)
                throw new InvalidOperationException("Fault report not found.");
            

            var technician = _employeeRepository.GetById(employeeId);
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

            _repairOperationRepository.Add(repairOperation);
        }

        public void AssignOperationToEmployee(Guid operationId, Guid employeeId)
        {
            var operation = _repairOperationRepository.GetById(operationId);
            if (operation is null)
                throw new InvalidOperationException("Repair operation not found.");

            var employee = _employeeRepository.GetById(employeeId);

            if (employee is null)
                throw new InvalidOperationException("Employee not found.");

            operation.EmployeeId = employeeId;
            operation.FaultReport.Status = FaultReportStatus.InProgress;
            _repairOperationRepository.Update(operation);
        }

        public void CompleteRepairOperation(Guid operationId)
        {
            var operation = _repairOperationRepository.GetById(operationId);
            if (operation == null || operation.FaultReport.Status != FaultReportStatus.InProgress)
                throw new InvalidOperationException("Repair operation not found or not in progress.");

            operation.FaultReport.Status = FaultReportStatus.Completed;
            _repairOperationRepository.Update(operation);
        }

        public RepairOperation? GetRepairOperation(Guid operationId)
        {
            return _repairOperationRepository.FindWithSpecification(new RepairOperationByOperationIdSpecification(operationId)).FirstOrDefault();
        }

        public IEnumerable<RepairOperation> GetAllRepairOperations()
        {
            return _repairOperationRepository.FindWithSpecification(new RepairOperationWithFaultReportSpecification());
        }

        public IEnumerable<RepairOperation> GetRepairOperationsByStatus(FaultReportStatus status)
        {
            return _repairOperationRepository.FindWithSpecification(new RepairOperationByFaultReportStatusSpecification(status));
        }
    }
}