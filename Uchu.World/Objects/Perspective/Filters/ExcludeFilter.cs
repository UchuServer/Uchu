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
        public Task Tick()
        {
            foreach (var filter in Player.Zone.Objects.OfType<MissionFilterComponent>())
            {
                var excluded = Excluded.Contains(filter.GameObject);
                var show = filter.Show(Player);

                if (show && excluded)
                {
                    Include(filter.GameObject);
                }
                else if (!show && !excluded)
                {
                    Exclude(filter.GameObject);
                }
            }

            return Task.CompletedTask;
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