using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Filters
{
    public class ExcludeFilter : IPerspectiveFilter
    {
        private Player _player { get; set; }
        
        private List<GameObject> _excluded { get; set; }

        public GameObject[] Excluded => _excluded.ToArray();
        
        public void Initialize(Player player)
        {
            _player = player;
            
            _excluded = new List<GameObject>();
        }

        public async Task Tick()
        {
            var filterTasks = _player.Zone.Objects.OfType<FilterComponent>()
                .Select(component => Task.Run(async () =>
                {
                    var excluded = _excluded.Contains(component.GameObject);
                    var check = await component.CheckAsync(_player);

                    if (excluded && check)
                    {
                        _excluded.Remove(component.GameObject);
                    }
                    else if (!excluded && !check)
                    {
                        _excluded.Add(component.GameObject);
                    }
                }))
                .ToList();
            await Task.WhenAll(filterTasks);
            
            for (var i = 0; i < _excluded.Count; i++)
            {
                var gameObject = _excluded[i];
                if (!gameObject.Alive)
                    _excluded.Remove(gameObject);
            }
        }

        public bool View(GameObject gameObject)
        {
            return !_excluded.Contains(gameObject);
        }

        public void Exclude(GameObject gameObject)
        {
            _excluded.Add(gameObject);
        }

        public void Include(GameObject gameObject)
        {
            _excluded.Remove(gameObject);
        }
    }
}