using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("ScriptComponent_1537_script_name__removed")]
    public class MaelstromConverter : ObjectScript
    {
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
                inventory.RemoveLotAsync(Lot.MaelstromInfectedBrick, 25);
                inventory.AddLotAsync(Lot.FactionTokenProxy, 5);

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
