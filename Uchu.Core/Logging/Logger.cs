using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Uchu.Core.Config;

namespace Uchu.Core
{
    public static class Logger
    {
        public static UchuConfiguration Config { get; set; }

        private static LogQueue logQueue = new LogQueue();

        private static bool outputThreadStarted = false;

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
#if DEBUG
            var trace = new StackTrace();

            Task.Run(() => { InternalLog(content.ToString(), LogLevel.Debug, ConsoleColor.Green, trace); });
#else
            Task.Run(() => { InternalLog(content.ToString(), LogLevel.Debug, ConsoleColor.Green); });
#endif
        }

        public static void Information(object content)
        {
#if DEBUG
            var trace = new StackTrace();

            Task.Run(() => { InternalLog(content.ToString(), LogLevel.Information, ConsoleColor.White, trace); });
#else
            Task.Run(() => { InternalLog(content.ToString(), LogLevel.Information, ConsoleColor.White); });
#endif
        }

        public static void Warning(object content)
        {
#if DEBUG
            var trace = new StackTrace();

            Task.Run(() => { InternalLog(content.ToString(), LogLevel.Warning, ConsoleColor.Yellow, trace); });
#else
            Task.Run(() => { InternalLog(content.ToString(), LogLevel.Warning, ConsoleColor.Yellow); });
#endif
        }

        public static void Error(object content)
        {
#if DEBUG
            var trace = new StackTrace();

            Task.Run(() => { InternalLog(content.ToString(), LogLevel.Error, ConsoleColor.Red, trace); });
#else
            Task.Run(() => { InternalLog(content.ToString(), LogLevel.Error, ConsoleColor.Red); });
#endif
        }

#if DEBUG
        private static void InternalLog(string message, LogLevel logLevel, ConsoleColor color, StackTrace trace)
#else
        private static void InternalLog(string message, LogLevel logLevel, ConsoleColor color)
#endif
        {
            if (message.Contains('\n', StringComparison.InvariantCulture))
            {
                var parts = message.Split('\n');

                var visual = parts.Max(p => p.Length);

                foreach (var part in parts)
                {
                    var padding = new string(Enumerable.Repeat(' ', visual - part.Length).ToArray());

                    var segment = $"{part}{padding}";

#if DEBUG
                    InternalLog(segment, logLevel, color, trace);

                    trace = default;
#else
                    InternalLog(segment, logLevel, color);
#endif
                }

                return;
            }

            {
                var level = logLevel.ToString();

                var padding = new string(Enumerable.Repeat(' ', 12 - level.Length).ToArray());

                message = $"[{level}]{padding} {message}";

#if DEBUG
                var amount = 140 - message.Length;
                padding = new string(Enumerable.Repeat(' ', amount > 0 ? amount : 1).ToArray());

                message = $"{message}{padding}|";

                if (trace != default)
                {
                    var invoker = TraceInvoker(trace);

                    if (invoker?.DeclaringType != default)
                        message = $"{message} {invoker.DeclaringType.Name}.{invoker.Name}";
                }
#endif

                var configuration = Config.ConsoleLogging;

                var consoleLogLevel = Enum.Parse<LogLevel>(configuration.Level);

                string finalMessage;

                if (consoleLogLevel <= logLevel)
                {
                    finalMessage = configuration.Timestamp ? $"[{GetCurrentTime()}] {message}" : message;
                    logQueue.AddLine(finalMessage,color);

                    if (!outputThreadStarted)
                    {
                        outputThreadStarted = true;
                        logQueue.StartConsoleOutput();
                    }
                }

                configuration = Config.FileLogging;

                var fileLogLevel = Enum.Parse<LogLevel>(configuration.Level);

                if (fileLogLevel == LogLevel.None || fileLogLevel > logLevel) return;

                var file = configuration.File;

                finalMessage = configuration.Timestamp ? $"[{GetCurrentTime()}] {message}" : message;

                File.AppendAllTextAsync(file, $"{finalMessage}\n");
            }
        }

        public static string GetCurrentTime()
        {
            return DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
        }

#if DEBUG
        private static MethodBase TraceInvoker(StackTrace trace)
        {
            var frames = trace.GetFrames();

            var avoid = new[]
            {
                typeof(Logger)
            };

            foreach (var frame in frames)
            {
                if (frame == default) continue;

                var method = frame.GetMethod();

                var type = method.DeclaringType;

                if (type?.Namespace == default) continue;
                
                var attribute = type.GetCustomAttribute<LoggerIgnoreAttribute>();

                if (avoid.Contains(type) || attribute != null) continue;

                if (type.Namespace != null && !type.Namespace.StartsWith(
                    "Uchu", StringComparison.InvariantCulture)
                ) continue;

                if (type.Name.Contains('<', StringComparison.InvariantCulture)) continue;

                if (method.Name.Contains('<', StringComparison.InvariantCulture)) continue;

                return method;
            }

            return default;
        }
#endif
    }
}