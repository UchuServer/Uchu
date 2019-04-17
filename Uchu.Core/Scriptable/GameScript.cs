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

        public virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }

        public virtual void OnUse(Player player)
        {
        }

        public virtual void OnRebuildCanceled(Player player)
        {
        }

        public virtual async Task OnSmash(Player player)
        {
        }

        public static implicit operator ReplicaPacket(GameScript gameScript) => gameScript.ReplicaPacket;

        #endregion
    }
}