using System.Collections.Generic;
using Uchu.Core.Resources;

namespace Uchu.World
{
    /// <summary>
    /// Filter to hide a game if a player doesn't have the correct filter to show it
    /// </summary>
    public class MissionFilterComponent : Component
    {
        public MissionFilterComponent()
        {
            MissionIdFilter = new List<MissionId>();
        }
        
        /// <summary>
        /// List of mission ids required to show 
        /// </summary>
        private List<MissionId> MissionIdFilter { get; set; }

        /// <summary>
        /// Adds a mission id to the filter, if a player has this mission active the game object will be hidden
        /// </summary>
        /// <param name="missionId"></param>
        public void AddMissionIdToFilter(MissionId missionId)
        {
            lock (MissionIdFilter)
            {
                MissionIdFilter.Add(missionId);
            }
        }

        /// <summary>
        /// Checks if the player has an active mission from the filter
        /// </summary>
        /// <param name="player">The player to check the missions inventory of</param>
        /// <returns><c>true</c> if the player has an active mission from the filter, <c>false</c> otherwise</returns>
        public bool Show(Player player)
        {
            if (MissionIdFilter.Count > 0)
            {
                var inventory = player.GetComponent<MissionInventoryComponent>();
                foreach (var missionId in MissionIdFilter)
                {
                    if (inventory.HasActive((int)missionId))
                        return true;
                }
            }

            return false;
        }
    }
}