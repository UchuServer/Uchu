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

        /// <summary>
        ///     Get all GameObjects that has a LUA script of name
        /// </summary>
        /// <remarks>
        ///     Looks at the server scripts in the LuaScriptComponent
        /// </remarks>
        /// <param name="script">LUA Script name</param>
        /// <returns>The results of query</returns>
        protected GameObject[] HasLuaScript(string script)
        {
            var list = new List<GameObject>();
            
            foreach (var gameObject in Zone.GameObjects)
            {
                var scriptComponent = gameObject.GetComponent<LuaScriptComponent>();
                
                if (scriptComponent?.ScriptName != null)
                {
                    if (scriptComponent.ScriptName.ToLower().EndsWith(script.ToLower())) list.Add(gameObject);
                }
                else if (gameObject.Settings.TryGetValue("custom_script_server", out var scriptOverride))
                {
                    if (((string) scriptOverride).ToLower().EndsWith(script.ToLower())) list.Add(gameObject);
                }
            }

            return list.ToArray();
        }

        protected GameObject[] GetGroup(string group) => GetGroup(Zone, group);

        /// <summary>
        ///     Get all GameObjects with a specific group
        /// </summary>
        /// <remarks>
        ///     GameObject's groups are defined by their "groupID" setting
        /// </remarks>
        /// <param name="zone">Zone to query</param>
        /// <param name="group">Group to query</param>
        /// <returns>The results of query</returns>
        public static GameObject[] GetGroup(Zone zone, string group)
        {
            var gameObjects = new List<GameObject>();
            
            foreach (var gameObject in zone.GameObjects)
            {
                if (gameObject?.Settings == default) continue;
                
                if (!gameObject.Settings.TryGetValue("groupID", out var groupId)) continue;

                if (!(groupId is string groupIdString)) continue;
                
                var groups = groupIdString.Split(';');
                
                if (groups.Contains(group)) gameObjects.Add(gameObject);
            }

            return gameObjects.ToArray();
        }
        
        /// <summary>
        ///     Get all triggers within the specifications provided
        /// </summary>
        /// <param name="ids">Ids for query</param>
        /// <returns>The results of query</returns>
        protected TriggerComponent[] GetTriggers(params (int primaryId, int id)[] ids)
        {
            var triggers = new List<TriggerComponent>();
            
            foreach (var gameObject in Zone.GameObjects)
            {
                if (!gameObject.TryGetComponent<TriggerComponent>(out var triggerComponent)) continue;

                foreach (var (primaryId, id) in ids)
                {
                    if (triggerComponent.Trigger?.Id /*TODO: Primary id*/ == primaryId && triggerComponent.Trigger?.Id == id)
                    {
                        triggers.Add(triggerComponent);
                    }
                }
            }

            return triggers.ToArray();
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