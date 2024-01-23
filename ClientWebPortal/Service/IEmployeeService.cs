using ClientWebPortal.Models;
using DataContextLib.Models;

namespace ClientWebPortal.Service
{
    public interface IEmployeeService
    {
        public IEnumerable<EmployeeViewModel>? GetAllEmployees();
        public IEnumerable<Position> GetAllPositions();
        void AddEmployee(EmployeeViewModel employeeViewModel);
        void DeleteById(Guid guid);
    }
}