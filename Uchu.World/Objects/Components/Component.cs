namespace Uchu.World
{
    public abstract class Component : Object
    {
        public Component()
        {
            OnDestroyed += () => { GameObject.RemoveComponent(this); };
        }

        public GameObject GameObject { get; set; }

        public Transform Transform => GameObject.Transform;
        
        protected T As<T>() where T : GameObject => GameObject as T;
    }
}