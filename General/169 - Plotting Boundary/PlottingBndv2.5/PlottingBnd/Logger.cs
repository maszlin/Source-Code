using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace AG.GTechnology.PlottingBnd
{
    public class Logger
    {
        private bool fileExists;
        private bool fileOpen;
        private StreamWriter logFile;
        private static Logger loggerInstance;

        private Logger()
        {
            this.OpenFile();
        }

        public void CloseFile()
        {
            if (this.fileExists && this.fileOpen)
            {
                this.logFile.Flush();
                this.logFile.Close();
                this.logFile.Dispose();
                this.fileOpen = false;
            }
        }

        public static Logger getInstance()
        {
            if (loggerInstance == null)
            {
                loggerInstance = new Logger();
            }
            else if (!loggerInstance.IsFileOpen())
            {
                loggerInstance.OpenFile();
            }
            return loggerInstance;
        }

        public bool IsFileOpen()
        {
            return this.fileOpen;
        }

        private void OpenFile()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("TEMP");
            this.fileExists = File.Exists(environmentVariable + @"\PlottingBnd.log");
            if (this.fileExists)
            {
                this.logFile = File.CreateText(environmentVariable + @"\PlottingBnd.log");
                this.fileOpen = true;
            }
            else
            {
                this.logFile = File.CreateText(environmentVariable + @"\PlottingBnd.log");
                this.fileOpen = true;
                //this.fileOpen = false;
            }
        }

        public void WriteLog(string message)
        {
            if (this.fileExists && this.fileOpen)
            {
                this.logFile.WriteLine(message);
                this.logFile.Flush();
            }
        }

        public void WriteLog(string messageClassFullName, string message)
        {
            if (this.fileExists && this.fileOpen)
            {
                this.logFile.WriteLine(messageClassFullName + " - " + message);
                this.logFile.Flush();
            }
        }

        public void WriteLog(string messageClassFullName, string methodName, string message)
        {
            if (this.fileExists && this.fileOpen)
            {
                this.logFile.WriteLine(messageClassFullName + "." + methodName + " - " + message);
                this.logFile.Flush();
            }
        }
    }
}
