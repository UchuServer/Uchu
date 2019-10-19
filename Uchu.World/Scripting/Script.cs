using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Scripting
{
    public abstract class Script
    {
        protected Zone Zone { get; private set; }

        protected Server Server => Zone.Server;

        protected void Start(Object obj) => Object.Start(obj);
        
        protected void Destroy(Object obj) => Object.Destroy(obj);

        protected void Construct(GameObject gameObject) => GameObject.Construct(gameObject);
        
        protected void Serialize(GameObject gameObject) => GameObject.Serialize(gameObject);
        
        protected void Destruct(GameObject gameObject) => GameObject.Destruct(gameObject);
        
        internal void SetZone(Zone zone)
        {
            Zone = zone;
        }
        
        public abstract Task LoadAsync();

        public virtual Task UnloadAsync()
        {
            return Task.CompletedTask;
        }
    }
}