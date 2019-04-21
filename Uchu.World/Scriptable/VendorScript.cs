using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;
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
        /// <summary>
        ///     Inherited Constructor.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="replicaPacket"></param>
        public VendorScript(Core.World world, ReplicaPacket replicaPacket) : base(world, replicaPacket)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called once when world loaded.
        /// </summary>
        public override void Start()
        {
            Console.WriteLine($"{ObjectID} got VendorScript");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when a player interacts with this vendor.
        /// </summary>
        /// <param name="player"></param>
        public override async Task OnUse(Player player)
        {
            Console.WriteLine($"Vendor Interaction! {ObjectID}");

            // Open a Vendor Window on the client.
            Server.Send(new VendorOpenWindowMessage
            {
                ObjectId = ObjectID
            }, player.EndPoint);

            var comp = await Server.CDClient.GetVendorComponent(
                (int) await Server.CDClient.GetComponentIdAsync(LOT, 16));
            var matrix = await Server.CDClient.GetLootMatrix(comp.LootMatrixIndex);
            var shopItems = new List<Tuple<int, int>>();

            foreach (var max in matrix)
            {
                shopItems.AddRange(from row in await Server.CDClient.GetItemDropsAsync(max.LootTableIndex)
                    select new Tuple<int, int>(row.ItemId, row.SortPriority));
            }
            
            // Send the LOTs for sale.
            Server.Send(new VendorStatusUpdateMessage
            {
                ObjectId = ObjectID,
                UpdateOnly = false,
                LOTs = shopItems.ToArray()
            }, player.EndPoint);
        }
    }
}