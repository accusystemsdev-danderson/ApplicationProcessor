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
        public string LogFileName { get; set; }
        public string LogFilePath { get; set; }
        private StreamWriter logWriter;

        public bool OpenLog()
        {
            bool success = false;
            try
            {
                if (LogFilePath.Substring(LogFilePath.Length - 1) != "\\")
                    LogFilePath = LogFilePath + "\\";

                if (!Directory.Exists(LogFilePath))
                {
                    Directory.CreateDirectory(LogFilePath);
                }

                logWriter = new StreamWriter(LogFilePath + LogFileName);
                success = true;
            }
            catch (Exception e)
            { 
                Console.WriteLine(e);
                Console.WriteLine();
            }
            return success;
        }

        public void RemovePreviousLogFiles(int RemovePreviousDays)
        {
            if (RemovePreviousDays > 0)
            {
                try
                {
                    string[] files = Directory.GetFiles(LogFilePath);
                    foreach (string file in files)
                    {
                        FileInfo info = new FileInfo(file);
                        if (info.CreationTime < DateTime.Now.AddDays(-RemovePreviousDays))
                        {
                            info.Delete();
                        }
                    }
                }
                catch (Exception e)
                {
                    LogMessage(e.ToString());
                    LogMessage();
                    LogMessage("Unable to remove previous log files");
                }
            }
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

        public void LogMessage()
        {
            logWriter.WriteLine();
            Console.WriteLine();
        }

        public void LogAllProperties(object objectToLog)
        {
            try
            {
                LogMessage();
                LogMessage(string.Format("Reading all properties of the {0} object", objectToLog.ToString()));
                LogMessage();

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

                LogMessage();
                LogMessage("Finished reading properties");
                LogMessage();
            }
            catch (Exception e)
            {
                LogMessage(e.ToString());
                LogMessage();
                LogMessage("Unable to read all object properties");
            }
        }

        public void CloseLog()
        {
            logWriter.Close();
        }
        
    }
}
