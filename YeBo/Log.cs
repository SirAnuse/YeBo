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
        public Level LogLevel { get; set; }
    }

    public class MasterLog
    {
    	public Level DisplayLevel { get; private set; }
        public Mode LogMode { get; private set; }
        public string LogFile { get; private set; }
        public RichTextBox OutputBox { get; private set; }
        public Form OutputForm { get; private set; }
        
        private readonly List<Log> LogInstances;
        
        public MasterLog(Level displayLevel, Mode logMode, string logFolder,
                        RichTextBox outputBox, Form outputForm)
        {
        	LogInstances = new List<Log>();
        	DisplayLevel = displayLevel;
        	LogMode = logMode;
        	logFolder = SetupLogFile(logMode, logFolder);
        }
        
        // Enumerate through all logs and tick them
        public void TickLogs()
        {
            foreach (var log in LogInstances)
                log.Tick();
        }
        
        public void AddInstance(Log log)
        {
        	LogInstances.Add(log);
        }
        
        public Log GetLogger(string name)
        {
        	return new Log(name, this);
        }
        
        public Log GetLogger(Type type)
        {
        	return new Log(type.Name, this);
        }
        
        public string SetupLogFile(Mode logMode, string logFolder)
        {
        	switch (logMode)
        	{
        		case Mode.NewFile:
        			return GenerateLogFile(logFolder);
        		case Mode.SingleFile:
        			return LoadLogFile(logFolder);
        		default:
        			return string.Empty;
        	}
        }
        
        public string LoadLogFile(string logFolder)
        {
            string logFileName = string.Format("log.txt", DateTime.Now);
            string logLocation = logFolder + logFileName;
 
            if (!File.Exists(logLocation))
            {
                DirectoryInfo di = Directory.CreateDirectory(logFolder);
                using (var stream = File.Create(logLocation))
                    stream.Dispose();
            }

            return logLocation;
        }

        public string GenerateLogFile(string logFolder)
        {
            string logFileName = string.Format("{0:yyyy-MM-ddTHH-mm}-log.txt", DateTime.Now);
            string logLocation = logFolder + logFileName;

            // Almost 100% certain but whatever
            if (!File.Exists(logLocation))
            {
                DirectoryInfo di = Directory.CreateDirectory(logFolder);
                using (var stream = File.Create(logLocation))
                    stream.Dispose();
            }

            return logLocation;
        }
    }
    
    public class Log
	{
        public readonly List<string> QueuedLogs;
        public readonly List<QueuedLogEntry> QueuedOutputs;
        public readonly string LogName;
        public readonly MasterLog MasterLog;

        public Log(string loggerName, MasterLog masterLog)
		{
			LogName = loggerName;
            QueuedLogs = new List<string>();
            QueuedOutputs = new List<QueuedLogEntry>();
            MasterLog = masterLog;
            MasterLog.AddInstance(this);
		}

        // Tick any updates logs may need
        // For now, this is only to update queued log entries.
        public void Tick()
        {
        	var m = MasterLog;
            if (QueuedLogs.Count > 0)
            {
                File.AppendAllLines(m.LogFile, QueuedLogs);
                QueuedLogs.Clear();
            }
            if (QueuedOutputs.Count > 0
                && m.OutputForm.IsHandleCreated)
            {
                foreach (var log in QueuedOutputs)
                {
                    // Mark log messages as it may append them multiple times.
                    if (log.Marked)
                        continue;

                    log.Marked = true;

                    // Use invoke as we can't use cross-thread operations
                    m.OutputForm.Invoke(new MethodInvoker(delegate
                    {
                        m.OutputBox.AppendText(log.Content, GetColorFromLogLevel(log.LogLevel));
                    }));
                }

                QueuedOutputs.Clear();
            }
        }

        // Log at a specified level.
        public void Print(Level logLevel, string message)
        {
        	var m = MasterLog;
        	
            var log = message;
            var date = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            log = string.Format("[{0}] [{2}] [{3}] {1}", date, message, logLevel, LogName);

            if (m.OutputForm.IsHandleCreated)
                m.OutputForm.Invoke(new MethodInvoker(delegate {
                    m.OutputBox.AppendText(log, GetColorFromLogLevel(logLevel));
                }));
            else
                QueuedOutputs.Add(new QueuedLogEntry
                {
                    Content = log,
                    LogLevel = logLevel,
                    Marked = false
                });
            
            switch (m.LogMode)
            {
                case Mode.SingleFile:
                case Mode.NewFile:
                    if (m.LogFile == null)
                        QueuedLogs.Add(log);
                    else
                        File.AppendAllText(m.LogFile, log + Environment.NewLine);
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
