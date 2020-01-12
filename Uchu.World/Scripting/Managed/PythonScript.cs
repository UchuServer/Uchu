using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Python;

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

            var variables = new Dictionary<string, dynamic>
            {
                ["Zone"] = Zone,
                ["Start"] = new Action<Object>(Object.Start),
                ["Destroy"] = new Action<Object>(Object.Destroy),
                ["Construct"] = new Action<GameObject>(GameObject.Construct),
                ["Serialize"] = new Action<GameObject>(GameObject.Serialize),
                ["Destruct"] = new Action<GameObject>(GameObject.Destruct),
                ["Instantiate"] = new Func<int, Vector3, Quaternion, GameObject>
                    ((lot, position, rotation) => GameObject.Instantiate(Zone, lot, position, rotation)),
                ["Broadcast"] = new Action<dynamic>(obj =>
                {
                    foreach (var player in Zone.Players)
                    {
                        player.SendChatMessage(obj, PlayerChatChannel.Normal);
                    }
                }),
                ["OnTick"] = new Action<GameObject, Action>((gameObject, action) => Listen(gameObject.OnTick, action)),
                ["OnStart"] = new Action<GameObject, Action>((gameObject, action) => Listen(gameObject.OnStart, action)),
                ["OnDestroy"] = new Action<GameObject, Action>((gameObject, action) => Listen(gameObject.OnDestroyed, action)),
                ["OnInteract"] = new Action<GameObject, Action<Player>>((gameObject, action) => Listen(gameObject.OnInteract, action))
            };

            Script.Run(variables.ToArray());
            
            Script.Execute("load");
            
            Listen(Proxy.OnTick, () =>
            {
                Script.Execute("tick");
            });

            return Task.CompletedTask;
        }

        public override Task UnloadAsync()
        {
            Object.Destroy(Proxy);
            
            ClearListeners();

            Script.Execute("unload");
            
            return Task.CompletedTask;
        }
    }
}