using System.Collections.Generic;
using System.Threading.Tasks;
using Uchu.World.Services;
using Uchu.World;
using System.Numerics;

namespace Uchu.World
{
    public class PropertyManagementComponent : Component
    {
        public Event<QueryPropertyData, Player> OnQueryPropertyData { get; } // Message, Origin

        public PropertyManagementComponent()
        {
            OnQueryPropertyData = new Event<QueryPropertyData, Player>();

            Listen(OnStart, () =>
            {
                Listen(GameObject.OnInteract, async player =>
                {
                    await OnInteract(player);
                });
            });

            Listen(OnQueryPropertyData, (message, origin) =>
            {
                List<Vector3> Paths = new List<Vector3>();
                foreach (var item in origin.Zone.ZoneInfo.LuzFile.PathData)
                {
                    foreach (var item2 in item.Waypoints)
                    {
                        Paths.Add(item2.Position);
                    }
                }

                origin.Message(new DownloadPropertyDataMessage
                {
                    Associate = message.Associate,
                    ZoneID = origin.Zone.ZoneId.Id,
                    VendorMapID = 1100,
                    OwnerName = origin.Name,
                    OwnerObjID = (long)origin.Id,
                    SpawnName = "AGSmallProperty",
                    SpawnPosition = origin.Zone.SpawnPosition,
                    MaxBuildHeight = 128.0f,
                    Paths = Paths
                });

                origin.Message(new UpdatePropertyModelCountMessage
                {
                    Associate = message.Associate,
                    ModelCount = 0
                });

                origin.Message(new ScriptNetworkVarUpdate
                {
                    Associate = origin.Zone.ZoneControlObject,
                    LDFInText = "unclaimed=7:1"
                });
            });
        }

        public async Task OnInteract(Player player)
        {
            await player.SetFlagAsync(71, true);
        }
    }
}