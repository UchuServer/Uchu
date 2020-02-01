using System.Collections.Generic;
using System.Threading.Tasks;

namespace Uchu.World.Filters
{
    public class ExcludeFilter : IPerspectiveFilter
    {
        private List<GameObject> _excluded;

        public GameObject[] Excluded => _excluded.ToArray();
        
        public void Initialize(Player player)
        {
            _excluded = new List<GameObject>();
        }

        public Task Tick()
        {
            for (var index = 0; index < _excluded.Count; index++)
            {
                var gameObject = _excluded[index];
                
                if (!gameObject.Alive) _excluded.Remove(gameObject);
            }
            
            return Task.CompletedTask;
        }

        public bool View(GameObject gameObject)
        {
            return !_excluded.Contains(gameObject);
        }

        public void Exclude(GameObject gameObject) => _excluded.Add(gameObject);

        public void Include(GameObject gameObject) => _excluded.Remove(gameObject);
    }
}