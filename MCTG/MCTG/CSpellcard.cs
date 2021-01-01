using System;
using System.Collections.Generic;
using System.Text;

namespace MCTG
{
    class CSpellcard
    {
        public CSpellcard(string _name, string _element, double _damage, string _cardtype, int _health)
        {
            Name = _name;
            Element = _element;
            Damage = _damage;
            CardType = _cardtype;
            Health = _health;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Element { get; set; }
        public int Health { get; set; }
        public double Damage { get; set; }
        public string CardType { get; set; }
        public string Type { get; set; }

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

