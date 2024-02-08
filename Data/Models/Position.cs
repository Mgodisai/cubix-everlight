namespace DataContextLib.Models;

public class Position : BaseEntity
{
    public string Name { get; set; }

    public override string ToString()
    {
        return Name;
    }
}