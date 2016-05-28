using System.IO;

namespace Sudoku.Log
{
    class Logger
    {
        private static Logger INSTANCE;
        private static string logFilePath;
        private static TextWriter writer;

        private Logger() { }

        public static Logger Instance
        {
            get
            {
                if (INSTANCE == null)
                {
                    INSTANCE = new Logger();
                }
                return INSTANCE;
            }
        }

        public void SetLogFilePathForApplicationStartupPath(string appStartUpPath)
        {
            logFilePath = appStartUpPath + @"\log.txt";
        }

        public void Open()
        {
            writer = new StreamWriter(logFilePath, false);
        }

        public void Info(string comment)
        {
            writer.WriteLine(comment);
        }

        public void Close(string message)
        {
            Info(message);
            Close();
        }

        public void Close()
        {
            writer.Close();
        }

        public static string GetLogFilePath()
        {
            return logFilePath;
        }
    }
}
