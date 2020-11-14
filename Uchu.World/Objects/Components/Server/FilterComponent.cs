using System.Collections.Generic;
using System.Threading.Tasks;
using Uchu.World.Services;

namespace Uchu.World
{
    public class MissionFilterComponent : Component
    {
        public List<int> MissionIdFilter { get; set; }

        public MissionFilterComponent()
        {
            MissionIdFilter = new List<int>();
        }

        public void AddMissionIdToFiler(int id)
        {
            lock (MissionIdFilter)
            {
                MissionIdFilter.Add(id);
            }
        }

        public bool CheckAsync(Player player)
        {
            if (MissionIdFilter.Count > 0)
            {
                var inventory = player.GetComponent<MissionInventoryComponent>();
                foreach (var mission in MissionIdFilter)
                {
                    if (inventory.HasActive(mission))
                        return false;
                }
            }

            return true;
        }
    }
}