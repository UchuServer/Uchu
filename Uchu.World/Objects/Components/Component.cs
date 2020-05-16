namespace Uchu.World
{
    public abstract class Component : Object
    {
        protected Component()
        {
            Listen(OnDestroyed, () => { GameObject.RemoveComponent(this); });
        }

        public GameObject GameObject { get; set; }

        public Transform Transform => GameObject.Transform;
    }
}