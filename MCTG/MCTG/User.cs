using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MCTG
{
    public class User
    {
        public static HttpClient ApiClient { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        List<ICard> stack = new List<ICard>();
        public int coins { get; set; }
        List<ICard> deck = new List<ICard>();

        public User(string _username, string _password)
        {
            username = _username;
            password = _username;
        }

        public static void initClient()
        {
            ApiClient = new HttpClient();
            ApiClient.BaseAddress = new Uri("");
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }
        public int register()
        {
            Console.WriteLine("Username: ");
            username = Console.ReadLine();
            Console.WriteLine("Password: ");
            password = Console.ReadLine();
            coins = 20;

            return 0;
        }

        public int login()
        {
            Console.WriteLine("Username: ");
            username = Console.ReadLine();
            Console.WriteLine("Password: ");
            password = Console.ReadLine();

            return 0;
        }

        public int findBattle()
        {
            return 0;
        }

        public int defineDeck()
        {
            return 0;
        }

        public int TradeCard(ICard card)
        {
            return 0;
        }

        public int Tradinglist()
        {
            return 0;
        }

        public List<ICard> shop()
        {
            List<ICard> pack = new List<ICard>();
            return pack;
        }
    }
}
