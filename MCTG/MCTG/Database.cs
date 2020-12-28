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
            //NpgsqlCommand cmd = new NpgsqlCommand("CREATE TABLE packages ( Id varchar(255), Name varchar(255)); ", connection);
            //cmd.ExecuteNonQuery();
            NpgsqlCommand cmd2 = new NpgsqlCommand("CREATE TABLE cards ( Id varchar(255), Name varchar(255), Damage DECIMAL(5,1)); ", connection);
            cmd2.ExecuteNonQuery();
            connection.Close();
        }

        public void createUser(string username, string pw)
        {
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("insert into Persons (username, password) values('"+username+"', '"+pw+"')", connection);
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
            return check;
        }

        public bool createCard(string Id, string Name, double Damage)
        {
            connection.Open();
            NpgsqlCommand cmd2 = new NpgsqlCommand("insert into cards (Id, Name, Damage) values('" + Id + "', '" + Name + "', '"+ Damage +"')", connection);
            cmd2.ExecuteNonQuery();

            connection.Close();
            return true;
        }
    }
}
