using System.Collections.Generic;
using System.Linq;

namespace Uchu.World
{
    public class Perspective
    {
        private readonly Dictionary<GameObject, ushort> _networkDictionary;

        private readonly Stack<ushort> _droppedIds;

        public Perspective()
        {
            _networkDictionary = new Dictionary<GameObject, ushort>();
            
            _droppedIds = new Stack<ushort>();
        }

        public ushort Reveal(GameObject gameObject)
        {
            if (!_droppedIds.TryPop(out var networkId))
            {
                if (_networkDictionary.Any()) networkId = (ushort) (_networkDictionary.Values.Max() + 1);
                else networkId = 1;
            }

            _networkDictionary.Add(gameObject, networkId);

            return networkId;
        }

        public void Drop(GameObject gameObject)
        {
            if (!_networkDictionary.TryGetValue(gameObject, out var id)) return;
            
            _droppedIds.Push(id);
            _networkDictionary.Remove(gameObject);
        }
        
        public bool TryGetNetworkId(GameObject gameObject, out ushort id)
        {
            return _networkDictionary.TryGetValue(gameObject, out id);
        }
    }
}