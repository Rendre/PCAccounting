namespace DB.Entities
{
    public class Computer : BaseEntity
    {
        public string Name { get; set; }
        public int Status { get; set; }
        public int EmployerId { get; set; }
        public DateTime Date { get; set; }
        public string? Cpu { get; set; }
        public decimal Price { get; set; }
    }
}
