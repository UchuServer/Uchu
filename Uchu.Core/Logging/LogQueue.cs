using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Uchu.Core.Config;

namespace Uchu.Core
{
    public class LogEntry
    {
        public string message { get; set; }
        public ConsoleColor color { get; set; }
        public LogLevel logLevel { get; set; }
        public StackTrace trace { get; set; }
        public LogEntry nextEntry { get; set; }
    }
    
    public class LogQueue : IDisposable
    {
        private readonly Mutex logMutex = new Mutex();
        private LogEntry nextEntry = null;
        private LogEntry lastEntry = null;
        
        public static UchuConfiguration Config { get; set; }
        
        /// <summary>
        /// Adds a line to the log queue.
        /// </summary>
#if DEBUG
        public void AddLine(string line, LogLevel logLevel, ConsoleColor color, StackTrace trace)
#else
        public void AddLine(string line, LogLevel logLevel, ConsoleColor color)
#endif
        {
            // Lock the log.
            logMutex.WaitOne();
            
            // Create the new entry.
            var entry = new LogEntry()
            {
                message = line,
                color = color,
#if DEBUG
                trace = trace,
#endif
                logLevel = logLevel
            };
            
            // Update the pointers.
            if (this.nextEntry == null)
            {
                this.nextEntry = entry;
                this.lastEntry = entry;
            }
            else
            {
                this.lastEntry.nextEntry = entry;
                this.lastEntry = entry;
            }

            // Unlock the log.
            this.logMutex.ReleaseMutex();
        }

        /// <summary>
        /// Pops the next entry to output.
        /// Returns null if there is no pending entry.
        /// </summary>
        public LogEntry PopEntry()
        {
            // Lock the log.
            logMutex.WaitOne();
            
            // Pop the next entry.
            LogEntry entry = null;
            if (this.nextEntry != null)
            {
                entry = this.nextEntry;
                this.nextEntry = entry.nextEntry;
            }
            
            // Unlock the log.
            this.logMutex.ReleaseMutex();
            
            // Return the entry.
            return entry;
        }
        
#if DEBUG
        public static string GenerateMessage(string message, LogLevel logLevel, StackTrace trace)
#else
        public static string GenerateMessage(string message, LogLevel logLevel)
#endif
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

            return message;
        }

        /// <summary>
        /// Starts a thread for outputting the log. This prevents
        /// the console IO from blocking the application.
        /// </summary>
        public void StartConsoleOutput()
        {
            Task.Factory.StartNew(async () =>
            {
                // Run the output loop.
                while (true)
                {
                    // Read the entries until they are exhausted.
                    var entry = this.PopEntry();
                    while (entry != null)
                    {
                        if (entry.message.Contains('\n', StringComparison.InvariantCulture))
                        {
                            var parts = entry.message.Split('\n');

                            var visual = parts.Max(p => p.Length);

                            foreach (var part in parts)
                            {
                                var padding = new string(Enumerable.Repeat(' ', visual - part.Length).ToArray());

                                var segment = $"{part}{padding}";

#if DEBUG
                                await InternalLog(segment, entry.color, entry.logLevel, entry.trace).ConfigureAwait(false);
#else
                                await InternalLog(segment, entry.color, entry.logLevel).ConfigureAwait(false);
#endif
                            }
                            entry = this.PopEntry();
                            continue;
                        }
#if DEBUG
                        await InternalLog(entry.message, entry.color, entry.logLevel, entry.trace).ConfigureAwait(false);
#else
                        await InternalLog(entry.message, entry.color, entry.logLevel).ConfigureAwait(false);
#endif
                        entry = this.PopEntry();
                    }
                    
                    // Reset the color.
                    Console.ResetColor();
                    
                    // Sleep for a bit before resuming.
                    await Task.Delay(50).ConfigureAwait(false);
                }
            },CancellationToken.None,TaskCreationOptions.LongRunning,TaskScheduler.Default);
        }
        
#if DEBUG
        private static async Task InternalLog(string message, ConsoleColor color, LogLevel logLevel, StackTrace trace)
#else
        private static async Task InternalLog(string message, ConsoleColor color, LogLevel logLevel)
#endif
        {
#if DEBUG
            string finalMessage = GenerateMessage(message, logLevel, trace);
#else
            string finalMessage = GenerateMessage(message, logLevel);
#endif

            var fileLoggingConfiguration = Config.FileLogging;
            var fileLogLevel = Enum.Parse<LogLevel>(fileLoggingConfiguration.Level);
            if (fileLogLevel != LogLevel.None && fileLogLevel <= logLevel)
            {
                var file = fileLoggingConfiguration.File;

                var fileMessage = fileLoggingConfiguration.Timestamp ? $"[{GetCurrentTime()}] {finalMessage}" : finalMessage;

                await File.AppendAllTextAsync(file, $"{fileMessage}\n").ConfigureAwait(false);
            }

            var consoleLoggingConfiguration = Config.ConsoleLogging;
            var consoleLogLevel = Enum.Parse<LogLevel>(consoleLoggingConfiguration.Level);                
            if (consoleLogLevel<= logLevel)
            {
                var consoleMessage = consoleLoggingConfiguration.Timestamp ? $"[{GetCurrentTime()}] {finalMessage}" : finalMessage;
                
                // Set the color to output.
                if (color == ConsoleColor.White)
                {
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = color;
                }
                        
                // Write the entry.
                await Console.Out.WriteLineAsync(consoleMessage).ConfigureAwait(false);
            }
            
            
        }
        
        /// <summary>
        /// Disposed the object.
        /// Implemented to silence CA1001 warning.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.logMutex.Dispose();
            }
        }

        /// <summary>
        /// Disposed the object.
        /// Implemented to silence CA1001 warning.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static string GetCurrentTime()
        {
            return DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt", CultureInfo.InvariantCulture);
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