using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MCTG
{
    class HTTPServer
    {
        public void Start()
        {
            RequestContext rc = new RequestContext();
            TcpListener listener = new TcpListener(IPAddress.Loopback, 10001);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;
            listener.Start(5);
            Console.WriteLine(listener.LocalEndpoint);
            counter = 0;
            Console.CancelKeyPress += (sender, e) => Environment.Exit(0);
            //String data = null;
            Byte[] bytes = new Byte[256];
            while (true)
            {
                try
                {
                    counter += 1;
                    clientSocket = listener.AcceptTcpClient();
                    Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started!");
                    HandleClient client = new HandleClient();
                    client.startClient(clientSocket, Convert.ToString(counter));
                    
                }
                catch (Exception exc)
                {
                    Console.WriteLine("error occurred: " + exc.Message);
                }
            }

        }
    }
}
