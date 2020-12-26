using System;
using System.Collections.Generic;
using System.Text;

namespace MCTG
{
    class CSpellcard : ICard
    {
        public CSpellcard(string _name, string _element, int _damage)
        {
            Name = _name;
            Element = _element;
            Damage = _damage;
        }

        public string Name { get; set; }
        public string Element { get; set; }
        public int Damage { get; set; }
        public int Verfügbarkeit { get; set; }

        public void attack()
        {
            Console.WriteLine(Name + " hat " + Damage + " Schaden gemacht");
        }

        public bool isActive()
        {
            return false;
        }
    }
}
