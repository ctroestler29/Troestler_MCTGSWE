using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCTG;
using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace MCTG.Tests
{
    [TestClass()]
    public class DatabaseTests
    {
        public static string connstring = "Server=127.0.0.1;Port=5432;Database=testdb;User Id=postgres;Password=postgres;";
        NpgsqlConnection connection = new NpgsqlConnection(connstring);

        [TestMethod()]
        public void dropTablesTest()
        {
            
            //Arrange
            NpgsqlCommand cmd2 = new NpgsqlCommand("DROP TABLE Persons;", connection);
            NpgsqlCommand cmd3 = new NpgsqlCommand("DROP TABLE Sessions;", connection);
            //Act
            connection.Open();
            int z = cmd2.ExecuteNonQuery();
            connection.Close();
            connection.Open();
            int z2 = cmd3.ExecuteNonQuery();
            connection.Close();
            //Assert
            Assert.IsTrue(z == -1 && z2 == -1);
            
        }

        [TestMethod()]
        public void createTablesTest()
        {
           
            //Arrange
            NpgsqlCommand cmd2 = new NpgsqlCommand("CREATE TABLE Persons(username varchar(255) PRIMARY KEY NOT NULL, pw varchar(255) NOT NULL, coins INTEGER NOT NULL);", connection);
            NpgsqlCommand cmd3 = new NpgsqlCommand("CREATE TABLE Sessions(Id INTEGER PRIMARY KEY NOT NULL, username varchar(255) NOT NULL, sessionid varchar(255) NOT NULL);", connection);

            //Act
            connection.Open();
            int z = cmd2.ExecuteNonQuery();
            connection.Close();
            connection.Open();
            int z2 = cmd3.ExecuteNonQuery();
            connection.Close();
            //Assert
            Assert.IsTrue(z == -1 && z2 == -1);

        }

        [TestMethod()]
        public void createUserTest()
        {
            //Arrange
            string username = "testUser";
            string pw = "test";
            NpgsqlCommand cmd2 = new NpgsqlCommand("insert into Persons (username, pw, coins) values('" + username + "', '" + pw + "', 20);", connection);

            //Act
            connection.Open();
            int z = cmd2.ExecuteNonQuery();

            //Assert   
            Assert.IsTrue(z >= 1);
            connection.Close();
        }

        [TestMethod()]
        public void checkUserTest()
        {
            //Arrange
            string username = "testUser";
            bool check = true;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT username FROM Persons;", connection);

            //ACT
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

            //Assert
            Assert.IsFalse(check);
        }

        [TestMethod()]
        public void LoginTest()
        {
            string username = "testUser";
            string pw = "test";
            string sessionid = "Basic " + username + "-mtcgToken";
            bool check = false;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT username, pw FROM Persons WHERE username='" + username + "';", connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();

            for (int i = 0; dataReader.Read(); i++)
            {
                if (dataReader[0].ToString() == username && dataReader[1].ToString() == pw)
                {
                    check = true;
                }
            }
            connection.Close();
            int z = 0;
            if (check == true)
            {
                connection.Open();
                NpgsqlCommand cmd2 = new NpgsqlCommand("insert into sessions (username, sessionid) values('" + username + "', '" + sessionid + "');", connection);
                z = cmd2.ExecuteNonQuery();
                connection.Close();
            }


            Assert.IsTrue(z==1);
        }



        [TestMethod()]
        public void GetCoinsTest()
        {
            //Arrange
            string username = "testUser";
            int coins = 0;
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand("SELECT coins FROM Persons WHERE username='" + username + "';", connection);

            //Act
            NpgsqlDataReader dataReader = command.ExecuteReader();

            for (int i = 0; dataReader.Read(); i++)
            {
                coins = int.Parse(dataReader[0].ToString());
            }
            connection.Close();

            //Assert
            Assert.IsTrue(coins == 20);
        }

        [TestMethod()]
        public void bezahlenTest()
        {
            //Arrange
            int coins = 20;
            string username = "testUser";
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("UPDATE persons SET coins = '" + (coins - 5) + "' WHERE username='" + username + "';", connection);

            //Act
            int z = cmd.ExecuteNonQuery();
            connection.Close();
            Assert.IsTrue(z == 1);
        }

        [TestMethod()]
        public void GetUsernameBySessionIDTest()
        {
            string sessionid = "Basic testUser-mtcgToken";
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("Select username FROM sessions WHERE sessionid='" + sessionid + "';", connection);
            NpgsqlDataReader dataReader = cmd.ExecuteReader();
            string username = "";

            for (int i = 0; dataReader.Read(); i++)
            {
                username = dataReader[0].ToString();
            }

            connection.Close();

            Assert.IsTrue(username == "testUser");
        }

        [TestMethod()]
        public void GetCardsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void showDeckTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void setDeckTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getUserDataTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void setUserDataTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getStatsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getScoreboardTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void findBattleDBTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FillDeckTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AnzWarteschlangeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void setScoreTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getBattleIDTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getTradingDealsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void endBattleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getCardByIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getCardbyTradeIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void createTradeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getTradeConditionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void makeTradeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void deleteTradeTest()
        {
            Assert.Fail();
        }
    }
}