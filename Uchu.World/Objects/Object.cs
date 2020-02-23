using System;
using Uchu.Core;

namespace Uchu.World
{
    public class Object : ObjectBase
    {
        private bool _started;
        
        public Zone Zone { get; protected set; }

        public Server Server => Zone.Server;

        public Event OnStart { get; } = new Event();

        public Event OnDestroyed { get; } = new Event();

        public Event OnTick { get; } = new Event();

        protected Object()
        {
        }
        
        public static Object Instantiate(Type type, Zone zone)
        {
            if (Activator.CreateInstance(type, true) is Object instance)
            {
                instance.Zone = zone;

                return instance;
            }

            Logger.Error($"{type.FullName} does not inherit from Object but is being Instantiated as one.");
            return null;
        }

        public static T Instantiate<T>(Zone zone) where T : Object
        {
            return Instantiate(typeof(T), zone) as T;
        }
        
        public static Object Instantiate(Zone zone)
        {
            return Instantiate(typeof(Object), zone);
        }

        public static void Start(Object obj)
        {
            if (obj?._started ?? true) return;
            
            obj._started = true;
            
            obj.Zone.RegisterObject(obj);

            obj.OnStart?.Invoke();
        }

        public static void Destroy(Object obj)
        {
            obj.Zone.UnregisterObject(obj);

            obj.OnDestroyed.Invoke();
            
            obj.OnStart.Clear();
            obj.OnDestroyed.Clear();
            obj.OnTick.Clear();

            obj.ClearListeners();
        }
        
        protected static void Update(Object obj)
        {
            if (obj == null || !obj.OnTick.Any) return;
            
            obj.OnTick.Invoke();
        }
    }
}