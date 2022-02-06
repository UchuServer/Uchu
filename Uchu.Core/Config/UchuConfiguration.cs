using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace Uchu.Core.Config
{
    /// <summary>
    /// Configuration wrapper used to configure the master server
    /// </summary>
    [XmlRoot("Uchu")]
    public class UchuConfiguration
    {
        /// <summary>
        /// Name of the server. Used as author name for achievement reward mails.
        /// </summary>
        [XmlElement] public string ServerName { get; set; } = "LEGO Universe";

        /// <summary>
        /// Database configuration like provider, host and credentials
        /// </summary>
        [XmlElement]
        public DatabaseConfiguration Database { get; set; } = new DatabaseConfiguration
        {
            Provider = "sqlite",
            Database = "uchu",
            Host = "localhost",
            Username = "username",
            Password = "password"
        };

        /// <summary>
        /// Optional sentry DSN to use for tracking errors and exceptions
        /// </summary>
        [XmlElement]
        public string SentryDsn { get; set; } = "";

        /// <summary>
        /// The level of console logging (debug or production)
        /// </summary>
        [XmlElement]
        public LoggingConfiguration ConsoleLogging { get; set; } = new LoggingConfiguration
        {
            Level = LogLevel.Information.ToString()
        };

        /// <summary>
        /// The logging level along with the file to log to
        /// </summary>
        [XmlElement]
        public LoggingConfiguration FileLogging { get; set; } = new LoggingConfiguration
        {
            Level = LogLevel.None.ToString(),
            File = "uchu.log"
        };

        /// <summary>
        /// The source location of the server DLL
        /// </summary>
        [XmlElement] public ServerDllSource DllSource { get; set; } = new ServerDllSource();

        /// <summary>
        /// The source location of the managed scripts
        /// </summary>
        [XmlElement] public ManagedScriptSources ManagedScriptSources { get; set; } = new ManagedScriptSources();
        
        /// <summary>
        /// The location of the LU game resources
        /// </summary>
        [XmlElement]
        public ResourcesConfiguration ResourcesConfiguration { get; set; } =
            new ResourcesConfiguration {GameResourceFolder = "path to res folder"};
        
        /// <summary>
        /// Networking settings like character- and world server ports and certificates
        /// </summary>
        [XmlElement] public Networking Networking { get; set; } = new Networking();
        
        /// <summary>
        /// Gameplay settings like AI and pathfinding
        /// </summary>
        [XmlElement] public GamePlay GamePlay { get; set; } = new GamePlay();
        
        /// <summary>
        /// API host, protocol and domain configuration
        /// </summary>
        [XmlElement("Api")] public ApiConfig ApiConfig { get; set; } = new ApiConfig();
        
        /// <summary>
        /// Optional Single Sign On (SSO) configuration
        /// </summary>
        [XmlElement("Sso")] public SsoConfig SsoConfig { get; set; } = new SsoConfig();
        
        /// <summary>
        /// Optional Redis cache configuration
        /// </summary>
        [XmlElement("Cache")] public CacheConfig CacheConfig { get; set; } = new CacheConfig();
        
        /// <summary>
        /// General behaviour of the program
        /// </summary>
        [XmlElement("ServerBehaviour")] public ServerBehaviour ServerBehaviour { get; set; } = new ServerBehaviour();
        
        /// <summary>
        /// Options for development and debugging
        /// </summary>
        [XmlElement("Debugging")] public DebugConfig DebugConfig { get; set; } = new DebugConfig();

        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(UchuConfiguration));

        /// <summary>
        /// Save the configuration to a file.
        /// </summary>
        /// <param name="path">File to save to.</param>
        public void Save(string path)
        {
            using var streamWriter = new XmlTextWriter(path, Encoding.UTF8);
            streamWriter.Formatting = Formatting.Indented;
            Serializer.Serialize(streamWriter, this);
        }

        /// <summary>
        /// Load the configuration from a file.
        /// </summary>
        /// <param name="path">File to load from.</param>
        /// <returns>The <see cref="UchuConfiguration"/>, if the file exists, otherwise null.</returns>
        public static UchuConfiguration Load(string path)
        {
            if (!File.Exists(path))
                return null;

            using var file = File.OpenRead(path);
            using var reader = XmlReader.Create(file);
            return (UchuConfiguration) Serializer.Deserialize(reader);
        }
    }

    /// <summary>
    /// Cache configuration
    /// </summary>
    public class CacheConfig
    {
        /// <summary>
        /// Whether to use an external caching service, like Redis.
        /// Setting to false reduces the initial server load times
        /// if no service is installed and the connection will
        /// always timeout.
        /// </summary>
        [XmlElement] public bool UseService { get; set; } = false;
        
        /// <summary>
        /// Hostname to use when connecting to the cache service
        /// </summary>
        [XmlElement]
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// Port to use when connecting to the cache service
        /// </summary>
        [XmlElement]
        public int Port { get; set; } = 6379;
    }

    /// <summary>
    /// The source location of the managed Python scripts to use
    /// </summary>
    public class ManagedScriptSources
    {
        /// <summary>
        /// Additional managed script packs to load
        /// </summary>
        [XmlElement("Script")] public List<string> Scripts { get; } = new List<string>();
        
        /// <summary>
        /// Additional sources to load managed scripts from
        /// </summary>
        [XmlElement("Library")] public List<string> Paths { get; } = new List<string>();
    }

    /// <summary>
    /// The location of the server DLL
    /// </summary>
    public class ServerDllSource
    {
        /// <summary>
        /// The path to the dotnet command, for example when not in $PATH
        /// </summary>
        [XmlElement] public string DotNetPath { get; set; } = "dotnet";

        /// <summary>
        /// The path to the Uchu.Instance DLL
        /// </summary>
        [XmlElement] public string Instance { get; set; } = "../../../../Uchu.Instance/bin/Debug/net6.0/Uchu.Instance.dll";

        /// <summary>
        /// The path to the script source DLLs
        /// </summary>
        [XmlElement]
        public List<string> ScriptDllSource { get; } = new List<string>();
    }

    /// <summary>
    /// Networking configuration like manging world- and character ports
    /// </summary>
    public class Networking
    {
        /// <summary>
        /// Optional certificate file to use for connections
        /// </summary>
        [XmlElement] public string Certificate { get; set; } = "";

        /// <summary>
        /// The hostname of the Uchu servers
        /// </summary>
        [XmlElement] public string Hostname { get; set; } = "";
        
        /// <summary>
        /// The port to run the authentication server at
        /// </summary>
        [XmlElement] public int AuthenticationPort { get; set; } = 21836;
        
        /// <summary>
        /// The port to run the character server at
        /// </summary>
        [XmlElement] public int CharacterPort { get; set; } = 2002;

        /// <summary>
        /// Whether to authenticate hosts or not
        /// </summary>
        [XmlElement] public bool HostAuthentication { get; set; } = true;

        /// <summary>
        /// Whether to host the character server or not
        /// </summary>
        [XmlElement] public bool HostCharacter { get; set; } = true;

        /// <summary>
        /// The maximum amount of world servers that may be generated
        /// </summary>
        [XmlElement] public int MaxWorldServers { get; set; } = 100;

        /// <summary>
        /// The amount of heart beats the server should send per heart beat interval for it to retain it's healthy
        /// status for the master server
        /// </summary>
        [XmlElement] public int WorldServerHeartBeatsPerInterval { get; set; } = 10;

        /// <summary>
        /// The interval over which heart beats should be received in minutes
        /// </summary>
        /// <remarks>
        /// Note that this also corresponds to the max loading time for a world before the master server shuts it down.
        /// As a world server is started in the healthy phase with no heart beats it takes 5 times this amount for the
        /// world server to be shut down.
        /// </remarks>
        [XmlElement] public int WorldServerHeartBeatIntervalInMinutes { get; set; } = 5;
        
        /// <summary>
        /// The ports to run the world servers at
        /// </summary>
        [XmlElement("WorldPort")] public List<int> WorldPorts { get; } = new List<int>();
    }

    /// <summary>
    /// Single Sign On (SSO) configuration
    /// </summary>
    public class SsoConfig
    {
        /// <summary>
        /// The domain to find the SSO server at
        /// </summary>
        [XmlElement] public string Domain { get; set; } = "";
    }

    /// <summary>
    /// Configuration for the API server
    /// </summary>
    public class ApiConfig
    {
        /// <summary>
        /// The protocol to run the API on
        /// </summary>
        [XmlElement] public string Protocol { get; set; } = "http";
        
        /// <summary>
        /// The hostname to run the API on
        /// </summary>
        [XmlElement] public string Domain { get; set; } = "localhost";

        /// <summary>
        /// The port to run the API on
        /// </summary>
        [XmlElement] public int Port { get; set; } = 10000;

        /// <summary>
        /// The key to access the API
        /// </summary>
        [XmlElement] public string Key { get; set; } = "";
    }

    /// <summary>
    /// Game resource configuration
    /// </summary>
    public class ResourcesConfiguration
    {
        /// <summary>
        /// The location of the local game resource folder
        /// </summary>
        [XmlElement] public string GameResourceFolder { get; set; }
    }

    /// <summary>
    /// Logging configuration
    /// </summary>
    public class LoggingConfiguration
    {
        /// <summary>
        /// The logging level (Debug or Production)
        /// </summary>
        [XmlElement] public string Level { get; set; }

        /// <summary>
        /// The file to log to
        /// </summary>
        [XmlElement] public string File { get; set; }
        
        /// <summary>
        /// Whether to log timestamps or not
        /// </summary>
        [XmlElement] public bool Timestamp { get; set; }
    }

    /// <summary>
    /// Database configuration
    /// </summary>
    public class DatabaseConfiguration
    {
        /// <summary>
        /// The database provider to use (Postgres, MySQL, MSSQL)
        /// </summary>
        [XmlElement] public string Provider { get; set; }
        
        /// <summary>
        /// The database name
        /// </summary>
        [XmlElement] public string Database { get; set; }

        /// <summary>
        /// The database host (localhost)
        /// </summary>
        [XmlElement] public string Host { get; set; }

        /// <summary>
        /// The database username
        /// </summary>
        [XmlElement] public string Username { get; set; }

        /// <summary>
        /// The database password
        /// </summary>
        [XmlElement] public string Password { get; set; }
    }

    /// <summary>
    /// Gameplay specific configuration
    /// </summary>
    public class GamePlay
    {
        /// <summary>
        /// Whether or not AI uses pathfinding to find players
        /// </summary>
        [XmlElement] public bool PathFinding { get; set; }
        
        /// <summary>
        /// Whether or not AI wanders around
        /// </summary>
        [XmlElement] public bool AiWander { get; set; }
    }

    /// <summary>
    /// Behaviour of the program
    /// </summary>
    public class ServerBehaviour
    {
        /// <summary>
        /// Whether the server should display a "Press any key to exit" prompt before exiting due to a fatal error
        /// </summary>
        [XmlElement] public bool PressKeyToExit { get; set; } = true;

        /// <summary>
        /// How long the server should wait for newly created instances to get ready before throwing a timeout exception
        /// </summary>
        [XmlElement] public int InstanceCommissionTimeout { get; set; } = 30000;
    }

    /// <summary>
    /// Options for development and debugging
    /// </summary>
    public class DebugConfig
    {
        /// <summary>
        /// Whether to use threads instead of processes. Only available for builds in Debug mode.
        /// </summary>
        [XmlElement] public bool StartInstancesAsThreads { get; set; } = false;
    }
}
