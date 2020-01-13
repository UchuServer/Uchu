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

        [XmlElement] public ManagedScriptSources ManagedScriptSources { get; set; } = new ManagedScriptSources();
        
        [XmlElement]
        public ResourcesConfiguration ResourcesConfiguration { get; set; } =
            new ResourcesConfiguration {GameResourceFolder = "/res"};
        
        [XmlElement] public Networking Networking { get; set; } = new Networking();
    }

    public class ManagedScriptSources
    {
        [XmlElement("Script")] public List<string> Scripts { get; set; } = new List<string>();
    }

    public class ServerDllSource
    {
        [XmlElement] public string ServerDllSourcePath { get; set; } = "../../../../";

        [XmlElement] public string DotNetPath { get; set; } = "dotnet";
        
        [XmlElement]
        public List<string> ScriptDllSource { get; set; } = new List<string>
        {
            "Uchu.StandardScripts"
        };
    }

    public class Networking
    {
        [XmlElement] public string Certificate { get; set; } = "";

        [XmlElement] public string Hostname { get; set; } = "";
        
        [XmlElement] public int CharacterPort { get; set; } = 2002;
        
        [XmlElement("WorldPort")] public List<int> WorldPorts { get; set; }
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