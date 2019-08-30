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

        private readonly Dictionary<GameObject, ushort> _networkIds = new Dictionary<GameObject, ushort>();

        private readonly List<ushort> _droppedIds = new List<ushort>();

        private int _ticks;

        private long _passedTickTime;

        /// <summary>
        /// This should be set to something the server can sustain.
        /// </summary>
        public const int TicksPerSecondLimit = 60;

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
            
            foreach (var levelObject in objects)
            {
                var obj = GameObject.Instantiate(levelObject, this);

                if (levelObject.Settings.TryGetValue("loadSrvrOnly", out var serverOnly) && (bool) serverOnly ||
                    levelObject.Settings.TryGetValue("carver_only", out var carverOnly) && (bool) carverOnly ||
                    levelObject.Settings.TryGetValue("renderDisabled", out var disabled) && (bool) disabled) // is this right?
                    continue;
                
                if (obj.GetType().GetCustomAttribute<Unconstructed>() == null) obj.Construct();

                var spawner = obj.GetComponent<SpawnerComponent>();
                spawner?.Spawn();
            }

            Task.Run(async () => { await ExecuteUpdate(); });
        }

        public void RequestConstruction(Player player)
        {
            Players.Add(player);

            // TODO: Add filters
            foreach (var gameObject in GameObjects.Where(g => g.Constructed))
            {
                SendConstruction(gameObject, new List<IPEndPoint> {player.EndPoint});
            }
        }

        public void ConstructObject(GameObject gameObject)
        {
            // TODO: Add filters
            SendConstruction(gameObject, Players.Select(p => p.EndPoint).ToArray());
        }

        public void UpdateObject(GameObject gameObject)
        {
            // TODO: Add filters
            SendSerialization(gameObject, Players.Select(p => p.EndPoint).ToArray());
        }

        public void DestroyObject(GameObject gameObject)
        {
            if (gameObject is Player player) Players.Remove(player);
            
            // TODO: Add filters
            SendDestruction(gameObject, Players.Select(p => p.EndPoint).ToArray());
        }

        public void SendConstruction(GameObject gameObject, ICollection<IPEndPoint> recipients = null)
        {
            if (recipients == null) recipients = Players.Select(p => p.EndPoint).ToArray();
            if (!recipients.Any()) return;

            if (!_networkIds.ContainsKey(gameObject))
            {
                ushort netId;
                if (_droppedIds.Any())
                {
                    netId = _droppedIds.First();
                    _droppedIds.RemoveAt(0);
                }
                else
                {
                    netId = (ushort) (_networkIds.Count + 1);
                }
                
                _networkIds.Add(gameObject, netId);
            }

            var networkId = _networkIds[gameObject];
            
            var stream = new MemoryStream();
            
            using (var writer = new BitWriter(stream))
            {
                writer.Write((byte) MessageIdentifiers.ReplicaManagerConstruction);
                writer.WriteBit(true);
                writer.Write(networkId);

                gameObject.WriteConstruct(writer);
            }

            var data = stream.ToArray();
            foreach (var endPoint in recipients)
            {
                Server.Send(data, endPoint);
            }
        }

        public void SendSerialization(GameObject gameObject, ICollection<IPEndPoint> recipients = null)
        {
            if (recipients == null) recipients = Players.Select(p => p.EndPoint).ToArray();
            
            var stream = new MemoryStream();

            using (var writer = new BitWriter(stream))
            {
                writer.Write((byte) MessageIdentifiers.ReplicaManagerSerialize);
                writer.Write(_networkIds[gameObject]);

                gameObject.WriteSerialize(writer);
            }

            var data = stream.GetBuffer();
            foreach (var endPoint in recipients)
            {
                Server.Send(data, endPoint);
            }
        }

        public void SendDestruction(GameObject gameObject, ICollection<IPEndPoint> recipients = null)
        {
            if (recipients == null) recipients = Players.Select(p => p.EndPoint).ToArray();
            
            var stream = new MemoryStream();

            var netId = _networkIds[gameObject];
            _droppedIds.Add(netId);
            _networkIds.Remove(gameObject);
            
            using (var writer = new BitWriter(stream))
            {
                writer.Write((byte) MessageIdentifiers.ReplicaManagerDestruction);
                writer.Write(netId);
            }

            var data = stream.GetBuffer();
            foreach (var endPoint in recipients)
            {
                Server.Send(data, endPoint);
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

                foreach (var obj in Objects)
                {
                    obj.Update();
                }

                _ticks++;
                _passedTickTime += stopWatch.ElapsedMilliseconds;

                stopWatch.Restart();
            }
        }
    }
}