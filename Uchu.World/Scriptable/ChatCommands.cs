using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Packets.Server.GameMessages;

namespace Uchu.World.Scriptable
{
    /// <summary>
    ///     Manage chat commends.
    /// </summary>
    public static class ChatCommands
    {
        /*
         * Purely for experimental purposes. Has to be developed on.
         */

        public static string NearCommand(string[] args, Player player)
        {
            var comp =
                player.ReplicaPacket.Components.First(c => c is ControllablePhysicsComponent) as
                    ControllablePhysicsComponent;
            if (comp == null) return "Error\0";

            var closest = (ReplicaPacket) null;
            foreach (var replica in player.World.Replicas)
            {
                if (closest == null)
                {
                    closest = replica;
                    continue;
                }
                if (Vector3.Distance(comp.Position, replica.Position) <
                    Vector3.Distance(comp.Position, closest.Position))
                {
                    closest = replica;
                }
            }

            return closest == null
                ? "No Objects in this world!\0"
                : $"ID: {closest.ObjectId}\nLOT: {closest.LOT}\nPOS: {closest.Position}\nROT: {closest.Rotation}\0";
        }
        
        public static string StateCommand(string[] args, Player player)
        {
            switch (args[0].ToLower())
            {
                case "off":
                    MovingPlatformScript.StateOff = (PlatformState) int.Parse(args[1]);
                    return $"Turned OFF {MovingPlatformScript.StateOff}\0";
                case "on":
                    MovingPlatformScript.StateOn = (PlatformState) int.Parse(args[1]);
                    return $"Turned ON {MovingPlatformScript.StateOn}\0";
                    break;
                default:
                    return "Not a pos!\0";
            }
        }

        public static async Task<string> Complete(string[] args, Player player)
        {
            using (var ctx = new UchuContext())
            {
                Console.WriteLine($"Complete Command form {player.EndPoint}");
                if (args.Length == 0)
                {
                    foreach (var mission in ctx.Missions.Where(m => m.CharacterId == player.CharacterId && m.CompletionCount == 0))
                    {
                        Console.WriteLine($"Mission {mission} is completing...");
                        var missionRow = await player.Server.CDClient.GetMissionAsync(mission.MissionId);
                        await player.CompleteMissionAsync(missionRow);
                    }
                    return "Completed all Missions!\0";
                }
                
                {
                    if (!int.TryParse(args[0], out var i))
                    {
                        return $"{args[0]} is not a mission id!\0";
                    }
                    
                    var missionRow = await player.Server.CDClient.GetMissionAsync(int.Parse(args[0]));
                    await player.CompleteMissionAsync(missionRow);
                    return $"{args[0]} completed!\0";
                }
            }
        }
        
        public static async Task<string> GiveCommand(string[] args, Player player)
        {
            if (args.Length > 2) player = player.World.Players.First(p => p.ReplicaPacket.Name == args[2]);

            if (args.Length < 1) return "Command Syntax: /give <lot> <count(optional)> <player(optional)>\0";

            try
            {
                await player.AddItemAsync(int.Parse(args[0]), args.Length > 1 ? int.Parse(args[1]) : 1);
            }
            catch
            {
                return $"{args[0]} is not a LOT.\0";
            }

            return $"Gave LOT: {args[0]}{(args.Length > 1 ? $" x {args[1]}" : "")} to {player.ReplicaPacket.Name}.\0";
        }

        public static string FlyCommand(string[] args, Player player)
        {
            /*
             * Flight does not work.
             */

            if (args.Length == 0) return "Command Syntax: /fly <true/false>\0";
            bool on;
            switch (args[0].ToLower())
            {
                case "true":
                    on = true;
                    break;
                case "false":
                    on = false;
                    break;
                default:
                    return "Command Syntax: /fly <true/false>\0";
            }

            player.World.Server.Send(new SetJetPackModeMessage
            {
                BypassChecks = true,
                DoHover = true,
                Use = on
            }, player.EndPoint);
            return $"Flight set to {on}\0";
        }
    }
}