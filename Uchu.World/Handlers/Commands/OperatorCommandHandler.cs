using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Uchu.Api.Models;
using Uchu.Core;
using Uchu.World.Scripting.Managed;
using Uchu.World.Social;

namespace Uchu.World.Handlers.Commands
{
    public class OperatorCommandHandler : HandlerGroup
    {
        [CommandHandler(Signature = "stop", Help = "Stop the server", GameMasterLevel = GameMasterLevel.Operator)]
        public async Task<string> Stop(string[] arguments, Player player)
        {
            await using var ctx = new UchuContext();

            var response = await Server.Api.RunCommandAsync<InstanceListResponse>(
                Server.MasterApi, "instance/list"
            ).ConfigureAwait(false);

            var world = response.Instances.FirstOrDefault(i => i.Type == (int) ServerType.World);

            foreach (var zonePlayer in player.Zone.Players)
            {
                if (world == default)
                {
                    zonePlayer.Message(new DisconnectNotifyPacket
                    {
                        DisconnectId = DisconnectId.ServerShutdown
                    });
                    
                    continue;
                }

                var zone = (ZoneId) world.Zones.First();

                zonePlayer.SendChatMessage($"This zone is closing, going to {zone}!");

                await zonePlayer.SendToWorldAsync(world, zone);
            }

            var delay = 1000;

            if (arguments.Length > 0)
            {
                int.TryParse(arguments[0], out delay);
            }

            await Task.Delay(delay);

            await Server.Api.RunCommandAsync<BaseResponse>(Server.MasterApi, $"instance/decommission?i={Server.Id}")
                .ConfigureAwait(false);
            
            return "Stopped server";
        }

        [CommandHandler(Signature = "save", Help = "Save a serialization", GameMasterLevel = GameMasterLevel.Operator)]
        public async Task<string> SaveSerialize(string[] arguments, Player player)
        {
            var current = player.Zone.GameObjects[0];

            foreach (var gameObject in player.Zone.GameObjects.Where(g => g != player && g != default))
            {
                if (gameObject.Transform == default) continue;

                if (gameObject.GetComponent<SpawnerComponent>() != default) continue;

                if (Vector3.Distance(current.Transform.Position, player.Transform.Position) >
                    Vector3.Distance(gameObject.Transform.Position, player.Transform.Position))
                    current = gameObject;
            }

            var path = Path.Combine(Server.MasterPath, "./packets/");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            Zone.SaveSerialization(current, new[] {player}, Path.Combine(path, $"./{current.ObjectId}_s.bin"));
            Zone.SaveCreation(current, new[] {player}, Path.Combine(path, $"./{current.ObjectId}_c.bin"));

            return "Saved packets";
        }

        [CommandHandler(Signature = "python", Help = "Run python", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Python(string[] arguments, Player player)
        {
            if (arguments.Length <= 1) return $"python <id> <python code to run>";

            var param = arguments.ToList();

            var name = param[0];

            param.RemoveAt(0);

            var source = string.Join(" ", param).Replace(@"\n", "\n").Replace(@"\t", "\t");

            await player.Zone.ScriptManager.SetManagedScript(name, source);

            return "Attempting to run python script...";
        }

        [CommandHandler(Signature = "python-load", Help = "Load a python file", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> PythonLoad(string[] arguments, Player player)
        {
            if (arguments.Length == 0) return "python-load <file>";

            await player.Zone.ScriptManager.SetManagedScript(arguments[0]);
            
            return "Attempting to run python pack...";
        }

        [CommandHandler(Signature = "python-unload", Help = "Unload a python script", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> PythonUnload(string[] arguments, Player player)
        {
            if (arguments.Length == 0) return "python-unload <id>";

            await player.Zone.ScriptManager.SetManagedScript(arguments[0]);
            
            return $"Unloaded: {arguments[0]}";
        }

        [CommandHandler(Signature = "python-list", Help = "List all python scripts", GameMasterLevel = GameMasterLevel.Admin)]
        public string PythonList(string[] arguments, Player python)
        {
            var builder = new StringBuilder();

            builder.Append("Loaded scripts:");

            foreach (var scriptPack in python.Zone.ScriptManager.ScriptPacks.OfType<PythonScriptPack>())
            {
                builder.Append($"\n{scriptPack.Name}");
            }

            return builder.ToString();
        }

        [CommandHandler(Signature = "failed", Help = "Display failed message", GameMasterLevel = GameMasterLevel.Admin)]
        public string Failed(string[] arguments, Player player)
        {
            if (arguments.Length == default) return "failed <id> <...message...>";
            
            var args = arguments.ToList();

            var id = args[0];

            args.RemoveAt(0);

            if (!uint.TryParse(id, out var eId)) return "Invalid <id>";

            var message = string.Join(" ", args);
            
            player.Message(new NotifyClientFailedPreconditionMessage
            {
                Associate = player,
                Id = (int) eId,
                Reason = message
            });

            return "Sent failed condition.";
        }
        
        [CommandHandler(Signature = "state", Help = "Send UI state message", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> State(string[] arguments, Player player)
        {
            var state = string.Join(" ", arguments);

            await UiHelper.StateAsync(player, state);
            
            return "Sent ui message.";
        }
        
        [CommandHandler(Signature = "toggle", Help = "Send toggle ui message", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Toggle(string[] arguments, Player player)
        {
            if (arguments.Length != 2) return "toggle <name> <state>";
            
            await UiHelper.ToggleAsync(player, arguments[0], !bool.TryParse(arguments[1], out var state) || state);
            
            return "Sent ui message.";
        }

        [CommandHandler(Signature = "imagination", Help = "Send imagination ui message", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Imagination(string[] arguments, Player player)
        {
            var current = uint.Parse(arguments[0]);

            var max = uint.Parse(arguments[1]);

            await player.MessageGuiAsync("SetImagination", new Dictionary<string, object>()
            {
                {"imaginationMax", max},
                {"imagination", current}
            });

            return "Sent";
        }

        [CommandHandler(Signature = "ping", Help = "Get you average ping", GameMasterLevel = GameMasterLevel.Player)]
        public string Ping(string[] arguments, Player player)
        {
            return $"{player.Ping}ms";
        }

        [CommandHandler(Signature = "movement", Help = "Toggle the movement of an npc", GameMasterLevel = GameMasterLevel.Admin)]
        public string Movement(string[] arguments, Player player)
        {
            var targets = new List<GameObject>();

            Ai(arguments, player);
            
            if (arguments.Contains("all"))
            {
                targets = player.Zone.Objects.OfType<MovementAiComponent>().Select(
                    m => m.GameObject
                ).ToList();
            }
            else
            {
                var current = player.Zone.GameObjects.First();
                foreach (var gameObject in player.Zone.GameObjects.Where(g => g != player && g != default))
                {
                    if (gameObject.Transform == default) continue;

                    if (gameObject.GetComponent<SpawnerComponent>() != default) continue;

                    if (Vector3.Distance(current.Transform.Position, player.Transform.Position) >
                        Vector3.Distance(gameObject.Transform.Position, player.Transform.Position))
                        current = gameObject;
                }

                targets.Add(current);
            }

            foreach (var current in targets)
            {
                if (!current.TryGetComponent<MovementAiComponent>(out var movementAiComponent))
                    return $"{current} does not have a movement AI component";

                movementAiComponent.Enabled = !movementAiComponent.Enabled;
            }

            return "Toggled movement for agents";
        }

        [CommandHandler(Signature = "ai", Help = "Toggle the ai all npcs", GameMasterLevel = GameMasterLevel.Admin)]
        public static string Ai(string[] arguments, Player player)
        {
            foreach (var component in player.Zone.Objects.OfType<BaseCombatAiComponent>())
            {
                component.Enabled = true;
            }

            return "Toggled";
        }

        [CommandHandler(Signature = "target", Help = "Get target of npc", GameMasterLevel = GameMasterLevel.Admin)]
        public string Target(string[] arguments, Player player)
        {
            var current = player.Zone.GameObjects.First();
            foreach (var gameObject in player.Zone.GameObjects.Where(g => g != player && g != default))
            {
                if (gameObject.Transform == default) continue;

                if (gameObject.GetComponent<SpawnerComponent>() != default) continue;

                if (Vector3.Distance(current.Transform.Position, player.Transform.Position) >
                    Vector3.Distance(gameObject.Transform.Position, player.Transform.Position))
                    current = gameObject;
            }

            if (!current.TryGetComponent<BaseCombatAiComponent>(out var baseCombatAiComponent)) return "Invalid nearby";

            return $"Target: {baseCombatAiComponent.Target}";
        }
    }
}