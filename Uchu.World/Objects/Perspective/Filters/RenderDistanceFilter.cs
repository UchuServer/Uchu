using System.Linq;
using System.Numerics;
using Uchu.Core.Client;

namespace Uchu.World.Filters
{
    public class RenderDistanceFilter : IPerspectiveFilter
    {
        public float Distance { get; set; }

        public Player Player { get; set; }

        public void Initialize(Player player)
        {
            Player = player;
            
            using var cdClient = new CdClientContext();

            var zone = cdClient.ZoneTableTable.FirstOrDefault(z => z.ZoneID == (int) Player.Zone.ZoneId);

            Distance = zone?.Ghostdistance ?? 500;
        }
        
        public bool View(GameObject gameObject)
        {
            if (gameObject?.Transform == default) return false;
            
            return Vector3.Distance(gameObject.Transform.Position, Player.Transform.Position) <= Distance;
        }
    }
}