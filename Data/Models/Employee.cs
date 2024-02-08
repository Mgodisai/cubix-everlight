using System.Text.Json.Serialization;

namespace DataContextLib.Models;

public class Employee : BaseEntity
{
    public string Username { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; }
    public string DisplayName { get; set; }
    public Guid PositionId { get; set; }
    public virtual Position Position { get; set; }
    public string Email { get; set; }

    public override string ToString()
    {
        return $"{DisplayName} ({Position})";
    }
}