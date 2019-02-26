using Humanizer;
using System;
using System.Drawing;
using Console = Colorful.Console;

namespace YeBo
{
	/// <summary>
	/// Description of Log.
	/// </summary>
	public class Log
	{
		private string logName = "NULL";
		public Log(string loggerName)
		{
			logName = loggerName;
		}
		
		public void Info(string message)
		{
			// Format the date to this format: 16:43:23 06/02/2019
			var date = String.Format("{0:HH:mm:ss dd/MM/yyyy}", DateTime.Now);
			var log = String.Format("[{0}] [{2}] [{3}] {1}", date, message, Level.Info, logName);
			Console.WriteLine(log);
		}
		
		public void Debug(string message)
		{
			
			// Don't do debug logging if we're not in debug mode, and check if the config is actually loaded.
			if (Config.Loaded && !Config.Instance.Settings.Debug)
				return;
			
			// Format the date to this format: 16:43:23 06/02/2019
			var date = String.Format("{0:HH:mm:ss dd/MM/yyyy}", DateTime.Now);
			var log = String.Format("[{0}] [{2}] [{3}] {1}", date, message, Level.Debug, logName);
			Console.WriteLine(log);
		}
		
		public void Warning(string message)
		{
			// Format the date to this format: 16:43:23 06/02/2019
			var date = String.Format("{0:HH:mm:ss dd/MM/yyyy}", DateTime.Now);
			var log = String.Format("[{0}] [{2}] [{3}] {1}", date, message, Level.Warning, logName);
			Console.WriteLine(log);
		}
		
		public void Error(string message)
		{
			// Format the date to this format: 16:43:23 06/02/2019
			var date = String.Format("{0:HH:mm:ss dd/MM/yyyy}", DateTime.Now);
			var log = String.Format("[{0}] [{2}] [{3}] {1}", date, message, Level.Error, logName);
			Console.WriteLine(log);
		}
		
		public enum Level
		{
			Info,
			Debug,
			Warning,
			Error
		}
	}
}
