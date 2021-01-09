using System;
using System.Collections.Generic;
using System.Text;

namespace MCTG
{
    class CMonster:ICard
    {
        public CMonster(string id,string _name, string _element, double _damage, string _type, string _cardtype)
        {
            ID = id;
            Name = _name;
            Element = _element;
            Damage = _damage;
            Type = _type;
            CardType = _cardtype;
            Health = 1000;
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Element { get; set; }
        public double Damage { get; set; }
        public string Type { get; set; }
        public string CardType { get; set; }
        public double Health { get; set; }

    }
}
