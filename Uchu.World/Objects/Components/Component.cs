namespace Uchu.World
{
    public abstract class Component : Object
    {
        public GameObject GameObject { get; set; }

        public Transform Transform => GameObject.Transform;

        protected override void End()
        {
            base.End();

            // Ensure this component is removed.
            GameObject.RemoveComponent(this);
        }
    }
}