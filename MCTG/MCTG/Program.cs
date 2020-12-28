using System;

namespace MCTG
{
    class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database();
            //db.createTables();
            //db.insert();
            //Console.WriteLine(db.checkUser());
            HTTPServer server = new HTTPServer();
            server.Start();







            //User user1 = new User();
            //User user2 = new User();
            //Console.WriteLine("Username für User1:");
            //user1.username = Console.ReadLine();
            //Console.WriteLine("Username für User2:");
            //user2.username = Console.ReadLine();
            //user1.shop();
            //user2.shop();
            //Arena arena = new Arena(user1, user2);
            //arena.StartBattle();

            Console.ReadKey();
        }
    }
}
