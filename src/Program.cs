using System;

namespace YeBo
{
    /* Current startup process:
     * Program runs -> loads config -> initializes Loop
     * 
     * Since nothing has been implemented yet, after this happens, the program closes after
     * a ReadKey function.
     */

    class Program
	{
		private static string configPath = "config.json";
		
		public static void Main(string[] args)
		{
            Log.CreateLogInstances();
            Config.Load(configPath);
            Loop.Initialize();
			Console.ReadKey();
		}
	}
}