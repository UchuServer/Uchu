using System;
using Uchu.Core;
using Uchu.Core.Packets.Server.GameMessages;
using Uchu.Core.Scriptable;

namespace Uchu.World.Scriptable
{
    /// <inheritdoc />
    /// <summary>
    ///     Script for every Vendor.
    /// </summary>
    [AutoAssign(typeof(VendorComponent))]
    public class VendorScript : GameScript
    {
        /*
         * Vendors should send a VendorOpenWindowMessage message and than a VendorStatusUpdateMessage to update the
         * client on the LOTs for sale.
         * TODO: Look into why this is not working...
         */

        /// <summary>
        ///     Inherited Constructor.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="replicaPacket"></param>
        public VendorScript(Core.World world, ReplicaPacket replicaPacket) : base(world, replicaPacket)
        {
        }

        /// <summary>
        ///     Called once when world loaded.
        /// </summary>
        public override void Start()
        {
            Console.WriteLine($"{ObjectID} got VendorScript");
        }

        /// <summary>
        ///     Called when a player interacts with this vendor.
        /// </summary>
        /// <param name="player"></param>
        public override void OnUse(Player player)
        {
            Console.WriteLine($"Vendor Interaction! {ObjectID}");

            return;
            // Open a Vendor Window on the client. TODO: This is not working, WHY!?
            Server.Send(new VendorOpenWindowMessage
            {
                ObjectId = ObjectID
            }, player.EndPoint);

            // Send the LOTs for sale. TODO: I have no idea if this is set up correctly or not.
            Server.Send(new VendorStatusUpdateMessage
            {
                ObjectId = ObjectID,
                UpdateOnly = false,
                LOTs = new[] {8036}
            }, player.EndPoint);
        }
    }
}