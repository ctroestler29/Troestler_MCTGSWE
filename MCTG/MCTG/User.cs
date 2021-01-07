using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;

namespace MCTG
{
    public class User
    {
        public TcpClient Client { get; set; }
        public string ClNo { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int battleID { get; set; }
        public string Authorization { get; set; }
        public List<ICard> stack = new List<ICard>();
        public bool changedStack = true;
        public int coins { get; set; }
        public List<ICard> deck = new List<ICard>();
        public bool changedDeck = true;
        public static List<User> userobj = new List<User>();


        public User(string _username)
        {
            username = _username;
            
        }

        public static User getUserObj(string username)
        {
            
            foreach (var item in User.userobj)
            {
                if (item.username == username)
                {
                    User user = item;
                    return user;
                }
            }
            return null;
            
        }

    }
}
