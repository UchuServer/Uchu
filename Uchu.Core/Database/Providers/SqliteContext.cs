using Microsoft.EntityFrameworkCore;

namespace Uchu.Core.Providers
{
    public class SqliteContext : UchuContextBase
    {
        public static string DatabasePath { get; set; } = "./Uchu.sqlite";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=\"{DatabasePath}\"");
        }
    }
}