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
            //CMonster Dragon = new CMonster("fire Dragon", "fire", 2, "dragon", "monster");
            //collec.Add(Dragon);
            //CMonster Goblin = new CMonster("small but strong Goblin", "normal", 14, "goblin", "monster");
            //collec.Add(Goblin);
            //CMonster Wizzard = new CMonster("ice Wizzard", "water", 19, "wizzard", "monster");
            //collec.Add(Wizzard);
            //CMonster Ork = new CMonster("big Ork", "normal", 18, "ork", "monster");
            //collec.Add(Ork);
            //CMonster Knight = new CMonster("dark Knight", "normal", 21, "knight", "monster");
            //collec.Add(Knight);
            //CMonster Kraken = new CMonster("blue-ringed Octopus", "water", 19, "kraken", "monster");
            //collec.Add(Kraken);
            //CMonster Elve = new CMonster("fire Elve", "fire", 17, "elve", "monster");
            //collec.Add(Elve);
            //CSpellcard WaterSpell = new CSpellcard("Wave", "water", 15, "spell", 1000);
            //collec.Add(WaterSpell);
            //CSpellcard FireSpell = new CSpellcard("fire rain", "fire", 17, "spell", 1000);
            //collec.Add(WaterSpell);
            //CSpellcard NormalSpell = new CSpellcard("earth-quake", "normal", 12, "spell", 1000);
            //collec.Add(NormalSpell);

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
