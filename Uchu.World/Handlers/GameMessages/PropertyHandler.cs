using System.Collections.Generic;
using System.Threading.Tasks;
using Uchu.World.Services;
using Uchu.World;
using System.Numerics;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class PropertyHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task QueryPropertyDataHandler(QueryPropertyData message, Player player)
        {
            
            List<Vector3> Paths = new List<Vector3>();
            foreach (var item in player.Zone.ZoneInfo.LuzFile.PathData)
            {
                foreach (var item2 in item.Waypoints)
                {
                    Paths.Add(item2.Position);
                }
            }

            player.Message(new DownloadPropertyDataMessage
            {
                Associate = message.Associate,
                ZoneID = player.Zone.ZoneId.Id,
                VendorMapID = 1100,
                OwnerName = player.Name,
                OwnerObjID = (long)player.Id,
                SpawnName = "AGSmallProperty",
                SpawnPosition = player.Zone.SpawnPosition,
                MaxBuildHeight = 128.0f,
                Paths = Paths
            });

            player.Message(new UpdatePropertyModelCountMessage
            {
                Associate = message.Associate,
                ModelCount = 0
            });

            /*if (message.Associate.TryGetComponent<PropertyManagementComponent>(out var comp))
            {
                await comp.OnQueryPropertyData.InvokeAsync(message, player);
            } 
            else
            {
                message.Associate.AddComponent<PropertyManagementComponent>();
                await comp.OnQueryPropertyData.InvokeAsync(message, player);
            }*/
            
        }


    }
}
