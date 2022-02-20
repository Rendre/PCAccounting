namespace DekstopClient.Entities
{
    public class User : BaseEntity
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public int EmployerId { get; set; }

    }
}
