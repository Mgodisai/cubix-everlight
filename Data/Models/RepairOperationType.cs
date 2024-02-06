namespace DataContextLib.Models;

public class RepairOperationType : BaseEntity
{
    public string Name { get; set; }

    public ICollection<RepairOperation> RepairOperations { get; set; }
}

