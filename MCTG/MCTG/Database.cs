using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace MCTG
{
    class Database
    {
        public static string connstring = "Server=127.0.0.1;Port=5432;Database=MCTGData;User Id=postgres;Password=postgres;";
        NpgsqlConnection connection = new NpgsqlConnection(connstring);
        List<string> OldDeck = new List<string>();
        Arena arena;
        //string dataItems = "";

        public void createTables()
        {
            connection.Open();
            //NpgsqlCommand cmd = new NpgsqlCommand("CREATE TABLE Offers ( Id integer PRIMARY KEY NOT NULL, username varchar(255) NOT NULL, CardToTrade varchar(255) NOT NULL);", connection);
            //cmd.ExecuteNonQuery();
            //NpgsqlCommand cmd = new NpgsqlCommand("DROP TABLE Kampfzüge;", connection);
            //cmd.ExecuteNonQuery();
            NpgsqlCommand cmd2 = new NpgsqlCommand("ALTER TABLE offers ADD TradeId varchar(255) NOT NULL", connection);
            cmd2.ExecuteNonQuery();
            connection.Close();
        }

        public void createUser(string username, string pw)
        {
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("insert into Persons (username, password, coins, gewonnen, verloren, ELO) values('" + username + "', '" + pw + "',20, 0, 0, 100);", connection);
            cmd2.ExecuteNonQuery();

            connection.Close();
        }

        public bool checkUser(string username)
        {
            bool check = true;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT username FROM Persons;", connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();
            for (int i = 0; dataReader.Read(); i++)
            {
                if (dataReader[0].ToString() == username)
                {
                    check = false;
                }
                //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
            }
            connection.Close();
            return check;
        }

        public int Login(string username, string pw)
        {
            string sessionid = "Basic " + username + "-mtcgToken";
            int check = 2;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT username, password FROM Persons WHERE username='" + username + "';", connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();

            for (int i = 0; dataReader.Read(); i++)
            {
                if (dataReader[0].ToString() == username && dataReader[1].ToString() == pw)
                {
                    check = 0;
                }
                //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
            }
            connection.Close();
            if (checkSession(sessionid) && check == 0)
            {
                return 1;
            }

            if (check == 0)
            {
                connection.Open();
                NpgsqlCommand cmd2 = new NpgsqlCommand("insert into sessions (SessionId, SessionTime, userId) values('" + sessionid + "', '" + DateTime.Now + "', '" + username + "');", connection);
                cmd2.ExecuteNonQuery();
                connection.Close();
            }
            return check;
        }

        public bool createCard(string Id, string Name, double Damage, string element, string type, string cardtype, int pack)
        {
            connection.Open();
            try
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("insert into cards (Id, Name, Damage,Pack, Element, Type, CardType) values('" + Id + "', '" + Name + "', '" + Damage + "', '" + pack + "', '" + element + "', '" + type + "', '" + cardtype + "');", connection);
                cmd2.ExecuteNonQuery();
            }
            catch { };
            connection.Close();
            return true;
        }

        public bool createPack(int pack)
        {
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("insert into packs (pack) values('" + pack + "');", connection);
            cmd2.ExecuteNonQuery();
            connection.Close();
            return true;
        }
        public int getMaxPack()
        {
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT MAX(Pack) FROM cards;", connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();
            int pack = 1;
            try
            {

                for (int i = 0; dataReader.Read(); i++)
                {
                    if (dataReader[0].ToString() != "")
                    {
                        pack = int.Parse(dataReader[0].ToString()) + 1;
                    }
                    //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
                }
            }
            catch { };
            connection.Close();
            return pack;
        }

        public int getMinPack()
        {
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT MIN(pack) FROM packs;", connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();
            int pack = 0;
            try
            {

                for (int i = 0; dataReader.Read(); i++)
                {
                    if (dataReader[0].ToString() != "")
                    {
                        pack = int.Parse(dataReader[0].ToString());
                    }
                    //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
                }
            }
            catch { };
            connection.Close();
            return pack;
        }

        public bool checkSession(string sessionId)
        {
            bool check = false;
            int zv = 0;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT SessionId, SessionTime FROM Sessions WHERE SessionId='" + sessionId + "';", connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();
            for (int i = 0; dataReader.Read(); i++)
            {
                if (DateTime.Now.Hour == DateTime.Parse(dataReader[1].ToString()).Hour)
                {
                    if (dataReader[0].ToString() == sessionId && DateTime.Now.Minute - DateTime.Parse(dataReader[1].ToString()).Minute < 10)
                    {
                        check = true;
                    }
                    else if (dataReader[0].ToString() == sessionId && DateTime.Now.Minute - DateTime.Parse(dataReader[1].ToString()).Minute > 10)
                    {
                        zv = 1;
                    }
                }
                else
                {
                    if (dataReader[0].ToString() == sessionId && (DateTime.Now.Minute + 60) - DateTime.Parse(dataReader[1].ToString()).Minute < 10)
                    {
                        check = true;
                    }
                    else if (dataReader[0].ToString() == sessionId && (DateTime.Now.Minute + 60) - DateTime.Parse(dataReader[1].ToString()).Minute > 10)
                    {
                        zv = 1;
                    }
                }
                //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
            }
            connection.Close();
            connection.Open();
            if (check)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("UPDATE Sessions SET SessionTime = '" + DateTime.Now + "' WHERE SessionId='" + sessionId + "';", connection);
                cmd.ExecuteNonQuery();
            }
            else if (zv == 1)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM Sessions WHERE SessionId='" + sessionId + "';", connection);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
            return check;
        }

        public bool getPack(string username)
        {
            //Pack bestimmen

            int pack = getMinPack();

            if (pack == 0)
            {
                return false;
            }
            //Karten des PAcks holen
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("Select id FROM cards WHERE pack='" + pack + "';", connection);
            NpgsqlDataReader dataReader = cmd.ExecuteReader();
            List<string> cardID = new List<string>();
            for (int i = 0; dataReader.Read(); i++)
            {
                cardID.Add(dataReader[0].ToString());
                //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
            }
            connection.Close();
            //DELETE Pack
            connection.Open();
            NpgsqlCommand cmd3 = new NpgsqlCommand("DELETE FROM packs WHERE pack='" + pack + "';", connection);
            cmd3.ExecuteNonQuery();
            connection.Close();

            //Karten dem User übertragen
            connection.Open();
            for (int i = 0; i < cardID.Count; i++)
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("insert into usercards (username, cardsid) values('" + username + "', '" + cardID[i] + "');", connection);
                cmd2.ExecuteNonQuery();
            }

            connection.Close();
            return true;

        }

        public int GetCoins(string username)
        {
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("Select coins FROM persons WHERE username='" + username + "';", connection);
            NpgsqlDataReader dataReader = cmd.ExecuteReader();
            int coins = 0;

            for (int i = 0; dataReader.Read(); i++)
            {
                coins = int.Parse(dataReader[0].ToString());
                //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
            }

            connection.Close();
            return coins;
        }

        public bool bezahlen(string username, int coins)
        {
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE persons SET coins = '" + coins + "' WHERE username='" + username + "';", connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        public string GetUsernameBySessionID(string sessionId)
        {
            //Username holen
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("Select userId FROM sessions WHERE sessionid='" + sessionId + "';", connection);
            NpgsqlDataReader dataReader = cmd.ExecuteReader();
            string username = "";

            for (int i = 0; dataReader.Read(); i++)
            {
                username = dataReader[0].ToString();
                //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
            }

            connection.Close();

            return username;
        }

        public List<string> GetCards(string username)
        {
            List<string> CardList = new List<string>();
            List<string> IDList = new List<string>();
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("Select cardsid FROM usercards WHERE username='" + username + "';", connection);
            NpgsqlDataReader dataReader = cmd.ExecuteReader();

            for (int i = 0; dataReader.Read(); i++)
            {
                IDList.Insert(i, dataReader[0].ToString());
            }
            connection.Close();


            for (int i = 0; i < IDList.Count; i++)
            {
                connection.Open();
                NpgsqlCommand cmd2 = new NpgsqlCommand("Select id, name, damage FROM cards WHERE id='" + IDList[i] + "';", connection);
                NpgsqlDataReader dataReader2 = cmd2.ExecuteReader();

                for (int ii = 0; dataReader2.Read(); ii++)
                {
                    //CardList.Insert(ii,"{\"Id\":\"" + dataReader2[0].ToString() + ", \"Name\":\"" + dataReader2[1].ToString() + ", \"Damage\":" + dataReader2[2].ToString() + "}");
                    CardList.Insert(ii, @"{  'Id': '" + dataReader2[0].ToString() + "', 'Name': '" + dataReader2[1].ToString() + "', 'Damage': '" + dataReader2[2].ToString() + "'} ");
                    //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
                }

                connection.Close();
            }
            return CardList;
        }

        public List<string> showDeck(string username)
        {
            List<string> CardID = new List<string>();
            List<string> Deck = new List<string>();
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("Select cardsid FROM usercards WHERE username='" + username + "' AND imDeck='true';", connection);
            NpgsqlDataReader dataReader = cmd.ExecuteReader();

            for (int i = 0; dataReader.Read(); i++)
            {
                CardID.Insert(i, dataReader[0].ToString());
            }
            connection.Close();

            for (int i = 0; i < CardID.Count; i++)
            {
                connection.Open();
                NpgsqlCommand cmd2 = new NpgsqlCommand("Select id, name, damage, element, type, cardtype FROM cards WHERE id='" + CardID[i] + "';", connection);
                NpgsqlDataReader dataReader2 = cmd2.ExecuteReader();

                for (int ii = 0; dataReader2.Read(); ii++)
                {
                    //CardList.Insert(ii,"{\"Id\":\"" + dataReader2[0].ToString() + ", \"Name\":\"" + dataReader2[1].ToString() + ", \"Damage\":" + dataReader2[2].ToString() + "}");
                    Deck.Insert(ii, @"{  'Id': '" + dataReader2[0].ToString() + "', 'Name': '" + dataReader2[1].ToString() + "', 'Damage': '" + dataReader2[2].ToString() + "', 'Element': '" + dataReader2[3].ToString() + "', 'Type': '" + dataReader2[4].ToString() + "', 'CardType': '" + dataReader2[5].ToString() + "'} ");
                    //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
                }

                connection.Close();
            }

            return Deck;
        }

        public bool setDeck(string username, string id, int i, int anz)
        {

            if (i == 0)
            {
                connection.Open();
                NpgsqlCommand cmd3 = new NpgsqlCommand("Select cardsid FROM usercards WHERE username ='" + username + "' AND imDeck='true';", connection);
                NpgsqlDataReader dataReader3 = cmd3.ExecuteReader();

                for (int ii = 0; dataReader3.Read(); ii++)
                {
                    OldDeck.Insert(ii, dataReader3[0].ToString());
                }

                connection.Close();

                connection.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("UPDATE usercards SET imDeck = 'false' WHERE username = '" + username + "';", connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("UPDATE usercards SET imDeck = 'true' WHERE username = '" + username + "' AND cardsid='" + id + "';", connection);
            cmd2.ExecuteNonQuery();
            connection.Close();

            if (i == anz - 1)
            {
                List<string> NewDeck = new List<string>();
                connection.Open();
                NpgsqlCommand cmd4 = new NpgsqlCommand("Select cardsid FROM usercards WHERE username = '" + username + "' AND imDeck='true';", connection);
                NpgsqlDataReader dataReader4 = cmd4.ExecuteReader();

                for (int ii = 0; dataReader4.Read(); ii++)
                {
                    NewDeck.Insert(ii, dataReader4[0].ToString());
                }

                connection.Close();

                if (NewDeck.Count != 4)
                {
                    for (int ii = 0; ii < OldDeck.Count; ii++)
                    {
                        setDeck(username, OldDeck[ii].ToString(), ii, OldDeck.Count);
                    }
                    OldDeck.Clear();
                    return false;
                }
            }
            return true;
        }

        public string getUserData(string username)
        {
            string UserData = "";
            connection.Open();
            NpgsqlCommand cmd4 = new NpgsqlCommand("Select Name, Bio, Image, coins FROM persons WHERE username = '" + username + "';", connection);
            NpgsqlDataReader dataReader2 = cmd4.ExecuteReader();

            for (int ii = 0; dataReader2.Read(); ii++)
            {

                UserData = @"{  'Username': '" + username + "', 'Name': '" + dataReader2[0].ToString() + "', 'Bio': '" + dataReader2[1].ToString() + "', 'Image': '" + dataReader2[2].ToString() + "', 'Coins': '" + dataReader2[3].ToString() + "'} ";

            }

            connection.Close();
            return UserData;
        }

        public bool setUserData(string username, string name, string bio, string image)
        {
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE persons SET Name = '" + name + "', bio = '" + bio + "', image = '" + image + "' WHERE username='" + username + "';", connection);
            cmd.ExecuteNonQuery();
            connection.Close();

            return true;
        }

        public string getStats(string username)
        {
            string stats = "";
            connection.Open();
            NpgsqlCommand cmd4 = new NpgsqlCommand("Select gewonnen, verloren, ELO FROM persons WHERE username = '" + username + "';", connection);
            NpgsqlDataReader dataReader2 = cmd4.ExecuteReader();

            for (int ii = 0; dataReader2.Read(); ii++)
            {

                stats = @"{  'Username': '" + username + "', 'Gewonnen': '" + dataReader2[0].ToString() + "', 'Verloren': '" + dataReader2[1].ToString() + "', 'ELO': '" + dataReader2[2].ToString() + "'} ";

            }

            connection.Close();
            return stats;
        }

        public List<string> getScoreboard(string username)
        {
            int platz = 0;
            List<string> scoreboard = new List<string>();
            connection.Open();
            NpgsqlCommand cmd4 = new NpgsqlCommand("Select username, ELO FROM persons WHERE NOT username = 'admin' ORDER BY ELO DESC;", connection);
            NpgsqlDataReader dataReader2 = cmd4.ExecuteReader();

            for (int ii = 0; dataReader2.Read(); ii++)
            {
                if (dataReader2[0].ToString() == username)
                {
                    platz = ii + 1;
                }
                scoreboard.Insert(ii, @"{'Platz': '" + (ii + 1).ToString() + "',  'Username': '" + dataReader2[0].ToString() + "', 'ELO': '" + dataReader2[1].ToString() + "', 'UserPlatz': '" + platz.ToString() + "'} ");

            }
            connection.Close();
            return scoreboard;
        }

        public string findBattle(string username)
        {
            List<string> warteschlange = new List<string>();
            string oponent = "";
            string response = "";
            warteschlange = AnzWarteschlange(username);
            if (warteschlange.Count == 0)
            {
                connection.Open();
                NpgsqlCommand cmd2 = new NpgsqlCommand("insert into warteschlange (username) values('" + username + "');", connection);
                cmd2.ExecuteNonQuery();
                connection.Close();

                Console.WriteLine("Gegner für User: " + username + " wird gesucht .....");

                Arena.event_2.WaitOne();


            }
            else
            {
                oponent = warteschlange[0];
                connection.Open();
                NpgsqlCommand cmd4 = new NpgsqlCommand("DELETE FROM warteschlange WHERE username ='" + oponent + "';", connection);
                cmd4.ExecuteNonQuery();
                connection.Close();

                int battleID = getBattleID(username);
                connection.Open();
                NpgsqlCommand cmd3 = new NpgsqlCommand("insert into arena (username1, username2, battleid) values('" + username + "', '" + oponent + "', '" + battleID + "');", connection);
                cmd3.ExecuteNonQuery();
                connection.Close();

                User user1 = new User(username);
                User user2 = new User(oponent);

                List<string> deck1 = showDeck(user1.username);
                List<string> deck2 = showDeck(user2.username);

                user1.deck = FillDeck(deck1);
                user2.deck = FillDeck(deck2);
                arena = new Arena(user1, user2);
                response = arena.StartBattle();

                //if (!Arena.PrepareFight(user1))
                //{
                //Arena.event_2.WaitOne();
                //}

                Arena.event_2.Set();

            }

            if (response == "")
            {
                int z = 0;
                bool check = false;
                while (z < Arena.result.Count)
                {
                    check = Arena.result[z].Contains(username);
                    if (check == true)
                    {
                        response = Arena.result[z];
                        Arena.result.RemoveAt(z);
                        break;
                    }
                    z++;
                }
            }
            return response;
        }

        public List<ICard> FillDeck(List<string> deck)
        {
            List<ICard> IDeck = new List<ICard>();

            int i = 0;
            while (i < deck.Count)
            {
                ICard card = new ICard();
                JObject json = JObject.Parse(deck[i]);
                card.ID = json.SelectToken("Id").ToString();
                card.Name = json.SelectToken("Name").ToString();
                card.Damage = double.Parse(json.SelectToken("Damage").ToString());
                card.Element = json.SelectToken("Element").ToString();
                card.Type = json.SelectToken("Type").ToString();
                card.CardType = json.SelectToken("CardType").ToString();
                IDeck.Add(card);
                i++;
            }

            return IDeck;
        }

        public List<string> AnzWarteschlange(string username)
        {
            List<string> warteschlange = new List<string>();
            List<string> arena = new List<string>();

            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("Select username1, username2 FROM arena;", connection);
            NpgsqlDataReader dataReader = cmd.ExecuteReader();

            int i = 0;
            while (dataReader.Read())
            {
                arena.Insert(i, dataReader[0].ToString());
                i++;
                arena.Insert(i, dataReader[1].ToString());
                i++;
            }
            connection.Close();

            connection.Open();
            NpgsqlCommand cmd4 = new NpgsqlCommand("Select username FROM warteschlange;", connection);
            NpgsqlDataReader dataReader2 = cmd4.ExecuteReader();

            int ii = 0;
            while (dataReader2.Read())
            {
                if (dataReader2[0].ToString() != username && arena.Contains(dataReader2[0].ToString()) == false)
                {
                    warteschlange.Insert(ii, dataReader2[0].ToString());
                    ii++;
                }
            }
            connection.Close();

            return warteschlange;
        }



        public int getBattleID(string username)
        {
            int battleID = 1;
            bool check = false;
            connection.Open();
            NpgsqlCommand cmd1 = new NpgsqlCommand("Select username1, username2, battleid  FROM arena;", connection);
            NpgsqlDataReader dataReader1 = cmd1.ExecuteReader();

            for (int ii = 0; dataReader1.Read(); ii++)
            {
                if (username == dataReader1[0].ToString() || username == dataReader1[1].ToString())
                {
                    try
                    {
                        battleID = int.Parse(dataReader1[2].ToString());
                        check = true;
                    }
                    catch { };
                }

            }
            connection.Close();

            if (!check)
            {
                connection.Open();
                NpgsqlCommand cmd2 = new NpgsqlCommand("Select DISTINCT MAX(BattleID)  FROM arena;", connection);
                NpgsqlDataReader dataReader2 = cmd2.ExecuteReader();

                for (int ii = 0; dataReader2.Read(); ii++)
                {
                    try
                    {
                        battleID = int.Parse(dataReader2[0].ToString()) + 1;
                    }
                    catch { };
                }
                connection.Close();
            }
            return battleID;
        }


        public bool endBattle(string username, int battleid)
        {
            connection.Open();
            NpgsqlCommand cmd3 = new NpgsqlCommand("DELETE FROM arena WHERE battleid='" + battleid + "';", connection);
            cmd3.ExecuteNonQuery();
            connection.Close();


            return true;
        }

        public bool setScore(string winner, string looser)
        {
            bool check = false;
            List<string> score = new List<string>();
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("Select username, gewonnen, verloren, draw, elo  FROM persons WHERE username ='" + winner + "' OR username ='" + looser + "';", connection);
            NpgsqlDataReader dataReader2 = cmd2.ExecuteReader();

            for (int ii = 0; dataReader2.Read(); ii++)
            {
                try
                {
                    score.Insert(ii, dataReader2[0].ToString() + ";" + dataReader2[1].ToString() + ";" + dataReader2[2].ToString() + ";" + dataReader2[3].ToString() + ";" + dataReader2[4].ToString());
                }
                catch { };
            }
            connection.Close();
            int i = 0;
            while (i <= score.Count - 1)
            {
                string[] strarr = score[i].Split(";");
                if (winner != "" && strarr[0] == winner)
                {
                    connection.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("UPDATE persons SET gewonnen = '" + (int.Parse(strarr[1]) + 1) + "', elo ='" + (int.Parse(strarr[4]) + 3) + "' WHERE username='" + winner + "';", connection);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                if (looser != "" && strarr[0] == looser)
                {
                    connection.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("UPDATE persons SET verloren = '" + (int.Parse(strarr[2]) + 1) + "', elo ='" + (int.Parse(strarr[4]) - 5) + "' WHERE username='" + looser + "';", connection);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                if (winner == "" && looser == "")
                {
                    connection.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("UPDATE persons SET draw = '" + (int.Parse(strarr[3]) + 1) + "' WHERE username='" + strarr[0] + "';", connection);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                i++;
            }
            return check;
        }

        public List<string> getTradingDeals(string username)
        {
            List<string> deals = new List<string>();

            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("Select Id, username, CardToTrade FROM tradinglist ORDER BY Id DESC;", connection);
            NpgsqlDataReader dataReader2 = cmd2.ExecuteReader();

            for (int ii = 0; dataReader2.Read(); ii++)
            {
                if (dataReader2[1].ToString() != username)
                {
                    deals.Add(@"{  'Id': '" + dataReader2[0].ToString() + "', 'Username': '" + dataReader2[1].ToString() + "','CardToTrade': '" + dataReader2[2].ToString() + "'} ");
                }
            }
            connection.Close();

            return deals;
        }

        public string getCardById(string id)
        {
            string card = "";

            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("Select id, name, cardtype ,  damage FROM cards WHERE id='" + id + "';", connection);
            NpgsqlDataReader dataReader2 = cmd2.ExecuteReader();

            for (int ii = 0; dataReader2.Read(); ii++)
            {
                //CardList.Insert(ii,"{\"Id\":\"" + dataReader2[0].ToString() + ", \"Name\":\"" + dataReader2[1].ToString() + ", \"Damage\":" + dataReader2[2].ToString() + "}");
                card = @"{  'Id': '" + dataReader2[0].ToString() + "', 'Name': '" + dataReader2[1].ToString() + "','cardtype': '" + dataReader2[2].ToString() + "', 'Damage': '" + dataReader2[3].ToString() + "'} ";
                //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
            }

            connection.Close();


            return card;
        }

        public string getCardbyTradeId(string id)
        {
            string card = "";
            string cardid = "";
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("Select CardToTrade FROM Tradinglist WHERE id='" + id + "';", connection);
            NpgsqlDataReader dataReader2 = cmd2.ExecuteReader();

            for (int ii = 0; dataReader2.Read(); ii++)
            {
                cardid = dataReader2[0].ToString();
            }

            connection.Close();

            card = getCardById(cardid);

            return card;
        }

        public bool createTrade(string id, string username, string cardtotrade, string type, double damage)
        {

            try
            {
                connection.Open();
                NpgsqlCommand cmd2 = new NpgsqlCommand("insert into tradinglist (id, username, CardToTrade, type, minimumdamage) values('" + id + "', '" + username + "','" + cardtotrade + "', '" + type + "', '" + damage + "');", connection);
                cmd2.ExecuteNonQuery();

                connection.Close();
            }
            catch { return false; };
            return true;
        }

        public string getTradeCondition(string tradeid)
        {
            string condition = "";
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("Select type, minimumdamage FROM Tradinglist WHERE id='" + tradeid + "';", connection);
            NpgsqlDataReader dataReader2 = cmd2.ExecuteReader();

            for (int ii = 0; dataReader2.Read(); ii++)
            {
                condition = @"{  'type': '" + dataReader2[0].ToString() + "', 'MinDamage': '" + dataReader2[1].ToString() + "'} ";
            }

            connection.Close();

            return condition;
        }

        public bool makeTrade(string tradeid, string offerid, string username)
        {
            try
            {
                string username1 = "";
                string usertrade = "";
                connection.Open();
                NpgsqlCommand cmd2 = new NpgsqlCommand("Select id, username FROM usercards WHERE cardsid='" + tradeid + "';", connection);
                NpgsqlDataReader dataReader2 = cmd2.ExecuteReader();

                for (int ii = 0; dataReader2.Read(); ii++)
                {
                    usertrade = dataReader2[0].ToString();
                    username1 = dataReader2[1].ToString();
                }

                connection.Close();

                string username2 = "";
                string useroffer = "";
                connection.Open();
                NpgsqlCommand cmd3 = new NpgsqlCommand("Select id, username FROM usercards WHERE cardsid='" + offerid + "';", connection);
                NpgsqlDataReader dataReader3 = cmd3.ExecuteReader();

                for (int ii = 0; dataReader3.Read(); ii++)
                {
                    useroffer = dataReader3[0].ToString();
                    username2 = dataReader3[1].ToString();
                }

                connection.Close();
                if (username2 != username)
                {
                    return false;
                }
                else if (username1 == username2)
                {
                    return false;
                }
                else
                {
                    connection.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("UPDATE usercards SET cardsid = '" + tradeid + "' WHERE id='" + useroffer + "';", connection);
                    cmd.ExecuteNonQuery();
                    connection.Close();

                    connection.Open();
                    NpgsqlCommand cmd4 = new NpgsqlCommand("UPDATE usercards SET cardsid = '" + offerid + "' WHERE id='" + usertrade + "';", connection);
                    cmd4.ExecuteNonQuery();
                    connection.Close();

                    connection.Open();
                    NpgsqlCommand cmd5 = new NpgsqlCommand("DELETE FROM tradinglist WHERE cardtotrade = '" + tradeid + "';", connection);
                    cmd5.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch { return false; };
            return true;
        }

        public bool deleteTrade(string tradeid)
        {
            try
            {
                connection.Open();
                NpgsqlCommand cmd5 = new NpgsqlCommand("DELETE FROM tradinglist WHERE id = '" + tradeid + "';", connection);
                cmd5.ExecuteNonQuery();
                connection.Close();
            }
            catch { return false; };

            return true;
        }
    }
}
