using System;
using Uchu.Core;

namespace Uchu.World
{
    public class Object : ObjectBase
    {
        public bool Started { get; private set; }

        public Zone Zone { get; protected set; }

        public UchuServer UchuServer => Zone.Server;

        public Event OnStart { get; }

        public Event OnDestroyed { get; }

        protected Object()
        {
            OnStart = new Event();
            
            OnDestroyed = new Event();
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
            if (obj?.Started ?? true) return;
            
            obj.Started = true;
            
            obj.Zone.RegisterObject(obj);

            obj.OnStart?.Invoke();
        }

        public static void Destroy(Object obj)
        {
            obj.Zone.UnregisterObject(obj);

            obj.OnDestroyed.Invoke();
            
            obj.OnStart.Clear();
            
            obj.OnDestroyed.Clear();

            obj.ClearListeners();
        }
    }
}