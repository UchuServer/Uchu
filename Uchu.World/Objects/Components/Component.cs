namespace Uchu.World
{
    public abstract class Component : Object
    {
        protected Component()
        {
            OnDestroyed.AddListener(() => { GameObject.RemoveComponent(this); });
        }

        public GameObject GameObject { get; set; }

        public Transform Transform => GameObject.Transform;
        
        protected T As<T>() where T : GameObject => GameObject as T;
    }
}