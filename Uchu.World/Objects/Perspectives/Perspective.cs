using System;
using System.Collections.Generic;
using System.Linq;

namespace Uchu.World
{
    public class Perspective
    {
        private readonly Stack<ushort> _droppedIds;
        private readonly List<GameObject> _hallucinations;

        private readonly Dictionary<GameObject, ushort> _networkDictionary;
        private readonly Player _player;

        private Mask _viewMask;

        public Perspective(Player player, Mask mask)
        {
            _networkDictionary = new Dictionary<GameObject, ushort>();

            _hallucinations = new List<GameObject>();

            _droppedIds = new Stack<ushort>();

            _player = player;

            _viewMask = mask;
        }

        public Mask ViewMask
        {
            get => _viewMask;
            set
            {
                _viewMask = value;

                var passableGameObjects = _player.Zone.GameObjects.Where(
                    g => g.Layer == _viewMask && !_networkDictionary.ContainsKey(g)
                ).ToArray();

                foreach (var gameObject in passableGameObjects)
                    _player.Zone.SendConstruction(gameObject, _player);

                var revealedGameObjects = _networkDictionary.Keys.Where(
                    gameObject => gameObject.Layer != _viewMask
                ).ToArray();

                foreach (var gameObject in revealedGameObjects)
                    gameObject.Zone.SendDestruction(gameObject, _player);
            }
        }

        public void Reveal(GameObject gameObject, Action<ushort> callback)
        {
            if (!gameObject.Alive || _networkDictionary.ContainsKey(gameObject)) return;

            if (ViewMask != gameObject.Layer)
            {
                gameObject.OnLayerChanged += layer =>
                {
                    if (ViewMask == layer) Reveal(gameObject, callback);
                };

                return;
            }

            gameObject.OnLayerChanged += layer =>
            {
                if (ViewMask != layer) gameObject.Zone.SendDestruction(gameObject, _player);
                else Reveal(gameObject, callback);
            };

            if (!_droppedIds.TryPop(out var networkId))
            {
                if (_networkDictionary.Any()) networkId = (ushort) (_networkDictionary.Values.Max() + 1);
                else networkId = 1;
            }

            _networkDictionary.Add(gameObject, networkId);

            callback(networkId);
        }

        public void Drop(GameObject gameObject)
        {
            if (!_networkDictionary.TryGetValue(gameObject, out var id)) return;

            _droppedIds.Push(id);
            _networkDictionary.Remove(gameObject);

            if (_hallucinations.Contains(gameObject))
                _hallucinations.Remove(gameObject);
        }

        public void Hallucinate(GameObject gameObject)
        {
            if (_networkDictionary.ContainsKey(gameObject)) return;

            _hallucinations.Add(gameObject);
            gameObject.Zone.SendConstruction(gameObject, _player);
        }

        public bool TryGetNetworkId(GameObject gameObject, out ushort id)
        {
            return _networkDictionary.TryGetValue(gameObject, out id);
        }
    }
}