using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Uchu.Core.Config;

namespace Uchu.Core
{
    public static class Logger
    {
        private static LogQueue logQueue = new LogQueue();

        private static bool outputTaskStarted = false;

        public static void Log(object content, LogLevel logLevel = LogLevel.Information)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Debug(content);
                    break;
                case LogLevel.Information:
                    Information(content);
                    break;
                case LogLevel.Warning:
                    Warning(content);
                    break;
                case LogLevel.Error:
                    Error(content);
                    break;
                case LogLevel.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        public static void Debug(object content)
        {
            if (content is null) throw new ArgumentNullException(nameof(content));
#if DEBUG
            var trace = new StackTrace();
            AddToQueue(content.ToString(), LogLevel.Debug, ConsoleColor.Green, trace);
#endif
        }

        public static void Information(object content)
        {
            if (content is null) throw new ArgumentNullException(nameof(content));
#if DEBUG
            var trace = new StackTrace();
            AddToQueue(content.ToString(), LogLevel.Information, ConsoleColor.White, trace); 
#else
            AddToQueue(content.ToString(), LogLevel.Information, ConsoleColor.White);
#endif
        }

        public static void Warning(object content)
        {
            if (content is null) throw new ArgumentNullException(nameof(content));
#if DEBUG
            var trace = new StackTrace();
            AddToQueue(content.ToString(), LogLevel.Warning, ConsoleColor.Yellow, trace);
#else
            AddToQueue(content.ToString(), LogLevel.Warning, ConsoleColor.Yellow);
#endif
        }

        public static void Error(object content)
        {
            if (content is null) throw new ArgumentNullException(nameof(content));
#if DEBUG
            var trace = new StackTrace();
            AddToQueue(content.ToString(), LogLevel.Error, ConsoleColor.Red, trace); 
#else
            AddToQueue(content.ToString(), LogLevel.Error, ConsoleColor.Red);
#endif
        }

#if DEBUG
        private static void AddToQueue(string message, LogLevel logLevel, ConsoleColor color, StackTrace trace)
#else
        private static void AddToQueue(string message, LogLevel logLevel, ConsoleColor color)
#endif
        {
#if DEBUG
            logQueue.AddLine(message, logLevel, color, trace);
#else
            logQueue.AddLine(message, logLevel, color);
#endif

            if (!outputTaskStarted)
            {
                outputTaskStarted = true;
                logQueue.StartConsoleOutput();
            }
            
        }
    }
}