using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World.Filters
{
    public class RenderDistanceFilter : IPerspectiveFilter
    {
        public float Distance { get; set; }

        public Player Player { get; set; }
        
        public bool Override { get; set; }
        
        public Vector3 OverrideReferencePosition { get; set; }

        private List<GameObject> BypassFilter { get; set; }

        public void Initialize(Player player)
        {
            Player = player;
            var zone = ClientCache.Find<ZoneTable>((int) Player.Zone.ZoneId);

            Distance = zone?.Ghostdistance ?? 500;

            this.BypassFilter = new List<GameObject>();
        }

        public Task Tick()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Determines if the gameObject is within the player's render distance.
        /// Players, and any object in the <see cref="BypassFilter"/> list, will
        /// always pass this check, regardless of distance.
        /// </summary>
        /// <param name="gameObject">Object to check the distance for.</param>
        /// <returns>Whether the object passes the render distance check.</returns>
        public bool View(GameObject gameObject)
        {
            if (gameObject?.Transform == default)
                return false;

            if (gameObject is Player)
                return true;

            if (this.BypassFilter.Contains(gameObject))
                return true;

            if (Override && Vector3.Distance(gameObject.Transform.Position, OverrideReferencePosition) <= Distance)
                return true;

            return Vector3.Distance(gameObject.Transform.Position, Player.Transform.Position) <= Distance;
        }

        /// <summary>
        /// Add an override to make an object bypass the filter.
        /// This ensures a player receives object construction even when out of range.
        /// </summary>
        /// <param name="gameObject">Object to add the override for.</param>
        public void AddBypassFilter(GameObject gameObject)
        {
            if (this.BypassFilter.Contains(gameObject))
                return;

            this.BypassFilter.Add(gameObject);
        }

        /// <summary>
        /// Remove an override added through <see cref="AddBypassFilter"/>.
        /// </summary>
        /// <param name="gameObject">Object to remove the override for.</param>
        public void RemoveBypassFilter(GameObject gameObject)
        {
            this.BypassFilter.Remove(gameObject);
        }
    }
}
