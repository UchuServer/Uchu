using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace Uchu.Core
{
    public static class Logger
    {
        private static readonly object Lock;

        public static Configuration Config;

        static Logger()
        {
            Lock = new object();
        }

        public static void Log(object message, LogLevel logLevel = LogLevel.Information)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Debug(message);
                    break;
                case LogLevel.Information:
                    Information(message);
                    break;
                case LogLevel.Warning:
                    Warning(message);
                    break;
                case LogLevel.Error:
                    Error(message);
                    break;
                case LogLevel.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        public static void Debug(object obj)
        {
            Task.Run(() => { InternalLog(obj.ToString(), LogLevel.Debug, ConsoleColor.Green); });
        }

        public static void Information(object obj)
        {
            Task.Run(() => { InternalLog(obj.ToString(), LogLevel.Information, ConsoleColor.White, true); });
        }

        public static void Warning(object obj)
        {
            Task.Run(() => { InternalLog(obj.ToString(), LogLevel.Warning, ConsoleColor.Yellow); });
        }

        public static void Error(object obj)
        {
            Task.Run(() => { InternalLog($"{obj}\n{new StackTrace(1)}", LogLevel.Error, ConsoleColor.Red); });
        }

        private static void InternalLog(string message, LogLevel logLevel, ConsoleColor color, bool clearColor = false)
        {
            lock (Lock)
            {
                if (clearColor)
                    Console.ResetColor();
                else
                    Console.ForegroundColor = color;

                message = $"[{logLevel}] {message}";

                var consoleLogLevel = Enum.Parse<LogLevel>(Config.ConsoleLogging.Level);

                if (consoleLogLevel <= logLevel)
                {
                    Console.WriteLine(message);
                    Console.ResetColor();
                }

                var fileLogLevel = Enum.Parse<LogLevel>(Config.FileLogging.Level);

                if (fileLogLevel != LogLevel.None && fileLogLevel <= logLevel)
                {
                    var file = Config.FileLogging.Logfile;

                    File.AppendAllTextAsync(file, $"{DateTime.Now} {message}\n");
                }
            }
        }
    }
}