using System;
using System.IO;
using Newtonsoft.Json;

namespace YeBo
{
    /* Function documentation:
     * - Constructor (Inputs: string): load the config from the specified path.
     * - SetDefaultConfig (Inputs: string): creates a default config file at the specified path.
     * - Load (Inputs: string): instantiate a new config from the specified path.
     */

	public static class Config
	{
		public static bool Loaded;
		
		public static Settings Settings;
        public static Log Log;
		
		public static void SetDefaultConfig(string path)
		{
			var cfg = JsonConvert.SerializeObject(new Settings(), Formatting.Indented);
			using (var stream = File.Create(path))
				stream.Dispose();
			File.WriteAllText(path, cfg);
			Log.Debug("A default config has been created.");
		}
		
		public static void Load(string path)
		{
            
            Log = new Log("Config");
            try
            {
                Log.Debug(string.Format("Loading config from {0}...", path));
                var cfg = File.ReadAllText(path);
                Settings = JsonConvert.DeserializeObject<Settings>(cfg);
            }
            catch
            {
                Log.Debug("Config could not be found, creating a default config file.");
                SetDefaultConfig(path);
                return;
            }
            Log.Debug("Config loaded successfully.");
            Log.Setup();
            Loaded = true;
        }
	}
	
	public class Settings
	{
		// Debug mode enables a bunch of debuggy stuff.
		public bool Debug = false;

        // Display all logging above this level.
        // 0 = Info+
        // 1 = Debug+
        // 2 = Warnings+
        // 3 = Errors only
        // 4 = No logging
        public int LoggingLevel = 0;

        // 0 = Write to the same file every session, just add on to it
        // 1 = Write to a new file every session
        // 2 = Do not write to a file
        public int LoggingMode = 0;

        // Path to store logs.
        public string LogPath = "logs/";

        // Ticks per second.
        public int TPS = 5;
	}
}
