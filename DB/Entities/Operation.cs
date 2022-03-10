namespace DB.Entities;

public class Operation : BaseEntity
{
    public string Name { get; set; }
    public byte Status { get; set; }
    public DateTime Date { get; set; }
    public ulong ComputerID { get; set; }
}