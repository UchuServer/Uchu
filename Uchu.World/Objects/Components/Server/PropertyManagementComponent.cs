using System.Collections.Generic;
using System.Threading.Tasks;
using Uchu.World.Services;
using Uchu.World;
using System.Numerics;
using Uchu.Core.Client;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
                Listen(message.Associate.OnReady, () => {
                    using var ctx = new CdClientContext();
                    var column = ctx.PropertyTemplateTable.FirstOrDefaultAsync(a => a.MapID == 1150);

                    List<string> items = column.Result.Path.Split(' ').ToList();

                    origin.Message(new DownloadPropertyDataMessage
                    {
                        Associate = message.Associate,
                        ZoneID = origin.Zone.ZoneId.Id,
                        VendorMapID = 1100,
                        OwnerName = origin.Name,
                        OwnerObjID = origin.Id,
                        SpawnName = "AGSmallProperty",
                        SpawnPosition = new Vector3((float)column.Result.ZoneX, (float)column.Result.ZoneY, (float)column.Result.ZoneZ),
                        MaxBuildHeight = 128.0f,
                        Paths = items
                    });

                    origin.Message(new UpdatePropertyModelCountMessage
                    {
                        Associate = message.Associate,
                        ModelCount = 0
                    });
                });
            });
        }

        public async Task OnInteract(Player player)
        {
            await player.SetFlagAsync(108, true);        }
    }
}