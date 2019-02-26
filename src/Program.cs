using System;

namespace YeBo
{
	class Program
	{
		private static string configPath = "config.json";
		
		public static void Main(string[] args)
		{
			Config.Load(configPath);
			GameLoop.Initialize();
			Console.ReadKey();
		}
	}
}