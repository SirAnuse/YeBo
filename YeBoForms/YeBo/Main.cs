using System;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace YeBo
{
    /* Current startup process:
     * Program runs -> loads config -> initializes Loop
     * 
     * Since nothing has been implemented yet, after this happens, the program closes after
     * a ReadKey function.
     */

    public class Main
	{
        public static ConcurrentQueue<string> QueuedCommands;
    	private static Log Log;
		private static string configPath = "config.json";

        public static void SetOutputBox(RichTextBox outputBox)
        {
            Log.SetOutputBox(outputBox);
        }

        public static void SetOutputForm(Form outputForm)
        {
            Log.SetOutputForm(outputForm);
        }

        public static void StopThreads()
        {
            foreach (var i in Loop.Loops)
                i.Stop();
        }

        public static void Initialize()
		{
            QueuedCommands = new ConcurrentQueue<string>();
            Log.CreateLogInstances();
            Config.Load(configPath);
            Loop.Setup();
            new GameLoop();
            new MasterLoop();
            Loop.InitializeLoops();
            CommandHandler.Initialize();
            Log = new Log("Program");
        }
	}
}