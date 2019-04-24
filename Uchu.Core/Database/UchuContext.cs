using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.IO;
using System.Xml.Serialization;

namespace Uchu.Core
{
    public class UchuContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<MissionTask> MissionTasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var serializer = new XmlSerializer(typeof(Configuration));

            Configuration config;

            var fn = File.Exists("config.xml") ? "config.xml" : "config.default.xml";

            using (var file = File.OpenRead(fn))
            {
                config = (Configuration)serializer.Deserialize(file);
            }

            optionsBuilder.UseNpgsql(new NpgsqlConnectionStringBuilder
            {
                Host = config.Postgres.Host,
                Database = config.Postgres.Database,
                Username = config.Postgres.Username,
                Password = config.Postgres.Password
            }.ToString());
        }
    }
}