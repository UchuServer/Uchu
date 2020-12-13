using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Scripting.Lua
{
    public abstract class LuaNativeScript : LuaScript
    {
        protected Zone Zone { get; private set; }
        protected UchuServer UchuServer => Zone.UchuServer;
        
        protected static void Start(Object obj) => Object.Start(obj);
        protected static void Destroy(Object obj) => Object.Destroy(obj);
        protected static void Construct(GameObject gameObject) => GameObject.Construct(gameObject);
        protected static void Serialize(GameObject gameObject) => GameObject.Serialize(gameObject);
        protected static void Destruct(GameObject gameObject) => GameObject.Destruct(gameObject);
        
        protected GameObject[] GetGroup(string group)
        {
            return GetGroup(Zone, group);
        }
        public static GameObject[] GetGroup(Zone zone, string group)
        {
            var gameObjects = new List<GameObject>();
            foreach (var gameObject in zone.GameObjects)
            {
                if (gameObject?.Settings == default) continue;
                if (!gameObject.Settings.TryGetValue("groupID", out var groupId)) continue;
                if (!(groupId is string groupIdString)) continue;
                var groups = groupIdString.Split(';').ToList();
                if (groups.Contains(group)) gameObjects.Add(gameObject);
            }
            return gameObjects.ToArray();
        }
        
        public static GameObject[] GetVolumeGroup(Zone zone, string group)
        {
            var gameObjects = new List<GameObject>();
            foreach (var gameObject in zone.GameObjects)
            {
                if (gameObject?.Settings == default) continue;
                if (!gameObject.Settings.TryGetValue("volGroup", out var groupId)) continue;
                if (!(groupId is string groupIdString)) continue;
                var groups = groupIdString.Split(';').ToList();
                if (groups.Contains(group)) gameObjects.Add(gameObject);
            }

            return gameObjects.ToArray();
        }
        protected TriggerComponent[] GetTriggers(params (int primaryId, int id)[] ids)
        {
            var triggers = new List<TriggerComponent>();
            foreach (var gameObject in Zone.GameObjects)
            {
                if (!gameObject.TryGetComponent<TriggerComponent>(out var triggerComponent)) continue;
                foreach (var (primaryId, id) in ids)
                {
                    if (triggerComponent.Trigger?.Id == primaryId && triggerComponent.Trigger?.Id == id) triggers.Add(triggerComponent);
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
        
        internal void SetZone(Zone zone) => Zone = zone;
        public abstract string ScriptName { get; set; }
        public override Task LoadAsync(GameObject self) => Task.CompletedTask;
        public override Task UnloadAsync() => Task.CompletedTask;
    }
}