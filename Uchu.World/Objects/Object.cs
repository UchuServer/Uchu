using System;
using Uchu.Core;

namespace Uchu.World
{
    public abstract class Object
    {
        public Zone Zone;

        public Server Server => Zone.Server;
        
        public event Action OnInstantiated;

        public event Action OnDestroyed;
        
        public static Object Instantiate(Type type, Zone zone, bool callInstantiated = true)
        {
            if (Activator.CreateInstance(type) is Object instance)
            {
                instance.Zone = zone;
                zone.Objects.Add(instance);

                if (callInstantiated) instance.Instantiated();
                
                return instance;
            }
            
            Logger.Error($"{type.FullName} does not inherit from Object but is being Created as one.");
            return null;
        }

        public static T Instantiate<T>(Zone zone) where T : Object
        {
            return Instantiate(typeof(T), zone) as T;
        }

        public static void Destroy(Object obj)
        {
            if (obj.Zone.Objects.Contains(obj))
            {
                obj.Zone.Objects.Remove(obj);
                obj.End();
            }
            else
                Logger.Error($"{obj} is already destroyed!");
        }

        public static bool operator ==(Object object1, Object object2)
        {
            if (ReferenceEquals(object2, null))
            {
                return ReferenceEquals(object1, null) || object1.Zone.Objects.Contains(object1);
            }

            return ReferenceEquals(object1, object2);
        }

        public static bool operator !=(Object object1, Object object2)
        {
            return !(object1 == object2);
        }

        public virtual void Instantiated()
        {
            OnInstantiated?.Invoke();
        }

        public virtual void Serialize(){}

        public virtual void Update(){}

        public virtual void End()
        {
            OnDestroyed?.Invoke();
        }
    }
}