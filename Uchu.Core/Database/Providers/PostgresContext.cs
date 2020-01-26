using Microsoft.EntityFrameworkCore;

namespace Uchu.Core.Providers
{
    public class PostgresContext : UchuContextBase
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                $"Host={Config.Database.Host};" +
                $"Database={Config.Database.Database};" +
                $"Username={Config.Database.Username};" +
                $"Password={Config.Database.Password}"
            );
        }
    }
}