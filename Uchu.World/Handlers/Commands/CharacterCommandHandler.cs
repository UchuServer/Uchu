using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Collections;
using Uchu.World.Parsers;

namespace Uchu.World.Handlers.Commands
{
    public class CharacterCommandHandler : HandlerGroup
    {
        [CommandHandler(Signature = "give", Help = "Give an item to yourself", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> GiveItem(string[] arguments, Player player)
        {
            if (arguments.Length == 0 || arguments.Length > 2) return "give <lot> <count(optional)>";

            if (!int.TryParse(arguments[0], out var lot)) return "Invalid <lot>";

            uint count = 1;
            if (arguments.Length == 2)
                if (!uint.TryParse(arguments[1], out count))
                    return "Invalid <count(optional)>";

            await player.GetComponent<InventoryManagerComponent>().AddItemAsync(lot, count);

            return $"Successfully added {lot} x {count} to your inventory";
        }

        [CommandHandler(Signature = "remove", Help = "Remove an item from yourself",
            GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> RemoveItem(string[] arguments, Player player)
        {
            if (arguments.Length == 0 || arguments.Length > 2) return "remove <lot> <count(optional)>";

            if (!int.TryParse(arguments[0], out var lot)) return "Invalid <lot>";

            uint count = 1;
            if (arguments.Length == 2)
                if (!uint.TryParse(arguments[1], out count))
                    return "Invalid <count(optional)>";

            await player.GetComponent<InventoryManagerComponent>().RemoveItemAsync(lot, count);

            return $"Successfully removed {lot} x {count} to your inventory";
        }

        [CommandHandler(Signature = "coin", Help = "Add or remove coin from yourself",
            GameMasterLevel = GameMasterLevel.Admin)]
        public string ChangeCoin(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "coin <delta>";

            if (!int.TryParse(arguments[0], out var delta) || delta == default) return "Invalid <delta>";

            player.Currency += delta;

            return $"Successfully {(delta > 0 ? "added" : "removed")} {delta} coins";
        }

        [CommandHandler(Signature = "spawn", Help = "Spawn an object", GameMasterLevel = GameMasterLevel.Admin)]
        public string Spawn(string[] arguments, Player player)
        {
            if (arguments.Length != 1 || arguments.Length > 4)
                return "spawn <lot> <x(optional)> <y(optional)> <z(optional)>";

            arguments = arguments.Select(a => a.Replace('.', ',')).ToArray();

            if (!int.TryParse(arguments[0], out var lot)) return "Invalid <lot>";

            var position = player.Transform.Position;
            if (arguments.Length >= 4)
                try
                {
                    position = new Vector3
                    {
                        X = float.Parse(arguments[1].Replace('.', ',')),
                        Y = float.Parse(arguments[2].Replace('.', ',')),
                        Z = float.Parse(arguments[3].Replace('.', ','))
                    };
                }
                catch
                {
                    return "Invalid <x(optional)>, <y(optional)>, or <z(optional)>";
                }

            var rotation = player.Transform.Rotation;

            var obj = GameObject.Instantiate(new LevelObject
            {
                Lot = lot,
                Position = position,
                Rotation = rotation,
                Scale = 1,
                Settings = new LegoDataDictionary()
            }, player.Zone);

            Object.Start(obj);
            GameObject.Construct(obj);

            return $"Successfully spawned {lot} at\npos: {position}\nrot: {rotation}";
        }

        [CommandHandler(Signature = "position", Help = "Get your position", GameMasterLevel = GameMasterLevel.Mythran)]
        public string Position(string[] arguments, Player player)
        {
            return $"{player.Transform.Position}";
        }

        [CommandHandler(Signature = "rotation", Help = "Get your rotation", GameMasterLevel = GameMasterLevel.Mythran)]
        public string Rotation(string[] arguments, Player player)
        {
            return $"{player.Transform.Rotation}";
        }

        [CommandHandler(Signature = "smash", Help = "Smash yourself", GameMasterLevel = GameMasterLevel.Admin)]
        public string Smash(string[] arguments, Player player)
        {
            player.GetComponent<DestructibleComponent>().Smash(player, player);

            return "You smashed yourself";
        }

        [CommandHandler(Signature = "freecam", Help = "(Broken)", GameMasterLevel = GameMasterLevel.Admin)]
        public string Freecam(string[] arguments, Player player)
        {
            player.Message(new ToggleFreeCamModeMessage
            {
                Associate = player
            });

            return "Toggled freecam.";
        }

        [CommandHandler(Signature = "fly", Help = "Change jetpack state", GameMasterLevel = GameMasterLevel.Admin)]
        public string Fly(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "fly <state(on/off)>";

            bool state;
            switch (arguments[0].ToLower())
            {
                case "true":
                case "on":
                    state = true;
                    break;
                case "false":
                case "off":
                    state = false;
                    break;
                default:
                    return "Invalid <state(on/off)>";
            }

            player.Message(new SetJetPackModeMessage
            {
                Associate = player,
                BypassChecks = true,
                Use = state,
                EffectId = 36
            });

            return $"Toggled jetpack state: {state}";
        }

        [CommandHandler(Signature = "near", Help = "Get nearest object", GameMasterLevel = GameMasterLevel.Admin)]
        public string Near(string[] arguments, Player player)
        {
            var current = player.Zone.GameObjects[0];

            if (!arguments.Contains("-m"))
                foreach (var gameObject in player.Zone.GameObjects.Where(g => g != player && g != default))
                {
                    if (gameObject.Transform == default) continue;

                    if (gameObject.GetComponent<SpawnerComponent>() != default) continue;

                    if (Vector3.Distance(current.Transform.Position, player.Transform.Position) >
                        Vector3.Distance(gameObject.Transform.Position, player.Transform.Position))
                        current = gameObject;
                }
            else
                current = player;

            if (current == default) return "No objects in this zone.";

            var info = new StringBuilder();

            if (arguments.Contains("-i"))
                info.Append($"[{current.ObjectId}] ");

            info.Append($"[{current.Lot}] \"{current.ClientName}\"");

            if (arguments.Contains("-cq"))
            {
                if (current.TryGetComponent<LuaScriptComponent>(out var scriptComponent))
                    info.AppendLine($"\nScript: {scriptComponent.ScriptName}");
            }

            if (arguments.Contains("-s")) info.Append($"\n{current.Settings}");

            if (arguments.Contains("-l")) info.Append($"\nLayers: {Convert.ToString(current.Layer.Value, 2)}");

            var components = arguments.Contains("-r") ? current.ReplicaComponents : current.GetAllComponents();

            if (!arguments.Contains("-c") && !arguments.Contains("-r")) goto finish;

            foreach (var component in components)
            {
                info.Append($"\n{component.GetType().Name}");

                if (component is StatsComponent stats)
                {
                    info.Append($" {stats.HasStats}");
                }

                if (!arguments.Contains("-p")) continue;

                foreach (var property in component.GetType().GetProperties())
                    info.Append($"\n: {property.Name} = {property.GetValue(component)}");
            }

            finish:

            return info.ToString();
        }

        [CommandHandler(Signature = "score", Help = "Change your U-score", GameMasterLevel = GameMasterLevel.Admin)]
        public string Score(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "score <delta>";

            if (!int.TryParse(arguments[0], out var delta)) return "Invalid <delta>";

            player.UniverseScore += delta;

            GameObject.Serialize(player);

            return $"Successfully {(delta > 0 ? "added" : "removed")} {delta} score";
        }

        [CommandHandler(Signature = "level", Help = "Set your level", GameMasterLevel = GameMasterLevel.Admin)]
        public string Level(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "level <level>";

            if (!long.TryParse(arguments[0], out var level)) return "Invalid <level>";

            player.Level = level;

            GameObject.Serialize(player);

            return $"Successfully set your level to {level}";
        }
        
        [CommandHandler(Signature = "stat", Help = "Set a stat", GameMasterLevel = GameMasterLevel.Admin)]
        public string Stat(string[] arguments, Player player)
        {
            if (arguments.Length != 2) return "stat <stat> <value>";

            if (!long.TryParse(arguments[1], out var value)) return "Invalid <value>";

            var stats = player.GetComponent<Stats>();

            var stat = arguments[0].ToLower().Replace("-", "").Replace("_", "");
            switch (stat)
            {
                case "health":
                    stats.Health = (uint) value;
                    break;
                case "maxhealth":
                    stats.MaxHealth = (uint) value;
                    break;
                case "armor":
                    stats.Armor = (uint) value;
                    break;
                case "maxarmor":
                    stats.MaxArmor = (uint) value;
                    break;
                case "imagination":
                    stats.Imagination = (uint) value;
                    break;
                case "maximagination":
                    stats.MaxImagination = (uint) value;
                    break;
                default:
                    return $"{stat} is not a valid <stat>";
            }
            
            GameObject.Serialize(player);

            return $"Successfully set {arguments[0]} to {value}";
        }
        
        [CommandHandler(Signature = "pvp", Help = "Change PvP state", GameMasterLevel = GameMasterLevel.Admin)]
        public string Pvp(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "pvp <state(on/off)>";

            bool state;
            switch (arguments[0].ToLower())
            {
                case "true":
                case "on":
                    state = true;
                    break;
                case "false":
                case "off":
                    state = false;
                    break;
                default:
                    return "Invalid <state(on/off)>";
            }

            player.GetComponent<CharacterComponent>().IsPvP = state;

            GameObject.Serialize(player);

            return $"Successfully set your pvp state to {state}";
        }

        [CommandHandler(Signature = "gm", Help = "Change Game Master state", GameMasterLevel = GameMasterLevel.Mythran)]
        public string GameMaster(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "gm <state(on/off)>";

            bool state;
            switch (arguments[0].ToLower())
            {
                case "true":
                case "on":
                    state = true;
                    break;
                case "false":
                case "off":
                    state = false;
                    break;
                default:
                    return "Invalid <state(on/off)>";
            }

            player.GetComponent<CharacterComponent>().IsGameMaster = state;

            GameObject.Serialize(player);

            return $"Successfully set your GameMaster state to {state}";
        }

        [CommandHandler(Signature = "gmlevel", Help = "Set GameMaster Level state",
            GameMasterLevel = GameMasterLevel.Admin)]
        public string GmLevel(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "gmlevel <level>";

            if (!byte.TryParse(arguments[0], out var gmlevel)) return "Invalid <level>";

            player.GetComponent<CharacterComponent>().GameMasterLevel = gmlevel;

            GameObject.Serialize(player);

            return $"Successfully set your GameMaster level to {gmlevel}";
        }

        [CommandHandler(Signature = "layer", Help = "Change your layer", GameMasterLevel = GameMasterLevel.Admin)]
        public string Layer(string[] arguments, Player player)
        {
            if (arguments.Length != 1) return "layer <layer>";

            if (!long.TryParse(arguments[0], out var layer)) return "Invalid <layer>";

            if (player.Perspective.ViewMask == layer)
                player.Perspective.ViewMask -= layer;
            else player.Perspective.ViewMask += layer;

            return $"Layer set to {Convert.ToString(player.Perspective.ViewMask.Value, 2)}";
        }

        [CommandHandler(Signature = "brick", Help = "Spawns a floating brick",
            GameMasterLevel = GameMasterLevel.Mythran)]
        public async Task<string> Brick(string[] arguments, Player player)
        {
            var bricks = new List<GameObject>();
            
            for (var i = 0; i < 10; i++)
            {
                var brick = GameObject.Instantiate(
                    player.Zone,
                    31,
                    player.Transform.Position + Vector3.UnitY * (7 + i),
                    Quaternion.Identity
                );

                Object.Start(brick);
                GameObject.Construct(brick);

                bricks.Add(brick);
                
                await Task.Delay(100 - i * 10);
                
                if (i == 0) i++;
            }

            await Task.Delay(500);

            foreach (var brick in bricks)
            {
                Object.Destroy(brick);
            }

            return $"Spawned thing";
        }

        [CommandHandler(Signature = "tp", Help = "Teleport", GameMasterLevel = GameMasterLevel.Admin)]
        public string Teleport(string[] arguments, Player player)
        {
            if (arguments.Length == default)
                return "tp <target>/<x> <y> <z>";

            Vector3 position;

            var relativeX = arguments[0].StartsWith('~');
            if (relativeX) arguments[0] = arguments[0].Remove(default, 1);

            if (!relativeX && !float.TryParse(arguments[0], out _))
            {
                var target = player.Zone.Players.FirstOrDefault(p => p.Name == arguments[0]);

                if (target == default) return $"No player named {arguments[0]}";
                
                player.Teleport(target.Transform.Position);

                return $"Going to {player.Name}";
            }

            if (arguments.Length != 3) return "tp <x> <y> <z>";
            
            var relativeY = arguments[1].StartsWith('~');
            if (relativeY) arguments[1] = arguments[1].Remove(default, 1);
            
            var relativeZ = arguments[2].StartsWith('~');
            if (relativeZ) arguments[2] = arguments[2].Remove(default, 1);

            if (!float.TryParse(arguments[0], out position.X)) return "Invalid <x>";
            if (!float.TryParse(arguments[1], out position.Y)) return "Invalid <y>";
            if (!float.TryParse(arguments[2], out position.Z)) return "Invalid <z>";
            
            if (relativeX) position.X += player.Transform.Position.X;
            if (relativeY) position.Y += player.Transform.Position.Y;
            if (relativeZ) position.Z += player.Transform.Position.Z;
            
            player.Teleport(position);

            return $"Going to {position}";
        }

        [CommandHandler(Signature = "animate", Help = "Preform an animation", GameMasterLevel = GameMasterLevel.Mythran)]
        public string Animate(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
                return "animate <animationId>";
            
            player.Message(new PlayAnimationMessage
            {
                Associate = player,
                AnimationsId = arguments[0]
            });

            return $"Attempting to play {arguments[0]} animation";
        }

        [CommandHandler(Signature = "addcomponent", Help = "Add a component to an object", GameMasterLevel = GameMasterLevel.Admin)]
        public string AddComponent(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
                return "addcomponent <type>";

            var type = Type.GetType(arguments[0]);

            if (type == default) return "Invalid <type>";

            player.AddComponent(type);

            return $"Successfully added {type.Name} to {player}";
        }
        
        [CommandHandler(Signature = "world", Help = "Transfer to world", GameMasterLevel = GameMasterLevel.Admin)]
        public string World(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
                return "world <zoneId>";

            if (!Enum.TryParse<ZoneId>(arguments[0], out var id)) return "Invalid <zoneId>";

            if (!Enum.IsDefined(typeof(ZoneId), id)) return "Invalid <zoneId>";

            if (player.Zone.ZoneId == id) return $"You are already in {id}";

            if (id == ZoneId.FrostBurgh) return $"Sorry, {id} is disabled in the client...";
            
            player.SendToWorld(id);
            
            player.Message(new SetStunnedMessage
            {
                Associate = player,
                CantMove = true,
                CantJump = true,
                CantAttack = true,
                CantTurn = true,
                CantUseItem = true,
                CantEquip = true,
                CantInteract = true
            });
            
            return $"Requesting transfer to {id}, please wait...";
        }
        
        [CommandHandler(Signature = "getemote", Help = "Unlock an emote", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> GetEmote(string[] arguments, Player player)
        {
            if (arguments.Length == default)
                return "getemote <emote>";

            await using var cdClient = new CdClientContext();

            Emotes emote;
            
            if (int.TryParse(arguments[0], out var id))
            {
                emote = await cdClient.EmotesTable.FirstOrDefaultAsync(c => c.Id == id);
            }
            else
            {
                emote = await cdClient.EmotesTable.FirstOrDefaultAsync(c => c.AnimationName == arguments[0].ToLower());
            }

            if (emote?.Id == default)
            {
                return "Invalid <emote>";
            }

            var state = false;

            if (arguments.Length == 2)
            {
                switch (arguments[1].ToLower())
                {
                    case "true":
                    case "on":
                        state = true;
                        break;
                    case "false":
                    case "off":
                        break;
                    default:
                        return "Invalid <state(on/off)>";
                }
            }

            player.Message(new SetEmoteLockStateMessage
            {
                Associate = player,
                EmoteId = emote.Id.Value,
                Lock = state
            });

            return $"Set emote: \"{emote.AnimationName}\" [{emote.Id}] lock state to {state}";
        }
    }
}