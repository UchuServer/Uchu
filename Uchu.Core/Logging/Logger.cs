using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nexus.Logging.Output;
using Uchu.Core.Config;

namespace Uchu.Core
{
    public static class Logger
    {
        /// <summary>
        /// Logger used by Uchu.
        /// </summary>
        private static readonly Nexus.Logging.Logger NexusLogger = new Nexus.Logging.Logger();

        /// <summary>
        /// Prepares logging.
        /// </summary>
        /// <param name="configuration">Configuration to use.</param>
        public static void SetConfiguration(UchuConfiguration configuration)
        {
            // Get the log levels. They are used referenced twice.
            var consoleLogLevel = LogLevel.Debug;
            var fileLogLevel = LogLevel.None;
            if (Enum.TryParse<LogLevel>(configuration?.ConsoleLogging.Level, out var newConsoleLogLevel))
            {
                consoleLogLevel = newConsoleLogLevel;
            }
            if (Enum.TryParse<LogLevel>(configuration?.FileLogging.Level, out var newFileLogLevel))
            {
                fileLogLevel = newFileLogLevel;
            }
            
            // Add console logging.
            if (consoleLogLevel != LogLevel.None)
            {
                NexusLogger.Outputs.Add(new ConsoleOutput()
                {
                    IncludeDate = configuration?.ConsoleLogging.Timestamp ?? false,
                    NamespaceWhitelist = new List<string>() { "Uchu" },
                    MinimumLevel = consoleLogLevel,
                });
            }
            
            // Add file logging.
            if (fileLogLevel != LogLevel.None)
            {
                NexusLogger.Outputs.Add(new FileOutput()
                {
                    IncludeDate = configuration?.FileLogging.Timestamp ?? false,
                    NamespaceWhitelist = new List<string>() { "Uchu" },
                    MinimumLevel = fileLogLevel,
                    FileLocation = configuration?.FileLogging.File ?? "uchu.log",
                });
            }
        }

        /// <summary>
        /// Sets the server information to the log messages used to
        /// identify the server type. Used for conditions where
        /// the logs are together, such as in a Docker container.
        /// </summary>
        /// <param name="type">Server type to add.</param>
        public static void SetServerTypeInformation(string type)
        {
            foreach (var output in NexusLogger.Outputs)
            {
                output.AdditionalLogInfo.Add(type);
            }
        }
        
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="content">Content to log. Can be an object, like an exception.</param>
        /// <param name="logLevel">Log level to output with.</param>
        public static void Log(object content, LogLevel logLevel = LogLevel.Information)
        {
            NexusLogger.Log(content, logLevel);
        }

        /// <summary>
        /// Logs a message as a Debug level.
        /// </summary>
        /// <param name="content">Content to log. Can be an object, like an exception.</param>
        public static void Debug(object content)
        {
            NexusLogger.Debug(content);
        }

        /// <summary>
        /// Logs a message as an Information level.
        /// </summary>
        /// <param name="content">Content to log. Can be an object, like an exception.</param>
        public static void Information(object content)
        {
            NexusLogger.Info(content);
        }

        /// <summary>
        /// Logs a message as a Warning level.
        /// </summary>
        /// <param name="content">Content to log. Can be an object, like an exception.</param>
        public static void Warning(object content)
        {
            NexusLogger.Warn(content);
        }

        /// <summary>
        /// Logs a message as a Error level.
        /// </summary>
        /// <param name="content">Content to log. Can be an object, like an exception.</param>
        public static void Error(object content)
        {
            NexusLogger.Error(content);
        }
    }
}