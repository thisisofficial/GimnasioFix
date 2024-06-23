using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimnasioFix
{
    public class Cliente
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string AccNum { get; set; }
        public string Tel { get; set; }

    }

    public class Payment
    {
        public string AccNum { get; set; }
        public DateTime LastDate { get; set; }
        public string Price { get; set; }
        public string Owed { get; set; }
        public string Type { get; set; }
    }
}
