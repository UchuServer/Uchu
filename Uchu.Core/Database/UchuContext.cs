using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core.Providers;

namespace Uchu.Core
{
    public sealed class UchuContext : IAsyncDisposable, IDisposable
    {
        public UchuContextBase ContextBase { get; set; }

        public DbSet<User> Users => ContextBase.Users;

        public DbSet<Character> Characters => ContextBase.Characters;

        public DbSet<InventoryItem> InventoryItems => ContextBase.InventoryItems;

        public DbSet<Mission> Missions => ContextBase.Missions;

        public DbSet<MissionTask> MissionTasks => ContextBase.MissionTasks;

        public DbSet<FriendRequest> FriendRequests => ContextBase.FriendRequests;

        public DbSet<SessionCache> SessionCaches => ContextBase.SessionCaches;

        public DbSet<CharacterMail> Mails => ContextBase.Mails;

        public DbSet<Guild> Guilds => ContextBase.Guilds;

        public DbSet<GuildInvite> GuildInvites => ContextBase.GuildInvites;

        public DbSet<Friend> Friends => ContextBase.Friends;

        public DbSet<ChatTranscript> ChatTranscript => ContextBase.ChatTranscript;

        public DbSet<CharacterTrade> Trades => ContextBase.Trades;

        public DbSet<TradeTransactionItem> TransactionItems => ContextBase.TransactionItems;

        public UchuContext()
        {
            var config = UchuContextBase.Config;

            switch (config.Database.Provider)
            {
                case "postgres":
                    ContextBase = new PostgresContext();
                    break;
                case "mysql":
                    ContextBase = new MySqlContext();
                    break;
                case "sqlite":
                    ContextBase = new SqliteContext();
                    break;
                default:
                    Logger.Error($"{config.Database.Provider} is a invalid or unsupported database provider");
                    throw new Exception($"Invalid database provider: \"{config.Database.Provider}\"");
            }
        }
        
        public async Task EnsureUpdatedAsync()
        {
            await ContextBase.EnsureUpdatedAsync().ConfigureAwait(false);
        }

        public Task SaveChangesAsync() => ContextBase.SaveChangesAsync();
        
        public void SaveChanges() => ContextBase.SaveChanges();
        
        public ValueTask DisposeAsync() => ContextBase.DisposeAsync();

        public void Dispose() => ContextBase?.Dispose();
    }
}