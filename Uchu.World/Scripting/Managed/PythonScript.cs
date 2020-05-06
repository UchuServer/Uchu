using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Python;
using Uchu.World.Social;

namespace Uchu.World.Scripting.Managed
{
    public class PythonScript : Script
    {
        public string Source { get; }
        
        public Zone Zone { get; }
        
        public ManagedScript Script { get; set; }
        
        public Object Proxy { get; set; }
        
        public PythonScript(string source, Zone zone)
        {
            Source = source;
            
            Zone = zone;
        }

        public override Task LoadAsync()
        {
            Proxy = Object.Instantiate(Zone);

            Object.Start(Proxy);
            
            Script = new ManagedScript(
                Source,
                Zone.ManagedScriptEngine
            );

            dynamic layers = new ExpandoObject();
            layers.None = (Mask) StandardLayer.None;
            layers.Default = (Mask) StandardLayer.Default;
            layers.Environment = (Mask) StandardLayer.Environment;
            layers.Npc = (Mask) StandardLayer.Npc;
            layers.Smashable = (Mask) StandardLayer.Smashable;
            layers.Player = (Mask) StandardLayer.Player;
            layers.Enemy = (Mask) StandardLayer.Enemy;
            layers.Spawner = (Mask) StandardLayer.Spawner;
            layers.Hidden = (Mask) StandardLayer.Hidden;
            layers.All = (Mask) StandardLayer.All;

            var variables = new Dictionary<string, dynamic>
            {
                ["Zone"] = Zone,
                ["Start"] = new Action<Object>(Object.Start),
                ["Destroy"] = new Action<Object>(Object.Destroy),
                ["Construct"] = new Action<GameObject>(GameObject.Construct),
                ["Serialize"] = new Action<GameObject>(GameObject.Serialize),
                ["Destruct"] = new Action<GameObject>(GameObject.Destruct),
                ["Create"] = new Func<int, Vector3, Quaternion, GameObject>
                    ((lot, position, rotation) => GameObject.Instantiate(Zone, lot, position, rotation)),
                ["Broadcast"] = new Action<dynamic>(obj =>
                {
                    foreach (var player in Zone.Players)
                    {
                        player.SendChatMessage(obj, PlayerChatChannel.Normal);
                    }
                }),
                ["OnStart"] = new Func<GameObject, Action, Delegate>((gameObject, action) => Listen(gameObject.OnStart, action)),
                ["OnDestroy"] = new Func<GameObject, Action, Delegate>((gameObject, action) => Listen(gameObject.OnDestroyed, action)),
                ["OnInteract"] = new Func<GameObject, Action<Player>, Delegate>((gameObject, action) => Listen(gameObject.OnInteract, action)),
                ["OnHealth"] = new Func<GameObject, Action<int, int, GameObject>, Delegate>((gameObject, action) =>
                {
                    if (!gameObject.TryGetComponent<Stats>(out var stats)) return null;

                    return Listen(stats.OnHealthChanged, (newHealth, delta) =>
                    {
                        action((int) newHealth, delta, stats.LatestDamageSource);
                    });
                }),
                ["OnArmor"] = new Func<GameObject, Action<int, int, GameObject>, Delegate>((gameObject, action) =>
                {
                    if (!gameObject.TryGetComponent<Stats>(out var stats)) return null;

                    return Listen(stats.OnArmorChanged, (newArmor, delta) =>
                    {
                        action((int) newArmor, delta, stats.LatestDamageSource);
                    });
                }),
                ["OnDeath"] = new Func<GameObject, Action<GameObject>, Delegate>((gameObject, action) =>
                {
                    if (!gameObject.TryGetComponent<Stats>(out var stats)) return null;
                    
                    return Listen(stats.OnDeath, () =>
                    {
                        action(stats.LatestDamageSource);
                    });
                }),
                ["OnChat"] = new Func<Action<Player, string>, Delegate>(action =>
                {
                    return Listen(Zone.OnChatMessage, (player, message) =>
                    {
                        action(player, message);
                    });
                }),
                ["Release"] = new Action<Delegate>(ReleaseListener),
                ["Drop"] = new Action<int, Vector3, GameObject, Player>((lot, position, source, player) =>
                {
                    var loot = InstancingUtilities.InstantiateLoot(lot, player, source, position);

                    Object.Start(loot);
                }),
                ["Currency"] = new Action<int, Vector3, GameObject, Player>((count, position, source, player) =>
                {
                    InstancingUtilities.InstantiateCurrency(count, player, source, position);
                }),
                ["GetComponent"] = new Func<GameObject, string, Component>((gameObject, name) =>
                {
                    var type = Type.GetType($"Uchu.World.{name}");
                    
                    return type != default ? gameObject.GetComponent(type) : null;
                }),
                ["AddComponent"] = new Func<GameObject, string, Component>((gameObject, name) =>
                {
                    var type = Type.GetType($"Uchu.World.{name}");
                    
                    return type != default ? gameObject.AddComponent(type) : null;
                }),
                ["RemoveComponent"] = new Action<GameObject, string>((gameObject, name) =>
                {
                    var type = Type.GetType($"Uchu.World.{name}");

                    if (type == null) return;
                    
                    gameObject.RemoveComponent(type);
                }),
                ["Vector3"] = new Func<float, float, float, Vector3>((x, y, z) => new Vector3(x, y, z)),
                ["Distance"] = new Func<Vector3, Vector3, float>(Vector3.Distance),
                ["Quaternion"] = new Func<float, float, float, float, Quaternion>((x, y, z, w) => new Quaternion(x, y, z, w)),
                ["Layer"] = layers,
                ["Chat"] = new Action<Player, string>((player, message) =>
                {
                    player.SendChatMessage(message, PlayerChatChannel.Normal);
                }),
                ["ClientContext"] = new Func<CdClientContext>(() => new CdClientContext()),
                ["UchuContext"] = new Func<UchuContext>(() => new UchuContext()),
                ["CentralNotice"] = new Action<Player, string>((player, text) => player.CentralNoticeGui(text)),
                ["StoryBox"] = new Action<Player, string>(async (player, text) => await player.StoryBoxGuiAsync(text))
            };

            Script.Run(variables.ToArray());

            Task.Run(() =>
            {
                Script.Execute("load", out var exception);

                if (exception != default)
                    Logger.Error(exception);
            });
            
            Zone.Update(Proxy, () => { Task.Run(() => {
                    Script.Execute("tick", out var exception);
                    
                    if (exception != default)
                        Logger.Error(exception);
                });
            
                return Task.CompletedTask;
            }, 1);
            
            return Task.CompletedTask;
        }

        public override Task UnloadAsync()
        {
            Object.Destroy(Proxy);
            
            ClearListeners();

            Task.Run(() =>
            {
                Script.Execute("unload", out var exception);

                if (exception != default)
                    Logger.Error(exception);
            });
            
            return Task.CompletedTask;
        }
    }
}