using AutoMapper;
using ClientWebPortal.Models;
using ClientWebPortal.Service.Specifications;
using Data.Models;
using Data.Repository;
using DataContextLib.Models;

namespace ClientWebPortal.Service
{

    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Position> _positionRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IRepository<Employee> employeeRepository, IMapper mapper, IRepository<Position> positionRepository)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _positionRepository = positionRepository;
        }

        public void AddEmployee(EmployeeViewModel employeeViewModel)
        {
            var employee = new Employee
            {
                Username = employeeViewModel.Username,
                DisplayName = employeeViewModel.DisplayName,
                PositionId = employeeViewModel.PositionId,
                Email = employeeViewModel.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(employeeViewModel.Password)
            };
            _employeeRepository.Add(employee);
        }

        public void DeleteById(Guid employeeId)
        {
            var employee = _employeeRepository.GetById(employeeId);
            if (employee is not null)
            {
                _employeeRepository.Delete(employee);
            }
        }

        public IEnumerable<EmployeeViewModel>? GetAllEmployees()
        {
                List<EmployeeViewModel> employeeViewModels = [];
                var result = _employeeRepository.FindWithSpecification(new EmployeeWithPositionSpecification());
                return result.Select(e => _mapper.Map<EmployeeViewModel>(e)).ToList();
        }

        public IEnumerable<Position> GetAllPositions()
        {
            return _positionRepository.GetAll();
        }

    }
}