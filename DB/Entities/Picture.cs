namespace DB.Entities;

public class Picture : BaseEntity
{
    public uint ComputerId { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }

}