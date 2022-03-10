namespace DB.Entities;

public class BaseEntity
{
    public uint ID { get; set; }
    public bool IsDeleted { get; set; }
}