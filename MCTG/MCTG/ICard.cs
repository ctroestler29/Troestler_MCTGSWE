using System;
using System.Collections.Generic;
using System.Text;

namespace MCTG
{
    public class ICard
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Element { get; set; }
        public double Damage { get; set; }
        public string CardType { get; set; }
        public string Type { get; set; }
        public double Health = 1000;

    }
}
