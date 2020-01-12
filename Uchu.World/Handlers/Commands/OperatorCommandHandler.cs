using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Python;

namespace Uchu.World.Handlers.Commands
{
    public class OperatorCommandHandler : HandlerGroup
    {
        [CommandHandler(Signature = "stop", Help = "Stop the server", GameMasterLevel = GameMasterLevel.Operator)]
        public async Task<string> Stop(string[] arguments, Player player)
        {
            await using var ctx = new UchuContext();

            var world = await ctx.Specifications.FirstOrDefaultAsync(
                s => s.ServerType == ServerType.World && s.Id != Server.Id
            );

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
                
                zonePlayer.SendChatMessage($"This zone is closing, going to {world.ZoneId}!");
                
                await zonePlayer.SendToWorldAsync(world);
            }

            var delay = 1000;

            if (arguments.Length > 0)
            {
                int.TryParse(arguments[0], out delay);
            }

            await Task.Delay(delay);

            Environment.Exit(0);

            return "Stopped server";
        }

        [CommandHandler(Signature = "python", Help = "Run python", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Python(string[] arguments, Player player)
        {
            if (arguments.Length <= 1) return $"python <id> <python code to run>";

            var param = arguments.ToList();

            var name = param[0];

            param.RemoveAt(0);

            var source = string.Join(" ", param).Replace(@"\n", "\n").Replace(@"\t", "\t");

            var script = new ManagedScript(source, player.Zone.ManagedScriptEngine);

            var success = script.Run();

            if (success)
                player.Zone.ManagedScripts[name] = script;

            return !success ? "Failed" : "";
        }

        [CommandHandler(Signature = "python-load", Help = "Load a python file", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> PythonLoad(string[] arguments, Player player)
        {
            if (arguments.Length == 0) return "python-load <file>";
            
            var source = await File.ReadAllTextAsync(Path.Combine(Server.MasterPath, arguments[0]));

            var managedScript = new ManagedScript(
                source,
                player.Zone.ManagedScriptEngine
            );

            var success = managedScript.Run();

            if (success)
                player.Zone.ManagedScripts[Path.GetFileNameWithoutExtension(arguments[0])] = managedScript;
            
            return !success ? "Failed" : "";
        }

        [CommandHandler(Signature = "python-unload", Help = "Unload a python script", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> PythonUnload(string[] arguments, Player player)
        {
            if (arguments.Length == 0) return "python-unload <id>";

            if (!player.Zone.ManagedScripts.TryGetValue(arguments[0], out _)) return $"No script found with id: {arguments[0]}";

            player.Zone.ManagedScripts.Remove(arguments[0]);
            
            return $"Unloaded: {arguments[0]}";
        }
    }
}