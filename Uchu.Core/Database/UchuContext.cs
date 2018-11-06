using Microsoft.EntityFrameworkCore;

namespace Uchu.Core
{
    public class UchuContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=uchu;Username=postgres;Password=postgres");
        }
    }
}