using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
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

        public DbSet<Friend> Friends => ContextBase.Friends;

        public DbSet<ServerSpecification> Specifications => ContextBase.Specifications;

        public DbSet<WorldServerRequest> WorldServerRequests => ContextBase.WorldServerRequests;

        public DbSet<SessionCache> SessionCaches => ContextBase.SessionCaches;

        public DbSet<CharacterMail> Mails => ContextBase.Mails;

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