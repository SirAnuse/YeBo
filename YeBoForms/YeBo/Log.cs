using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

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

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color, Font font, bool addNewLine = true)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.SelectionFont = font;
            box.AppendText(addNewLine
            ? text + Environment.NewLine
            : text);
            box.SelectionColor = box.ForeColor;
        }

        public static void AppendText(this RichTextBox box, string text, Color color, bool addNewLine = true)
        {
            try
            {
                box.SelectionStart = box.TextLength;
                box.SelectionLength = 0;

                box.SelectionColor = color;
                box.SelectionFont = box.Font;
                if (box.Text == string.Empty)
                    box.AppendText(text);
                else
                    box.AppendText(addNewLine
                    ? Environment.NewLine + text
                    : text);
                box.SelectionColor = box.ForeColor;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error outputting to log:\n"+ex, "Error");
            }
        }
    }

    public class QueuedLogEntry
    {
        public bool Marked { get; set; }
        public string Content { get; set; }
        public Log.Level LogLevel { get; set; }
    }

    public class Log
	{
        public static Level DisplayLevel { get; private set; }
        public static Mode LogMode { get; private set; }
        public static string LogFile;
        public static RichTextBox OutputBox;
        public static Form OutputForm;
        private static List<Log> LogInstances;

        private List<string> queuedLogs;
        private List<QueuedLogEntry> queuedOutputs;
        private string logName;

        public Log(string loggerName)
		{
			logName = loggerName;
            queuedLogs = new List<string>();
            queuedOutputs = new List<QueuedLogEntry>();
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
            if (queuedLogs.Count > 0)
            {
                File.AppendAllLines(LogFile, queuedLogs);
                queuedLogs.Clear();
            }
            if (queuedOutputs.Count > 0
                && OutputForm.IsHandleCreated)
            {
                foreach (var log in queuedOutputs)
                {
                    // Mark log messages as it may append them multiple times.
                    if (log.Marked)
                        continue;

                    log.Marked = true;

                    // Use invoke as we can't use cross-thread operations
                    OutputForm.Invoke(new MethodInvoker(delegate
                    {
                        OutputBox.AppendText(log.Content, GetColorFromLogLevel(log.LogLevel));
                    }));
                }

                queuedOutputs.Clear();
            }
        }

        // Log at a specified level.
        public void Print(Level logLevel, string message, bool logFormat = true)
        {
            var log = message;
            var date = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            log = string.Format("[{0}] [{2}] [{3}] {1}", date, message, logLevel, logName);

            if (OutputForm.IsHandleCreated)
                OutputForm.Invoke(new MethodInvoker(delegate {
                    OutputBox.AppendText(log, GetColorFromLogLevel(logLevel));
                }));
            else
                queuedOutputs.Add(new QueuedLogEntry
                {
                    Content = log,
                    LogLevel = logLevel,
                    Marked = false
                });
            
            switch (LogMode)
            {
                case Mode.SingleFile:
                case Mode.NewFile:
                    if (LogFile == null)
                        queuedLogs.Add(log);
                    else
                        File.AppendAllText(LogFile, log + Environment.NewLine);
                    break;
                case Mode.ConsoleOnly:
                    break;
            }
        }

        public Color GetColorFromLogLevel(Level logLevel)
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

        public static void SetOutputBox(RichTextBox outputBox)
        {
            OutputBox = outputBox;
        }

        public static void SetOutputForm(Form outputForm)
        {
            OutputForm = outputForm;
        }

        public static void SetLogMode(Mode logMode)
        {
            LogMode = LogMode;
        }

        public static void SetDisplayLevel(Level displayLevel)
        {
            DisplayLevel = DisplayLevel;
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
