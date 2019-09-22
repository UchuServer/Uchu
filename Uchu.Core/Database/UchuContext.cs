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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                $"Host={Configuration.Singleton.Postgres.Host};" +
                $"Database={Configuration.Singleton.Postgres.Database};" +
                $"Username={Configuration.Singleton.Postgres.Username};" +
                $"Password={Configuration.Singleton.Postgres.Password}"
            );
        }
    }
}