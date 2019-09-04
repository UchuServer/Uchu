using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace Uchu.Core
{
    public static class Logger
    {
        private static LogLevel _fileMinLevel;
        
        private static LogLevel _consoleMinLevel;

        private static string _file;

        private static object _lock;
        
        static Logger()
        {
            ReloadConfig();

            _lock = new object();
        }
        
        private static string GetConfigFile()
        {
            var current = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            
            var configFileNames = new[] { "Config/logging.config", "logging.config" };

            var configFile =
                (from configFileName in configFileNames
                    where File.Exists($"{current}/{configFileName}")
                    select $"{current}/{configFileName}").FirstOrDefault();

            if (configFile == null) throw new NullReferenceException("Logging config file not found.");

            return configFile;
        }

        public static void ReloadConfig()
        {
            var document = new XmlDocument();
            document.Load(GetConfigFile());

            var current = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            
            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                switch (node.Name)
                {
                    case "file":
                        _file = $"{current}/{node.InnerText.Replace("\n", "").Replace("\t", "").Replace(" ", "")}";
                        if (!Enum.TryParse(node.Attributes["level"].Value, out _fileMinLevel))
                        {
                            throw new ArgumentException($"{node.InnerText} is not a valid LogLevel.");
                        }
                        break;
                    case "console":
                        if (!Enum.TryParse(node.Attributes["level"].Value, out _consoleMinLevel))
                        {
                            throw new ArgumentException($"{node.InnerText} is not a valid LogLevel.");
                        }
                        break;
                }
            }
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
            Task.Run(() => { InternalLog(obj.ToString(), LogLevel.Error, ConsoleColor.Red); });
        }

        private static void InternalLog(string message, LogLevel logLevel, ConsoleColor color, bool clearColor = false)
        {
            lock (_lock)
            {
                if (clearColor)
                    Console.ResetColor();
                else
                    Console.ForegroundColor = color;

                message = $"[{logLevel}] {message}";

                if (_fileMinLevel <= logLevel)
                {
                    var path = Path.GetDirectoryName(_file);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    if (!File.Exists(_file)) File.Create(_file).Dispose();
                    File.AppendAllTextAsync(_file, $"{DateTime.Now} {message}\n");
                }

                if (_consoleMinLevel <= logLevel)
                {
                    Console.WriteLine(message);
                    Console.ResetColor();
                }
            }
        }
    }
}