using System;

namespace Ion.Core
{
    public class Logging
    {
        #region Fields
        private LogType mMinimumLogImportancy;
        #endregion

        #region Properties
        public LogType MinimumLogImportancy
        {
            get { return mMinimumLogImportancy; }
            set
            {
                if (value != mMinimumLogImportancy)
                {
                    WriteInformation("Changed log type to " + value + ".");
                }
                mMinimumLogImportancy = value;
            }
        }
        #endregion

        #region Methods
        private void WriteLineInternal(ref string sLine, LogType pLogType, bool ignoreLogType)
        {
            lock (this)
            {
                Console.Write("[{0}]", DateTime.Now.ToString());
                Console.Write(" -- ");

                if (pLogType == LogType.Information)
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                else if (pLogType == LogType.Warning)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else if (pLogType == LogType.Error)
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (pLogType == LogType.Debug)
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                else
                    Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine(sLine);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        public void WriteLine(string sLine)
        {
            WriteLineInternal(ref sLine, LogType.Information, true);
        }
        public void WriteInformation(string sLine)
        {
            if(mMinimumLogImportancy <= LogType.Information)
                WriteLineInternal(ref sLine, LogType.Information, false);
        }
        public void WriteWarning(string sLine)
        {
            if (mMinimumLogImportancy <= LogType.Warning)
                WriteLineInternal(ref sLine, LogType.Warning, false);
        }
        public void WriteError(string sLine)
        {
            if (mMinimumLogImportancy <= LogType.Error)
                WriteLineInternal(ref sLine, LogType.Error, false);
        }
        public void WriteUnhandledExceptionError(string sMethodName, Exception ex)
        {
            WriteError("Unhandled exception in " + sMethodName + "() method, "
            + "exception message: " + ex.Message + ", "
            + "stack trace: " + ex.StackTrace);
        }
        public void WriteConfigurationParseError(string sField)
        {
            if (mMinimumLogImportancy <= LogType.Error)
            {
                string sLine = string.Format("Could not parse configuration field '{0}'.", sField);
                WriteLineInternal(ref sLine, LogType.Error, false);
            }
        }
        #endregion
    }

    public enum LogType
    {
        Debug = 0,
        Information = 1,
        Warning = 2,
        Error = 3
    }
}
