using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ScriptName("ScriptComponent_1586_script_name__removed")]
    public class BabySpiderMission : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public BabySpiderMission(GameObject gameObject) : base(gameObject)
        {
            // Listen to the caged spider pile being interacted with.
            Listen(gameObject.OnInteract, async player =>
            {
                if (player.TryGetComponent<CharacterComponent>(out var character))
                {
                    // Set the character flag and toggle the caged spider.
                    await character.SetFlagAsync(74, true);
                    foreach (var spider in this.GetGroup("cagedSpider"))
                    {
                        player.Message(new FireEventClientSideMessage
                        {
                            Associate = spider,
                            Sender = player,
                            Arguments = "toggle",
                            Target = player
                        });
                    }

                    // Remove the cube from the character's inventory.
                    if (player.TryGetComponent<InventoryManagerComponent>(out var inventoryManager))
                    {
                        await inventoryManager.RemoveAllAsync(14553);
                    }
                }
            });
        }
    }
}