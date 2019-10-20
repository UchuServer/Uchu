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

        [XmlElement] public ServerDLLSource DllSource { get; set; } = new ServerDLLSource();

        [XmlElement]
        public ResourcesConfiguration ResourcesConfiguration { get; set; } =
            new ResourcesConfiguration {GameResourceFolder = "/res"};
    }

    public class ServerDLLSource
    {
        [XmlElement] public string ServerDLLSourcePath { get; set; } = "../../../../";

        [XmlElement] public string[] ScriptDLLSource { get; set; } =
        {
            "StandardScripts"
        };
    }

    public class ResourcesConfiguration
    {
        [XmlElement] public string GameResourceFolder { get; set; }
    }

    public class LoggingConfiguration
    {
        [XmlElement] public string Level;

        [XmlElement] public string Logfile;
    }

    public class DatabaseConfiguration
    {
        [XmlElement] public string Database { get; set; }

        [XmlElement] public string Host { get; set; }

        [XmlElement] public string Username { get; set; }

        [XmlElement] public string Password { get; set; }
    }
}