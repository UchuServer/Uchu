using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    public class ImaginationFountain : NativeScript
    {
        private readonly (Lot, int)[] ImaginationDrops = {
            (Lot.Imagination, 1),
            (Lot.TwoImagination, 2),
            (Lot.ThreeImagination, 3),
            (Lot.FiveImagination, 5),
            (Lot.TenImagination, 10)
        };
        
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects.Where(g => g.Lot == 12940))
            {
                Listen(gameObject.OnInteract, player =>
                {
                    if (!player.TryGetComponent<DestroyableComponent>(out var stats)) return;

                    var toGive = (int) stats.MaxImagination;

                    while (toGive > 0)
                    {
                        var array = ImaginationDrops.Where((_, i) => i >= toGive).ToArray();

                        var (lot, cost) = array.Length == 0 ? ImaginationDrops.Last() : array.Max();

                        toGive -= cost;

                        var loot = InstancingUtilities.InstantiateLoot(lot, player, gameObject, gameObject.Transform.Position+ Vector3.UnitY * 3);

                        Start(loot);
                    }
                });
            }
            
            return Task.CompletedTask;
        }
    }
}