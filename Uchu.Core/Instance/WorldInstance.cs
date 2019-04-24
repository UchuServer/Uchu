using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Neo.IronLua;
using RakDotNet;
using Uchu.Core.IO;
using Uchu.Core.Scripting.Lua;

namespace Uchu.Core
{
    public class WorldInstance
    {
        private readonly List<ISpawnable> _objects;
        private readonly Dictionary<IPEndPoint, PlayerObject> _players;

        internal readonly Server Server;

        public Zone Zone { get; private set; }
        public ZoneTableRow ZoneInfo { get; private set; }
        public ReplicaManager ReplicaManager { get; }

        public IReadOnlyList<ISpawnable> Objects => _objects;
        public IReadOnlyDictionary<IPEndPoint, PlayerObject> Players => _players;

        public Lua Lua { get; }
        public dynamic LuaEnvironment { get; }
        public LuaResourceManager LuaResourceManager { get; }
        public LuaObjectManager LuaObjectManager { get; }

        public WorldInstance(Server server)
        {
            _objects = new List<ISpawnable>();
            _players = new Dictionary<IPEndPoint, PlayerObject>();

            Server = server;

            ReplicaManager = server.CreateReplicaManager();

            Lua = new Lua();
            LuaEnvironment = Lua.CreateEnvironment();
            LuaResourceManager = new LuaResourceManager(this);
            LuaObjectManager = new LuaObjectManager(this);

            LuaEnvironment.RESMGR = LuaResourceManager;
            LuaEnvironment.GAMEOBJ = LuaObjectManager;
        }

        public async Task InitAsync(ZoneId zoneId)
        {
            ZoneInfo = await Server.CDClient.GetZoneAsync((ushort) zoneId);
            Zone = await Server.ZoneParser.ParseAsync(ZoneInfo.FileName);

            if (ZoneInfo.ScriptComponentId != -1)
            {
                var script = await Server.CDClient.GetScriptComponentAsync(ZoneInfo.ScriptComponentId);

                if (!string.IsNullOrEmpty(script.ServerScript) && !script.ServerScript.Contains("__removed"))
                    LuaEnvironment.dofile(Path.Combine(FileResources.AssemblyDirectory, "Scripts",
                        script.ServerScript));
            }

            foreach (var lvlObject in Zone.Scenes.Where(s => !s.Audio).SelectMany(s => s.Objects))
            {
                if (lvlObject.Settings.TryGetValue("renderDisabled", out var renderDisabled) && (bool) renderDisabled ||
                    lvlObject.Settings.TryGetValue("carver_only", out var carverOnly) && (bool) carverOnly ||
                    lvlObject.Settings.TryGetValue("loadOnSrvrOnly", out var serverOnly) && (bool) serverOnly)
                    continue;

                await AddObjectAsync(lvlObject);
            }
        }

        public void AddPlayer(Character character, IPEndPoint endpoint)
        {
            var player = new PlayerObject(this, character, endpoint);

            ReplicaManager.AddConnection(endpoint);

            _players[endpoint] = player;
            _objects.Add(player);

            player.Spawn();
        }

        public PlayerObject GetPlayer(IPEndPoint endpoint)
            => Players[endpoint];

        public PlayerObject GetPlayer(long objectId)
            => Players.Values.First(p => p.ObjectId == objectId);

        public async Task AddObjectAsync(LevelObject lvlObject)
        {
            var obj = new WorldObject(this, lvlObject);

            await obj.SpawnAsync();

            _objects.Add(obj);
        }

        public ISpawnable GetObject(long objectId)
            => _objects.First(o => o.ObjectId == objectId);
    }
}