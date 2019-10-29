using System;
using System.Reflection;
using Uchu.Core;

namespace Uchu.World
{
    public abstract class Object
    {
        private bool _started;
        
        public Zone Zone;

        public Server Server => Zone.Server;

        public readonly Event OnStart = new Event();

        public readonly Event OnDestroyed = new Event();

        public readonly Event OnTick = new Event();

        public static Object Instantiate(Type type, Zone zone)
        {
            if (Activator.CreateInstance(type, true) is Object instance)
            {
                instance.Zone = zone;

                return instance;
            }

            Logger.Error($"{type.FullName} does not inherit from Object but is being Created as one.");
            return null;
        }

        public static T Instantiate<T>(Zone zone) where T : Object
        {
            return Instantiate(typeof(T), zone) as T;
        }

        public static void Start(Object obj)
        {
            if (obj._started) return;
            obj._started = true;
            
            obj.Zone.ManagedObjects.Add(obj);

            obj.OnStart?.Invoke();
        }

        public static void Destroy(Object obj)
        {
            obj.Zone.ManagedObjects.Remove(obj);

            obj.OnDestroyed.Invoke();
            
            obj.OnStart.Clear();
            obj.OnDestroyed.Clear();
            obj.OnTick.Clear();
        }

        protected static void Update(Object obj)
        {
            obj.OnTick.Invoke();
        }
    }
}