using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Uchu.Core
{
    public class InstanceManager
    {
        private readonly Server _server;
        private readonly List<WorldInstance> _instances;

        public InstanceManager(Server server)
        {
            _server = server;
            _instances = new List<WorldInstance>();
        }

        public async Task<WorldInstance> JoinZoneAsync(int zoneId, Character character, IPEndPoint endpoint, bool friendJoin = false)
        {
            WorldInstance instance;

            if (_instances.Exists(inst => inst.ZoneInfo.ZoneId == zoneId && inst.Players.Count < inst.ZoneInfo.SoftPlayerCap))
            {
                instance = _instances.Find(inst => inst.ZoneInfo.ZoneId == zoneId && inst.Players.Count < inst.ZoneInfo.SoftPlayerCap);
            }
            else
            {
                instance = new WorldInstance(_server);

                await instance.InitAsync((ZoneId) (ushort) zoneId);

                _instances.Add(instance);
            }

            instance.AddPlayer(character, endpoint);

            return instance;
        }

        public WorldInstance GetWorld(IPEndPoint endpoint)
            => _instances.Find(inst => inst.Players.ContainsKey(endpoint));
    }
}