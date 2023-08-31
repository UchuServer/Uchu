using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class BuildBorderComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.BuildBorder;

        protected BuildBorderComponent()
        {
            Logger.Debug("New BuildBorderComponent");
            Listen(OnStart, () => Listen(GameObject.OnInteract, OnInteract));
        }

        private async void OnInteract(Player player)
        {
            Logger.Debug($"{player} interacted with BuildBorderComponent on {GameObject}");

            // Maybe this is usefull. It's no implemented yet
            var inventory = player.GetComponent<InventoryComponent>();
            inventory.PushEquippedItemState();

            return;

            // This is not needed
            var inventoryManager = player.GetComponent<InventoryManagerComponent>();
            var thinkingHat = inventoryManager.FindItem(Lot.ThinkingHat);
            await thinkingHat.EquipAsync(true);

            // This is in DLU but seems to mess things up
            player.Message(new StartArrangingWithItemMessage
            {
                Associate = player,
                FirstTime = true,
                BuildArea = GameObject,
                StartPosition = player.Transform.Position
            });

        }

        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
        }
    }
}