using System.Threading.Tasks;
using Uchu.World.Scripting.Lua;

namespace Uchu.World.Scripts.AvantGardens
{
    public class BabySpiderMission : LuaNativeScript
    {
        public override string ScriptName { get; set; } = "ScriptComponent_1586_script_name__removed";

        public override Task LoadAsync(GameObject self)
        {
            Listen(self.OnInteract, async player =>
            {
                await player.SetFlagAsync(74, true);

                foreach (var spider in GetGroup("cagedSpider"))
                {
                    player.Message(new FireClientEventMessage
                    {
                        Associate = spider,
                        Sender = player,
                        Arguments = "toggle",
                        Target = player
                    });
                }

                if (player.TryGetComponent<InventoryManagerComponent>(out var inventoryManager)) await inventoryManager.RemoveAllAsync(14553);
            });
            
            return Task.CompletedTask;
        }
    }
}