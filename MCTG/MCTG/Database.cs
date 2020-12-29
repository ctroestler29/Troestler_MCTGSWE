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
            //NpgsqlCommand cmd = new NpgsqlCommand("CREATE TABLE cards ( Id varchar(255), Name varchar(255), Damage NUMERIC(5,2), Pack INTEGER);", connection);
            //cmd.ExecuteNonQuery();
            //NpgsqlCommand cmd2 = new NpgsqlCommand("CREATE TABLE sessions ( SessionId varchar(255), SessionTime time); ", connection);
            //cmd2.ExecuteNonQuery();
            //NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM cards;", connection);
            //cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void createUser(string username, string pw)
        {
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("insert into Persons (username, password) values('"+username+"', '"+pw+"');", connection);
            cmd2.ExecuteNonQuery();
            
            connection.Close();
        }

        public bool checkUser(string username)
        {
            bool check=true;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT username FROM Persons;", connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();
            for (int i = 0; dataReader.Read(); i++)
            {
                if (dataReader[0].ToString()==username)
                {
                    check = false;
                }
                //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
            }
            connection.Close();
            return check;
        }

        public bool Login(string username, string pw)
        {
            bool check = false;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT username, password FROM Persons WHERE username='"+username+"';", connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();
            for (int i = 0; dataReader.Read(); i++)
            {
                if (dataReader[0].ToString() == username && dataReader[1].ToString() == pw)
                {
                    check = true;
                }
                //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
            }
            connection.Close();
            
            if (check)
            {
                connection.Open();
                string sessionid = "Basic " + username + "-mtcgToken";
                NpgsqlCommand cmd2 = new NpgsqlCommand("insert into sessions (SessionId, SessionTime) values('" + sessionid + "', '" + DateTime.Now + "');", connection);
                cmd2.ExecuteNonQuery();
                connection.Close();
            }
            return check;
        }

        public bool createCard(string Id, string Name, double Damage, int pack)
        {
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("insert into cards (Id, Name, Damage, Pack) values('" + Id + "', '" + Name + "', '"+ Damage +"', '"+pack+"');", connection);
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

        public bool checkSession(string sessionId)
        {
            bool check = false;
            int zv = 0;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT SessionId, SessionTime FROM Sessions WHERE SessionId='" + sessionId + "';", connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();
            for (int i = 0; dataReader.Read(); i++)
            {
                if (dataReader[0].ToString() == sessionId && DateTime.Now.Minute - DateTime.Parse(dataReader[1].ToString()).Minute < 2)
                {
                    check = true;
                }
                else if (dataReader[0].ToString() == sessionId && DateTime.Now.Minute - DateTime.Parse(dataReader[1].ToString()).Minute > 2)
                {
                    zv = 1;
                }
                //dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
            }
            connection.Close();
            connection.Open();
            if (check)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("UPDATE Sessions SET SessionTime = '"+DateTime.Now+"' WHERE SessionId='"+sessionId+"';", connection);
                cmd.ExecuteNonQuery();
            }
            else if(zv == 1)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM Sessions WHERE SessionId='"+sessionId+"';", connection);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
            return check;
        }
    }
}
