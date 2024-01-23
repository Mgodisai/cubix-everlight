using Data.Repository;
using DataContextLib.Models;
using ServiceConsole.Specifications;

namespace ServiceConsole.Service
{
    public class ConsoleAuthService
    {
        private readonly IRepository<Employee> _employeeRepository;

        public ConsoleAuthService(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public AuthenticationResult Authenticate(string username, string password)
        {
            var employee = _employeeRepository.FindWithSpecification(new FindEmployeeByUsernameSpecification(username)).FirstOrDefault();
            if (employee != null && BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash))
            {
                return new AuthenticationResult(true, employee.Username, employee.DisplayName, employee.Email);
            }
            return new AuthenticationResult(false);
        }
    }
}
