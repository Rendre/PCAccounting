namespace DekstopClient.Entities
{
    internal class Employer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Tel { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
