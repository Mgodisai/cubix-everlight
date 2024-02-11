using DataContextLib;
using DataContextLib.Models;
using DataContextLib.UnitOfWorks;
using RepairOperationService.Specifications;

namespace RepairOperationService;

public class RepairOperationService(IUnitOfWork<DataDbContext> unitOfWork) : IRepairOperationService
{
    public async Task<IEnumerable<RepairOperation>> ListTakenReportsByEmployee(Employee? userAuthEmployee)
    {
        var repairOperationRepository = unitOfWork.GetRepository<RepairOperation>();
        if (userAuthEmployee is null) { return Enumerable.Empty<RepairOperation>(); }
        var result = await repairOperationRepository.FindWithSpecificationAsync(new RepairOperationByEmployeeSpec(userAuthEmployee.Id));
        return result;
    }

    public async Task<bool> AssignFaultReportToEmployee(Guid faultReportId, Employee? employee)
    {
        if (employee == null) throw new InvalidOperationException("Employee not found");

        var faultReport = await ValidateFaultReportAndFetch(faultReportId);
        var repairOperationType = await EnsureRepairOperationType();

        var newRepairOperation = CreateNewRepairOperation(faultReportId, faultReport, repairOperationType, employee);

        return await TryAssignRepairOperation(newRepairOperation, faultReport);
    }

    private async Task<FaultReport> ValidateFaultReportAndFetch(Guid faultReportId)
    {
        var repairOperation = 
            await unitOfWork
                .GetRepository<RepairOperation>()
                .FindWithSpecificationAsync(new RepairOperationByFaultReportIdSpecification(faultReportId));

        if (repairOperation.Any())
        {
            throw new InvalidOperationException($"Fault report has already been assigned with guid: {repairOperation.First().Id} to {repairOperation.First()?.Employee?.DisplayName}");
        }

        var faultReport = 
            await unitOfWork
                .GetRepository<FaultReport>()
                .GetByIdAsync(faultReportId);
        return faultReport ?? throw new InvalidOperationException("Fault report cannot be found");
    }

    private async Task<RepairOperationType> EnsureRepairOperationType()
    {
        var repairOperationTypeList = 
            await unitOfWork
                .GetRepository<RepairOperationType>()
                .FindWithSpecificationAsync(new RepairTypeByNameSpec("Undefined"));
        return repairOperationTypeList.FirstOrDefault() ?? new RepairOperationType { Name = "Undefined" };
    }

    private RepairOperation CreateNewRepairOperation(Guid faultReportId, FaultReport faultReport, RepairOperationType repairOperationType, Employee employee)
    {
        return new RepairOperation
        {
            StartDate = DateTime.UtcNow,
            FaultReport = faultReport,
            FaultReportId = faultReportId,
            OperationType = repairOperationType,
            EmployeeId = employee.Id,
            Employee = employee
        };
    }

    private async Task<bool> TryAssignRepairOperation(RepairOperation newRepairOperation, FaultReport faultReport)
    {
        try
        {
            await unitOfWork.CreateTransactionAsync();
            await unitOfWork.GetRepository<RepairOperation>().InsertAsync(newRepairOperation);
            faultReport.Status = FaultReportStatus.InProgress;
            await unitOfWork.SaveAsync();
            await unitOfWork.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            return false;
        }
    }

    public async Task CompletedRepairOperationAsync(Guid parsedGuid, Employee? userAuthEmployee, string operation)
    {
        var repo = unitOfWork.GetRepository<RepairOperation>();
        if (userAuthEmployee is null)
        {
            throw new InvalidOperationException("A dolgozó nem található!");
        }
        var repairOperation = await unitOfWork
            .GetRepository<RepairOperation>()
            .FindWithSpecificationAsync(new RepairOperationByOperationIdAndEmployeeIdSpecification(parsedGuid, userAuthEmployee.Id));
        if (repairOperation.FirstOrDefault() is null)
        {
            throw new InvalidOperationException($"A munka ({parsedGuid}) nem szerepel az Ön által felvettek között!");
        }

        var updatedRepairOperation = repairOperation.FirstOrDefault();

        if (updatedRepairOperation?.FaultReport?.Status != FaultReportStatus.InProgress)
        {
            throw new InvalidOperationException("A kiválasztott munka státusza nem megfelelő!");
        }

        var opTypeRepo = unitOfWork.GetRepository<RepairOperationType>();
        var operationTypeList = 
            await opTypeRepo
                .FindWithSpecificationAsync(new RepairTypeByNameSpec(operation));

        await unitOfWork.CreateTransactionAsync();
        var operationType = operationTypeList.FirstOrDefault();

        try
        {
            if (operationType == null)
            {
                var newOpType = new RepairOperationType() { Name = operation, Id = Guid.NewGuid() };
                await opTypeRepo.InsertAsync(newOpType);
                operationType = newOpType;
            }
            updatedRepairOperation.EndDate = DateTime.UtcNow;
            updatedRepairOperation.FaultReport.Status = FaultReportStatus.Completed;
            updatedRepairOperation.OperationType = operationType;
            updatedRepairOperation.OperationTypeId = operationType.Id;
            await repo.UpdateAsync(updatedRepairOperation);

            await unitOfWork.SaveAsync();
            await unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            throw new InvalidOperationException("A mentés során hiba merült fel!");
        }

    }

    public async Task<IEnumerable<RepairOperation>> GetAllOperations()
    {
        return await unitOfWork
            .GetRepository<RepairOperation>()
            .FindWithSpecificationAsync(new RepairOperationWithFaultReportSpecification());
    }

    public async Task<IEnumerable<RepairOperation>> GetOperationsByEmployee(string employeeName)
    {
        return await unitOfWork
            .GetRepository<RepairOperation>()
            .FindWithSpecificationAsync(new RepairOperationByEmployeeNameSpec(employeeName));
    }

    public async Task<IEnumerable<RepairOperation>> GetOperationsByDate(int year, int month)
    {
        return await unitOfWork
            .GetRepository<RepairOperation>()
            .FindWithSpecificationAsync(new RepairOperationByYearAndMonthSpec(year, month));
    }

    public async Task<IEnumerable<RepairOperation>> GetOperationsByWorkType(string workType)
    {
        return await unitOfWork
            .GetRepository<RepairOperation>()
            .FindWithSpecificationAsync(new RepairOperationByOperationTypeNameSpec(workType));
    }

    public async Task<IEnumerable<Employee>> GetAllEmployees()
    {
        return await unitOfWork
            .GetRepository<Employee>()
            .GetAllAsync();
    }

    public async Task<IEnumerable<RepairOperationType>> GetAllRepairOperationTypes()
    {
        return await unitOfWork
            .GetRepository<RepairOperationType>()
            .GetAllAsync();
    }

    public async Task<IEnumerable<RepairOperation>> GetOperationsByStatus(FaultReportStatus status)
    {
        return await unitOfWork
            .GetRepository<RepairOperation>()
            .FindWithSpecificationAsync(new RepairOperationByStatusSpec(status));
    }
}