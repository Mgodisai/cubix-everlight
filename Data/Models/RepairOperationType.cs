using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DataContextLib.Models;

public class RepairOperationType : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    [XmlIgnore]
    [JsonIgnore]
    public ICollection<RepairOperation> RepairOperations { get; set; } = [];

    public override string ToString()
    {
        return Name;
    }
}

