namespace DB.Entities;

//Table - Tasks
[Serializable]
public class Task : BaseEntity
{
    public string Name { get; set; }

    public byte Type { get; set; }

    public DateTime Date { get; set; }

    public byte Status { get; set; }

    public string Json { get; set; }
}