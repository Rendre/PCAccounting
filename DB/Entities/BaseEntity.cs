namespace DB.Entities;

public class BaseEntity
{
    public uint Id { get; set; }
    public bool IsDeleted { get; set; }
}