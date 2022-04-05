namespace DB.Entities;
public class ProjectProperties
{
    public string SendersName { get; set; }
    public string MyEmail { get; set; }
    public string PasswordForMyEmail { get; set; }
    public string AddressSMTP { get; set; }
    public int PortSMTP { get; set; }
}