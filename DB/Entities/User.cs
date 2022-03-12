namespace DB.Entities;

public class User : BaseEntity
{
    public string? Login { get; set; }
    public string? Pass { get; set; }
    public uint EmployerId { get; set; }

}