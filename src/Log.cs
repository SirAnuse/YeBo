using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Console = Colorful.Console;

namespace YeBo
{
    /* Logging format is as follows:
     * [HH:mm:ss dd/MM/yyyy] [logging level] [logger name] Message
     * 
     * Example: It is 4:46:22PM, on the 26th of February, 2019. The logger name is Config and logging level is Debug.
     * Example Message: The config has been loaded.
     * Output: [16:46:22 26/02/2019] [Debug] [Config] The config has been loaded.
     * 
     * Function documentation: 
     * - Constructor (Inputs: string): set logger name.
     * - LogLevelToColor (Inputs: Log.Level): return a colour based on the 
     * - Print (Inputs: Log.Level, string): log with a specified log level. No real use, just use the other methods.
     * - Info (Inputs: string): log a message with info priority in grey, used for low level logging
     * - Debug (Inputs: string): log a debug message in white, giving it slightly higher priority. Used for debugging.
     * - Warning (Inputs: string): log a warning message in yellow, giving it 2nd highest priority. Shown
     *   when a warning is incurred (something that shouldn't happen, but isn't critical)
     * - Error (Inputs: string): log an error in red, giving it highest priority. Only shown if an error occurs. Self explanatory.
     * 
     * Enum documentation:
     * - Level: the level of a logging entry, e.g. Warning or Error.
     * - Mode: the logging mode, e.g. SingleFile, ConsoleOnly.
     *  > SingleFile: Only log to a single log file which is simply added to.
     *  > NewFile: Create a new log file every time the program is started.
     *  > ConsoleOnly: Do not print to a log file.
     */

    public class Log
	{
        public static Level DisplayLevel { get; private set; }
        public static Mode LogMode { get; private set; }
        public static string LogFile;
        private static List<Log> LogInstances;

        private List<string> queuedLogs;
        private string logName;

        public Log(string loggerName)
		{
			logName = loggerName;
            queuedLogs = new List<string>();
            LogInstances.Add(this);
		}

        // Enumerate through all logs and tick them
        public static void TickLogs()
        {
            foreach (var log in LogInstances)
                log.Tick();
        }

        // Tick any updates logs may need
        // For now, this is only to update queued log entries.
        public void Tick()
        {
            if (queuedLogs.Count <= 0)
                return;

            File.AppendAllLines(LogFile, queuedLogs);
            queuedLogs.Clear();
        }

        // Log at a specified level.
        public void Print(Level logLevel, string message)
        {
            var date = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            var log = string.Format("[{0}] [{2}] [{3}] {1}", date, message, logLevel, logName);
            Console.WriteLine(log, LogLevelToColor(logLevel));

            switch (LogMode)
            {
                case Mode.SingleFile:
                case Mode.NewFile:
                    if (LogFile == null)
                        queuedLogs.Add(log);
                    else
                        File.AppendAllText(LogFile, log);
                    break;
                case Mode.ConsoleOnly:
                    break;
            }
        }

        public Color LogLevelToColor(Level logLevel)
        {
            Color ret = Color.LightGray;
            switch (logLevel)
            {
                case Level.Info:
                    ret = Color.LightGray;
                    break;
                case Level.Debug:
                    ret = Color.White;
                    break;
                case Level.Warning:
                    ret = Color.Yellow;
                    break;
                case Level.Error:
                    ret = Color.Red;
                    break;
                default:
                    break;
            }
            return ret;
        }

        // Log in grey to indicate information.
        public void Info(string message)
		{
            Print(Level.Info, message);
		}

        // Log in white to indicate debug.
        public void Debug(string message)
		{
            Print(Level.Debug, message);
		}
		
        // Log in yellow to indicate a warning.
		public void Warning(string message)
		{
            Print(Level.Warning, message);
		}
		
        // Log in red to indicate an error.
		public void Error(string message)
		{
            Print(Level.Error, message);
		}

        public static void Setup()
        {
            SetLogMode((Mode)Config.Settings.LoggingMode);
            SetDisplayLevel((Level)Config.Settings.LoggingLevel);
            switch (LogMode)
            {
                case Mode.SingleFile:
                    LoadLogFile();
                    break;
                case Mode.NewFile:
                    GenerateLogFile();
                    break;
                case Mode.ConsoleOnly:
                    break;
            }
        }

        public static void SetLogMode(Mode LogMode)
        {
            Log.LogMode = LogMode;
        }

        public static void SetDisplayLevel(Level DisplayLevel)
        {
            Log.DisplayLevel = DisplayLevel;
        }

        public static void CreateLogInstances()
        {
            LogInstances = new List<Log>();
        }

        public static void LoadLogFile()
        {
            
            string logFileName = string.Format("log.txt", DateTime.Now);
            string logFilePath = Config.Settings.LogPath;
            string logLocation = logFilePath + logFileName;
 
            if (!File.Exists(logLocation))
            {
                DirectoryInfo di = Directory.CreateDirectory(logFilePath);
                using (var stream = File.Create(logLocation))
                    stream.Dispose();
            }

            LogFile = logLocation;
        }

        public static void GenerateLogFile()
        {
            string logFileName = string.Format("{0:yyyy-MM-ddTHH-mm}-log.txt", DateTime.Now);
            string logFilePath = Config.Settings.LogPath;
            string logLocation = logFilePath + logFileName;

            // Almost 100% certain but whatever
            if (!File.Exists(logLocation))
            {
                DirectoryInfo di = Directory.CreateDirectory(logFilePath);
                using (var stream = File.Create(logLocation))
                    stream.Dispose();
            }

            LogFile = logLocation;
        }


        public enum Level
        {
            Info,
            Debug,
            Warning,
            Error
        }

        public enum Mode
        {
            SingleFile,
            NewFile,
            ConsoleOnly
        }
    }
    
}
