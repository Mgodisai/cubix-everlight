using DataContextLib.Models;

namespace ConsoleAuthenticationService;

public class AuthenticationResult(Employee employee, bool isAuthenticated = false, string username = "", string displayName = "", string email = "", string message = "")
{
    public bool IsAuthenticated { get; set; } = isAuthenticated;
    public Employee? Employee { get; set; } = employee;
    public string Username { get; set; } = username;
    public string DisplayName { get; set; } = displayName;
    public string Email { get; set; } = email;
    public string Message { get; set; } = message;
}