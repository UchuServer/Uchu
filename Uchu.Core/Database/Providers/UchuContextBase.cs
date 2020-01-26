using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Uchu.Core.Providers
{
    public abstract class UchuContextBase : DbContext, IAsyncDisposable
    {
        public static Configuration Config { get; set; } = new Configuration();
        
        public DbSet<User> Users { get; set; }

        public DbSet<Character> Characters { get; set; }

        public DbSet<InventoryItem> InventoryItems { get; set; }

        public DbSet<Mission> Missions { get; set; }

        public DbSet<MissionTask> MissionTasks { get; set; }

        public DbSet<Friend> Friends { get; set; }
        
        public DbSet<ServerSpecification> Specifications { get; set; }
        
        public DbSet<WorldServerRequest> WorldServerRequests { get; set; }
        
        public DbSet<SessionCache> SessionCaches { get; set; }
        
        public DbSet<CharacterMail> Mails { get; set; }
        
        public virtual async Task EnsureUpdatedAsync()
        {
            await Database.MigrateAsync().ConfigureAwait(false);
        }

        public virtual async ValueTask DisposeAsync()
        {
            Dispose();
        }
    }
}