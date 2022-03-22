namespace DB.Entities;

public class Files : BaseEntity
{
    public uint ComputerId { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }

}