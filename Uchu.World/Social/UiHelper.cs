using System.IO;
using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Social
{
    public static class UiHelper
    {
        public static async Task OpenMailboxAsync(Player player)
        {
            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);

            writer.Write((byte) Amf3Type.Array);
            writer.Write<byte>(1);
            Amf3Helper.WriteText(writer, "state");

            writer.Write((byte) Amf3Type.String);
            Amf3Helper.WriteText(writer, "Mail");

            writer.Write((byte) Amf3Type.Null);
            
            player.Message(new UiMessageServerToSingleClientMessage
            {
                Associate = player,
                Content = stream.ToArray(),
                MessageName = "pushGameState"
            });
        }

        public static async Task CloneMailboxAsync(Player player)
        {
            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);

            writer.Write((byte) Amf3Type.Array);
            writer.Write<byte>(1);
            Amf3Helper.WriteText(writer, "visible");

            writer.Write((byte) Amf3Type.False);
            writer.Write((byte) Amf3Type.Null);
            
            player.Message(new UiMessageServerToSingleClientMessage
            {
                Associate = player,
                Content = stream.ToArray(),
                MessageName = "ToggleMail"
            });
        }

        public static async Task AnnouncementAsync(Player player, string title, string message)
        {
            await using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);
            
            writer.Write((byte) Amf3Type.Array);
            writer.Write<byte>(1);
            Amf3Helper.WriteText(writer, "message");
            
            writer.Write((byte) Amf3Type.String);
            Amf3Helper.WriteText(writer, message);
            Amf3Helper.WriteText(writer, "title");

            writer.Write((byte) Amf3Type.String);
            Amf3Helper.WriteText(writer, title);
            Amf3Helper.WriteText(writer, "visible");

            writer.Write((byte) Amf3Type.True);
            writer.Write((byte) Amf3Type.Null);
            
            player.Message(new UiMessageServerToSingleClientMessage
            {
                Associate = player,
                Content = stream.ToArray(),
                MessageName = "ToggleAnnounce"
            });
        }
    }
}