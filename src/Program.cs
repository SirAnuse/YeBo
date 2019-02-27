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
    	private static Log Log;
		private static string configPath = "config.json";
		
		public static void Main(string[] args)
		{
            
            Initialize();
            Log = new Log("Program");
			Console.ReadKey();
			CommandHandler.Instance.ProcessCommand("test");
			Log.Info("Press any key to exit.");
			Console.ReadKey();
		}
		
		private static void Initialize()
		{
			Log.CreateLogInstances();
			Config.Load(configPath);
			Loop.Setup();
			GameLoop.Instance = new GameLoop();
			MasterLoop.Instance = new MasterLoop();
			Loop.InitializeLoops();
			CommandHandler.Initialize();
		}
	}
}