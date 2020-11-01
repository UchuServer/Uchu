using System;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Triggers;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class PropertyVendorComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.PropertyVendorComponent;

        private bool ClaimedProperty = false;

        protected PropertyVendorComponent()
        {
            Listen(OnStart, async () =>
            {
                Listen(GameObject.OnInteract, async player =>
                {
                    await OnInteract(player);
                });
            });
        }

        private async Task OnInteract(Player player)
        {
            if (!ClaimedProperty)
            {
                player.Message(new OpenPropertyVendorMessage
                {
                    Associate = this.GameObject
                });

                Listen(player.OnVendorPurchase, async (message) => {
                    if (message.Associate == GameObject)
                    {
                        ClaimedProperty = true;

                        await player.SetFlagAsync(108, true);

                        player.Message(new PropertyRentalResponseMessage
                        {
                            Associate = this.GameObject,
                            CloneID = player.Zone.CloneId,
                            Code = PropertyRentalResponseCode.Success,
                            PropertyID = GameObject.Id,
                            RentDue = 0
                        });
                    }
                });
            }
        }

        public override void Construct(BitWriter writer) { }

        public override void Serialize(BitWriter writer) { }
    }
}