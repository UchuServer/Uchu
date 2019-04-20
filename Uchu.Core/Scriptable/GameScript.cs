using System;
using System.Linq;
using System.Threading.Tasks;

namespace Uchu.Core.Scriptable
{
    /// <summary>
    ///     Class every game script inherits from.
    /// </summary>
    public class GameScript
    {
        #region Management

        /// <summary>
        ///     The Replica Packet of the object this script is assigned to.
        /// </summary>
        public readonly ReplicaPacket ReplicaPacket;

        /// <summary>
        ///     The world the object this script is assigned to is.
        /// </summary>
        public readonly World World;

        #endregion

        #region Properties

        /// <summary>
        ///     The server the object this script is assigned to is.
        /// </summary>
        public Server Server => World.Server;

        /// <summary>
        ///     The zone the object this script is assigned to is.
        /// </summary>
        public Zone Zone => World.Zone;

        /*
         * Most of these does nothing at the moment.
         * TODO: Make them update properties on the replica.
         */

        public string Name { get; set; }

        public float Scale { get; set; } = 1;

        public string[] Tags { get; set; } = {"Untagged"};

        public int Layers { get; set; }

        public int LOT => ReplicaPacket.LOT;

        public long ObjectID => ReplicaPacket.ObjectId;

        #endregion

        #region Script Methods

        public GameScript(World world, ReplicaPacket replicaPacket)
        {
            World = world;
            ReplicaPacket = replicaPacket;
        }

        /// <summary>
        ///     Update this Object.
        /// </summary>
        public void UpdateObject()
        {
            /*
             * This is not used at this moment in time.
             * TODO: Call this once every 1/60 of a second or so.
             */

            ReplicaPacket.Name = Name;

            ReplicaPacket.Scale = Scale;

            Update();

            World.UpdateObject(this);
        }

        /// <summary>
        ///     Called once at the start of world load.
        /// </summary>
        public virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }

        /// <summary>
        ///     Called when a player interacts with the Replica Object this script is assigned to.
        /// </summary>
        /// <param name="player">Player who interacted</param>
        public virtual void OnUse(Player player)
        {
        }

        /// <summary>
        ///     Called when a player stops a quickbuild on the Replica Object this script is assigned to.
        /// </summary>
        /// <param name="player">Player who stopped the quickbuild</param>
        public virtual void OnRebuildCanceled(Player player)
        {
        }

        /// <summary>
        ///     Called when a player smashes the Replica Object this script is assigned to.
        /// </summary>
        /// <param name="player">Player who smashed</param>
        /// <returns></returns>
        public virtual async Task OnSmash(Player player)
        {
        }

        /// <summary>
        ///     Called then a player collects the Replica Object this script is assigned to.
        /// </summary>
        /// <param name="player">Player who collected</param>
        /// <returns></returns>
        public virtual async Task OnCollected(Player player)
        {
            
        }

        /// <summary>
        ///     Implicit operator to access the ReplicaPacket this script is assigned to.
        /// </summary>
        /// <param name="gameScript"></param>
        /// <returns></returns>
        public static implicit operator ReplicaPacket(GameScript gameScript)
        {
            return gameScript.ReplicaPacket;
        }

        #endregion

        #region GameScript Methods

        /// <summary>
        ///     Get a GameScript assigned to this object.
        /// </summary>
        /// <typeparam name="T">Type of Script</typeparam>
        /// <returns>Script of type T. Null if not found.</returns>
        public T GetScript<T>() where T : GameScript
        {
            return ReplicaPacket.GameScripts.FirstOrDefault(c => c is T) as T;
        }

        /// <summary>
        ///     Get a GameScript assigned to this object.
        /// </summary>
        /// <param name="type">Type of Script</param>
        /// <returns>Script of type passed. Null if not found.</returns>
        public GameScript GetScript(Type type)
        {
            return ReplicaPacket.GameScripts.FirstOrDefault(c => c.GetType() == type);
        }

        /// <summary>
        ///     Get ReplicaComponent on this object.
        /// </summary>
        /// <typeparam name="T">Type of Component</typeparam>
        /// <returns>Component of type T. Null if not found.</returns>
        public T GetComponent<T>() where T : ReplicaComponent
        {
            return ReplicaPacket.Components.FirstOrDefault(c => c is T) as T;
        }

        /// <summary>
        ///     Get ReplicaComponent on this object.
        /// </summary>
        /// <param name="type">Type of Component</param>
        /// <returns>Component of type passed. Null if not found.</returns>
        public ReplicaComponent GetComponent(Type type)
        {
            return ReplicaPacket.Components.FirstOrDefault(c => c.GetType() == type) as ReplicaComponent;
        }

        #endregion
    }
}