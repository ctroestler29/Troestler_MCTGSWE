using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MCTG
{
    class RequestContext
    {
        User user = new User("");
        Database db = new Database();
        public string http_verb;
        public IDictionary<string, string> header;
        public string protocol;
        public int statusCode;
        public string statusPhrase;
        public string msg = "";
        public string msgID;
        public string response;
        public string directory;
        public string path = "";
        string[] line;
        public int receive(string req)
        {
            http_verb = "";
            protocol = "";
            statusCode = 0;
            statusPhrase = "";
            msg = "";
            msgID = "";
            response = "";
            directory = "";

            header = new Dictionary<string, string>();

            line = req.Split("\r\n");
            string[] first_line = line[0].Split(' ');
            string[] res = first_line[1].Split('/');
            http_verb = first_line[0];

            protocol = first_line[2];
            if (protocol != "HTTP/1.1")
            {
                statusCode = 500;
                statusPhrase = "Internal Server Error";
                response = "Wrong protocol! You need the " + protocol + " protocol.";
                return 1;
            }

            if (res.Count() == 2)
            {
                directory = res[1];
            }
            else if (res.Count() == 3)
            {
                directory = res[1];
                msgID = res[2];
            }

            GetHeader();


            if (http_verb == "POST" || http_verb == "PUT")
            {
                GetBody();
            }

            path = Path.Combine(Environment.CurrentDirectory, directory);
            switch (http_verb)
            {
                case "POST":
                    POST(path, msg);
                    break;
                case "GET":
                    GET(path);
                    break;
                case "PUT":
                    PUT(path, msg);
                    break;
                case "DELETE":
                    DEL(path);
                    break;
                default:
                    statusCode = 501;
                    statusPhrase = "Not Implemented";
                    response = "http verb not found!";
                    return 1;
            }

            return 0;
        }

        public int POST(string path, string msg)
        {
            try
            {
                user.Authorization = header.ElementAt(4).Value;
            }
            catch { };
            if (directory == "users")
            {
                JObject json = JObject.Parse(msg);
                string username = json.SelectToken("Username").ToString();
                string pw = json.SelectToken("Password").ToString();

                if (db.checkUser(username))
                {
                    db.createUser(username, pw);
                    statusCode = 200;
                    statusPhrase = "Ok";
                    response = "User " + username + " successfully created!";
                    return 0;
                }
                else
                {
                    statusCode = 900;
                    statusPhrase = "Username already exists!";
                    response = "Username: " + username + " already exists! Please choose a different one!";
                    return 1;
                }
            }

            if (directory == "sessions")
            {
                JObject json = JObject.Parse(msg);
                string username = json.SelectToken("Username").ToString();
                string pw = json.SelectToken("Password").ToString();

                if (db.Login(username, pw) == 0)
                {
                    statusCode = 200;
                    statusPhrase = "Ok";
                    response = "Successfull login with User " + username;
                    user.username = username;
                    return 0;
                }
                else if (db.Login(username, pw) == 1)
                {
                    statusCode = 800;
                    statusPhrase = "Already Loggedin!";
                    response = "User with Username: " + username + " is already Loggedin!";
                    return 1;
                }
                else if (db.Login(username, pw) == 2)
                {
                    statusCode = 800;
                    statusPhrase = "Wrong Username/password";
                    response = "Wrong login-Data! Please try again.";
                    return 1;
                }
            }

            if (directory == "packages")
            {
                if (!db.checkSession(user.Authorization))
                {
                    statusCode = 600;
                    statusPhrase = "No valid Session!";
                    response = "Please Login again! Your Session expired!";
                    return 1;
                }

                if (user.Authorization != "Basic admin-mtcgToken")
                {
                    statusCode = 700;
                    statusPhrase = "No Admin!";
                    response = "Only admins are allowed to create packages!";
                    return 1;
                }
                int pack = db.getMaxPack();
                db.createPack(pack);

                JObject json;
                string[] arr = msg.Split("},");
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i != arr.Length - 1)
                    {
                        arr[i] += "}";
                    }
                    arr[i] = arr[i].Replace("[", "");
                    arr[i] = arr[i].Replace("]", "");
                    json = JObject.Parse(arr[i]);
                    string element = "";
                    string type = "";
                    string cardtype = "";

                    if(json.SelectToken("Name").ToString().Contains("Spell"))
                    {
                        cardtype = "spell";
                        element = json.SelectToken("Name").ToString().Replace("Spell", "");
                    }
                    else
                    {
                        cardtype = "monster";

                        if(json.SelectToken("Name").ToString().Contains("Fire"))
                        {
                            element = "Fire";
                            type = json.SelectToken("Name").ToString().Replace("Fire", "");
                        }
                        else if(json.SelectToken("Name").ToString().Contains("Water"))
                        {
                            element = "Water";
                            type = json.SelectToken("Name").ToString().Replace("Water", "");
                        }
                        else
                        {
                            element = "Regular";
                            type = json.SelectToken("Name").ToString();
                        }
                    }
                    db.createCard(json.SelectToken("Id").ToString(), json.SelectToken("Name").ToString(), double.Parse(json.SelectToken("Damage").ToString()),element,type, cardtype, pack);
                }



            }

            if (directory == "transactions")
            {
                string username = db.GetUsernameBySessionID(user.Authorization);
                if (!db.checkSession(user.Authorization))
                {
                    statusCode = 600;
                    statusPhrase = "No valid Session!";
                    response = "Please Login again! Your Session expired!";
                    return 1;
                }
                int coins = db.GetCoins(username);
                if (coins < 5)
                {
                    statusCode = 400;
                    statusPhrase = "Zu wenig Coins!";
                    response = "Pack konte nicht geöffent werden! Kostet 5 Coins! Kontostand: " + coins;
                    return 1;
                }

                if (db.getPack(username))
                {
                    coins -= 5;
                    statusCode = 600;
                    statusPhrase = "Pack geöffnet!";
                    response = "Pack um 5 Coins geöffnet! Neuer Kontostand: " + coins;
                    db.bezahlen(username, coins);
                    return 0;
                }
                else
                {
                    statusCode = 500;
                    statusPhrase = "Kein Pack verfügbar!";
                    response = "Zur Zeit ist kein Pack verfügbar! Bitte versuchen Sie es später erneut!";
                    return 1;
                }

            }

            if(directory=="battles")
            {
                
                user.Authorization = header.ElementAt(3).Value;
                string username = db.GetUsernameBySessionID(user.Authorization);
                if (!db.checkSession(user.Authorization))
                {
                    statusCode = 600;
                    statusPhrase = "No valid Session!";
                    response = "Please Login again! Your Session expired!";
                    return 1;
                }

                if(db.showDeck(username).Count!=4)
                {
                    statusCode = 999;
                    statusPhrase = "No valid Deck!";
                    response = "Before entering the Arena you need 4 Cards in your Deck!";
                    return 1;
                }

                string oponent = db.findBattle(username);
                if (oponent!="")
                {
                    statusCode = 200;
                    statusPhrase = "Ok";
                    response = "Oponent found!";
                    User user1 = new User(username);
                    User user2 = new User(oponent);

                    List<string> deck1 = db.showDeck(user1.username);
                    List<string> deck2 = db.showDeck(user2.username);

                    List<ICard> IDeck1 = new List<ICard>();
                    List<ICard> IDeck2 = new List<ICard>();
                    
                    int i = 0;
                    while (i < deck1.Count())
                    {
                        ICard card = new ICard();
                        JObject json = JObject.Parse(deck1[i]);
                        card.ID = json.SelectToken("Id").ToString();
                        card.Name = json.SelectToken("Name").ToString();
                        card.Damage = double.Parse(json.SelectToken("Damage").ToString());
                        card.Element = json.SelectToken("Element").ToString();
                        card.Type = json.SelectToken("Type").ToString();
                        card.CardType = json.SelectToken("CardType").ToString();
                        IDeck1.Add(card);
                        i++;
                    }

                    
                    int ii = 0;
                    while (ii < deck2.Count())
                    {
                        ICard card2 = new ICard();
                        JObject json = JObject.Parse(deck2[ii]);
                        card2.ID = json.SelectToken("Id").ToString();
                        card2.Name = json.SelectToken("Name").ToString();
                        card2.Damage = double.Parse(json.SelectToken("Damage").ToString());
                        card2.Element = json.SelectToken("Element").ToString();
                        card2.Type = json.SelectToken("Type").ToString();
                        card2.CardType = json.SelectToken("CardType").ToString();
                        IDeck2.Add(card2);
                        ii++;
                    }

                    user1.deck = IDeck1;
                    user2.deck = IDeck2;
                    user1.battleID = db.getBattleID(user1.username, user2.username);
                    user2.battleID = db.getBattleID(user1.username, user2.username);

                    Arena arena = new Arena(user1, user2);

                    string log = arena.StartBattle();
                    response = log;
                    db.endBattle(user1.battleID);
                    return 0;
                }
                else
                {
                    statusCode = 1001;
                    statusPhrase = "Already in a Game!";
                    response = "User: "+username+" is already in a Game!";
                    return 1;
                }

                
            }


            //if (msg.Length == 0)
            //{
            //    NoContent();
            //    return 1;
            //}

            //if (!Directory.Exists(path))
            //{
            //    DirNotFound();
            //    return 1;
            //}

            //if (msgID.Length > 0)
            //{
            //    DontUseFileID();
            //    return 1;
            //}

            //int i = 1;
            //string hv2;
            //while (true)
            //{
            //    hv2 = path + @"\" + i;
            //    if (!File.Exists(hv2))
            //    {
            //        break;
            //    }
            //    i++;
            //}

            //File.WriteAllText(hv2, msg);
            //statusCode = 200;
            //statusPhrase = "OK";
            //response = "File with ID: " + i + " successfully created";
            //msgID = i.ToString();

            return 0;
        }

        public int GET(string path)
        {
            try
            {
                user.Authorization = header.ElementAt(3).Value;
            }
            catch { };
            string username = db.GetUsernameBySessionID(user.Authorization);
            if (directory == "cards")
            {
                if (!db.checkSession(user.Authorization) || user.Authorization == null)
                {
                    statusCode = 600;
                    statusPhrase = "No valid Session!";
                    response = "Your SessionID expired or is wrong! Please Login";
                    return 1;
                }

                List<string> strarr = db.GetCards(username);

                int i = 0;
                while (i < strarr.Count())
                {
                    JObject json = JObject.Parse(strarr[i]);
                    response += "Card-ID: " + json.SelectToken("Id").ToString() + ":\n";
                    response += " {\n";
                    response += "   Name: " + json.SelectToken("Name").ToString() + "\n";
                    response += "   Damage: " + json.SelectToken("Damage").ToString() + "\n";
                    response += " }\n";
                    i++;
                }
                statusCode = 200;
                statusPhrase = "OK";
                return 0;

            }

            if (directory == "deck")
            {
                if (!db.checkSession(user.Authorization) || user.Authorization == null)
                {
                    statusCode = 600;
                    statusPhrase = "No valid Session!";
                    response = "Your SessionID expired or is wrong! Please Login";
                    return 1;
                }

                List<string> deck = db.showDeck(username);

                if (deck.Count == 0)
                {
                    statusCode = 300;
                    statusPhrase = "Deck unconfigured!";
                    response = "Your Deck is empty! Please configure your Deck! (4 Cards)";
                    return 1;
                }

                int i = 0;
                while (i < deck.Count())
                {
                    JObject json = JObject.Parse(deck[i]);
                    response += "Card-ID: " + json.SelectToken("Id").ToString() + ":\n";
                    response += " {\n";
                    response += "   Name: " + json.SelectToken("Name").ToString() + "\n";
                    response += "   Damage: " + json.SelectToken("Damage").ToString() + "\n";
                    response += " }\n";
                    i++;
                }
                statusCode = 200;
                statusPhrase = "OK";
                return 0;
            }

            if (directory == "users")
            {
                username = msgID;
                if (!db.checkSession(user.Authorization))
                {
                    statusCode = 600;
                    statusPhrase = "No valid Session!";
                    response = "Please Login again! Your Session expired!";
                    return 1;
                }
                if (username == db.GetUsernameBySessionID(user.Authorization))
                {
                    string UserData = db.getUserData(username);

                    JObject json = JObject.Parse(UserData);
                    response += "Username: " + json.SelectToken("Username").ToString() + "\n";
                    response += " {\n";
                    response += "   Name: " + json.SelectToken("Name").ToString() + "\n";
                    response += "   Bio: " + json.SelectToken("Bio").ToString() + "\n";
                    response += "   Image: " + json.SelectToken("Image").ToString() + "\n";
                    response += "   Coins: " + json.SelectToken("Coins").ToString() + "\n";
                    response += " }\n";
                    statusCode = 200;
                    statusPhrase = "OK";
                    return 0;
                }
                else
                {
                    statusCode = 1000;
                    statusPhrase = "Not your Username!";
                    response = "Cant access user data from another User!";
                    return 1;
                }
            }

            if (directory == "stats")
            {
                if (!db.checkSession(user.Authorization))
                {
                    statusCode = 600;
                    statusPhrase = "No valid Session!";
                    response = "Please Login again! Your Session expired!";
                    return 1;
                }

                string stats = db.getStats(username);

                JObject json = JObject.Parse(stats);
                response += "Username: " + json.SelectToken("Username").ToString() + "\n";
                response += " {\n";
                response += "   Gespielte Spiele: " + (int.Parse(json.SelectToken("Gewonnen").ToString()) + int.Parse(json.SelectToken("Verloren").ToString())) + "\n";
                response += "       Gewonnene Spiele: " + json.SelectToken("Gewonnen").ToString() + "\n";
                response += "       Verlorene Spiele: " + json.SelectToken("Verloren").ToString() + "\n";
                response += "   ELO-Value: " + json.SelectToken("ELO").ToString() + "\n";
                response += " }\n";
                statusCode = 200;
                statusPhrase = "OK";
                return 0;
            }

            if (directory == "score")
            {
                if (!db.checkSession(user.Authorization))
                {
                    statusCode = 600;
                    statusPhrase = "No valid Session!";
                    response = "Please Login again! Your Session expired!";
                    return 1;
                }

                List<string> scoreboard = db.getScoreboard(username);

                int i = 0;
                while (i < scoreboard.Count())
                {
                    JObject json = JObject.Parse(scoreboard[i]);
                    if (json.SelectToken("UserPlatz").ToString() != "0")
                    {
                        response += "Deine Platzierung: " + json.SelectToken("UserPlatz").ToString() + ":\n";
                        response += "\n";
                        break;
                    }
                    i++;
                }
                i = 0;
                while (i < scoreboard.Count())
                {
                    JObject json = JObject.Parse(scoreboard[i]);
                    
                    response += "Platzierung: " + json.SelectToken("Platz").ToString() + "\n";
                    response += " {\n";
                    response += "   Username: " + json.SelectToken("Username").ToString() + "\n";
                    response += "   ELO-Value: " + json.SelectToken("ELO").ToString() + "\n";
                    response += " }\n";
                    i++;
                }
                statusCode = 200;
                statusPhrase = "OK";
                return 0;
            }

            return 1;

            //if (!Directory.Exists(path))
            //{
            //    DirNotFound();
            //    return 1;
            //}

            //if (msgID.Length < 1)
            //{
            //    int i = ListAllMsg(path);
            //    return i;

            //}
            //path = Path.Combine(path, msgID.ToString());
            //if (File.Exists(path))
            //{
            //    //txt = File.ReadAllText(path);
            //    statusCode = 200;
            //    statusPhrase = "OK";
            //    response += Path.GetFileName(path);
            //    response += "\n{";
            //    response += File.ReadAllText(path);
            //    response += "}\n";

            //}
            //else
            //{
            //    FileNotFound();
            //}
            //return 0;
        }

        public int PUT(string path, string msg)
        {
            try
            {
                user.Authorization = header.ElementAt(4).Value;
            }
            catch { };

            string username = db.GetUsernameBySessionID(user.Authorization);

            if (directory == "deck")
            {
                if (!db.checkSession(user.Authorization) || user.Authorization == null)
                {
                    statusCode = 600;
                    statusPhrase = "No valid Session!";
                    response = "Your SessionID expired or is wrong! Please Login";
                    return 1;
                }
                msg = msg.Replace("[", "");
                msg = msg.Replace("]", "");
                msg = msg.Replace(@"\", "");
                msg = msg.Replace('"', ' ');
                msg = msg.Replace(" ", "");

                string[] strarr = msg.Split(",");

                for (int i = 0; i < strarr.Length; i++)
                {
                    if (!db.setDeck(username, strarr[i].ToString(), i, strarr.Length))
                    {
                        statusCode = 900;
                        statusPhrase = "Not a valid Deck!";
                        response = "Please use 4 cards of your Card-Collection!";
                        return 1;
                    }
                }
            }


            if (directory == "users")
            {
                string uname = msgID;
                if (!db.checkSession(user.Authorization))
                {
                    statusCode = 600;
                    statusPhrase = "No valid Session!";
                    response = "Please Login again! Your Session expired!";
                    return 1;
                }
                if (uname == db.GetUsernameBySessionID(user.Authorization))
                {
                    JObject json = JObject.Parse(msg);
                    string name = json.SelectToken("Name").ToString();
                    string bio = json.SelectToken("Bio").ToString();
                    string image = json.SelectToken("Image").ToString();

                    if (db.setUserData(uname, name, bio, image))
                    {
                        statusCode = 200;
                        statusPhrase = "OK";
                        response = "User Data updated!";
                        return 0;
                    }
                    else
                    {
                        statusCode = 300;
                        statusPhrase = "No valid UserData";
                        response = "Please try again. The UserData was invalid!";
                        return 1;
                    }
                }
                else
                {
                    statusCode = 1000;
                    statusPhrase = "Not your Username!";
                    response = "Cant access user data from another User!";
                    return 1;
                }

            }

            //if (msg.Length == 0)
            //{
            //    NoContent();
            //    return 1;
            //}

            //if (!Directory.Exists(path))
            //{
            //    DirNotFound();
            //    return 1;
            //}

            //if (msgID.ToString().Length < 1)
            //{
            //    NoFileID();
            //    return 1;
            //}
            //path = Path.Combine(path, msgID.ToString());
            //if (File.Exists(path))
            //{
            //    File.WriteAllText(path, msg);
            //    statusCode = 200;
            //    statusPhrase = "OK";
            //    response = "File with ID: " + msgID + " successfully updated";
            //}
            //else
            //{
            //    FileNotFound();
            //}
            return 0;
        }

        public int DEL(string path)
        {
            if (!Directory.Exists(path))
            {
                DirNotFound();
                return 1;
            }

            if (msgID.ToString().Length < 1)
            {
                NoFileID();
                return 1;
            }
            path = Path.Combine(path, msgID.ToString());
            if (File.Exists(path))
            {
                File.Delete(path);
                statusCode = 200;
                statusPhrase = "OK";
                response = "File with ID: " + msgID + " successfully deleted";
            }
            else
            {
                FileNotFound();
            }
            return 0;
        }

        public int ListAllMsg(string path)
        {
            if (!Directory.Exists(path))
            {
                DirNotFound();
                return 1;
            }

            string[] files = Directory.GetFiles(path);
            if (files.Count() <= 0)
            {
                statusCode = 404;
                statusPhrase = "Directory empty!";
                response = "The Directory " + directory + " is empty! No messages!";
                return 1;
            }
            int i = 0;
            while (i < files.Count())
            {
                response += Path.GetFileName(files[i]) + ":\n";
                response += " {\n";
                response += "  " + File.ReadAllText(files[i]) + "\n";
                response += " }\n";
                i++;
            }
            statusCode = 200;
            statusPhrase = "OK";
            return 0;
        }

        public string CreateHttpResponse()
        {
            string sb;
            sb = protocol + " " + statusCode + " " + statusPhrase + "\r\n";
            sb += "Server: if20b295_Troestler\r\n";
            sb += "Content-Type: plain/text\r\n";
            sb += $"Content-Length: " + response.Length + "\r\n";
            sb += "\r\n";
            sb += response;
            sb += "\r\n\r\n";

            return sb;
        }

        public void GetHeader()
        {
            int ii = 0;
            try
            {
                while (line[ii] != "")
                {

                    if (line[ii].Contains(":"))
                    {
                        string[] h = line[ii].Split(": ");
                        header.Add(h[0], h[1]);
                    }
                    ii++;
                }
            }
            catch
            { }
            foreach (var item in header)
            {
                Console.WriteLine("Key: " + item.Key + " Value: " + item.Value);
            }
        }

        public void GetBody()
        {
            int i = 0;
            int hv = 0;
            while (i <= line.Count() - 1)
            {
                if (line[i] == "")
                {
                    if (hv == 0)
                    {
                        msg += line[i + 1];
                    }
                    hv = 1;
                }
                i++;
            }

        }
        public void FileNotFound()
        {
            statusCode = 403;
            statusPhrase = "File Not Found";
            response = "File with ID: " + msgID + " could not be found";
        }

        public void DirNotFound()
        {
            statusCode = 404;
            statusPhrase = "Directory Not Found";
            response = "Path ..." + directory + " could not be found!";
        }

        public void DontUseFileID()
        {
            statusCode = 405;
            statusPhrase = "Don't use a File-ID!";
            response = "Please don't specify a File-ID, when making a POST Request!";

        }
        public void NoFileID()
        {
            statusCode = 406;
            statusPhrase = "No File-ID";
            response = "Please use a File-ID!";
        }
        public void NoContent()
        {
            statusCode = 407;
            statusPhrase = "No Content!";
            response = "Please write something into the body!";
        }
    }
}
