using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MCTG
{
    class RequestContext
    {
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
            if (msg.Length == 0)
            {
                NoContent();
                return 1;
            }

            if (!Directory.Exists(path))
            {
                DirNotFound();
                return 1;
            }

            if (msgID.Length > 0)
            {
                DontUseFileID();
                return 1;
            }

            int i = 1;
            string hv2;
            while (true)
            {
                hv2 = path + @"\" + i;
                if (!File.Exists(hv2))
                {
                    break;
                }
                i++;
            }

            File.WriteAllText(hv2, msg);
            statusCode = 200;
            statusPhrase = "OK";
            response = "File with ID: " + i + " successfully created";
            msgID = i.ToString();

            return 0;
        }

        public int GET(string path)
        {
            if (!Directory.Exists(path))
            {
                DirNotFound();
                return 1;
            }

            if (msgID.Length < 1)
            {
                int i = ListAllMsg(path);
                return i;

            }
            path = Path.Combine(path, msgID.ToString());
            if (File.Exists(path))
            {
                //txt = File.ReadAllText(path);
                statusCode = 200;
                statusPhrase = "OK";
                response += Path.GetFileName(path);
                response += "\n{";
                response += File.ReadAllText(path);
                response += "}\n";

            }
            else
            {
                FileNotFound();
            }
            return 0;
        }

        public int PUT(string path, string msg)
        {
            if (msg.Length == 0)
            {
                NoContent();
                return 1;
            }

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
                File.WriteAllText(path, msg);
                statusCode = 200;
                statusPhrase = "OK";
                response = "File with ID: " + msgID + " successfully updated";
            }
            else
            {
                FileNotFound();
            }
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
