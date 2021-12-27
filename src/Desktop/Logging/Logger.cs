using System;
using System.IO;

namespace Noteapp.Desktop.Logging
{
    public static class Logger
    {
        private static readonly string _logFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            "noteapp_logs.txt"
        );

        public static void Log(string message)
        {
            File.AppendAllText(_logFile, $"{DateTime.Now.ToString("G")}: {message}\n");
        }
    }
}
