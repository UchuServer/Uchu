using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Collections;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class GameObject : Object
    {
        private readonly List<Component> _components = new List<Component>();

        private readonly List<ReplicaComponent> _replicaComponents = new List<ReplicaComponent>();
        private Mask _layer = new Mask(World.Layer.Default);

        public GameObject()
        {
            OnStart.AddListener(() =>
            {
                Zone.RegisterGameObject(this);

                foreach (var component in _components.ToArray()) Start(component);
            });

            OnDestroyed.AddListener(() =>
            {
                OnLayerChanged.Clear();
                
                OnInteract.Clear();
                
                Zone.UnregisterGameObject(this);

                foreach (var component in _components.ToArray()) Destroy(component);

                Zone.SendDestruction(this);
            });
        }

        public long ObjectId { get; private set; }

        public Lot Lot { get; private set; }

        public Mask Layer
        {
            get => _layer;
            set
            {
                _layer = value;

                OnLayerChanged?.Invoke(_layer);
            }
        }

        public string Name { get; set; }
        
        /// <summary>
        ///     Also known as ExtraInfo
        /// </summary>
        public LegoDataDictionary Settings { get; set; } = new LegoDataDictionary();

        public string ClientName { get; private set; }

        public SpawnerComponent SpawnerObject { get; private set; }

        public virtual Transform Transform => _components.First(c => c is Transform) as Transform;

        public bool Alive => Zone?.TryGetGameObject(ObjectId, out _) ?? false;

        public ReplicaComponent[] ReplicaComponents => _replicaComponents.ToArray();

        public readonly Event<Mask> OnLayerChanged = new Event<Mask>();

        public readonly Event<Player> OnInteract = new Event<Player>();

        #region Operators

        public static implicit operator long(GameObject gameObject)
        {
            if (gameObject == null) return -1;
            return gameObject.ObjectId;
        }

        #endregion

        #region Events

        public void Interact(Player player)
        {
            OnInteract?.Invoke(player);
        }

        #endregion
        
        #region Utils

        public override string ToString()
        {
            return $"[{ObjectId}] \"{(Name == "" ? ClientName : Name)}\"";
        }

        #endregion

        #region Component Management

        public Component AddComponent(Type type)
        {
            if (Object.Instantiate(type, Zone) is Component component)
            {
                component.GameObject = this;

                _components.Add(component);

                var requiredComponents = type.GetCustomAttributes<RequireComponentAttribute>().ToArray();

                foreach (var attribute in requiredComponents.Where(r => r.Priority)) AddComponent(attribute.Type);

                if (component is ReplicaComponent replicaComponent) _replicaComponents.Add(replicaComponent);

                foreach (var attribute in requiredComponents.Where(r => !r.Priority)) AddComponent(attribute.Type);

                Start(component);

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

        public Component[] GetComponents(Type type)
        {
            return _components.Where(c => c.GetType() == type).ToArray();
        }

        public T GetComponent<T>() where T : Component
        {
            return _components.FirstOrDefault(c => c is T) as T;
        }

        public T[] GetComponents<T>() where T : Component
        {
            return _components.OfType<T>().ToArray();
        }

        public Component[] GetAllComponents()
        {
            return _components.ToArray();
        }

        public bool TryGetComponent(Type type, out Component result)
        {
            result = GetComponent(type);
            return result != default;
        }

        public bool TryGetComponents(Type type, out Component[] result)
        {
            result = GetComponents(type);
            return result.Length != default;
        }

        public bool TryGetComponent<T>(out T result) where T : Component
        {
            result = GetComponent<T>();
            return result != default;
        }

        public bool TryGetComponents<T>(out T[] result) where T : Component
        {
            result = GetComponents<T>();
            return result.Length != default;
        }

        public void RemoveComponent(Type type)
        {
            foreach (var required in from component in _components
                from required in component.GetType().GetCustomAttributes<RequireComponentAttribute>()
                where required.Type == type
                select required)
                Logger.Error(
                    $"{type} Component on {this} is Required by {required.Type} and cannot be removed.");

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

        #region Networking

        public static void Construct(GameObject gameObject)
        {
            gameObject.Zone.SendConstruction(gameObject);
        }

        public static void Serialize(GameObject gameObject)
        {
            gameObject.Zone.SendSerialization(gameObject);
        }

        public static void Destruct(GameObject gameObject)
        {
            gameObject.Zone.SendDestruction(gameObject);
        }

        #endregion

        #region Instaniate

        #region From Raw

        public static GameObject Instantiate(Type type, Object parent, string name = "", Vector3 position = default,
            Quaternion rotation = default, float scale = 1, long objectId = default, Lot lot = default,
            SpawnerComponent spawner = default)
        {
            if (type.IsSubclassOf(typeof(GameObject)) || type == typeof(GameObject))
            {
                var instance = (GameObject) Object.Instantiate(type, parent.Zone);
                instance.ObjectId = objectId == default ? IdUtils.GenerateObjectId() : objectId;

                instance.Lot = lot;

                instance.Name = name;

                using (var cdClient = new CdClientContext())
                {
                    var obj = cdClient.ObjectsTable.FirstOrDefault(o => o.Id == lot);
                    instance.ClientName = obj?.Name;
                }

                instance.SpawnerObject = spawner;

                var transform = instance.AddComponent<Transform>();
                transform.Position = position;
                transform.Rotation = rotation;
                transform.Scale = scale;

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
            Quaternion rotation = default, float scale = 1, long objectId = default, Lot lot = default,
            SpawnerComponent spawner = default)
            where T : GameObject
        {
            return Instantiate(typeof(T), parent, name, position, rotation, scale, objectId, lot, spawner) as T;
        }
        
        public static GameObject Instantiate(Object parent, string name = "", Vector3 position = default,
            Quaternion rotation = default, float scale = 1, long objectId = default, Lot lot = default,
            SpawnerComponent spawner = default)
        {
            return Instantiate(typeof(GameObject), parent, name, position, rotation, scale, objectId, lot, spawner);
        }
        
        #endregion

        #region From Template

        public static GameObject Instantiate(Type type, Object parent, Lot lot, Vector3 position = default,
            Quaternion rotation = default)
        {
            return Instantiate(type, new LevelObject
            {
                Lot = lot,
                Position = position,
                Rotation = rotation,
                Scale = 1,
                Settings = new LegoDataDictionary()
            }, parent);
        }

        public static T Instantiate<T>(Object parent, Lot lot, Vector3 position = default,
            Quaternion rotation = default) where T : GameObject
        {
            return Instantiate(typeof(T), parent, lot, position, rotation) as T;
        }

        public static GameObject Instantiate(Object parent, Lot lot, Vector3 position = default,
            Quaternion rotation = default)
        {
            return Instantiate(typeof(GameObject), parent, lot, position, rotation);
        }

        #endregion

        #region From LevelObject

        public static GameObject Instantiate(Type type, LevelObject levelObject, Object parent,
            SpawnerComponent spawner = default)
        {
            // ReSharper disable PossibleInvalidOperationException

            //
            // Check if spawner
            //

            if (levelObject.Settings.TryGetValue("spawntemplate", out _))
                return InstancingUtil.Spawner(levelObject, parent);

            using var ctx = new CdClientContext();
            
            var name = levelObject.Settings.TryGetValue("npcName", out var npcName) ? (string) npcName : "";

            //
            // Create GameObject
            //

            var instance = Instantiate(
                type,
                parent,
                name,
                levelObject.Position,
                levelObject.Rotation,
                levelObject.Scale,
                IdUtils.GenerateObjectId(),
                levelObject.Lot
            );

            instance.SpawnerObject = spawner;
            instance.Settings = levelObject.Settings;

            //
            // Collect all the components on this object
            //

            var registryComponents = ctx.ComponentsRegistryTable.Where(
                r => r.Id == levelObject.Lot
            ).ToArray();

            //
            // Select all the none networked components on this object
            //

            var componentEntries = registryComponents.Where(o =>
                o.Componenttype != null && !ReplicaComponent.ComponentOrder.Contains(o.Componenttype.Value)
            ).ToArray();

            foreach (var component in componentEntries)
            {
                //
                // Add components from the entries
                //

                if ((ComponentId) (int) component.Componenttype == ComponentId.QuestGiver)
                    Logger.Information($"{instance} has a Quest Giver component.");

                var componentType =
                    ReplicaComponent.GetReplica((ComponentId) (int) component.Componenttype);

                if (componentType != default) instance.AddComponent(componentType);
            }

            //
            // Select all the networked components on this object
            //

            registryComponents = registryComponents.Where(
                c => ReplicaComponent.ComponentOrder.Contains(c.Componenttype.Value)
            ).ToArray();

            // Sort components

            Array.Sort(registryComponents, (c1, c2) =>
                ReplicaComponent.ComponentOrder.IndexOf((int) c1.Componenttype)
                    .CompareTo(ReplicaComponent.ComponentOrder.IndexOf((int) c2.Componenttype))
            );

            foreach (var component in registryComponents)
            {
                var componentType = ReplicaComponent.GetReplica((ComponentId) component.Componenttype);

                if (componentType == null) Logger.Warning($"No component of ID {component.Componentid}");
                else instance.AddComponent(componentType);
            }

            //
            // Check if this object is a trigger
            //

            if (levelObject.Settings.ContainsKey("trigger_id") && instance.GetComponent<TriggerComponent>() == null)
            {
                Logger.Information($"{instance} is trigger!");

                instance.AddComponent<TriggerComponent>();
            }

            //
            // Check if this object has a spawn activator attached to it
            //

            //
            // Setup all the components
            //

            foreach (var component in instance._replicaComponents) component.FromLevelObject(levelObject);

            return instance;
        }

        public static T Instantiate<T>(LevelObject levelObject, Object parent, SpawnerComponent spawner = default)
            where T : GameObject
        {
            return Instantiate(typeof(T), levelObject, parent, spawner) as T;
        }

        public static GameObject Instantiate(LevelObject levelObject, Object parent, SpawnerComponent spawner = default)
        {
            return Instantiate(typeof(GameObject), levelObject, parent, spawner);
        }

        #endregion

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

            var hasTriggerId = trigger != null && trigger.TriggerId != -1;

            writer.WriteBit(hasTriggerId);

            var hasSpawner = SpawnerObject != null;

            writer.WriteBit(hasSpawner);

            if (hasSpawner)
                writer.Write(SpawnerObject.GameObject.ObjectId);

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

            foreach (var replicaComponent in _replicaComponents) replicaComponent.Construct(writer);
        }

        public void WriteSerialize(BitWriter writer)
        {
            WriteHierarchy(writer);

            //
            // Serialize replica components.
            //

            foreach (var replicaComponent in _replicaComponents) replicaComponent.Serialize(writer);
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

            var hasChildren = Transform.Children.Length != default;

            writer.WriteBit(hasChildren);

            if (!hasChildren) return;
            writer.Write((ushort) Transform.Children.Length);

            foreach (var child in Transform.Children) writer.Write(child.GameObject);
        }

        #endregion
    }
}