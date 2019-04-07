using System;
using System.Threading;

namespace ExampleServer.Content
{
    public static class Game
    {
        //Run once at startup
        public static void Start()
        {
            Console.WriteLine("Hello, world!");
        }

        //Run once at shutdown
        public static void Stop()
        {

        }

        //Game Loop
        public static void Update()
        {
            Thread.Sleep(25);
        }
    }
}
 