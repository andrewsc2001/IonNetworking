﻿using System;
using System.Diagnostics;

namespace Unity_Dedicated_Server.GameLogic
{
    public static class Game
    {
        //Settings
        public static int UpdatesPerSecond = 10;

        public static bool isRunning = false;
        public static Stopwatch Time;
        
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

        }
    }
}