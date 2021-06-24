namespace Uchu.World.Scripting.Native
{
    public abstract class ObjectScript : ObjectBase
    {
        /// <summary>
        /// Object controlled by the script.
        /// </summary>
        public GameObject GameObject { get; }
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ObjectScript(GameObject gameObject)
        {
            this.GameObject = gameObject;
            
            // Connect clearing on destroyed.
            Listen(gameObject.OnDestroyed, this.Clear);
        }

        /// <summary>
        /// Clears a 
        /// </summary>
        public virtual void Clear()
        {
            this.ClearListeners();
        }
    }
}