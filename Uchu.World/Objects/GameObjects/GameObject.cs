using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Internal;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class GameObject : Object
    {
        public long ObjectId { get; private set; }
        
        public int Lot { get; private set; }
        
        public bool Constructed { get; private set; }
        
        public Spawner SpawnerObject { get; private set; }
        
        public string Name { get; set; }
        
        public Transform Transform => _components.First(c => c is Transform) as Transform;

        private readonly List<Component> _components = new List<Component>();

        private readonly List<ReplicaComponent> _replicaComponents = new List<ReplicaComponent>();

        #region Component Management
        
        public Component AddComponent(Type type)
        {
            if (_components.Any(c => c.GetType() == type))
            {
                Logger.Warning($"{type.FullName} Component is already on {ObjectId} \"{Name}\"");
                return null;
            }
            
            if (Object.Instantiate(type, Zone) is Component component)
            {
                component.GameObject = this;
                
                _components.Add(component);

                if (component is ReplicaComponent replicaComponent) _replicaComponents.Add(replicaComponent);

                foreach (var required in type.GetCustomAttributes<RequireComponentAttribute>()
                    .Where(a => GetComponent(a.Type) == null))
                {
                    AddComponent(required.Type);
                }

                return component;
            }
            
            Logger.Error($"{type.FullName} does not inherit form Components but is being Created as one.");
            return null;
        }

        public T AddComponent<T>() where T : Component
        {
            return AddComponent(typeof(T)) as T;
        }

        public Component GetComponent(Type type)
        {
            return _components.FirstOrDefault(c => c.GetType() == type);
        }

        public T GetComponent<T>() where T : Component
        {
            return _components.FirstOrDefault(c => c is T) as T;
        }

        public void RemoveComponent(Type type)
        {
            if (type.GetCustomAttribute<EssentialAttribute>() != null || type.IsAssignableFrom(typeof(ReplicaComponent)))
            {
                Logger.Error($"{type} Component on {this} is Essential and cannot be removed.");
            }

            foreach (var component in _components)
            {
                foreach (var required in component.GetType().GetCustomAttributes<RequireComponentAttribute>())
                {
                    if (required.Type == type)
                    {
                        Logger.Error(
                            $"{type} Component on {this} is Required by {required.Type} and cannot be removed.");
                    }
                }
            }

            var comp = GetComponent(type);
            _components.Remove(comp);
            
            Destroy(comp);
        }

        public void RemoveComponent<T>() where T : Component
        {
            RemoveComponent(typeof(T));
        }

        public void RemoveComponent(Component component)
        {
            if (_components.Contains(component)) RemoveComponent(component.GetType());
        }
        
        #endregion

        protected override void Start()
        {
            Zone.GameObjects.Add(this);
        }
        
        protected override void End()
        {
            Zone.DestroyObject(this);
            
            Zone.GameObjects.Remove(this);
        }
        
        public void Construct()
        {
            if (Constructed)
            {
                Logger.Error($"{ObjectId} : {Name} has already been constructed.");
                return;
            }
            
            Constructed = true;

            Zone.ConstructObject(this);
        }

        public void Update()
        {
            Zone.UpdateObject(this);
        } 

        #region Operators

        public static bool operator ==(GameObject object1, GameObject object2)
        {
            if (ReferenceEquals(object1, null)) return ReferenceEquals(object2, null);
            if (ReferenceEquals(object2, null)) return object1.Zone.Objects.Contains(object1);
            
            return object1.ObjectId == object2.ObjectId;
        }

        public static bool operator !=(GameObject object1, GameObject object2)
        {
            return !(object1 == object2);
        }

        public static implicit operator long(GameObject gameObject)
        {
            if (gameObject == null) return -1;
            return gameObject.ObjectId;
        }
        
        #endregion

        #region Static
        
        public static GameObject Instantiate(Type type, Object parent, string name = "", Vector3 position = default,
            Quaternion rotation = default, long objectId = default, int lot = default, Spawner spawner = default)
        {
            if (type.IsSubclassOf(typeof(GameObject)) || type == typeof(GameObject))
            {
                var instance = (GameObject) Object.Instantiate(type, parent.Zone);
                instance.ObjectId = objectId == default ? Utils.GenerateObjectId() : objectId;

                instance.Lot = lot;
                
                instance.Name = name;

                instance.SpawnerObject = spawner;
                
                var transform = instance.AddComponent<Transform>();
                transform.Position = position;
                transform.Rotation = rotation;

                switch (parent)
                {
                    case Zone _:
                        transform.Parent = null;
                        break;
                    case GameObject parentObject:
                        transform.Parent = parentObject.Transform;
                        break;
                    case Transform parentTransform:
                        transform.Parent = parentTransform;
                        break;
                }
                
                return instance;
            }
            
            Logger.Error($"{type.FullName} does not inherit from GameObject but is being Instantiated as one.");
            return null;
        }

        public static T Instantiate<T>(Object parent, string name = "", Vector3 position = default,
            Quaternion rotation = default, long objectId = default, int lot = default, Spawner spawner = default) 
            where T : GameObject
        {
            return Instantiate(typeof(T), parent, name, position, rotation, objectId, lot, spawner) as T;
        }

        public static GameObject Instantiate(LevelObject levelObject, Object parent, Spawner spawner = default)
        {
            if (levelObject.Settings.TryGetValue("spawntemplate", out _))
                return Spawner.Instantiate(levelObject, parent);
            
            using (var ctx = new CdClientContext())
            {
                var name = levelObject.Settings.TryGetValue("npcName", out var npcName) ? (string) npcName : "";
                
                var instance = Instantiate<GameObject>(parent, name, levelObject.Position, levelObject.Rotation,
                    Utils.GenerateObjectId(), levelObject.LOT);
                
                instance.SpawnerObject = spawner;
                
                var registryComponents = ctx.ComponentsRegistryTable.Where(r => r.Id == levelObject.LOT).ToList();

                var order = ReplicaComponent.ComponentOrder;
                registryComponents.Sort((c1, c2) => order.IndexOf((int) c1.Componenttype) - order.IndexOf((int) c2.Componenttype));

                foreach (var component in registryComponents)
                {
                    var type = ReplicaComponent.GetReplica((ReplicaComponentsId) component.Componenttype);
                    
                    if (type == null) Logger.Warning($"No component of ID {component.Componentid}");
                    else instance.AddComponent(type);
                }

                foreach (var component in instance._replicaComponents)
                {
                    component.FromLevelObject(levelObject);
                }

                if (levelObject.Settings.ContainsKey("trigger_id") && instance.GetComponent<TriggerComponent>() == null)
                {
                    var trigger = instance.AddComponent<TriggerComponent>();
                
                    trigger.FromLevelObject(levelObject);
                }

                return instance;
            }
        }
        
        #endregion

        #region Replica

        public void WriteConstruct(BitWriter writer)
        {
            writer.Write(ObjectId);

            writer.Write(Lot);

            writer.Write((byte) Name.Length);
            writer.WriteString(Name, Name.Length, true);

            writer.Write<uint>(0); // TODO: Add creation time?

            writer.WriteBit(false); // TODO: figure this out

            var trigger = GetComponent<TriggerComponent>();

            var hasTriggerId = !ReferenceEquals(trigger, null) && trigger.TriggerId != -1;

            writer.WriteBit(hasTriggerId);

            var hasSpawner = SpawnerObject != null;

            writer.WriteBit(hasSpawner);

            if (hasSpawner)
                writer.Write(SpawnerObject);

            var hasSpawnerNode = SpawnerObject != null && SpawnerObject.SpawnTemplate != 0;

            writer.WriteBit(hasSpawnerNode);

            if (hasSpawnerNode)
                writer.Write(SpawnerObject.SpawnTemplate);

            var hasScale = !Transform.Scale.Equals(-1);
            
            writer.WriteBit(hasScale);

            if (hasScale)
                writer.Write(Transform.Scale);

            writer.WriteBit(false); // TODO: Add World State
            
            writer.WriteBit(false); // TODO: Add GM Level

            WriteHierarchy(writer);

            //
            // Construct replica components.
            //

            foreach (var replicaComponent in _replicaComponents)
            {
                replicaComponent.Construct(writer);
            }
        }
        
        public void WriteSerialize(BitWriter writer)
        {
            WriteHierarchy(writer);

            //
            // Serialize replica components.
            //

            foreach (var replicaComponent in _replicaComponents)
            {
                replicaComponent.Serialize(writer);
            }
        }

        private void WriteHierarchy(BitWriter writer)
        {
            writer.WriteBit(true);

            var hasParent = Transform.Parent != null;

            writer.WriteBit(hasParent);

            if (hasParent)
            {
                writer.Write(Transform.Parent.GameObject);
                writer.WriteBit(false);
            }

            var hasChildren = Transform.Children.Length > 0;

            writer.WriteBit(hasChildren);

            if (!hasChildren) return;
            writer.Write((ushort) Transform.Children.Length);

            foreach (var child in Transform.Children)
            {
                writer.Write(child.GameObject);
            }
        }
        
        #endregion
    }
}