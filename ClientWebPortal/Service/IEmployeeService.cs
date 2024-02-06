using ClientWebPortal.Models;
using DataContextLib.Models;

namespace ClientWebPortal.Service
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeViewModel>> GetAllEmployeesAsync();
        Task<IEnumerable<Position>> GetAllPositionsAsync();
        Task AddEmployeeAsync(EmployeeViewModel employeeViewModel);
        Task DeleteByIdAsync(Guid guid);
    }
}