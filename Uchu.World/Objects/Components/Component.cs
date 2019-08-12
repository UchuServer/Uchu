namespace Uchu.World
{
    public abstract class Component : Object
    {
        public GameObject GameObject { get; set; }

        public Player Player => GameObject as Player;

        public Transform Transform => GameObject.Transform;

        public override void End()
        {
            base.End();

            // Ensure this component is removed.
            GameObject.RemoveComponent(this);
        }
    }
}