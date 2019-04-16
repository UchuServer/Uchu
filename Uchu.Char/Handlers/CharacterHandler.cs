using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterHandler : HandlerGroupBase
    {
        public async Task SendCharacterList(IPEndPoint endpoint, long userId)
        {
            using (var ctx = new UchuContext())
            {
                var user = await ctx.Users.Include(u => u.Characters).ThenInclude(c => c.Items)
                    .SingleAsync(u => u.UserId == userId);

                var charCount = user.Characters.Count;
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
                        LastActivity = (ulong) chr.LastActivity,
                        Items = chr.Items.Where(itm => itm.IsEquipped).Select(itm => (uint) itm.LOT).ToArray()
                    };
                }

                Server.Send(new CharacterListResponsePacket
                {
                    CharacterCount = (byte) charCount,
                    CharacterIndex = (byte) user.CharacterIndex,
                    Characters = chars
                }, endpoint);
            }
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
            /*
             * TODO: The Server should add the chosen shirt and legs to the player inventory and equip them.
             */
            
            var session = Server.SessionCache.GetSession(endpoint);

            var first = (await Server.Resources.ReadTextAsync("Names/first.txt")).Split('\n');
            var middle = (await Server.Resources.ReadTextAsync("Names/middle.txt")).Split('\n');
            var last = (await Server.Resources.ReadTextAsync("Names/last.txt")).Split('\n');

            /*
             * Make sure there are no awkward newlines in the player name.
             */
            var rawName = first[packet.Predefined.First] + middle[packet.Predefined.Middle] + last[packet.Predefined.Last];
            var name = "";

            foreach (var c in rawName)
            {
                if (c != (char) 13)
                    name += c;
            }

            using (var ctx = new UchuContext())
            {
                var user = await ctx.Users.Include(u => u.Characters).SingleAsync(u => u.UserId == session.UserId);

                user.Characters.Add(new Character
                {
                    CharacterId = Utils.GenerateObjectId(),
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
                });

                await ctx.SaveChangesAsync();

                Server.Send(new CharacterCreateResponsePacket {ResponseId = CharacterCreationResponse.Success}, endpoint);

                await SendCharacterList(endpoint, session.UserId);
            }
        }

        [PacketHandler]
        public async Task DeleteCharacter(CharacterDeleteRequestPacket packet, IPEndPoint endpoint)
        {
            using (var ctx = new UchuContext())
            {
                ctx.Characters.Remove(await ctx.Characters.FindAsync(packet.CharacterId));

                await ctx.SaveChangesAsync();

                Server.Send(new CharacterDeleteResponsePacket(), endpoint);
            }
        }

        [PacketHandler]
        public async Task RenameCharacter(CharacterRenameRequestPacket packet, IPEndPoint endpoint)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(packet.CharacterId);

                chr.Name = packet.Name;

                await ctx.SaveChangesAsync();
            }
        }

        [PacketHandler]
        public void JoinWorld(JoinWorldPacket packet, IPEndPoint endpoint)
        {
            Server.SessionCache.SetCharacter(endpoint, packet.CharacterId);

            var addresses = NetworkInterface.GetAllNetworkInterfaces()
                .Where(i => (i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                             i.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
                            i.OperationalStatus == OperationalStatus.Up)
                .SelectMany(i => i.GetIPProperties().UnicastAddresses).Select(a => a.Address)
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray();

            var address = endpoint.Address.ToString() == "127.0.0.1" ? "127.0.0.1" : addresses[0].ToString();

            Server.Send(new ServerRedirectionPacket
            {
                Address = address,
                Port = 2003
            }, endpoint);
        }
    }
}