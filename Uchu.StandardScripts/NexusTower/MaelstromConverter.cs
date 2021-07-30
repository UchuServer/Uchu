using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("ScriptComponent_1537_script_name__removed")]
    public class MaelstromConverter : ObjectScript
    {
        private const int BrickCount = 25;

        private const int FactionTokenCount = 5;

        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public MaelstromConverter(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnInteract, player =>
            {
                // Let the player trade 25 Maelstrom Infected Bricks for 5 faction tokens
                var inventory = player.GetComponent<InventoryManagerComponent>();
                // Ensure player has at least 25 Maelstrom Infected Bricks
                if (inventory.FindItem(Lot.MaelstromInfectedBrick, InventoryType.Items, BrickCount) == default)
                    return;
                inventory.RemoveLotAsync(Lot.MaelstromInfectedBrick, BrickCount);
                inventory.AddLotAsync(Lot.FactionTokenProxy, FactionTokenCount);

                // Terminate interaction so the player can interact again.
                player.Message(new TerminateInteractionMessage
                {
                    Associate = player,
                    Terminator = gameObject,
                    Type = TerminateType.FromInteraction,
                });
            });
        }
    }
}
