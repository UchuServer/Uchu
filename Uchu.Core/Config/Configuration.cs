using System.Collections.Generic;
using System.Xml.Serialization;

namespace Uchu.Core
{
    [XmlRoot("Uchu")]
    public class Configuration
    {
        [XmlElement]
        public DatabaseConfiguration Postgres { get; set; } = new DatabaseConfiguration
        {
            Database = "uchu",
            Host = "localhost",
            Username = "postgres",
            Password = "postgres"
        };

        [XmlElement]
        public LoggingConfiguration ConsoleLogging { get; set; } = new LoggingConfiguration
        {
            Level = LogLevel.Debug.ToString()
        };

        [XmlElement]
        public LoggingConfiguration FileLogging { get; set; } = new LoggingConfiguration
        {
            Level = LogLevel.None.ToString(),
            Logfile = "uchu.log"
        };

        [XmlElement] public ServerDllSource DllSource { get; set; } = new ServerDllSource();

        [XmlElement]
        public ResourcesConfiguration ResourcesConfiguration { get; set; } =
            new ResourcesConfiguration {GameResourceFolder = "/res"};
    }

    public class ServerDllSource
    {
        [XmlElement] public string ServerDllSourcePath { get; set; } = "../../../../";

        [XmlElement]
        public List<string> ScriptDllSource { get; set; } = new List<string>
        {
            "Uchu.StandardScripts"
        };
    }

    public class ResourcesConfiguration
    {
        [XmlElement] public string GameResourceFolder { get; set; }
    }

    public class LoggingConfiguration
    {
        [XmlElement] public string Level { get; set; }

        [XmlElement] public string Logfile { get; set; }
    }

    public class DatabaseConfiguration
    {
        [XmlElement] public string Database { get; set; }

        [XmlElement] public string Host { get; set; }

        [XmlElement] public string Username { get; set; }

        [XmlElement] public string Password { get; set; }
    }
}