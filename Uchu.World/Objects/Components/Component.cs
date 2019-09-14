namespace Uchu.World
{
    public abstract class Component : Object
    {
        public GameObject GameObject { get; set; }

        public Player Player => GameObject as Player;

        public Transform Transform => GameObject.Transform;

        public Component()
        {
            OnDestroyed += () => { GameObject.RemoveComponent(this); };
        }
    }
}