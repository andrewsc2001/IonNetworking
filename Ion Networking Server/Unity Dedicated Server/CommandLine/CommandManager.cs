using System;
using System.Collections;

namespace IonServer.CommandLine
{
    public static class CommandManager
    {
        private static readonly Hashtable _commandList = new Hashtable();
        public delegate void CommandAction(string[] arguments);

        public static void AddCommand(string keyword, CommandAction action)
        {
            if (_commandList.ContainsKey(keyword))
                return;

            _commandList.Add(keyword, action);
        }

        public static void RemoveCommand(string keyword)
        {
            _commandList.Remove(keyword);
        }

        public static CommandAction GetCommandAction(string keyword)
        {
            return (CommandAction)_commandList[keyword];
        }

        public static void HandleArguments(string[] arguments)
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
