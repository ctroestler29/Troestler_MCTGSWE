using System;
using System.Collections.Generic;
using System.Text;

namespace MCTG
{
    public interface ICard
    {
        string Name { get; set; }
        string Element { get; set; }
        int Damage { get; set; }
        string CardType { get; set; }
        int Health { get; set; }
        void attack();
        bool isActive();

    }
}
