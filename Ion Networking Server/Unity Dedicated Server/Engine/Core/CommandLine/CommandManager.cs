using System;
using System.Collections;
using System.Collections.Generic;

namespace IonServer.Engine.Core.CommandLine
{
    public static class CommandManager
    {
        private static readonly Hashtable _commandList = new Hashtable();
        private static readonly List<string[]> _queue = new List<string[]>();

        public delegate void CommandAction(string[] arguments);

        //registers a command with the CommandManager. Allows developers to make custom commands.
        public static void AddCommand(string keyword, CommandAction action)
        {
            if (_commandList.ContainsKey(keyword))
                return;

            _commandList.Add(keyword, action);
        }

        //Removes a command from the list
        public static void RemoveCommand(string keyword)
        {
            _commandList.Remove(keyword);
        }

        //returns the command action for a keyword
        private static CommandAction GetCommandAction(string keyword)
        {
            return (CommandAction)_commandList[keyword];
        }

        //Adds a command to the queue. Used to keep threading issues from popping up where a command is given in the middle of a game update.
        public static void QueueCommand(string[] arguments)
        {
            lock (_queue)
            {
                _queue.Add(arguments);
            }
        }

        //Handles all queued commands in the queue before clearing it
        public static void HandleQueue()
        {
            lock (_queue)
            {
                foreach(string[] arguments in _queue)
                {
                    HandleArguments(arguments);
                }
                _queue.Clear();
            }
        }

        //Routes arguments to the correct commands
        private static void HandleArguments(string[] arguments)
        {
            try
            {
                CommandAction action = GetCommandAction(arguments[0]);

                if (action == null)
                {
                    Console.WriteLine("Unrecognized command '" + arguments[0] + "'");
                    return;
                }

                action(arguments);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                return;
            }
        }
    }
}
