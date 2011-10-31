using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics; // EventLog
using System.Net; // Dns
using System.Threading;

// using System.Windows.Forms;

namespace finantic.OQPlugins
{
    public enum LogDestination { File, WinEvent };

    public enum LoggerLevel
    {
        None = 0,
        /// <summary>least detailed</summary>
        System = 1,
        /// <summary>default, if no level is specified</summary>
        Error,
        /// <summary>warning</summary>
        Warning,
        /// <summary>information</summary>
        Information,
        /// <summary>most detailed <note>Setting the log level to Detail=5 will have a 
        /// performance overhead, and should only be used when trying to resolve an 
        /// issue</note></summary>
        Detail
    };

    /// <summary>
    /// Logger is used for creating a customized error log files or an error can be registered as
    /// a log entry in the Windows Event Log on the administrator's machine.
    /// </summary>
    public class Logger
    {
        #region private variables
        string strLogFilePath = string.Empty;
        LogDestination m_logDest = LogDestination.File;
        string m_eventLogName = "Logger"; // Name in WinEvent List
        string m_source; // Name of event source
        LoggerLevel m_logLevel = LoggerLevel.System;

        private static StreamWriter sw = null;
        #endregion private variables

        #region properties
        /// <summary>
        /// Setting LogFile path. If the logfile path is null then it will update error info into LogFile.txt under
        /// application directory.
        /// </summary>
        public string LogFilePath
        {
            set { strLogFilePath = value; }
            get { return strLogFilePath; }
        }

        public LogDestination LogDestination
        {
            set { m_logDest = value; }
            get { return m_logDest; }
        }

        public LoggerLevel LoggerLevel
        {
            set { m_logLevel = value; }
            get { return m_logLevel; }
        }

        public string Source
        {
            get { return m_source; }
            set { m_source = value; }
        }

        #endregion properties

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public Logger(string source, string Name)
        {
            //MessageBox.Show("Start Logger " + source + ", Name: " + Name);
            m_eventLogName = Name;
            m_source = source;
        }

        public void AddLog(LoggerLevel level, string message)
        {
            AddLog(level, message, 0);
        }

        public void AddLog(LoggerLevel level, string message, int code)
        {
            // check Log level
            if (level > this.m_logLevel) return; // ignore

            //Write to Windows event log
            if (m_logDest == LogDestination.WinEvent)
            {
                if (!EventLog.SourceExists(m_source))
                    EventLog.CreateEventSource(m_source, m_eventLogName);

                // Inserting into event log
                EventLog Log = new EventLog();
                Log.Source = m_source;
                EventLogEntryType type = EventLogEntryType.Error;
                short category = (short)level;
                int id = code;
                switch (level)
                {
                    case LoggerLevel.System:
                        type = EventLogEntryType.Error;
                        break;
                    case LoggerLevel.Error:
                        type = EventLogEntryType.Error;
                        break;
                    case LoggerLevel.Warning:
                        type = EventLogEntryType.Warning;
                        break;
                    case LoggerLevel.Information:
                        type = EventLogEntryType.Information;
                        break;
                    case LoggerLevel.Detail:
                        type = EventLogEntryType.Information;
                        break;
                }
                Log.WriteEntry(message, type, id, category);
            }
            else //Custom text-based event log
            { 
                AddLogFile(level, message, code);
            }
        }

        /// <summary>
        /// Write error log entry for window event if the bLogType is true. Otherwise, write the log entry to
        /// customized text-based text file
        /// </summary>
        /// <param name="bLogType"></param>
        /// <param name="objException"></param>
        /// <returns>false if the problem persists</returns>
        public bool ErrorRoutine(Exception objException)
        {
            try
            {
                //Write to Windows event log
                if (m_logDest == LogDestination.WinEvent)
                {
                    if (!EventLog.SourceExists(m_eventLogName))
                        EventLog.CreateEventSource(objException.Message, m_eventLogName);

                    // Inserting into event log
                    EventLog Log = new EventLog();
                    Log.Source = m_eventLogName;
                    Log.WriteEntry(objException.Message, EventLogEntryType.Error);
                }
                //Custom text-based event log
                else
                {
                    if (true != CustomErrorRoutine(objException))
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void AddLogFile(LoggerLevel level, string message, int code)
        {
            if (strLogFilePath == null || strLogFilePath.Length == 0)
            {
                throw new ApplicationException("no log file path specified");
            }
            //If the log file path is not empty but the file is not available it will create it
            if (false == File.Exists(strLogFilePath))
            {
                if (false == CheckDirectory(strLogFilePath))
                {
                    throw new ApplicationException("problems with log file path");
                }
                FileStream fs = new FileStream(strLogFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Close();
            }
            Exception last_exception = null;
            StreamWriter sw = null;
            for (int timeout = 0; timeout < 30; timeout++)
            {
                try
                {
                    sw = new StreamWriter(strLogFilePath, true);
                    if (sw != null) break;
                }
                catch (Exception ex)
                {
                    last_exception = ex;
                    sw = null;
                }
                Thread.Sleep(100);
            }
            if (sw == null)
            {
                // MessageBox.Show("can't open log file: " + last_exception.Message);
                throw new Exception("can't open log file: " + last_exception.Message);
            }


            StringBuilder line = new StringBuilder();
            line.Append(DateTime.Now.ToString("u"));
            line.Append(" ");
            line.Append(m_source);
            line.Append(" ");
            line.Append(level.ToString());
            line.Append(" ");
            line.Append(message);
            if (code != 0)
            {
                line.Append(" (code=");
                line.Append(code.ToString());
                line.Append(")");
            }
 
            sw.WriteLine(line.ToString());

            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// If the LogFile path is empty then, it will write the log entry to LogFile.txt under application directory.
        /// If the LogFile.txt is not availble it will create it
        /// If the Log File path is not empty but the file is not availble it will create it.
        /// </summary>
        /// <param name="objException"></param>
        /// <returns>false if the problem persists</returns>
        private bool CustomErrorRoutine(Exception objException)
        {
            string strPathName = string.Empty;
            if (strLogFilePath.Equals(string.Empty))
            {
                //Get Default log file path "LogFile.txt"
                strPathName = GetLogFilePath();
            }
            else
            {
                //If the log file path is not empty but the file is not available it will create it
                if (false == File.Exists(strLogFilePath))
                {
                    if (false == CheckDirectory(strLogFilePath))
                        return false;

                    FileStream fs = new FileStream(strLogFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Close();
                }
                strPathName = strLogFilePath;

            }

            bool bReturn = true;
            // write the error log to that text file
            if (true != WriteErrorLog(strPathName, objException))
            {
                bReturn = false;
            }
            return bReturn;
        }
        /// <summary>
        /// Write Source,method,date,time,computer,error and stack trace information to the text file
        /// </summary>
        /// <param name="strPathName"></param>
        /// <param name="objException"></param>
        /// <returns>false if the problem persists</returns>
        private static bool WriteErrorLog(string strPathName, Exception objException)
        {
            bool bReturn = false;
            string strException = string.Empty;
            try
            {
                sw = new StreamWriter(strPathName, true);
                sw.WriteLine("Source		: " + objException.Source.ToString().Trim());
                sw.WriteLine("Method		: " + objException.TargetSite.Name.ToString());
                sw.WriteLine("Date		: " + DateTime.Now.ToLongTimeString());
                sw.WriteLine("Time		: " + DateTime.Now.ToShortDateString());
                sw.WriteLine("Computer	: " + Dns.GetHostName().ToString());
                sw.WriteLine("Error		: " + objException.Message.ToString().Trim());
                sw.WriteLine("Stack Trace	: " + objException.StackTrace.ToString().Trim());
                sw.WriteLine("^^-------------------------------------------------------------------^^");
                sw.Flush();
                sw.Close();
                bReturn = true;
            }
            catch (Exception)
            {
                bReturn = false;
            }
            return bReturn;
        }


        /// <summary>
        /// Check the log file in applcation directory. If it is not available, creae it
        /// </summary>
        /// <returns>Log file path</returns>
        private static string GetLogFilePath()
        {
            try
            {
                // get the base directory
                string baseDir = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.RelativeSearchPath;

                // search the file below the current directory
                string retFilePath = baseDir + "//" + "LogFile.txt";

                // if exists, return the path
                if (File.Exists(retFilePath) == true)
                    return retFilePath;
                //create a text file
                else
                {
                    if (false == CheckDirectory(retFilePath))
                        return string.Empty;

                    FileStream fs = new FileStream(retFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Close();
                }

                return retFilePath;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Create a directory if not exists
        /// </summary>
        /// <param name="strLogPath"></param>
        /// <returns></returns>
        private static bool CheckDirectory(string strLogPath)
        {
            try
            {
                int nFindSlashPos = strLogPath.Trim().LastIndexOf("\\");
                string strDirectoryname = strLogPath.Trim().Substring(0, nFindSlashPos);

                if (false == Directory.Exists(strDirectoryname))
                    Directory.CreateDirectory(strDirectoryname);

                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }

        private static string GetApplicationPath()
        {
            try
            {
                string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory.ToString();
                int nFirstSlashPos = strBaseDirectory.LastIndexOf("\\");
                string strTemp = string.Empty;

                if (0 < nFirstSlashPos)
                    strTemp = strBaseDirectory.Substring(0, nFirstSlashPos);

                int nSecondSlashPos = strTemp.LastIndexOf("\\");
                string strTempAppPath = string.Empty;
                if (0 < nSecondSlashPos)
                    strTempAppPath = strTemp.Substring(0, nSecondSlashPos);

                string strAppPath = strTempAppPath.Replace("bin", "");
                return strAppPath;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
