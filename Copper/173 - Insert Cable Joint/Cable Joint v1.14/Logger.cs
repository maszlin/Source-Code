using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

/// <summary>
/// This Class logs messages to the file %TEMP%\InitRoute.log only if it exists.
/// OBS: This class is a singleton.
/// </summary>
public class Logger
{
    StreamWriter logFile;
    private bool fileExists;
    private bool fileOpen;
    private static Logger loggerInstance;

    /// <summary>
    /// Gets an instance of the class Logger.
    /// </summary>
    /// <returns>An instance of the class Logger.</returns>
    public static Logger getInstance()
    {
        if (loggerInstance == null)
        {
            loggerInstance = new Logger();
        }
        /*
         * Sometimes it is necessary to reopen the log file, because when this custom command is closed the objects that 
         * were created when this it was running are not destroyed. This means that since this class is a
         * singleton the same instance of Logger that was used in the last time this custom command was ran will be used
         * the next time, but with the log file closed (the log file is closed on NonGraphic.ExitCmd()) thus generating
         * an error.
         */
        //else if (!loggerInstance.IsFileOpen())
        //    loggerInstance.OpenFile();

        return loggerInstance;
    }

    private Logger()
    {
        //OpenFile();
    }

    /// <summary>
    /// Opens the log file for writing if it exists.
    /// </summary>
    public void OpenFile(string title)
    {
        try
        {
            string tempDir = System.Environment.GetEnvironmentVariable("TEMP");
            DeleteLog(tempDir, title);
            logFile = File.CreateText(tempDir + "\\" + title + " [" + DateTime.Now.ToString("yyMMdd HHmm") + "].log");
        }
        catch { }
    }

    /// <summary>
    /// Logs the content of the String message into the log file.
    /// </summary>
    /// <param name="message"></param>
    public void WriteLog(string message)
    {
        try
        {
            logFile.WriteLine(DateTime.Now.ToString("HH:mm ") + message);
            logFile.Flush();
        }
        catch { }
    }
    public void WriteErr(Exception ex)
    {
        try
        {
            logFile.WriteLine(DateTime.Now.ToString("HH:mm ERROR ") + ex.Source + ":" + ex.Message);
            logFile.Flush();
        }
        catch { }
    }

    /// <summary>
    /// Logs the input Strings in the following format: "[Namespace].[Class].[Method] - Message"
    /// </summary>
    public void WriteLog(string messageClassFullName, string methodName, string message)
    {
        if (fileExists && fileOpen)
        {
            logFile.WriteLine(messageClassFullName + "." + methodName + " - " + message);
            logFile.Flush();
        }
    }

    /// <summary>
    /// Logs the input Strings in the following format: "[ClassFullName] - Message"
    /// </summary>
    public void WriteLog(string messageClassFullName, string message)
    {
        if (fileExists && fileOpen)
        {
            logFile.WriteLine(messageClassFullName + " - " + message);
            logFile.Flush();
        }
    }

    /// <summary>
    /// Checks if the log file is open for writing. Returns true if the file is open or false otherwise.
    /// </summary>
    /// <returns>True if the file is open or false otherwise.</returns>
    public bool IsFileOpen()
    {
        return fileOpen;
    }

    /// <summary>
    /// Closes and flushes the log file. 
    /// </summary>
    public void CloseFile()
    {
        if (fileExists && fileOpen)
        {
            logFile.Flush();
            logFile.Close();
            logFile.Dispose();

            fileOpen = false;
        }
    }

    private void DeleteLog(string path, string title)
    {
        try
        {
            string[] files = Directory.GetFiles(path, title + "*.log");
            foreach (string f in files)
            {
                if (File.Exists(f)) File.Delete(f);
            }
        }
        catch { }
    }
}

