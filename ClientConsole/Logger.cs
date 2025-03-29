using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole
{
    public class Logger
    {
        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            Console.ForegroundColor = level switch
            {
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.White
            };

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
            Console.ResetColor();
        }


        public void Info(string message) => Log(message, LogLevel.Info);

        public void Warning(string message) => Log(message, LogLevel.Warning);

        public void Error(string message) => Log(message, LogLevel.Error);
    }
}
