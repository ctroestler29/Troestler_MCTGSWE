using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MCTG
{
    class Arena
    {
        Database db = new Database();
        //public static List<User> warteschlange = new List<User>();
        public static List<string> result = new List<string>();
        User user1;
        User user2;

        public Arena(User _user1, User _user2)
        {
            user1 = _user1;
            user2 = _user2;
        }

        public static ManualResetEvent Restart { get; } = new ManualResetEvent(false);
        public static System.Threading.AutoResetEvent event_2 = new System.Threading.AutoResetEvent(false);

        //public static bool PrepareFight(User user)
        //{
        //    bool check = false;
        //    warteschlange.Add(user);
        //    if (warteschlange.Count >= 2)
        //    {
        //        StartBattle(warteschlange[0], warteschlange[1]);
        //        warteschlange.RemoveAt(1);
        //        warteschlange.RemoveAt(0);
        //        check = true;
        //    }
        //    return check;
        //}

        public  string StartBattle()
        {
            string log = "";
            Random rnd = new Random();
            ICard ActCardUser1;
            ICard ActCardUser2;
            int i = 1;
            while (user1.deck.Count() > 0 && user2.deck.Count() > 0 && i <= 100)
            {
                log += i + ". Runde\n";
                log += "KartenAnz " + user1.username + ": " + user1.deck.Count() + "\n";
                log += "KartenAnz " + user2.username + ": " + user2.deck.Count() + "\n";
                log += "\n\n";

                int karteuser1 = rnd.Next(0, user1.deck.Count);
                //db.setCard(i,user1.username, karteuser1 ,user1.battleID);

                int karteuser2 = rnd.Next(0, user2.deck.Count);
                //int karteuser2 = db.getCard(i,user2.username, user2.battleID);

                ActCardUser1 = user1.deck[karteuser1];
                ActCardUser2 = user2.deck[karteuser2];


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

                    if (ActCardUser1.Name.Contains("Wizzard") && ActCardUser2.Name.Contains("Ork"))
                    {
                        ActCardUser2.Health -= ActCardUser1.Damage;
                        zv = true;
                    }
                    else if (ActCardUser2.Name.Contains("Wizzard") && ActCardUser1.Name.Contains("Ork"))
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
                        if (ActCardUser1.Element == "Water" && ActCardUser2.Element == "Fire")
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
                        if (ActCardUser1.Element == "Fire")
                        {
                            ActCardUser1.Health -= ActCardUser2.Damage * 2;
                        }
                        else
                        {
                            ActCardUser1.Health -= ActCardUser2.Damage;
                        }
                        zv = true;
                    }

                    if (ActCardUser1.Name.Contains("Knight") && ActCardUser2.Element == "Water")
                    {
                        ActCardUser1.Health = 0;
                        zv = true;
                    }
                    else if (ActCardUser2.Name.Contains("Knight") && ActCardUser1.Element == "Water")
                    {
                        ActCardUser2.Health = 0;
                        zv = true;
                    }

                    if (zv == false)
                    {
                        //water-- > fire
                        if (ActCardUser1.Element == "Water" && ActCardUser2.Element == "Fire")
                        {
                            ActCardUser2.Health -= ActCardUser1.Damage * 2;
                            ActCardUser1.Health -= ActCardUser2.Damage / 2;
                        }
                        else if (ActCardUser2.Element == "Water" && ActCardUser1.Element == "Fire")
                        {
                            ActCardUser1.Health -= ActCardUser2.Damage * 2;
                            ActCardUser2.Health -= ActCardUser1.Damage / 2;
                        }
                        //fire-- > normal
                        if (ActCardUser1.Element == "Fire" && ActCardUser2.Element == "Regular")
                        {
                            ActCardUser2.Health -= ActCardUser1.Damage * 2;
                            ActCardUser1.Health -= ActCardUser2.Damage / 2;
                        }
                        else if (ActCardUser2.Element == "Fire" && ActCardUser1.Element == "Regular")
                        {
                            ActCardUser1.Health -= ActCardUser2.Damage * 2;
                            ActCardUser2.Health -= ActCardUser1.Damage / 2;
                        }
                        //normal-- > water
                        if (ActCardUser1.Element == "Regular" && ActCardUser2.Element == "Water")
                        {
                            ActCardUser2.Health -= ActCardUser1.Damage * 2;
                            ActCardUser1.Health -= ActCardUser2.Damage / 2;
                        }
                        else if (ActCardUser2.Element == "Regular" && ActCardUser1.Element == "Water")
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
            log += i + ". Runde\n";
            log += "KartenAnz " + user1.username + ": " + user1.deck.Count() + "\n";
            log += "KartenAnz " + user2.username + ": " + user2.deck.Count() + "\n";
            log += "\n\n";
            log += "BATTLE END\n";
            string winner = "";
            string looser = "";
            if (user1.deck.Count() > user2.deck.Count())
            {
                log += user1.username + " hat gewonnen!\n";
                winner = user1.username;
                looser = user2.username;
            }
            else if (user1.deck.Count() < user2.deck.Count())
            {
                log += user2.username + " hat gewonnen!\n";
                winner = user2.username;
                looser = user1.username;
            }
            else
            {
                log += user1.username + " vs " + user2.username + " endet im Unentschieden!\n";
            }
            result.Add(log);
            db.setScore(winner, looser);
            
            return log;
        }


    }
}
