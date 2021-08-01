using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using InfectedRose.Lvl;
using InfectedRose.Core;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;
using Uchu.World.Scripting.Native;

namespace Uchu.World
{
    public class GameObject : Object
    {
        private List<Component> Components { get; }

        private Mask _layer = new Mask(StandardLayer.Default);

        private ObjectWorldState _worldState;

        protected string ObjectName { get; set; }

        public ObjectId Id { get; private set; }

        public Lot Lot { get; private set; }
        
        /// <summary>
        /// Also known as ExtraInfo
        /// TODO: Rename?
        /// </summary>
        public LegoDataDictionary Settings { get; set; }

        public string ClientName { get; private set; }

        public SpawnerComponent Spawner { get; private set; }

        public Mask Layer
        {
            get => _layer;
            set
            {
                _layer = value;

                OnLayerChanged?.Invoke(_layer);
            }
        }

        public virtual string Name
        {
            get => ObjectName;
            set
            {
                ObjectName = value;

                Reload();
            }
        }

        public ObjectWorldState WorldState
        {
            get => _worldState;
            set
            {
                _worldState = value;
                
                Zone.BroadcastMessage(new ChangeObjectWorldStateMessage
                {
                    Associate = this,
                    State = value
                });
            }
        }

        public int GameMasterLevel
        {
            get
            {
                if (Settings == default) return 0;
                
                if (!Settings.TryGetValue("gmlevel", out var level)) return default;

                return (int) level;
            }
            set
            {
                Settings["gmlevel"] = value;

                Reload();
            }
        }

        #region Events

        public Event<Mask> OnLayerChanged { get; }

        public Event<Player> OnInteract { get; }

        public Event<int, Player> OnEmoteReceived { get; }

        public Event<GameObject, string> OnSkillEvent { get; }

        public Event OnCancelRailMovement { get; }

        public Event OnRailMovementReady { get; }

        #endregion
        
        #region Macro

        public Transform Transform => GetComponent<Transform>();

        public bool Alive => Id != ObjectId.Invalid && Zone.GameObjects.Any(g => g.Id == Id);

        public ReplicaComponent[] ReplicaComponents => Components.OfType<ReplicaComponent>().ToArray();

        public Player[] Viewers => Zone.Players.Where(p => p.Perspective.TryGetNetworkId(this, out _)).ToArray();

        #endregion
        
        protected GameObject()
        {
            Settings = new LegoDataDictionary();
            
            Components = new List<Component>();
            
            OnLayerChanged = new Event<Mask>();
            
            OnInteract = new Event<Player>();

            OnEmoteReceived = new Event<int, Player>();

            OnSkillEvent = new Event<GameObject, string>();

            OnRailMovementReady = new Event();

            OnCancelRailMovement = new Event();

            Listen(OnStart, () =>
            {
                foreach (var component in Components.ToArray()) Start(component);
                
                // Load the script for the object.
                // Special case for when custom_script_server but there is no script component.
                if (!this.Settings.TryGetValue("custom_script_server", out var scriptNameValue) || 
                    this.GetComponent<LuaScriptComponent>() != default) return;
                var scriptName = ((string) scriptNameValue).ToLower();
                Logger.Debug($"{this} -> {scriptNameValue}");
                foreach (var (objectScriptName, objectScriptType) in Zone.ScriptManager.ObjectScriptTypes)
                {
                    if (!scriptName.EndsWith(objectScriptName)) continue;
                    this.Zone.LoadObjectScript(this, objectScriptType);
                    break;
                }
            });

            Listen(OnDestroyed, () =>
            {
                OnLayerChanged.Clear();
                
                OnInteract.Clear();
                
                OnEmoteReceived.Clear();
                
                Zone.UnregisterObject(this);

                foreach (var component in Components.ToArray()) Destroy(component);

                Destruct(this);
                
                Id = ObjectId.Invalid;
            });

            Listen(OnLayerChanged, mask =>
            {
                foreach (var player in Zone.Players)
                {
                    player.TriggerViewUpdate(this);
                }
            });
        }

        #region Operators

        public static implicit operator long(GameObject gameObject)
        {
            if (gameObject == null) return -1;
            return gameObject.Id;
        }

        #endregion

        #region Utils

        public override string ToString()
        {
            return $"[{Id}] \"{(string.IsNullOrWhiteSpace(ObjectName) ? ClientName : Name)}\"";
        }

        #endregion

        #region Component Management

        /// <summary>
        /// Adds a new component to the object, even if none exists.
        /// </summary>
        /// <param name="type">Type of the component.</param>
        /// <returns>The new component.</returns>
        private Component AddNewComponent(Type type)
        {
            // Create the component.
            if (Object.Instantiate(type, Zone) is Component component)
            {
                // Add the component.
                component.GameObject = this;
                Components.Add(component);

                // Add the required components.
                var requiredComponents = type.GetCustomAttributes<RequireComponentAttribute>().ToArray();
                foreach (var attribute in requiredComponents.Where(r => r.Priority)) AddComponent(attribute.Type);
                foreach (var attribute in requiredComponents.Where(r => !r.Priority)) AddComponent(attribute.Type);

                // Start and return the component.
                Start(component);
                return component;
            }

            // Show an error if the type is not a component.
            Logger.Error($"{type.FullName} does not inherit form Components but is being Created as one.");
            return null;
        }
        
        /// <summary>
        /// Adds a component to the object if none exists.
        /// </summary>
        /// <param name="type">Type of the component.</param>
        /// <returns>The new or existing component.</returns>
        public Component AddComponent(Type type)
        {
            if (TryGetComponent(type, out var addedComponent)) return addedComponent;
            return AddNewComponent(type);
        }

        /// <summary>
        /// Adds a component to the object if none exists.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        /// <returns>The new or existing component.</returns>
        public T AddComponent<T>() where T : Component
        {
            return AddComponent(typeof(T)) as T;
        }
        
        /// <summary>
        /// Initializes the components to the object.
        /// </summary>
        public void InitializeComponents()
        {
            // ReSharper disable PossibleInvalidOperationException
            // Collect all the components on this object.
            var registryComponents = ClientCache.FindAll<ComponentsRegistry>(this.Lot);

            // Select all the none networked components on this object.
            var componentEntries = registryComponents.Where(o =>
                o.Componenttype != null && !ReplicaComponent.ComponentOrder.Contains(o.Componenttype.Value)
            ).ToArray();

            // Add components from the entries
            foreach (var component in componentEntries)
            {
                var componentType = ReplicaComponent.GetReplica((ComponentId) (int) component.Componenttype);
                if (componentType != default) this.AddComponent(componentType);
            }

            // Select all the networked components on this object
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
                if (componentType == null) Logger.Warning($"No component of ID {(ComponentId) component.Componenttype}");
                else this.AddComponent(componentType);
            }

            // Check if this object is a trigger
            if (this.Settings.ContainsKey("trigger_id"))
            {
                this.AddComponent<TriggerComponent>();
            }
            if (this.Settings.ContainsKey("primitiveModelType"))
            {
                this.AddComponent<PrimitiveModelComponent>();
            }
        }

        public Component GetComponent(Type type)
        {
            return Components.FirstOrDefault(c => c.GetType() == type);
        }

        public Component[] GetComponents(Type type)
        {
            return Components.Where(c => c.GetType() == type).ToArray();
        }

        public T GetComponent<T>() where T : Component
        {
            return Components.FirstOrDefault(c => c is T) as T;
        }

        public T[] GetComponents<T>() where T : Component
        {
            return Components.OfType<T>().ToArray();
        }

        public Component[] GetAllComponents()
        {
            return Components.ToArray();
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
            var comp = GetComponent(type);
            
            Components.Remove(comp);

            Destroy(comp);
        }

        public void RemoveComponent<T>() where T : Component
        {
            RemoveComponent(typeof(T));
        }

        public void RemoveComponent(Component component)
        {
            if (Components.Contains(component)) RemoveComponent(component.GetType());
        }

        #endregion

        #region Networking

        public static void Construct(GameObject gameObject)
        {
            Zone.SendConstruction(gameObject, gameObject.Zone.Players);
        }

        public static void Serialize(GameObject gameObject)
        {
            Zone.SendSerialization(gameObject, gameObject.Zone.Players);
        }

        public static void Destruct(GameObject gameObject)
        {
            Zone.SendDestruction(gameObject, gameObject.Zone.Players);
        }

        /// <summary>
        ///     Causes this GameObject to be deconstructed and than reconstructed on all Viewers.
        /// </summary>
        public void Reload()
        {
            var viewers = Viewers;
            
            Zone.SendDestruction(this, viewers);

            Zone.SendConstruction(this, viewers);
        }

        #endregion

        #region Instaniate

        #region From Raw

        public static GameObject Instantiate(Type type, Object parent, string name = "", Vector3 position = default,
            Quaternion rotation = default, float scale = 1, ObjectId objectId = default, Lot lot = default,
            SpawnerComponent spawner = default)
        {
            if (type.IsSubclassOf(typeof(GameObject)) || type == typeof(GameObject))
            {
                var instance = (GameObject) Object.Instantiate(type, parent.Zone);
                
                instance.Id = objectId == 0L ? ObjectId.Standalone : objectId;

                instance.Lot = lot;

                instance.Name = name;

                var obj = ClientCache.Find<Core.Client.Objects>(lot);
                instance.ClientName = obj?.Name;

                instance.Spawner = spawner;

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
            Quaternion rotation = default, float scale = 1, ObjectId objectId = default, Lot lot = default,
            SpawnerComponent spawner = default)
            where T : GameObject
        {
            return Instantiate(typeof(T), parent, name, position, rotation, scale, objectId, lot, spawner) as T;
        }
        
        public static GameObject Instantiate(Object parent, string name = "", Vector3 position = default,
            Quaternion rotation = default, float scale = 1, ObjectId objectId = default, Lot lot = default,
            SpawnerComponent spawner = default)
        {
            return Instantiate(typeof(GameObject), parent, name, position, rotation, scale, objectId, lot, spawner);
        }
        
        #endregion

        #region From Template

        public static GameObject Instantiate(Type type, Object parent, Lot lot, Vector3 position = default,
            Quaternion rotation = default, GameObject author = default)
        {
            return Instantiate(type, new LevelObjectTemplate
            {
                Lot = lot,
                Position = position,
                Rotation = rotation,
                Scale = 1,
                LegoInfo = new LegoDataDictionary()
            }, parent, author: author);
        }

        public static T Instantiate<T>(Object parent, Lot lot, Vector3 position = default,
            Quaternion rotation = default, GameObject author = default) where T : GameObject
        {
            return Instantiate(typeof(T), parent, lot, position, rotation, author) as T;
        }

        public static GameObject Instantiate(Object parent, Lot lot, Vector3 position = default,
            Quaternion rotation = default, GameObject author = default)
        {
            return Instantiate(typeof(GameObject), parent, lot, position, rotation, author);
        }

        #endregion

        #region From LevelObject

        public static GameObject Instantiate(Type type, LevelObjectTemplate levelObject, Object parent,
            SpawnerComponent spawner = default, GameObject author = default)
        {
            // Check if spawner
            if (levelObject.LegoInfo.TryGetValue("spawntemplate", out _) && spawner == default)
                return InstancingUtilities.Spawner(levelObject, parent);

            var name = levelObject.LegoInfo.TryGetValue("npcName", out var npcName) ? (string) npcName : "";

            // Create GameObject
            var id = levelObject.ObjectId == ObjectId.Invalid
                ? ObjectId.FromFlags(ObjectIdFlags.Spawned | ObjectIdFlags.Client)
                : (ObjectId) levelObject.ObjectId;

            var instance = Instantiate(
                type,
                parent,
                name,
                levelObject.Position,
                levelObject.Rotation,
                levelObject.Scale,
                id,
                levelObject.Lot
            );

            instance.Spawner = spawner;
            instance.Settings = levelObject.LegoInfo;

            // Set the author.
            // Ensures the author is set before the object is started instead of after.
            if (author != default && instance is AuthoredGameObject authoredGameObject)
            {
                authoredGameObject.Author = author;
            }

            // Add the components.
            instance.InitializeComponents();
            return instance;
        }

        public static T Instantiate<T>(LevelObjectTemplate levelObject, Object parent,
            SpawnerComponent spawner = default, GameObject author = default)
            where T : GameObject
        {
            return Instantiate(typeof(T), levelObject, parent, spawner, author) as T;
        }

        public static GameObject Instantiate(LevelObjectTemplate levelObject, Object parent,
            SpawnerComponent spawner = default, GameObject author = default)
        {
            return Instantiate(typeof(GameObject), levelObject, parent, spawner, author);
        }

        #endregion

        #endregion

        #region Replica

        internal void WriteConstruct(BitWriter writer)
        {
            writer.Write(Id);

            writer.Write(Lot);

            writer.Write((byte) ObjectName.Length);
            writer.WriteString(ObjectName, ObjectName.Length, true);

            writer.Write<uint>(0); // TODO: Add creation time?

            writer.WriteBit(false); // TODO: figure this out

            var trigger = GetComponent<TriggerComponent>();

            var hasTriggerId = trigger?.Trigger != null;

            writer.WriteBit(hasTriggerId);

            if (writer.Flag(Spawner != null))
                writer.Write(Spawner.GameObject.Id);

            if (writer.Flag(Spawner != null && Spawner.IsNetworkSpawner))
                writer.Write(Spawner.SpawnerNodeId);

            var hasScale = !Transform.Scale.Equals(-1);

            writer.WriteBit(hasScale);

            if (hasScale)
                writer.Write(Transform.Scale);

            if (writer.Flag(GameMasterLevel != default))
            {
                writer.Write((byte) GameMasterLevel);
            }

            if (writer.Flag(WorldState != ObjectWorldState.World))
            {
                writer.Write((byte) WorldState);
            }

            WriteHierarchy(writer);

            //
            // Construct replica components.
            //

            foreach (var replicaComponent in ReplicaComponents) replicaComponent.Construct(writer);
        }

        internal void WriteSerialize(BitWriter writer)
        {
            WriteHierarchy(writer);

            //
            // Serialize replica components.
            //

            foreach (var replicaComponent in ReplicaComponents) replicaComponent.Serialize(writer);
        }

        private void WriteHierarchy(BitWriter writer)
        {
            writer.WriteBit(true);

            if (writer.Flag(Transform.Parent != null))
            {
                writer.Write(Transform.Parent.GameObject);
                writer.WriteBit(false);
            }

            if (!writer.Flag(Transform.Children.Length != default)) return;
            
            writer.Write((ushort) Transform.Children.Length);

            foreach (var child in Transform.Children) writer.Write(child.GameObject);
        }

        #endregion
        
        public static GameObject InvalidObject => new GameObject();
    }
}
