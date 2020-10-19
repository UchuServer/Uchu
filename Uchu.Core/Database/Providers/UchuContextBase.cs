using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core.Config;

namespace Uchu.Core.Providers
{
    public abstract class UchuContextBase : DbContext, IAsyncDisposable
    {
        public static UchuConfiguration Config { get; set; } = new UchuConfiguration();
        
        public DbSet<User> Users { get; set; }

        public DbSet<Character> Characters { get; set; }

        public DbSet<InventoryItem> InventoryItems { get; set; }

        public DbSet<Mission> Missions { get; set; }

        public DbSet<MissionTask> MissionTasks { get; set; }

        public DbSet<FriendRequest> FriendRequests { get; set; }

        public DbSet<SessionCache> SessionCaches { get; set; }
        
        public DbSet<CharacterMail> Mails { get; set; }
        
        public DbSet<Guild> Guilds { get; set; }
        
        public DbSet<GuildInvite> GuildInvites { get; set; }
        
        public DbSet<Friend> Friends { get; set; }
        
        public DbSet<ChatTranscript> ChatTranscript { get; set; }
        
        public DbSet<CharacterTrade> Trades { get; set; }
        
        public DbSet<TradeTransactionItem> TransactionItems { get; set; }
        
        public DbSet<CharacterFlag> Flags { get; set; }
        
        public async Task EnsureUpdatedAsync()
        {
            await Database.MigrateAsync().ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.Run(Dispose));
        }
    }
}