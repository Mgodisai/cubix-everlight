using AutoMapper;
using ClientWebPortal.Models;
using ClientWebPortal.Service.Specifications;
using DataContextLib;
using DataContextLib.Models;
using DataContextLib.UnitOfWorks;

namespace ClientWebPortal.Service
{

    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork<DataDbContext> _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeService(IMapper mapper, IUnitOfWork<DataDbContext> unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task AddEmployeeAsync(EmployeeViewModel employeeViewModel)
        {
            var employee = new Employee
            {
                Username = employeeViewModel.Username,
                DisplayName = employeeViewModel.DisplayName,
                PositionId = employeeViewModel.PositionId,
                Email = employeeViewModel.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(employeeViewModel.Password)
            };
            try
            {
                await _unitOfWork.CreateTransactionAsync();
                await _unitOfWork.GetRepository<Employee>().InsertAsync(employee);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }
        }

        public async Task DeleteByIdAsync(Guid employeeId)
        {
            try
            {
                await _unitOfWork.CreateTransactionAsync();
                var result = await _unitOfWork.GetRepository<Employee>().DeleteAsync(employeeId);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }
        }

        public async Task<IEnumerable<EmployeeViewModel>> GetAllEmployeesAsync()
        {
                List<EmployeeViewModel> employeeViewModels = [];
                var result = await _unitOfWork.GetRepository<Employee>().FindWithSpecificationAsync(new EmployeeWithPositionSpecification());
                return result.Select(e => _mapper.Map<EmployeeViewModel>(e)).ToList();
        }

        public async Task<IEnumerable<Position>> GetAllPositionsAsync()
        {
            return await _unitOfWork.GetRepository<Position>().GetAllAsync();
        }

    }
}