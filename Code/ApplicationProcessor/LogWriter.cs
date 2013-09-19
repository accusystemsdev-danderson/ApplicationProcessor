//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ApplicationProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides file based logging
    /// </summary>
    class LogWriter : IDisposable
    {
        private StreamWriter logWriter;
        
        public string LogFileName { get; set; }
        public string LogFilePath { get; set; }
        
        /// <summary>
        /// Creates log directory and log file.  Opens log file for writing
        /// </summary>
        /// <returns>True upon successfull creation and opening of log file</returns>
        public bool OpenLog()
        {
            try
            {
                if (LogFilePath.Substring(LogFilePath.Length - 1) != "\\")
                    LogFilePath = LogFilePath + "\\";

                if (!Directory.Exists(LogFilePath))
                {
                    Directory.CreateDirectory(LogFilePath);
                }

                logWriter = new StreamWriter(LogFilePath + LogFileName);
                return true;
            }
            catch (Exception e)
            { 
                Console.WriteLine(e);
                Console.WriteLine();
                return false;
            }
        }

        /// <summary>
        /// Removes all log files in the LogFilePath folder older than the number of days specified
        /// </summary>
        /// <param name="RemovePreviousDays">Number of days to keep log files</param>
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

        /// <summary>
        /// Writes a string message and a time stamp to the current log file
        /// </summary>
        /// <param name="msg">The string message to log</param>
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

        /// <summary>
        /// Writes a blank line to the log file
        /// </summary>
        public void LogMessage()
        {
            logWriter.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Reads all properties of a given object and writes them to the log file
        /// </summary>
        /// <param name="objectToLog">The object to read properties from</param>
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

        /// <summary>
        /// Closes the log file
        /// </summary>
        public void CloseLog()
        {
            logWriter.Close();
        }

        /// <summary>
        /// Implements the IDisposable method.
        /// </summary>
        public void Dispose()
        {
            CloseLog();
        }
    }
}
