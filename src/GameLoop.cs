using System;

namespace YeBo
{

	public class GameLoop
	{
		public static GameLoop Instance;
		public static bool Initialized;
		public Log Log;
		
		public DateTime start;
		
		public GameLoop()
		{
			// Instantiate the logger.
			// GetType().Name gets the name of the class, in this case, GameLoop.
			Log = new Log(GetType().Name);
			// Show that the GameLoop is being initalized.
			Log.Debug("Initializing GameLoop.");
			// Set the starting Date & Time (when the GameLoop started).
			start = DateTime.Now;
			// Show that the GameLoop has been initialized.
			Initialized = true;
			// Log the fact that the GameLoop has been initalized.
			Log.Debug("GameLoop has been initalized.");
		}
		
		public static void Initialize()
		{
			Instance = new GameLoop();
			Instance.StartLoop();
		}
		
		public void StartLoop()
		{
			Log.Debug("Starting GameLoop.");
		}
	}
}
