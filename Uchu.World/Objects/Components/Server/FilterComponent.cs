using System.Collections.Generic;
using Uchu.Core.Resources;

namespace Uchu.World
{
    public class MissionFilterComponent : Component
    {
        public List<MissionId> MissionIdFilter { get; set; }

        public MissionFilterComponent()
        {
            MissionIdFilter = new List<MissionId>();
        }

        public void AddMissionIdToFilter(MissionId missionId)
        {
            lock (MissionIdFilter)
            {
                MissionIdFilter.Add(missionId);
            }
        }

        public bool Check(Player player)
        {
            if (MissionIdFilter.Count > 0)
            {
                var inventory = player.GetComponent<MissionInventoryComponent>();
                foreach (var missionId in MissionIdFilter)
                {
                    if (inventory.HasActive((int)missionId))
                        return false;
                }
            }

            return true;
        }
    }
}