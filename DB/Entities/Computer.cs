namespace DB.Entities
{
    public class Computer : BaseEntity
    {
        public string Name { get; set; }
        public uint StatusID { get; set; }
        public uint EmployerId { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Cpu { get; set; }
        public decimal Price { get; set; }
    }
}
