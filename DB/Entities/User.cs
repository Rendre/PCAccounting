namespace DB.Entities;

public class User : BaseEntity
{
    public string? Login { get; set; }
    public string? Password { get; set; }
    public uint EmployerID { get; set; }

}