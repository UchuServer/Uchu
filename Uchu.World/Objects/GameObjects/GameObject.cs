using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Uchu.Core;

namespace Uchu.World
{
    public class GameObject : Object
    {
        public long ObjectId { get; private set; }
        
        public int Lot { get; private set; }
        
        public string Name { get; set; }
        
        public Transform Transform => _components.First(c => c is Transform) as Transform;

        private readonly List<Component> _components = new List<Component>();

        public Component AddComponent(Type type)
        {
            if (Create(type, Zone) is Component component)
            {
                component.GameObject = this;
                
                _components.Add(component);

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
            if (type.GetCustomAttribute<EssentialAttribute>() != null)
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

        protected override void Start()
        {
            Zone.GameObjects.Add(this);
        }
        
        protected override void End()
        {
            Zone.GameObjects.Remove(this);
        }

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

        public static GameObject Instantiate(Type type, Object parent, string name = default, Vector3 position = default,
            Quaternion rotation = default, long objectId = default, int lot = default)
        {
            if (Create(type, parent.Zone) is GameObject instance)
            {
                instance.ObjectId = objectId == default ? Utils.GenerateObjectId() : objectId;

                instance.Lot = lot;
                
                instance.Name = name;
                
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
            }
            
            Logger.Error($"{type.FullName} does not inherit from GameObject but is being Instantiated as one.");
            return null;
        }

        public static T Instantiate<T>(Object parent, string name = default, Vector3 position = default,
            Quaternion rotation = default, long objectId = default, int lot = default) where T : GameObject
        {
            return Instantiate(typeof(T), parent, name, position, rotation, objectId, lot) as T;
        }
    }
}