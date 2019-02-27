using System;
using System.Linq;

namespace YeBo.Commands
{
	internal class TestCommand : Command
	{
		public TestCommand() : base("test")
		{
			
		}
		
		protected override bool Process(string[] args)
		{
            
			return true;
		}
	}
}
