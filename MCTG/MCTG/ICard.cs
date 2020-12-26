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
        int Verfügbarkeit { get; set; }

        void attack();
        bool isActive();

    }
}
