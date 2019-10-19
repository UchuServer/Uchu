using System.Linq;
using System.Numerics;
using Uchu.Core;
using Uchu.World.Experimental;

namespace Uchu.World.Handlers.Commands
{
    public class UniverseEditorCommandHandler : HandlerGroup
    {
        [CommandHandler(Signature = "editor", Help = "Enable Universe Editor", GameMasterLevel = GameMasterLevel.Admin)]
        public string EnableUniverseEditor(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
                return "editor <on/off>";

            switch (arguments[0].ToLower())
            {
                case "on":
                case "true":

                    //
                    // Add Universe Editor Component
                    //
                    
                    if (!player.TryGetComponent<UniverseEditor>(out _)) player.AddComponent<UniverseEditor>();

                    return "Enable Universe Editor";
                case "off":
                case "false":
                    
                    //
                    // Remove Universe Editor Component
                    //

                    if (player.TryGetComponent<UniverseEditor>(out var comp)) Object.Destroy(comp);
                    
                    return "Disabled Universe Editor";
                default:
                    return "Invalid <on/off>";
            }
        }

        [CommandHandler(Signature = "target", Help = "Target GameObject", GameMasterLevel = GameMasterLevel.Admin)]
        public string TargetObject(string[] arguments, Player player)
        {
            if (!player.TryGetComponent<UniverseEditor>(out var editor)) editor = player.AddComponent<UniverseEditor>();

            if (arguments.Contains("-z"))
            {
                editor.SetTarget(default);

                return "Zeroed target";
            }
            
            var current = player.Zone.GameObjects[0];

            if (!arguments.Contains("-m"))
                foreach (var gameObject in player.Zone.GameObjects.Where(g => g != player && g != default))
                {
                    if (gameObject.Transform == default || gameObject.GetComponent<SpawnerComponent>() != null)
                        continue;

                    if (Vector3.Distance(current.Transform.Position, player.Transform.Position) >
                        Vector3.Distance(gameObject.Transform.Position, player.Transform.Position))
                        current = gameObject;
                }
            else
                current = player;

            if (current == default) return "No objects in this zone";

            editor.SetTarget(current);

            return $"Targeted {current}";
        }

        [CommandHandler(Signature = "setname", Help = "Set name of GameObject", GameMasterLevel = GameMasterLevel.Admin)]
        public string SetName(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
            {
                return "setname <name>";
            }
            
            if (!player.TryGetComponent<UniverseEditor>(out var editor)) editor = player.AddComponent<UniverseEditor>();

            editor.Target.Name = arguments[0];
            
            player.Zone.BroadcastMessage(new SetNameMessage
            {
                Associate = editor.Target,
                Name = arguments[0]
            });

            return $"Set {editor.Target}'s name to {arguments[0]}";
        }
    }
}