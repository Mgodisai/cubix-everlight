using Data.Models;

namespace DataContextLib.Models
{
    public class Employee : BaseEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string DisplayName { get; set; }
        public Guid PositionId { get; set; }
        public virtual Position Position { get; set; }
        public string Email { get; set; }
    }
}
