using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Scripting.Native
{
    public abstract class NativeScript : Script
    {
        protected Zone Zone { get; private set; }

        protected UchuServer UchuServer => Zone.Server;

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
        /// <param name="client">Look for client LUA Sciripts</param>
        /// <returns>The results of query</returns>
        protected GameObject[] HasLuaScript(string script, bool client = false)
        {
            script = script.ToLower();
            
            var list = new List<GameObject>();
            
            foreach (var gameObject in Zone.GameObjects)
            {
                if (HasLuaScript(gameObject, script, client))
                {
                    list.Add(gameObject);
                }
            }

            return list.ToArray();
        }

        protected bool HasLuaScript(GameObject gameObject, string script, bool client = false)
        {
            var scriptComponent = gameObject.GetComponent<LuaScriptComponent>();

            if (!client)
            {
                if (scriptComponent?.ScriptName != null)
                {
                    if (scriptComponent.ScriptName.ToLower().EndsWith(script)) return true;
                }
                else if (gameObject.Settings.TryGetValue("custom_script_server", out var scriptOverride))
                {
                    if (((string) scriptOverride).ToLower().EndsWith(script)) return true;
                }
            }
            else
            {
                if (scriptComponent?.ClientScriptName != null)
                {
                    if (scriptComponent.ClientScriptName.ToLower().EndsWith(script)) return true;
                }
                else if (gameObject.Settings.TryGetValue("custom_client_script", out var scriptOverride))
                {
                    if (((string) scriptOverride).ToLower().EndsWith(script)) return true;
                }
            }

            return false;
        }

        protected GameObject[] GetGroup(string group)
        {
            return GetGroup(Zone, group);
        }

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
        ///     Get all GameObjects with a specific volume group
        /// </summary>
        /// <remarks>
        ///     GameObject's volumes groups are defined by their "volGroup" setting
        /// </remarks>
        /// <param name="zone">Zone to query</param>
        /// <param name="group">Group to query</param>
        /// <returns>The results of query</returns>
        public static GameObject[] GetVolumeGroup(Zone zone, string group)
        {
            var gameObjects = new List<GameObject>();
            
            foreach (var gameObject in zone.GameObjects)
            {
                if (gameObject?.Settings == default) continue;
                
                if (!gameObject.Settings.TryGetValue("volGroup", out var groupId)) continue;

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
                    if (triggerComponent.Trigger?.FileId == primaryId && triggerComponent.Trigger?.Id == id)
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

        internal Task InternalUnloadAsync()
        {
            ClearListeners();

            return UnloadAsync();
        }
        
        public override Task UnloadAsync()
        {
            return Task.CompletedTask;
        }
    }
}