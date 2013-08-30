using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace ApplicationProcessor
{
    class LogWriter
    {
        private StreamWriter logWriter;

        /// <summary>
        /// Creates a log file in the directory specified.  Also removes any "*.log" files in the given directory older the number of days specified.
        /// </summary>
        /// <param name="logFilePath">Path of log file</param>
        /// <param name="logFileName">File Name for log</param>
        /// <param name="RemovePreviousDays">Remove log files older than number of days.  Specify '0' to not remove log files.</param>
        public LogWriter(string logFilePath, string logFileName, int RemovePreviousDays)
        {
            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }
            if (RemovePreviousDays > 0)
            { 
                string[] files = Directory.GetFiles(logFilePath);
                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);
                    if (info.CreationTime < DateTime.Now.AddDays(-RemovePreviousDays))
                    {
                        info.Delete();
                    }
                }
            }
            logWriter = new StreamWriter(logFilePath + logFileName);
        }

        public void LogMessage(string msg)
        {
            if (msg != "")
            {
            logWriter.WriteLine(DateTime.Now + " " + msg);
            Console.WriteLine(msg);
            }
            else
            {
                logWriter.WriteLine();
                Console.WriteLine();
            }
            logWriter.Flush();
        }

        public void LogAllProperties(object objectToLog)
        {
            LogMessage("");
            LogMessage(string.Format("Reading all properties of the {0} object", objectToLog.ToString()));
            LogMessage("");

            foreach (PropertyInfo prop in objectToLog.GetType().GetProperties())
            {
                if (prop.GetValue(objectToLog, null) != null)
                {
                    LogMessage(string.Format("Property Read: {0} - Value: {1}", prop.Name, prop.GetValue(objectToLog, null).ToString()));
                }
                else
                { 
                    LogMessage(string.Format("Property {0} does not have a value set", prop.Name));
                }
            }

            LogMessage("");
            LogMessage("Finished reading properties");
            LogMessage("");
        }

        public void CloseLog()
        {
            logWriter.Close();
        }
        
    }
}
