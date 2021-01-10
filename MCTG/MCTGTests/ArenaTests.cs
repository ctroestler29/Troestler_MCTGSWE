using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCTG;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace MCTG.Tests
{
    [TestClass()]
    public class ArenaTests
    {
        public static object _lock = new object();
        public static System.Threading.AutoResetEvent event_2 = new System.Threading.AutoResetEvent(false);
        public static ConcurrentQueue<User> warteschlange = new ConcurrentQueue<User>();
        public static ConcurrentDictionary<string, string> result2 = new ConcurrentDictionary<string, string>();
        

        [TestMethod()]
        public void CreateThread()
        {
            User user = new User("thread1");
            User user2 = new User("thread2");
            User user3 = new User("thread3");
            User user4 = new User("thread4");

            Thread thread = new Thread(() => FillArenaTest(user));
            Thread thread2 = new Thread(() => FillArenaTest(user2));
            Thread thread3 = new Thread(() => FillArenaTest(user3));
            Thread thread4 = new Thread(() => FillArenaTest(user4));

            thread.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            Thread.Sleep(1000);
        }

        public void FillArenaTest(User user)
        {
            string response = "";
            Monitor.Enter(_lock);
            if (warteschlange.Count == 0)
            {
                Debug.WriteLine("User: " + user.username + " wartet!");
                warteschlange.Enqueue(user);
                Monitor.Exit(_lock);
                event_2.WaitOne();
                result2.TryRemove(user.username, out response);
                response = user.username + " vs " + response;
            }
            else
            {
                Debug.WriteLine("User: " + user.username + " findet!");
                warteschlange.TryDequeue(out User user2);
                User user1 = user;
                Monitor.Exit(_lock);

                result2.TryAdd(user2.username, user1.username);
                response = user1.username + " vs " + user2.username;
                event_2.Set();
                event_2.Reset();


            }
            Debug.WriteLine("Match: " + response);
        }
    }
}