namespace DB.Entities;

public class Session : BaseEntity
{
    public string? Token { get; set; }
    public DateTime Time { get; set; }
    public uint UserID { get; set; }
    public string? UserIP { get; set; }

}