using System;
using Uchu.Core;

namespace Uchu.World
{
    public abstract class Object
    {
        public Zone Zone;

        public event Action OnCreated;

        public event Action OnDestroyed;
        
        public static Object Create(Type type, Zone zone)
        {
            if (Activator.CreateInstance(type) is Object instance)
            {
                instance.Zone = zone;
                zone.Objects.Add(instance);
                instance.OnCreated += instance.Start;
                instance.OnDestroyed += instance.End;
                instance.OnCreated?.Invoke();
                return instance;
            }
            
            Logger.Error($"{type.FullName} does not inherit from Object but is being Created as one.");
            return null;
        }

        public static T Create<T>(Zone zone) where T : Object
        {
            return Create(typeof(T), zone) as T;
        }

        public static void Destroy(Object obj)
        {
            if (obj.Zone.Objects.Contains(obj))
            {
                obj.Zone.Objects.Remove(obj);
                obj.OnDestroyed?.Invoke();
            }
            else
                Logger.Error($"{obj} is already destroyed!");
        }

        public static bool operator ==(Object object1, Object object2)
        {
            if (ReferenceEquals(object1, null)) return ReferenceEquals(object2, null);
            return ReferenceEquals(object2, null)
                ? object1.Zone.Objects.Contains(object1)
                : ReferenceEquals(object1, object2);
        }

        public static bool operator !=(Object object1, Object object2)
        {
            return !(object1 == object2);
        }
        
        protected virtual void Start(){}
        
        protected virtual void Update(){}
        
        protected virtual void End(){}
    }
}