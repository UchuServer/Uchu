using System.Linq;
using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    [ScriptName("ScriptComponent_1419_script_name__removed")]
    public class ImaginationFountain : ObjectScript
    {
        /// <summary>
        /// Imagination drop LOT to total.
        /// </summary>
        private static readonly (Lot, int)[] ImaginationDrops = {
            (Lot.Imagination, 1),
            (Lot.TwoImagination, 2),
            (Lot.ThreeImagination, 3),
            (Lot.FiveImagination, 5),
            (Lot.TenImagination, 10)
        };
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ImaginationFountain(GameObject gameObject) : base(gameObject)
        {
            // Listen to the fountain being interacted with.
            Listen(gameObject.OnInteract, player =>
            {
                if (!player.TryGetComponent<DestroyableComponent>(out var stats)) return;
                var toGive = (int) stats.MaxImagination;
                while (toGive > 0)
                {
                    // Get the imagination to drop.
                    var array = ImaginationDrops.Where((_, i) => i >= toGive).ToArray();
                    var (lot, cost) = array.Length == 0 ? ImaginationDrops.Last() : array.Max();
                    toGive -= cost;
                    
                    // Drop the loot.
                    var loot = InstancingUtilities.InstantiateLoot(lot, player, gameObject, gameObject.Transform.Position+ Vector3.UnitY * 3);
                    Start(loot);
                }
            });
        }
    }
}