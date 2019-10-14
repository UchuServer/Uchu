using System.IO;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Uchu.Core
{
    public class UchuContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Character> Characters { get; set; }

        public DbSet<InventoryItem> InventoryItems { get; set; }

        public DbSet<Mission> Missions { get; set; }

        public DbSet<MissionTask> MissionTasks { get; set; }

        public DbSet<Friend> Friends { get; set; }
        
        public DbSet<ServerSpecification> Specifications { get; set; }
        
        public DbSet<WorldServerRequest> WorldServerRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var fn = File.Exists("config.xml") ? "config.xml" : "config.default.xml";

            var config = new Configuration();

            if (File.Exists(fn))
            {
                var serializer = new XmlSerializer(typeof(Configuration));

                using var file = File.OpenRead(fn);
                
                config = (Configuration) serializer.Deserialize(file);
            }

            optionsBuilder.UseNpgsql(
                $"Host={config.Postgres.Host};" +
                $"Database={config.Postgres.Database};" +
                $"Username={config.Postgres.Username};" +
                $"Password={config.Postgres.Password}"
            );
        }
    }
}