using System;

namespace YeBo.Commands
{
	public abstract class Command
	{
		public string Name { get; private set; }
		
		public Command(string name)
		{
			Name = name;
		}
		
		protected abstract bool Process(string[] args);
		
		public bool Execute(string args)
        {
            try
            {
                string[] a = args.Split(' ');
                return Process(a);
            }
            catch
            {
                CommandHandler.Log.Error("Error when executing the command.");
                return false;
            }
        }
	}
}
