using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<ICard> stack = new List<ICard>();
        public int coins { get; set; }
        public List<ICard> deck = new List<ICard>();


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

        public void shop()
        {
            Random rnd = new Random();
            int r = 0;
            CardCollection cc = new CardCollection();
            cc.fill();

            for (int i = 0; i <= 4; i++)
            {
                r = rnd.Next(0, cc.collec.Count() - 1);
                deck.Add(cc.collec[r]);
            }
            coins -= 5;
        }
    }
}
