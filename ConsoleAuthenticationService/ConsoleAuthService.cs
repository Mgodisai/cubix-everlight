using ConsoleAuthenticationService.Specifications;
using DataContextLib.Models;
using DataContextLib.Repository;

namespace ConsoleAuthenticationService;

public class ConsoleAuthService
{
    private readonly IRepository<Employee> _employeeRepository;

    public ConsoleAuthService(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public AuthenticationResult Authenticate(string username, string password)
    {
        var employee = _employeeRepository.FindWithSpecificationAsync(new FindEmployeeByUsernameSpecification(username)).GetAwaiter().GetResult().FirstOrDefault();
        if (employee == null)
        {
            return new AuthenticationResult(null,false, message: "No such user!");
        }
        if (employee != null && BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash))
        {
            return new AuthenticationResult(employee,true, employee.Username, employee.DisplayName, employee.Email, "OK");
        }
        return new AuthenticationResult(null, false, message: "Wrong password!");
    }
}