using System.Collections.Generic;
using System.Threading.Tasks;
using Uchu.World.Services;

namespace Uchu.World
{
    public class FilterComponent : Component
    {
        public string Condition { get; set; }
        
        public List<int> OnMissions { get; set; }

        public FilterComponent()
        {
            OnMissions = new List<int>();
        }

        public async Task<bool> CheckAsync(Player player)
        {
            if (!string.IsNullOrWhiteSpace(Condition))
            {
                if (!await Requirements.CheckRequirementsAsync(Condition, player)) return false;
            }

            if (OnMissions.Count != default)
            {
                var inventory = player.GetComponent<MissionInventoryComponent>();
                
                foreach (var mission in OnMissions)
                {
                    if (!await inventory.OnMissionAsync(mission)) return false;
                }
            }

            return true;
        }
    }
}