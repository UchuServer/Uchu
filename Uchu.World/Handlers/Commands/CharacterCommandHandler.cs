using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Collections;
using Uchu.World.Experimental;
using Uchu.World.Parsers;

namespace Uchu.World.Handlers.Commands
{
    public class CharacterCommandHandler : HandlerGroup
    {
        [CommandHandler(Signature = "give", Help = "Give an item to yourself", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> GiveItem(string[] arguments, Player player)
        {
            if (arguments.Length == 0 || arguments.Length > 2)
            {
                return "give <lot> <count(optional)>";
            }

            if (!int.TryParse(arguments[0], out var lot))
            {
                return "Invalid <lot>";
            }

            uint count = 1;
            if (arguments.Length == 2)
            {
                if (!uint.TryParse(arguments[1], out count))
                {
                    return "Invalid <count(optional)>";
                }
            }

            await player.GetComponent<InventoryManager>().AddItemAsync(lot, count);

            return $"Successfully added {lot} x {count} to your inventory";
        }

        [CommandHandler(Signature = "remove", Help = "Remove an item from yourself", GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> RemoveItem(string[] arguments, Player player)
        {
            if (arguments.Length == 0 || arguments.Length > 2)
            {
                return "remove <lot> <count(optional)>";
            }

            if (!int.TryParse(arguments[1], out var lot))
            {
                return "Invalid <lot>";
            }

            uint count = 1;
            if (arguments.Length == 2)
            {
                if (!uint.TryParse(arguments[1], out count))
                {
                    return "Invalid <count(optional)>";
                }
            }

            await player.GetComponent<InventoryManager>().RemoveItemAsync(lot, count);

            return $"Successfully removed {lot} x {count} to your inventory";
        }

        [CommandHandler(Signature = "coin", Help = "Add or remove coin from yourself", GameMasterLevel = GameMasterLevel.Admin)]
        public string ChangeCoin(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
            {
                return "coin <delta>";
            }

            if (!int.TryParse(arguments[0], out var delta) || delta == default)
            {
                return "Invalid <delta>";
            }

            player.Currency += delta;

            return $"Successfully {(delta > 0 ? "added" : "removed")} {delta} coins";
        }

        [CommandHandler(Signature = "spawn", Help = "Spawn an object", GameMasterLevel = GameMasterLevel.Admin)]
        public string Spawn(string[] arguments, Player player)
        {
            if (arguments.Length != 1 || arguments.Length > 4)
            {
                return "spawn <lot> <x(optional)> <y(optional)> <z(optional)>";
            }

            arguments = arguments.Select(a => a.Replace('.', ',')).ToArray();

            if (!int.TryParse(arguments[0], out var lot))
            {
                return "Invalid <lot>";
            }
            
            var position = player.Transform.Position;
            if (arguments.Length >= 4)
            {
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
            if (arguments.Length != 1)
            {
                return "fly <state(on/off)>";
            }

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
        public async Task<string> Near(string[] arguments, Player player)
        {
            var current = player.Zone.GameObjects[0];

            foreach (var gameObject in player.Zone.GameObjects.Where(g => g != player && g != default))
            {
                if (gameObject.Transform == default || gameObject.GetComponent<SpawnerComponent>() != null) continue;

                if (Vector3.Distance(current.Transform.Position, player.Transform.Position) >
                    Vector3.Distance(gameObject.Transform.Position, player.Transform.Position))
                {
                    current = gameObject;
                }
            }

            if (current == default) return "No objects in this zone.";

            var info = new StringBuilder();
            
            var argument = "";

            if (arguments.Length == 1)
            {
                argument = arguments[0];
            }

            switch (argument)
            {
                case "-l":
                    info.Append(Convert.ToString(current.Layer.Value, 2));
                    break;
                default:
                    using (var cdClient = new CdClientContext())
                    {
                        var cdClientObject = await cdClient.ObjectsTable.FirstAsync(o => o.Id == current.Lot);

                        info.Append($"[{current.ObjectId}] [{current.Lot}] \"{cdClientObject.Name}\"");

                        var components = current.GetAllComponents().OfType<ReplicaComponent>().ToArray();
                        for (var index = 0; index < components.Length; index++)
                        {
                            var component = components[index];
                            info.Append($"\n[{index}] {component.Id}");
                        }
                    }
                    
                    break;
            }
            
            return info.ToString();
        }

        [CommandHandler(Signature = "score", Help = "Change your U-score", GameMasterLevel = GameMasterLevel.Admin)]
        public string Score(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
            {
                return "score <delta>";
            }
            
            if (!int.TryParse(arguments[0], out var delta))
            {
                return "Invalid <delta>";
            }

            player.UniverseScore += delta;
            
            GameObject.Serialize(player);
                    
            return $"Successfully {(delta > 0 ? "added" : "removed")} {delta} score";
        }

        [CommandHandler(Signature = "level", Help = "Set your U-score", GameMasterLevel = GameMasterLevel.Admin)]
        public string Level(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
            {
                return "level <level>";
            }
                    
            if (!long.TryParse(arguments[0], out var level))
            {
                return "Invalid <level>";
            }

            player.Level = level;
            
            GameObject.Serialize(player);

            return $"Successfully set your level to {level}";
        }

        [CommandHandler(Signature = "pvp", Help = "Change PvP state", GameMasterLevel = GameMasterLevel.Admin)]
        public string Pvp(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
            {
                return "pvp <state(on/off)>";
            }

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
            if (arguments.Length != 1)
            {
                return "gm <state(on/off)>";
            }

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

        [CommandHandler(Signature = "gmlevel", Help = "Set GameMaster Level state", GameMasterLevel = GameMasterLevel.Admin)]
        public string GmLevel(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
            {
                return "gmlevel <level>";
            }

            if (!byte.TryParse(arguments[0], out var gmlevel))
            {
                return "Invalid <level>";
            }

            player.GetComponent<CharacterComponent>().GameMasterLevel = gmlevel;

            GameObject.Serialize(player);

            return $"Successfully set your GameMaster level to {gmlevel}";
        }

        [CommandHandler(Signature = "layer", Help = "Change your layer", GameMasterLevel = GameMasterLevel.Admin)]
        public string Layer(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
            {
                return "layer <layer>";
            }
            
            if (!long.TryParse(arguments[0], out var layer))
            {
                return "Invalid <layer>";
            }

            if (player.Perspective.ViewMask == layer)
            {
                player.Perspective.ViewMask -= layer;
            }
            else player.Perspective.ViewMask += layer;

            return $"Layer set to {Convert.ToString(player.Perspective.ViewMask.Value, 2)}";
        }

        [CommandHandler(Signature = "brick", Help = "Spawns a floating brick", GameMasterLevel = GameMasterLevel.Mythran)]
        public string Brick(string[] arguments, Player player)
        {
            if (arguments.Length != 1)
            {
                return "brick <lot>";
            }

            if (!int.TryParse(arguments[0], out var lot))
            {
                return "Invalid <lot>";
            }

            var baseBrick = GameObject.Instantiate(player.Zone, lot, player.Transform.Position + Vector3.UnitY * 5, Quaternion.Identity);
            var floating = baseBrick.AddComponent<FloatingBrick>();
            
            floating.Target = player.Transform.Position + Vector3.UnitY * 7;
            floating.Speed = 1;
            
            Object.Start(baseBrick);
            
            GameObject.Construct(baseBrick);

            for (var i = 0; i < 8; i++)
            {
                var brick = GameObject.Instantiate(player.Zone, lot, player.Transform.Position + Vector3.UnitY * 7, Quaternion.Identity);
                
                floating = brick.AddComponent<FloatingBrick>();
                floating.Target = player.Transform.Position + Vector3.UnitY * (9 + i);
                floating.Speed = 1;
                
                Object.Start(brick);

                GameObject.Construct(brick);
            }

            return $"Spawned floating brick {lot}";
        }
    }
}