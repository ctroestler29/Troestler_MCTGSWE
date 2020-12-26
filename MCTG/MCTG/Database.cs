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
        string dataItems = "";

        public void insert()
        {
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("CREATE TABLE Persons ( username varchar(255), password varchar(255)); ", connection);
            cmd.ExecuteNonQuery();
            NpgsqlCommand cmd2 = new NpgsqlCommand("insert into Persons (username, password) values('name', 'pw')", connection);
            cmd2.ExecuteNonQuery();
            
            connection.Close();
        }

        public string GET()
        {
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Persons;", connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();
            for (int i = 0; dataReader.Read(); i++)
            {
                dataItems+=dataReader[0].ToString() + "," + dataReader[1].ToString() + "\r\n";
            }
            connection.Close();
            return dataItems;
        }
    }
}
