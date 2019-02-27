using System;

namespace YeBo.Commands
{
	internal class TestCommand : Command
	{
		public TestCommand() : base("test")
		{
			
		}
		
		protected override bool Process(string[] args)
		{
            CommandHandler.Log.Debug("test");
			return true;
		}
	}
}
