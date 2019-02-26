using System;
using System.IO;
using Newtonsoft.Json;

namespace YeBo
{
	public class Config
	{
		public static Config Instance;
		public static bool Loaded;
		
		public Settings Settings;
		public Log Log = new Log("Config");
		
		public Config(string path)
		{
			// Instantiate the logger.
			// GetType().Name gets the name of the class, in this case, Config.
			Log = new Log(GetType().Name);
			// Attempt to load config.
			try {
				// Log that we are loading the config.
				Log.Debug(String.Format("Loading config from {0}...", path));
				// Read all text from the config file.
				var cfg = File.ReadAllText(path);
				// Deserialize the config file to a Settings object, and set the instance's Settings
				// to the deserialized object.
				Settings = JsonConvert.DeserializeObject<Settings>(cfg);
			}
			catch {
				// If we reach this code, the config couldn't be found in the base directory.
				Log.Debug("Config could not be found, loading default values.");
				// Create a config file with the default settings.
				SetDefaultConfig(path);
				// Stop here.
				return;
			}
			
			// Log the loaded status.
			Log.Debug("Config loaded successfully.");
			// Show that the config has been loaded.
			Loaded = true;
		}
		
		public void SetDefaultConfig(string path)
		{
			// Serialize an object using a default Settings object.
			var cfg = JsonConvert.SerializeObject(new Settings(), Formatting.Indented);
			// If we don't use a Stream, we will get a file access error.
			using (var stream = File.Create(path))
				// Dispose of the stream. It should be disposed after this using instance anyway, but why not?
				stream.Dispose();
			// Write to the newly created config file.
			File.WriteAllText(path, cfg);
			// Log the fact that the default config has been created.
			Log.Debug("A default config has been created.");
		}
		
		public static void Load(string path)
		{
			// Set the instance to a new config.
			Instance = new Config(path);
		}
	}
	
	public class Settings
	{
		// Debug mode enables a bunch of debuggy stuff..
		public bool Debug = false;
	}
}
