using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DekstopClient
{
    internal class Computer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public int EmployerId { get; set; }
        public DateTime Date { get; set; }
        public string Cpu { get; set; }
        public decimal Price { get; set; }
    }
}
