using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MCTG
{
    //Class to handle each client request separatly
    public class HandleClient
    {
        User user = new User("");
        RequestContext rc = new RequestContext();
        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            user.Client = inClientSocket;
            user.ClNo = clineNo;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
            Console.WriteLine("TEST");
            //arena.ArenaList
            
        }
        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = user.Client.GetStream();
                    int i = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom, 0, i);
                    Console.WriteLine(" >> " + "From client-" + user.ClNo + dataFromClient);
                    rc.receive(dataFromClient);

                    string sb = rc.CreateHttpResponse();
                    rCount = Convert.ToString(requestCount);
                    //serverResponse = "Server to clinet(" + clNo + ") " + rCount;
                    sendBytes = Encoding.ASCII.GetBytes(sb);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                }
            }
        }
    }
}

