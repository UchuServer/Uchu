using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Uchu.Core
{
    public class CDClientContext : DbContext
    {
        public DbSet<MissionsRow> Missions { get; set; }
        public DbSet<ComponentsRegistryRow> ComponentsRegistry { get; set; }
        public DbSet<InventoryComponentRow> InventoryComponent { get; set; }
        public DbSet<ItemComponentRow> ItemComponent { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var file = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "CDClient.db");

            optionsBuilder.UseSqlite($"Data Source={file}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ComponentsRegistryRow>().HasKey(c => new {c.LOT});
        }
    }
}
