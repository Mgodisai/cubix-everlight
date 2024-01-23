namespace ServiceConsole.Service
{
    public class AuthenticationResult(bool isAuthenticated = false, string username = "", string displayName = "", string email = "")
    {
        public bool IsAuthenticated { get; set; } = isAuthenticated;
        public string Username { get; set; } = username;
        public string DisplayName { get; set; } = displayName;
        public string Email { get; set; } = email;
    }
}
