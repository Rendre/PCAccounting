namespace DB.Entities;

public class FileEntity : BaseEntity
{
    public uint ComputerID { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
}