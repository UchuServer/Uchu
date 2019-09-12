using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class Zone : Object
    {
        public readonly List<Object> Objects = new List<Object>();

        public readonly List<GameObject> GameObjects = new List<GameObject>();

        public readonly List<Player> Players = new List<Player>();

        public readonly ZoneInfo ZoneInfo;

        public readonly ushort InstanceId;

        public readonly uint CloneId;

        public new readonly Server Server;
        
        private int _ticks;

        private long _passedTickTime;

        /// <summary>
        /// Fractions of a second passed during the last tick. Should be used to fit actions with the TPS
        /// </summary>
        public float TimeDelta { get; private set; }
        
        /// <summary>
        /// This should be set to something the server can sustain.
        /// </summary>
        public const int TicksPerSecondLimit = 60;

        public ZoneId ZoneId => (ZoneId) ZoneInfo.ZoneId;

        public Zone(ZoneInfo zoneInfo, Server server, ushort instanceId = default, uint cloneId = default)
        {
            Zone = this;
            ZoneInfo = zoneInfo;
            Server = server;
            InstanceId = instanceId;
            CloneId = cloneId;
        }

        public void Initialize()
        {
            var objects = ZoneInfo.ScenesInfo.SelectMany(s => s.Objects).ToArray();
            
            Logger.Information($"Loading {objects.Length} objects for {ZoneId}");
            
            foreach (var levelObject in objects)
            {
                var obj = GameObject.Instantiate(levelObject, this);

                if (levelObject.Settings.TryGetValue("loadSrvrOnly", out var serverOnly) && (bool) serverOnly ||
                    levelObject.Settings.TryGetValue("carver_only", out var carverOnly) && (bool) carverOnly ||
                    levelObject.Settings.TryGetValue("renderDisabled", out var disabled) && (bool) disabled)
                    continue;
                
                if (obj.GetType().GetCustomAttribute<UnconstructedAttribute>() == null) obj.Construct();

                var spawner = obj.GetComponent<SpawnerComponent>();
                spawner?.Spawn();
            }
            
            Logger.Information($"Loaded {objects.Length} objects for {ZoneId}");

            Task.Run(async () => { await ExecuteUpdate(); });
        }

        #region Object Mangement

        public void RequestConstruction(Player player)
        {
            Players.Add(player);

            foreach (var gameObject in GameObjects.Where(g => g.Constructed))
            {
                SendConstruction(gameObject, new [] {player});
            }
        }

        public void ConstructObject(GameObject gameObject)
        {
            SendConstruction(gameObject, Players);
        }

        public void UpdateObject(GameObject gameObject)
        {
            SendSerialization(gameObject, Players);
        }

        public void DestroyObject(GameObject gameObject)
        {
            if (gameObject is Player player) Players.Remove(player);
            
            SendDestruction(gameObject, Players);
        }

        public void SendConstruction(GameObject gameObject, ICollection<Player> recipients = null)
        {
            if (recipients == null) recipients = Players;

            foreach (var recipient in recipients)
            {
                var id = recipient.Perspective.Reveal(gameObject);
                
                var stream = new MemoryStream();
            
                using (var writer = new BitWriter(stream))
                {
                    writer.Write((byte) MessageIdentifiers.ReplicaManagerConstruction);
                    
                    writer.WriteBit(true);
                    writer.Write(id);

                    gameObject.WriteConstruct(writer);
                }

                Server.Send(stream, recipient.EndPoint);
            }
        }

        public void SendSerialization(GameObject gameObject, ICollection<Player> recipients = null)
        {
            if (recipients == null) recipients = Players;
            
            foreach (var recipient in recipients)
            {
                if (!recipient.Perspective.TryGetNetworkId(gameObject, out var id)) continue;
                
                var stream = new MemoryStream();
                
                using (var writer = new BitWriter(stream))
                {
                    writer.Write((byte) MessageIdentifiers.ReplicaManagerSerialize);
                    
                    writer.Write(id);

                    gameObject.WriteSerialize(writer);
                }

                Server.Send(stream, recipient.EndPoint);
            }
        }

        public void SendDestruction(GameObject gameObject, ICollection<Player> recipients = null)
        {
            if (recipients == null) recipients = Players;

            foreach (var recipient in recipients)
            {
                if (!recipient.Perspective.TryGetNetworkId(gameObject, out var id)) continue;
                
                var stream = new MemoryStream();
            
                using (var writer = new BitWriter(stream))
                {
                    writer.Write((byte) MessageIdentifiers.ReplicaManagerDestruction);
                    
                    writer.Write(id);
                }

                Server.Send(stream, recipient.EndPoint);

                recipient.Perspective.Drop(gameObject);
            }
        }

        #endregion

        public void SelectiveMessage(IGameMessage message, IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                player.Message(message);
            }
        }

        public void ExcludingMessage(IGameMessage message, Player excluded)
        {
            foreach (var player in Players.Where(p => p != excluded))
            {
                player.Message(message);
            }
        }
        
        public void BroadcastMessage(IGameMessage message)
        {
            foreach (var player in Players)
            {
                player.Message(message);
            }
        }
        
        public static ZoneChecksum GetChecksum(ZoneId id)
        {
            var name = Enum.GetName(typeof(ZoneId), id);

            var names = Enum.GetNames(typeof(ZoneChecksum));
            var values = Enum.GetValues(typeof(ZoneChecksum));

            return (ZoneChecksum) values.GetValue(names.ToList().IndexOf(name));
        }

        private async Task ExecuteUpdate()
        {
            var timer = new Timer(1000);

            timer.Elapsed += (sender, args) =>
            {
                Logger.Debug($"TPS: {_ticks}/{TicksPerSecondLimit} TPT: {_passedTickTime / _ticks} ms");
                _passedTickTime = default;
                _ticks = default;
            };
            
            timer.Start();

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            
            while (true)
            {
                if (_ticks >= TicksPerSecondLimit) continue;
                
                await Task.Delay(1000 / TicksPerSecondLimit);

                Task.WaitAll(Objects.Select(o => Task.Run(() =>
                {
                    try
                    {
                        o.Update();
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                })).ToArray());
                
                _ticks++;

                var passedMs = stopWatch.ElapsedMilliseconds;
                
                TimeDelta = passedMs / 1000f;
                
                _passedTickTime += passedMs;

                stopWatch.Restart();
            }
        }
    }
}