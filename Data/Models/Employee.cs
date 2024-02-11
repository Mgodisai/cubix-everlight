using System.Text.Json.Serialization;

namespace DataContextLib.Models;

public class Employee : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid PositionId { get; set; }
    public virtual Position? Position { get; set; }
    public string Email { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{DisplayName} ({Position})";
    }
}