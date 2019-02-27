using System;
using System.Collections.Generic;
using YeBo.Commands;

namespace YeBo
{
	public class CommandHandler
	{
		public static CommandHandler Instance;
		public static Log Log;
		
		private readonly Dictionary<string, Command> cmds;
		
		public CommandHandler()
		{
            cmds = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);
            Type t = typeof (Command);
            foreach (Type i in t.Assembly.GetTypes())
                if (t.IsAssignableFrom(i) && i != t)
                {
                    Command instance = (Command) Activator.CreateInstance(i);
                    cmds.Add(instance.Name, instance);
                }
            Log = new Log("CommandHandler");
		}
		
		public static void Initialize()
		{
			Instance = new CommandHandler();
		}
		
		public bool ProcessCommand(string command)
		{
			int index = command.IndexOf(' ');
            string cmd = command.Substring(0, index == -1 ? command.Length : index);
            string args = index == -1 ? "" : command.Substring(index + 1);

            Command commandClass;
            if (!cmds.TryGetValue(cmd, out commandClass))
            {
            	Log.Warning("Undefined command '" + cmd + "'!");
                return false;
            }
            return commandClass.Execute(args);
		}
	}
}
