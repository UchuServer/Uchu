using System;
using System.Xml.Serialization;

namespace Uchu.Core
{
    [XmlRoot("Uchu")]
    public class Configuration
    {
        [Obsolete("Use the Config property provided by the server, or instantiate your own", true)]
        public static Configuration Singleton;

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

        [XmlElement]
        public ServerConfiguration Character { get; set; } = new ServerConfiguration {Port = 2002};

        [XmlElement]
        public ServerConfiguration World { get; set; } = new ServerConfiguration {Port = 2003};

        [XmlElement]
        public ServerConfiguration Chat { get; set; } = new ServerConfiguration {Port = 2004};

        [XmlElement]
        public ResourcesConfiguration ResourcesConfiguration { get; set; } = new ResourcesConfiguration {GameResourceFolder = "/res"};
    }

    public class ServerConfiguration
    {
        [XmlElement]
        public int Port { get; set; }
    }

    public class ResourcesConfiguration
    {
        [XmlElement]
        public string GameResourceFolder { get; set; }
    }

    public class LoggingConfiguration
    {
        [XmlElement]
        public string Level;

        [XmlElement]
        public string Logfile;
    }

    public class DatabaseConfiguration
    {
        [XmlElement]
        public string Database { get; set; }

        [XmlElement]
        public string Host { get; set; }

        [XmlElement]
        public string Username { get; set; }

        [XmlElement]
        public string Password { get; set; }
    }
}