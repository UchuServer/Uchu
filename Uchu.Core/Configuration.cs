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
        public ServerConfiguration Character { get; set; } = new ServerConfiguration {Port = 2002};

        [XmlElement]
        public ServerConfiguration World { get; set; } = new ServerConfiguration {Port = 2003};

        [XmlElement]
        public ServerConfiguration Chat { get; set; } = new ServerConfiguration {Port = 2004};

        [XmlElement]
        public TlsConfiguration Tls { get; set; } = null;

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

        public class ServerConfiguration
        {
            [XmlElement]
            public int Port { get; set; }
        }

        public class TlsConfiguration
        {
            [XmlElement]
            public string Certificate { get; set; }

            [XmlElement]
            public string Key { get; set; }
        }
    }
}