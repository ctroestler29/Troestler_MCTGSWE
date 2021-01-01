using System;
using System.Collections.Generic;
using System.Text;

namespace MCTG
{
    class CMonster
    {
        public CMonster(string _name, string _element, double _damage, string _type, string _cardtype)
        {
            Name = _name;
            Element = _element;
            Damage = _damage;
            Type = _type;
            CardType = _cardtype;
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Element { get; set; }
        public double Damage { get; set; }
        public string Type { get; set; }
        public string CardType { get; set; }

        public void attack()
        {
            Console.WriteLine(Name + " der " + Type + " hat " + Damage + " Schaden gemacht");
        }

        public bool isActive()
        {
            return false;
        }
    }
}
