using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Systems.Missions;

namespace Uchu.World.Client
{
    /// <summary>
    /// Cache of the cd client context table
    /// </summary>
    public static class ClientCache
    {
        /// <summary>
        /// All missions in the cd client
        /// </summary>
        public static MissionInstance[] Missions { get; private set; } = { };

        /// <summary>
        /// All achievements in the cd client
        /// </summary>
        public static MissionInstance[] Achievements { get; private set; } = { };

        public static async Task LoadAsync()
        {
            await using var cdContext = new CdClientContext();

            Logger.Debug("Setting up missions cache");
            var missionTasks = cdContext.MissionsTable
                .ToArray()
                .Select(async m =>
                {
                    var instance = new MissionInstance(m.Id ?? 0, default);
                    await instance.LoadAsync(cdContext);
                    return instance;
                }).ToList();

            await Task.WhenAll(missionTasks);
            
            Missions = missionTasks.Select(t => t.Result).ToArray();
            Achievements = Missions.Where(m => !m.IsMission).ToArray();
        }
    }
}