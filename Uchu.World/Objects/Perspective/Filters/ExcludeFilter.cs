using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Filters
{
    /// <summary>
    /// Handles hiding / showing game objects
    /// </summary>
    public class ExcludeFilter : IPerspectiveFilter
    {
        /// <summary>
        /// The player to which this filter belongs
        /// </summary>
        private Player Player { get; set; }
        
        /// <summary>
        /// The game objects that are in the filter
        /// </summary>
        private List<GameObject> Excluded { get; set; }

        /// <summary>
        /// Initializes the filter
        /// </summary>
        /// <param name="player">The player to which the filter should belong</param>
        public void Initialize(Player player)
        {
            Player = player;
            Excluded = new List<GameObject>();
        }

        /// <summary>
        /// Ticks the exclude filter, updating visible objects based on the filter components in the filter
        /// </summary>
        public async Task Tick()
        {
            var filterTasks = Player.Zone.Objects.OfType<MissionFilterComponent>()
                .Select(component => Task.Run(() =>
                {
                    var excluded = Excluded.Contains(component.GameObject);
                    var check = component.Check(Player);

                    if (excluded && check)
                    {
                        Excluded.Remove(component.GameObject);
                    }
                    else if (!excluded && !check)
                    {
                        Excluded.Add(component.GameObject);
                    }
                })).ToList();
            
            await Task.WhenAll(filterTasks);
            
            for (var i = 0; i < Excluded.Count; i++)
            {
                var gameObject = Excluded[i];
                if (!gameObject.Alive)
                    Excluded.Remove(gameObject);
            }
        }

        /// <summary>
        /// Whether a player can see a game object
        /// </summary>
        /// <param name="gameObject">The game object to show</param>
        /// <returns>Whether a player can see a game object</returns>
        public bool View(GameObject gameObject)
        {
            return !Excluded.Contains(gameObject);
        }

        /// <summary>
        /// Exclude a game object from the player vision, hiding it
        /// </summary>
        /// <param name="gameObject">The game object to hide</param>
        public void Exclude(GameObject gameObject)
        {
            Excluded.Add(gameObject);
        }

        /// <summary>
        /// Include a game object into the filter vision, showing it
        /// </summary>
        /// <param name="gameObject">The game object to include</param>
        public void Include(GameObject gameObject)
        {
            Excluded.Remove(gameObject);
        }
    }
}