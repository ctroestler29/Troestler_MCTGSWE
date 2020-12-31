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
        //string dataItems = "";

        public void createTables()
        {
            connection.Open();
            //NpgsqlCommand cmd = new NpgsqlCommand("CREATE TABLE UserCards ( Id int PRIMARY KEY, username varchar(255) NOT NULL, cardsId varchar(255) NOT NULL);", connection);
            //cmd.ExecuteNonQuery();
            //NpgsqlCommand cmd2 = new NpgsqlCommand("ALTER TABLE usercards ADD imDeck boolean NOT NULL DEFAULT false;", connection);
            //cmd2.ExecuteNonQuery();
            connection.Close();
        }

        public void createUser(string username, string pw)
        {
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("insert into Persons (username, password, coins) values('" + username + "', '" + pw + "',20);", connection);
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

        public bool createCard(string Id, string Name, double Damage, int pack)
        {
            connection.Open();
            try
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("insert into cards (Id, Name, Damage, Pack) values('" + Id + "', '" + Name + "', '" + Damage + "', '" + pack + "');", connection);
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

            if(pack==0)
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
            int coins=0;

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
                    CardList.Insert(ii, @"{  'Id': '" +dataReader2[0].ToString()+"', 'Name': '"+ dataReader2[1].ToString() + "', 'Damage': '"+ dataReader2[2].ToString() + "'} ");  
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
                NpgsqlCommand cmd2 = new NpgsqlCommand("Select id, name, damage FROM cards WHERE id='" + CardID[i] + "';", connection);
                NpgsqlDataReader dataReader2 = cmd2.ExecuteReader();

                for (int ii = 0; dataReader2.Read(); ii++)
                {
                    //CardList.Insert(ii,"{\"Id\":\"" + dataReader2[0].ToString() + ", \"Name\":\"" + dataReader2[1].ToString() + ", \"Damage\":" + dataReader2[2].ToString() + "}");
                    Deck.Insert(ii, @"{  'Id': '" + dataReader2[0].ToString() + "', 'Name': '" + dataReader2[1].ToString() + "', 'Damage': '" + dataReader2[2].ToString() + "'} ");
                    //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
                }

                connection.Close();
            }

            return Deck;
        }

        public bool setDeck(string username, string id, int i)
        {
            if (i == 0)
            {
                connection.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("UPDATE usercards SET imDeck = 'false' WHERE username = '" + username + "';", connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("UPDATE usercards SET imDeck = 'true' WHERE username = '" + username + "' AND cardsid='" + id + "';", connection);
            cmd2.ExecuteNonQuery();
            connection.Close();
            return true;
        }
    }
}
