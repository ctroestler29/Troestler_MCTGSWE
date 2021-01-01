using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace MCTG
{
    class Database
    {
        public static string connstring = "Server=127.0.0.1;Port=5432;Database=MCTGData;User Id=postgres;Password=postgres;";
        NpgsqlConnection connection = new NpgsqlConnection(connstring);
        List<string> OldDeck = new List<string>();
        //string dataItems = "";

        public void createTables()
        {
            connection.Open();
            //NpgsqlCommand cmd = new NpgsqlCommand("CREATE TABLE Kampfzüge ( Id int PRIMARY KEY NOT NULL, Runde int NOT NULL, username varchar(255) NOT NULL, Karte int NOT NULL, BattelID int NOT NULL);", connection);
            //cmd.ExecuteNonQuery();
            //NpgsqlCommand cmd = new NpgsqlCommand("DROP TABLE Kampfzüge;", connection);
            //cmd.ExecuteNonQuery();
            //NpgsqlCommand cmd2 = new NpgsqlCommand("ALTER TABLE Kampfzüge ADD id varchar(255)", connection);
            //cmd2.ExecuteNonQuery();
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
            NpgsqlCommand cmd4 = new NpgsqlCommand("Select username, ELO FROM persons ORDER BY ELO ASC;", connection);
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
            if (checkBattle(username))
            {
                return "";
            }

            List<string> warteschlange = new List<string>();
            string oponent = "";

            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("insert into warteschlange (username) values('" + username + "');", connection);
            cmd2.ExecuteNonQuery();
            connection.Close();

            warteschlange = AnzWarteschlange(username);
            if (AnzWarteschlange(username).Count == 0)
            {
                while (warteschlange.Count == 0)
                {
                    warteschlange = AnzWarteschlange(username);
                    Console.WriteLine("Gegner für User: " + username + " wird gesucht .....");
                }
                oponent = warteschlange[0];
                int battleID = getBattleID(username, oponent);
                connection.Open();
                NpgsqlCommand cmd3 = new NpgsqlCommand("insert into arena (username1,username2, battleid) values('" + username + "', '" + oponent + "', '" + battleID + "');", connection);
                cmd3.ExecuteNonQuery();
                connection.Close();
            }
            else
            {
                warteschlange = AnzWarteschlange(username);
                oponent = warteschlange[0];
                int battleID = getBattleID(username, oponent);
                connection.Open();
                NpgsqlCommand cmd3 = new NpgsqlCommand("insert into arena (username1, username2, battleid) values('" + username + "', '" + oponent + "', '" + battleID + "');", connection);
                cmd3.ExecuteNonQuery();
                connection.Close();
            }

            if (checkArena(username) == 2)
            {
                connection.Open();
                NpgsqlCommand cmd4 = new NpgsqlCommand("DELETE FROM warteschlange WHERE username ='" + username + "';", connection);
                cmd4.ExecuteNonQuery();
                connection.Close();

                connection.Open();
                NpgsqlCommand cmd5 = new NpgsqlCommand("DELETE FROM warteschlange WHERE username ='" + warteschlange[0] + "';", connection);
                cmd5.ExecuteNonQuery();
                connection.Close();
            }
            return oponent;
        }

        public List<string> AnzWarteschlange(string username)
        {
            List<string> warteschlange = new List<string>();
            connection.Open();
            NpgsqlCommand cmd4 = new NpgsqlCommand("Select username FROM warteschlange;", connection);
            NpgsqlDataReader dataReader2 = cmd4.ExecuteReader();

            int ii = 0;
            while (dataReader2.Read())
            {
                if (dataReader2[0].ToString() != username)
                {
                    warteschlange.Insert(ii, dataReader2[0].ToString());
                    ii++;
                }
            }
            connection.Close();

            return warteschlange;
        }

        public int checkArena(string username)
        {
            int check = 0;
            connection.Open();
            NpgsqlCommand cmd4 = new NpgsqlCommand("Select username1, username2  FROM arena;", connection);
            NpgsqlDataReader dataReader2 = cmd4.ExecuteReader();

            for (int ii = 0; dataReader2.Read(); ii++)
            {
                if (dataReader2[0].ToString() == username || dataReader2[1].ToString() == username)
                {
                    check++;
                }
            }
            connection.Close();

            return check;
        }

        public int getBattleID(string username1, string username2)
        {
            int battleID = 1;
            bool check = false;
            connection.Open();
            NpgsqlCommand cmd1 = new NpgsqlCommand("Select username1, username2, battleid  FROM arena;", connection);
            NpgsqlDataReader dataReader1 = cmd1.ExecuteReader();

            for (int ii = 0; dataReader1.Read(); ii++)
            {
                if (username1 == dataReader1[0].ToString() && username2 == dataReader1[1].ToString() || username2 == dataReader1[0].ToString() && username1 == dataReader1[1].ToString())
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

        public bool checkBattle(string username)
        {
            bool check = false;
            connection.Open();
            NpgsqlCommand cmd1 = new NpgsqlCommand("Select username1, username2 FROM arena;", connection);
            NpgsqlDataReader dataReader1 = cmd1.ExecuteReader();

            for (int ii = 0; dataReader1.Read(); ii++)
            {
                if (username == dataReader1[0].ToString() || username == dataReader1[1].ToString())
                {
                    check = true;
                }

            }
            connection.Close();
            return check;
        }

        public int getCard(int runde, string username, int battleID)
        {
            int z = 0;
            connection.Open();
            NpgsqlCommand cmd1 = new NpgsqlCommand("Select karte FROM kampfzüge where runde = '"+runde+"' AND username = '"+username+"' AND battelid='"+battleID+"';", connection);
            NpgsqlDataReader dataReader1 = cmd1.ExecuteReader();

            for (int ii = 0; dataReader1.Read(); ii++)
            {
                z = int.Parse(dataReader1[0].ToString());
            }
            connection.Close();
            return z;
        }

        public void setCard(int runde, string username, int z, int battleid)
        {
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("insert into kampfzüge (runde, username, karte, battelid) values('" + runde + "', '" + username + "', '" + z + "', '" + battleid + "');", connection);
            cmd2.ExecuteNonQuery();

            connection.Close();
        }

        public bool endBattle(int battleid)
        {
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("DELETE FROM kampfzüge WHERE battelid='"+battleid+"';",connection);
            cmd2.ExecuteNonQuery();
            connection.Close();

            connection.Open();
            NpgsqlCommand cmd3 = new NpgsqlCommand("DELETE FROM arena WHERE battleid='" + battleid + "';",connection);
            cmd3.ExecuteNonQuery();
            connection.Close();

            return true;
        }
    }
}
