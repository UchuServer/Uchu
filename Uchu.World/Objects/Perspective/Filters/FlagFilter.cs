using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Filters
{
    public class FlagFilter : IPerspectiveFilter
    {
        public Player Player { get; set; }
        
        public float[] Collected { get; set; }
        
        public void Initialize(Player player)
        {
            Player = player;
        }

        public async Task Tick()
        {
            // TODO: Check if this is still necessary
            // if (Player.TryGetComponent<CharacterComponent>(out var character))
            // {
            //     Collected = character.Fl
            // }
        }

        public bool View(GameObject gameObject)
        {
            if (Collected == default) return true;
            
            if (!gameObject.TryGetComponent<CollectibleComponent>(out var collectibleComponent)) return true;

            return !Collected.Contains((float) collectibleComponent.CollectibleId + ((int) gameObject.Zone.ZoneId << 8));
        }
    }
}