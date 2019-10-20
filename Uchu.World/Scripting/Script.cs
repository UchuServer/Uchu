using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Scripting
{
    public abstract class Script
    {
        protected Zone Zone { get; private set; }

        protected Server Server => Zone.Server;

        protected static void Start(Object obj) => Object.Start(obj);
        
        protected static void Destroy(Object obj) => Object.Destroy(obj);

        protected static void Construct(GameObject gameObject) => GameObject.Construct(gameObject);
        
        protected static void Serialize(GameObject gameObject) => GameObject.Serialize(gameObject);
        
        protected static void Destruct(GameObject gameObject) => GameObject.Destruct(gameObject);

        protected GameObject[] HasLuaScript(string script)
        {
            return (from gameObject in Zone.GameObjects
                let scriptComponent = gameObject.GetComponent<LuaScriptComponent>()
                where scriptComponent?.ScriptName != null
                where scriptComponent.ScriptName.ToLower().EndsWith(script.ToLower())
                select gameObject).ToArray();
        }

        protected GameObject[] GetGroup(string group)
        {
            var gameObjects = new List<GameObject>();
            
            foreach (var gameObject in Zone.GameObjects)
            {
                if (!gameObject.Settings.TryGetValue("groupID", out var groupId)) continue;

                Logger.Information($"GROUPID: {groupId}");
                
                if (!(groupId is string groupIdString)) continue;
                
                if (groupIdString == $"{group};") gameObjects.Add(gameObject);
            }

            return gameObjects.ToArray();
        }

        protected static void SetTimer(Action action, int time)
        {
            Task.Run(async () =>
            {
                await Task.Delay(time);

                action();
            });
        }
        
        internal void SetZone(Zone zone)
        {
            Zone = zone;
        }
        
        public abstract Task LoadAsync();

        public virtual Task UnloadAsync()
        {
            return Task.CompletedTask;
        }
    }
}