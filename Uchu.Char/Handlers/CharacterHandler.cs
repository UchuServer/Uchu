using System;
using System.Net;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterHandler : HandlerGroupBase
    {
        public async Task SendCharacterList(IPEndPoint endpoint, long userId)
        {
            var user = await Database.GetUserAsync(userId);

            if (user == null)
            {
                Server.DisconnectClient(endpoint);

                return;
            }

            var charCount = user.Characters?.Count ?? 0;

            var chars = new CharacterListResponsePacket.Character[charCount];

            for (var i = 0; i < charCount; i++)
            {
                var chr = user.Characters[i];

                chars[i] = new CharacterListResponsePacket.Character
                {
                    CharacterId = chr.CharacterId,
                    Name = chr.Name,
                    UnnaprovedName = chr.CustomName,
                    NameRejected = chr.NameRejected,
                    FreeToPlay = chr.FreeToPlay,
                    ShirtColor = (uint) chr.ShirtColor,
                    ShirtStyle = (uint) chr.ShirtStyle,
                    PantsColor = (uint) chr.PantsColor,
                    HairStyle = (uint) chr.HairStyle,
                    HairColor = (uint) chr.HairColor,
                    Lh = (uint) chr.Lh,
                    Rh = (uint) chr.Rh,
                    EyebrowStyle = (uint) chr.EyebrowStyle,
                    EyeStyle = (uint) chr.EyeStyle,
                    MouthStyle = (uint) chr.MouthStyle,
                    LastZone = (ZoneId) chr.LastZone,
                    LastInstance = (ushort) chr.LastInstance,
                    LastClone = (uint) chr.LastClone,
                    LastActivity = (ulong) chr.LastActivity
                };
            }

            Server.Send(new CharacterListResponsePacket
            {
                CharacterCount = (byte) charCount,
                CharacterIndex = (byte) user.CharacterIndex,
                Characters = chars
            }, endpoint);
        }

        [PacketHandler]
        public async Task CharacterList(CharacterListRequestPacket packet, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);

            await SendCharacterList(endpoint, session.UserId);
        }

        [PacketHandler]
        public async Task CharacterCreate(CharacterCreateRequestPacket packet, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);

            var first = (await Server.Resources.ReadTextAsync("Names/first.txt")).Split('\n');
            var middle = (await Server.Resources.ReadTextAsync("Names/middle.txt")).Split('\n');
            var last = (await Server.Resources.ReadTextAsync("Names/last.txt")).Split('\n');

            var name = first[packet.Predefined.First] + middle[packet.Predefined.Middle] + last[packet.Predefined.Last];

            await Database.CreateCharacterAsync(new Character
            {
                Name = name,
                CustomName = packet.CustomName,
                ShirtColor = packet.ShirtColor,
                ShirtStyle = packet.ShirtStyle,
                PantsColor = packet.PantsColor,
                HairStyle = packet.HairStyle,
                HairColor = packet.HairColor,
                Lh = packet.Lh,
                Rh = packet.Rh,
                EyebrowStyle = packet.EyebrowStyle,
                EyeStyle = packet.EyeStyle,
                MouthStyle = packet.MouthStyle,
                LastZone = (int) ZoneId.VentureExplorerCinematic,
                LastInstance = 0,
                LastClone = 0,
                LastActivity = DateTimeOffset.Now.ToUnixTimeSeconds()
            }, session.UserId);

            // TODO: check if creation actually succeeded
            Server.Send(new CharacterCreateResponsePacket {ResponseId = CharacterCreationResponse.Success}, endpoint);

            await SendCharacterList(endpoint, session.UserId);
        }

        [PacketHandler]
        public async Task DeleteCharacter(CharacterDeleteRequestPacket packet, IPEndPoint endpoint)
        {
            await Database.DeleteCharacterAsync(packet.CharacterId);

            Server.Send(new CharacterCreateResponsePacket(), endpoint);
        }

        [PacketHandler]
        public async Task RenameCharacter(CharacterRenameRequestPacket packet, IPEndPoint endpoint)
        {
            await Database.RenameCharacterAsync(packet.CharacterId, packet.Name);
        }

        [PacketHandler]
        public void JoinWorld(JoinWorldPacket packet, IPEndPoint endpoint)
        {
            Server.SessionCache.SetCharacter(endpoint, packet.CharacterId);

            Server.Send(new ServerRedirectionPacket
            {
                Address = "127.0.0.1",
                Port = 2003
            }, endpoint);
        }
    }
}