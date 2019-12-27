using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Uchu.Core;
using Uchu.World.Filters;

namespace Uchu.World
{
    public class Perspective
    {
        private readonly Stack<ushort> _droppedIds;

        private readonly Dictionary<GameObject, ushort> _networkDictionary;
        private readonly Player _player;

        private uint _clientLoadedObjectCount;

        public IEnumerable<GameObject> LoadedObjects => _networkDictionary.Keys;

        public Event OnLoaded { get; } = new Event();

        private List<IPerspectiveFilter> Filters { get; } = new List<IPerspectiveFilter>();
        
        public uint ClientLoadedObjectCount
        {
            get => _clientLoadedObjectCount;
            set
            {
                Logger.Debug($"PROGRESS: {value}/{_networkDictionary.Count}");
                
                if (value + 10 >= _networkDictionary.Count)
                {
                    OnLoaded.Invoke();
                    OnLoaded.Clear();

                    _clientLoadedObjectCount = default;
                }

                _clientLoadedObjectCount = value;
            }
        }

        public Perspective(Player player)
        {
            _networkDictionary = new Dictionary<GameObject, ushort>();

            _droppedIds = new Stack<ushort>();

            _player = player;

            player.OnTick.AddListener(() =>
            {
                foreach (var gameObject in _player.Zone.GameObjects)
                {
                    if (Filters.Any(f => !f.View(gameObject)))
                    {
                        Zone.SendDestruction(gameObject, _player);

                        continue;
                    }

                    Hallucinate(gameObject);
                }
            });
        }

        internal bool Reveal(GameObject gameObject, out ushort networkId)
        {
            lock (gameObject)
            {
                if (!gameObject.Alive || _networkDictionary.ContainsKey(gameObject))
                {
                    networkId = 0;
                    
                    return false;
                };
                
                if (!_droppedIds.TryPop(out networkId))
                {
                    if (_networkDictionary.Any()) networkId = (ushort) (_networkDictionary.Values.Max() + 1);
                    else networkId = 1;
                }

                _networkDictionary.Add(gameObject, networkId);

                return true;
            }
        }

        public void Drop(GameObject gameObject)
        {
            lock (gameObject)
            {
                if (!_networkDictionary.TryGetValue(gameObject, out var id)) return;
                _droppedIds.Push(id);
                _networkDictionary.Remove(gameObject);
            }
        }

        public void Hallucinate(GameObject gameObject)
        {
            lock (gameObject)
            {
                if (_networkDictionary.ContainsKey(gameObject)) return;

                Zone.SendConstruction(gameObject, _player);
            }
        }

        public bool View(GameObject gameObject)
        {
            return Filters.All(filter => filter.View(gameObject));
        }

        public bool TryGetNetworkId(GameObject gameObject, out ushort id)
        {
            return _networkDictionary.TryGetValue(gameObject, out id);
        }

        public T GetFilter<T>() where T : IPerspectiveFilter => Filters.OfType<T>().First();

        public bool TryGetFilter<T>(out T value) where T : IPerspectiveFilter
        {
            value = Filters.OfType<T>().FirstOrDefault();

            return value != null;
        }

        public T AddFilter<T>() where T : IPerspectiveFilter, new()
        {
            if (TryGetFilter<T>(out _)) throw new ArgumentException($"Can only have one {nameof(IPerspectiveFilter)} of {typeof(T)}");

            var instance = new T
            {
                Player = _player
            };
            
            instance.Start();

            Filters.Add(instance);

            return instance;
        }

        public bool TryAddFilter<T>(out T value) where T : IPerspectiveFilter, new()
        {
            if (TryGetFilter<T>(out _))
            {
                value = default;
                
                return false;
            }

            value = AddFilter<T>();

            return true;
        }
    }
}