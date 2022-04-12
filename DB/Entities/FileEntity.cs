namespace DB.Entities;

public class FileEntity : BaseEntity
{
    public uint ComputerID { get; set; }
    public string? FileName { get; set; }
    public string? Path { get; set; }
}