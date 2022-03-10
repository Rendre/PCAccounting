namespace DB.Entities;

public class Employer : BaseEntity
{
    public string? Name { get; set; }
    public string? Position { get; set; }
    public string? Tel { get; set; }

    public override string ToString()
    {
        return Name;
    }
}