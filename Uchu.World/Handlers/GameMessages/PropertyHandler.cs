using System.Collections.Generic;
using System.Threading.Tasks;
using Uchu.World.Services;
using Uchu.World;
using System.Numerics;
using Uchu.Core;
using Uchu.Core.Client;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Uchu.World.Social;
using RakDotNet.IO;
using System.IO;
using RakDotNet;

namespace Uchu.World.Handlers.GameMessages
{
    public class PropertyHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task QueryPropertyDataHandler(QueryPropertyData message, Player player)
        {
            using var ctx = new CdClientContext();
            var column = ctx.PropertyTemplateTable.FirstOrDefaultAsync(a => a.MapID == player.Zone.ZoneId.Id);

            List<string> items = column.Result.Path.Split(' ').ToList();

            string Name = player.Name;
            long OwnerObjectID = player.Id;

            player.Message(new DownloadPropertyDataMessage
            {
                Associate = message.Associate,
                ZoneID = player.Zone.ZoneId.Id,
                VendorMapID = (ushort)column.Result.VendorMapID,
                OwnerName = Name,
                OwnerObjID = OwnerObjectID,
                SpawnName = column.Result.SpawnName,
                SpawnPosition = new Vector3((float)column.Result.ZoneX, (float)column.Result.ZoneY, (float)column.Result.ZoneZ),
                MaxBuildHeight = 128.0f,
                Paths = items
            });

            player.Message(new UpdatePropertyModelCountMessage
            {
                Associate = message.Associate,
                ModelCount = 0
            });
        }

        [PacketHandler]
        public async Task PropertyEditorBeginHandler(PropertyEditorBeginMessage message, Player player)
        {
            var gameObject = player.Zone.GameObjects.FirstOrDefault(g => g.Id == message.propertyObjectID);

            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);

            writer.Write((byte)Amf3Type.Array);
            writer.Write<byte>(1);

            Amf3Helper.WriteText(writer, "bPaused");
            writer.Write((byte)(false ? Amf3Type.True : Amf3Type.False));
            writer.Write((byte)Amf3Type.Null);

            player.Message(new UiMessageServerToSingleClientMessage
            {
                Associate = player,
                Content = stream.ToArray(),
                MessageName = "UpdatePropertyStatus"
            });
        }
    }
}
