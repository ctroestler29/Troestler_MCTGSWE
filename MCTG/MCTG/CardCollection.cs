using System;
using System.Collections.Generic;
using System.Text;

namespace MCTG
{
    public class CardCollection
    {
        public List<ICard> collec = new List<ICard>();

        public void fill()
        {
            CMonster Dragon = new CMonster("fire Dragon", "fire", 21, "dragon");
            collec.Add(Dragon);
            CMonster Goblin = new CMonster("small but strong Goblin", "normal", 14, "goblin");
            collec.Add(Goblin);
            CMonster Wizzard = new CMonster("ice Wizzard", "water", 19, "wizzard");
            collec.Add(Wizzard);
            CMonster Ork = new CMonster("big Ork", "normal", 18, "ork");
            collec.Add(Ork);
            CMonster Knight = new CMonster("dark Knight", "normal", 21, "knight");
            collec.Add(Knight);
            CMonster Kraken = new CMonster("blue-ringed octopus", "water", 19, "kraken");
            collec.Add(Kraken);
            CMonster Elve = new CMonster("fire Elve", "fire", 17, "elve");
            collec.Add(Elve);
            CSpellcard WaterSpell = new CSpellcard("Wave", "water", 15);
            collec.Add(WaterSpell);
            CSpellcard FireSpell = new CSpellcard("fire rain", "fire", 17);
            collec.Add(WaterSpell);
            CSpellcard NormalSpell = new CSpellcard("earth-quake", "normal", 12);
            collec.Add(NormalSpell);

        }
        public int addCard(ICard card)
        {
            try
            {
                collec.Add(card);
                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public int deleteCard(ICard card)
        {
            try
            {
                collec.Remove(card);
                return 0;
            }
            catch
            {
                return 1;
            }
        }
    }
}
