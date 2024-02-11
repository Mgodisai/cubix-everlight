namespace DataContextLib.Models;

public class Position : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        return Name;
    }
}