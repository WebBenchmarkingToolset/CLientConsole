using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole
{
    public class Logger
    {
        StringBuilder logBuilder = new StringBuilder();

        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            Success
        }

        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            Console.ForegroundColor = level switch
            {
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Success => ConsoleColor.Green,
                _ => ConsoleColor.White
            };

            string lineStr = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            Console.WriteLine(lineStr);
            logBuilder.AppendLine(lineStr);
            Console.ResetColor();
        }

        public void saveFile(string fileName)
        {
            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }

            File.WriteAllText(fileName, logBuilder.ToString());
        }


        public void Info(string message) => Log(message, LogLevel.Info);

        public void Warning(string message) => Log(message, LogLevel.Warning);

        public void Error(string message) => Log(message, LogLevel.Error);
        public void Success(string message) => Log(message, LogLevel.Success);
    }
}
