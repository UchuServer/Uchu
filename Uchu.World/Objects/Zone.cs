using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public new readonly Server Server;

        private readonly Dictionary<GameObject, ushort> _networkIds = new Dictionary<GameObject, ushort>();

        private readonly List<ushort> _droppedIds = new List<ushort>();

        public Zone(ZoneInfo zoneInfo, Server server)
        {
            Zone = this;
            ZoneInfo = zoneInfo;
            Server = server;
        }

        public void Initialize()
        {
            foreach (var levelObject in ZoneInfo.ScenesInfo.SelectMany(s => s.Objects))
            {
                var obj = GameObject.Instantiate(levelObject, this);
            }
        }

        public void RequestConstruction(Player player)
        {
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
            // TODO: Add filters
            SendDestruction(gameObject, Players.Select(p => p.EndPoint).ToArray());
        }

        private void SendConstruction(GameObject gameObject, ICollection<IPEndPoint> recipients = null)
        {
            if (recipients == null) recipients = Players.Select(p => p.EndPoint).ToArray();

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

                Logger.Debug($"Construction [{gameObject.Lot}] {gameObject} [{netId}]");
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

            var data = stream.GetBuffer();
            foreach (var endPoint in recipients)
            {
                Server.Send(data, endPoint);
            }
        }

        private void SendSerialization(GameObject gameObject, ICollection<IPEndPoint> recipients = null)
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

        private void SendDestruction(GameObject gameObject, ICollection<IPEndPoint> recipients = null)
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
    }
}