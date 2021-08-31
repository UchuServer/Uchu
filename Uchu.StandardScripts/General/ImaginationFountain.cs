using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    [ScriptName("ScriptComponent_1419_script_name__removed")]
    public class ImaginationFountain : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ImaginationFountain(GameObject gameObject) : base(gameObject)
        {
            // Listen to the fountain being interacted with.
            Listen(gameObject.OnInteract, player =>
            {
                // Drop imagination and, if applicable, a water bottle
                gameObject.GetComponent<DestructibleComponent>().GenerateYieldsAsync(player);

                // Terminate interaction
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
