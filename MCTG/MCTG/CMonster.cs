using System;
using System.Collections.Generic;
using System.Text;

namespace MCTG
{
    class CMonster : ICard
    {
        public CMonster(string _name, string _element, int _damage, string _type, string _cardtype, int _health)
        {
            Name = _name;
            Element = _element;
            Damage = _damage;
            type = _type;
            CardType = _cardtype;
            Health = _health;
        }

        public string Name { get; set; }
        public string Element { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
        public string type { get; set; }
        public string CardType { get; set; }

        public void attack()
        {
            Console.WriteLine(Name + " der " + type + " hat " + Damage + " Schaden gemacht");
        }

        public bool isActive()
        {
            return false;
        }
    }
}
