using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public IEnumerable<GameObject> LoadedObjects => _networkDictionary.Keys;

        public Event OnLoaded { get; } = new Event();

        private List<IPerspectiveFilter> Filters { get; } = new List<IPerspectiveFilter>();

        public Perspective(Player player)
        {
            _networkDictionary = new Dictionary<GameObject, ushort>();

            _droppedIds = new Stack<ushort>();

            _player = player;
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

                _networkDictionary[gameObject] = networkId;

                return true;
            }
        }

        internal void Drop(GameObject gameObject)
        {
            lock (gameObject)
            {
                if (!_networkDictionary.TryGetValue(gameObject, out var id)) return;
                _droppedIds.Push(id);
                _networkDictionary.Remove(gameObject);
            }
        }

        internal bool View(GameObject gameObject)
        {
            return Filters.All(filter => filter.View(gameObject));
        }

        internal bool TryGetNetworkId(GameObject gameObject, out ushort id)
        {
            return _networkDictionary.TryGetValue(gameObject, out id);
        }

        internal async Task TickAsync()
        {
            foreach (var filter in Filters)
            {
                await filter.Tick();
            }
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

            var instance = new T();
            
            instance.Initialize(_player);

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