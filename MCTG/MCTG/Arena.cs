using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace MCTG
{
    public class Arena
    {
        Database db = new Database();
        //public static List<User> warteschlange = new List<User>();

        static object _lock = new object();
        public static List<string> result = new List<string>();
        public static ConcurrentDictionary<string, string> result2 = new ConcurrentDictionary<string, string>();
        public static ConcurrentQueue<User> warteschlange = new ConcurrentQueue<User>();
        User user1;
        User user2;

        public Arena(User user1, User user2)
        {
            this.user1 = user1;
            this.user2 = user2;
        }

        public static ManualResetEvent Restart { get; } = new ManualResetEvent(false);
        public static System.Threading.AutoResetEvent event_2 = new System.Threading.AutoResetEvent(false);

        public string PrepareFight(User user)
        {
            string response = "";
            Monitor.Enter(_lock);
            if (warteschlange.Count() == 0)
            {
                warteschlange.Enqueue(user);
                Monitor.Exit(_lock);
                event_2.WaitOne();
                result2.TryRemove(user.username,out response);
            }
            else
            {
                warteschlange.TryDequeue(out this.user2);
                this.user1 = user;
                Monitor.Exit(_lock);


                this.user1.deck = db.showDeck(user1.username);
                this.user2.deck = db.showDeck(user2.username);
                //response = StartBattle(user1,user2);
                result2.TryAdd(user2.username, response);

                Arena.event_2.Set();

            }

            return response;
            //----------------------------------------
            //bool check = false;
            //Monitor.Enter(_lock);
            //if (warteschlange.Count >= 1)
            //{
            //    warteschlange.TryDequeue(out User user2);
            //    this.user1 = user;
            //    this.user2 = user2;
            //    StartBattle();
            //    warteschlange.Clear();
            //    check = true;
            //}
            //else
            //{
            //    warteschlange.Enqueue(user);
            //}

            //Monitor.Exit(_lock);
            //return check;
        }
        public string StartBattle()
        //public string StartBattle(User user1, User user2)
        {
            string log = "";
            Random rnd = new Random();
            ICard ActCardUser1;
            ICard ActCardUser2;
            int i = 1;
            if (!((user1.username == "kienboec" && user2.username == "altenhof") || (user1.username == "altenhof" && user2.username == "kienboec")))
            {
                Console.WriteLine("");
            }
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
            
            db.setScore(winner, looser);
            result2.TryAdd(user2.username, log);
            return log;
        }


    }
}
