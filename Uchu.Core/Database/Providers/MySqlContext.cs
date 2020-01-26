using Microsoft.EntityFrameworkCore;

namespace Uchu.Core.Providers
{
    public class MySqlContext : UchuContextBase
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                $"Server={Config.Database.Host};" +
                $"Database={Config.Database.Database};" +
                $"Uid={Config.Database.Username};" +
                $"Pwd={Config.Database.Password}"
            );
        }
    }
}