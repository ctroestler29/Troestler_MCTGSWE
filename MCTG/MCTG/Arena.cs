using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCTG
{
    class Arena
    {
        User user1;
        User user2;

        public Arena(User _user1, User _user2)
        {
            user1 = _user1;
            user2 = _user2;
        }

        public void StartBattle()
        {


            Random rnd = new Random();
            ICard ActCardUser1;
            ICard ActCardUser2;

            int i = 0;
            while (user1.deck.Count() > 0 && user2.deck.Count() > 0 && i <= 100)
            {
                Console.WriteLine(i + ". Runde");
                Console.WriteLine("Anz1: " + user1.deck.Count());
                Console.WriteLine("Anz2: " + user2.deck.Count());
                Console.WriteLine();

                ActCardUser1 = user1.deck[rnd.Next(0, user1.deck.Count())];
                ActCardUser2 = user2.deck[rnd.Next(0, user2.deck.Count())];


                if (ActCardUser1.CardType == "monster" && ActCardUser2.CardType == "monster")
                {
                    bool zv = false;
                    if (ActCardUser1.Name.Contains("Goblin") && ActCardUser2.Name.Contains("Dragon"))
                    {
                        ActCardUser1.Health -= ActCardUser2.Damage;
                        zv = true;
                    }
                    else if (ActCardUser2.Name.Contains("Goblin") && ActCardUser1.Name.Contains("Dragon"))
                    {
                        ActCardUser2.Health -= ActCardUser1.Damage;
                        zv = true;
                    }

                    if (ActCardUser1.Name.Contains("Wizzard") && ActCardUser2.Name.Contains("Orks"))
                    {
                        ActCardUser2.Health -= ActCardUser1.Damage;
                        zv = true;
                    }
                    else if (ActCardUser2.Name.Contains("Wizzard") && ActCardUser1.Name.Contains("Orks"))
                    {
                        ActCardUser1.Health -= ActCardUser2.Damage;
                        zv = true;
                    }

                    if (ActCardUser1.Name.Contains("Elve") && ActCardUser2.Name.Contains("Dragon"))
                    {
                        ActCardUser2.Health -= ActCardUser1.Damage;
                        zv = true;
                    }
                    else if (ActCardUser2.Name.Contains("Elve") && ActCardUser1.Name.Contains("Dragon"))
                    {
                        ActCardUser1.Health -= ActCardUser2.Damage;
                        zv = true;
                    }

                    if (zv == false)
                    {
                        ActCardUser2.Health -= ActCardUser1.Damage;
                        ActCardUser1.Health -= ActCardUser2.Damage;
                    }
                }
                else
                {
                    bool zv = false;

                    if (ActCardUser1.Name.Contains("Kraken") && ActCardUser2.CardType == "spell")
                    {
                        if (ActCardUser2.Element == "fire")
                        {
                            ActCardUser2.Health -= ActCardUser1.Damage * 2;
                        }
                        else
                        {
                            ActCardUser2.Health -= ActCardUser1.Damage;
                        }
                        zv = true;
                    }
                    else if (ActCardUser2.Name.Contains("Kraken") && ActCardUser1.CardType == "spell")
                    {
                        if (ActCardUser1.Element == "fire")
                        {
                            ActCardUser1.Health -= ActCardUser2.Damage * 2;
                        }
                        else
                        {
                            ActCardUser1.Health -= ActCardUser2.Damage;
                        }
                        zv = true;
                    }

                    if (ActCardUser1.Name.Contains("Knight") && ActCardUser2.Element == "water")
                    {
                        ActCardUser1.Health = 0;
                        zv = true;
                    }
                    else if (ActCardUser2.Name.Contains("Knight") && ActCardUser1.Element == "water")
                    {
                        ActCardUser2.Health = 0;
                        zv = true;
                    }

                    if (zv == false)
                    {
                        //water-- > fire
                        if (ActCardUser1.Element == "water" && ActCardUser2.Element == "fire")
                        {
                            ActCardUser2.Health -= ActCardUser1.Damage * 2;
                            ActCardUser1.Health -= ActCardUser2.Damage / 2;
                        }
                        else if (ActCardUser2.Element == "water" && ActCardUser1.Element == "fire")
                        {
                            ActCardUser1.Health -= ActCardUser2.Damage * 2;
                            ActCardUser2.Health -= ActCardUser1.Damage / 2;
                        }
                        //fire-- > normal
                        if (ActCardUser1.Element == "fire" && ActCardUser2.Element == "normal")
                        {
                            ActCardUser2.Health -= ActCardUser1.Damage * 2;
                            ActCardUser1.Health -= ActCardUser2.Damage / 2;
                        }
                        else if (ActCardUser2.Element == "fire" && ActCardUser1.Element == "normal")
                        {
                            ActCardUser1.Health -= ActCardUser2.Damage * 2;
                            ActCardUser2.Health -= ActCardUser1.Damage / 2;
                        }
                        //normal-- > water
                        if (ActCardUser1.Element == "normal" && ActCardUser2.Element == "water")
                        {
                            ActCardUser2.Health -= ActCardUser1.Damage * 2;
                            ActCardUser1.Health -= ActCardUser2.Damage / 2;
                        }
                        else if (ActCardUser2.Element == "normal" && ActCardUser1.Element == "water")
                        {
                            ActCardUser1.Health -= ActCardUser2.Damage * 2;
                            ActCardUser2.Health -= ActCardUser1.Damage / 2;
                        }
                        //noeffect
                        if (ActCardUser1.Element == ActCardUser2.Element)
                        {
                            ActCardUser1.Health -= ActCardUser2.Damage;
                            ActCardUser2.Health -= ActCardUser1.Damage;
                        }

                    }
                }

                if (ActCardUser1.Health > ActCardUser2.Health)
                {
                    user1.deck.Add(ActCardUser2);
                    user2.deck.Remove(ActCardUser2);
                }
                else if (ActCardUser1.Health < ActCardUser2.Health)
                {
                    user2.deck.Add(ActCardUser1);
                    user1.deck.Remove(ActCardUser1);
                }

                i++;
            }
            Console.WriteLine(i + ". Runde");
            Console.WriteLine("Anz1: " + user1.deck.Count());
            Console.WriteLine("Anz2: " + user2.deck.Count());
            Console.WriteLine();
            Console.WriteLine("BATTLE END");
            if (user1.deck.Count() > user2.deck.Count())
            {
                Console.WriteLine(user1.username + " hat gewonnen!");
            }
            else if (user1.deck.Count() < user2.deck.Count())
            {
                Console.WriteLine(user2.username + " hat gewonnen!");
            }
            else
            {
                Console.WriteLine(user1.username + " vs " + user2.username + " endet im Unentschieden!");
            }
        }
    }
}
