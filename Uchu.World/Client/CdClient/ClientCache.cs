using System.Linq;
using Uchu.Core.Client;

namespace Uchu.World.Client
{
    public static class ClientCache
    {
        public static MissionTasks[] Tasks { get; }

        static ClientCache()
        {
            using var cdClient = new CdClientContext();

            Tasks = cdClient.MissionTasksTable.ToArray();
        }
    }
}