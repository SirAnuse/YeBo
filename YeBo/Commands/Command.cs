using System;

namespace YeBo.Commands
{
	public abstract class Command
	{
		public string Name { get; private set; }
		public string[] Aliases { get; private set; }
		
		public Command(string name, params string[] aliases)
		{
			Name = name;
			Aliases = aliases;
		}
		
		protected abstract bool Process(string[] args);
		
		public bool Execute(string args)
        {
            try
            {
                string[] a = args.Split(' ');
                return Process(a);
            }
            catch (Exception ex)
            {
                CommandHandler.Log.Error("Error when executing the command: " + ex);
                return false;
            }
        }
	}
}
