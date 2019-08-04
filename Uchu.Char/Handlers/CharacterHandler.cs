using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.CdClient;

namespace Uchu.Char.Handlers
{
    public class CharacterHandler : HandlerGroup
    {
        public async Task SendCharacterList(IPEndPoint endPoint, long userId)
        {
            using (var ctx = new UchuContext())
            {
                var user = await ctx.Users.Include(u => u.Characters).ThenInclude(c => c.Items)
                    .SingleAsync(u => u.UserId == userId);

                var charCount = user.Characters.Count;
                var chars = new CharacterListResponse.Character[charCount];

                user.Characters.Sort((u2, u1) =>
                    DateTimeOffset.Compare(DateTimeOffset.FromUnixTimeSeconds(u1.LastActivity),
                        DateTimeOffset.FromUnixTimeSeconds(u2.LastActivity)));

                for (var i = 0; i < charCount; i++)
                {
                    var chr = user.Characters[i];

                    var items = chr.Items.Where(itm => itm.IsEquipped).Select(itm => (uint) itm.LOT).ToArray();
                    chars[i] = new CharacterListResponse.Character
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
                        ItemCount = (ushort) items.Length,
                        Items = items
                    };
                }

                Server.Send(new CharacterListResponse
                {
                    CharacterCount = (byte) charCount,
                    CharacterIndex = (byte) user.CharacterIndex,
                    Characters = chars
                }, endPoint);

                Logger.Debug($"Sent character list to {endPoint}");
            }
        }

        [PacketHandler]
        public async Task CharacterList(CharacterListRequest packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);

            await SendCharacterList(endPoint, session.UserId);
        }

        [PacketHandler]
        public async Task CharacterCreate(CharacterCreateRequest packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);

            uint shirtLot;
            uint pantsLot;

            using (var ctx = new CdClientContext())
            {
                //
                //    Shirt
                //
                var shirtColor = await ctx.BrickColorsTable.FirstOrDefaultAsync(c => c.Id == packet.ShirtColor);
                var shirtName = $"{(shirtColor != null ? shirtColor.Description : "Bright Red")} Shirt {packet.ShirtStyle}";
                var shirt = await ctx.ObjectsTable.FirstOrDefaultAsync(o =>
                    string.Equals(o.Name, shirtName, StringComparison.CurrentCultureIgnoreCase));
                shirtLot = (uint) (shirt != null ? shirt.Id : 4049); // Select 'Bright Red Shirt 1' if not found.

                //
                //    Pants
                //
                var pantsColor = await ctx.BrickColorsTable.FirstOrDefaultAsync(c => c.Id == packet.PantsColor);
                var pantsName = $"{(pantsColor != null ? pantsColor.Description : "Bright Red")} Pants";
                var pants = await ctx.ObjectsTable.FirstOrDefaultAsync(o =>
                    string.Equals(o.Name, pantsName, StringComparison.CurrentCultureIgnoreCase));
                pantsLot = (uint) (pants != null ? pants.Id : 2508); // Select 'Bright Red Pants' if not found.
            }

            var first = (await Server.Resources.ReadTextAsync("Names/first.txt")).Split('\n');
            var middle = (await Server.Resources.ReadTextAsync("Names/middle.txt")).Split('\n');
            var last = (await Server.Resources.ReadTextAsync("Names/last.txt")).Split('\n');

            var name =
                (first[packet.Predefined.First] + middle[packet.Predefined.Middle] + last[packet.Predefined.Last])
                .Replace("\r", "");

            using (var ctx = new UchuContext())
            {
                if (ctx.Characters.Any(c => c.Name == packet.CharacterName))
                {
                    Logger.Debug($"{endPoint} character create rejected due to duplicate name");
                    Server.Send(new CharacterCreateResponse {ResponseId = CharacterCreationResponse.CustomNameInUse},
                        endPoint);
                    return;
                }
                
                if (ctx.Characters.Any(c => c.Name == name))
                {
                    Logger.Debug($"{endPoint} character create rejected due to duplicate pre-made name");
                    Server.Send(new CharacterCreateResponse {ResponseId = CharacterCreationResponse.PredefinedNameInUse},
                        endPoint);
                    return;
                }
                
                var user = await ctx.Users.Include(u => u.Characters).SingleAsync(u => u.UserId == session.UserId);

                user.Characters.Add(new Character
                {
                    CharacterId = Utils.GenerateObjectId(),
                    Name = name,
                    CustomName = packet.CharacterName,
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
                    LastActivity = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    Items = new List<InventoryItem>
                    {
                        new InventoryItem
                        {
                            InventoryItemId = Utils.GenerateObjectId(),
                            LOT = (int) shirtLot,
                            Slot = 0,
                            Count = 1,
                            InventoryType = (int) InventoryType.Items,
                            IsEquipped = true
                        },
                        new InventoryItem
                        {
                            InventoryItemId = Utils.GenerateObjectId(),
                            LOT = (int) pantsLot,
                            Slot = 1,
                            Count = 1,
                            InventoryType = (int) InventoryType.Items,
                            IsEquipped = true
                        }
                    },
                    CurrentImagination = 0,
                    MaximumImagination = 0
                });

                Logger.Debug($"{user.Username} created character {packet.CharacterName} \"{name}\"");
                
                await ctx.SaveChangesAsync();

                Server.Send(new CharacterCreateResponse {ResponseId = CharacterCreationResponse.Success}, endPoint);

                await SendCharacterList(endPoint, session.UserId);
            }
        }

        [PacketHandler]
        public async Task DeleteCharacter(CharacterDeleteRequest packet, IPEndPoint endPoint)
        {
            using (var ctx = new UchuContext())
            {
                try
                {
                    ctx.Characters.Remove(await ctx.Characters.FindAsync(packet.CharacterId));

                    await ctx.SaveChangesAsync();
                
                    Server.Send(new CharacterDeleteResponse(), endPoint);
                }
                catch (Exception e)
                {
                    Logger.Error($"Character deletion failed for {endPoint}'s character {packet.CharacterId}\n{e}");
                    
                    Server.Send(new CharacterDeleteResponse {Success = false}, endPoint);
                }
            }
        }

        [PacketHandler]
        public async Task RenameCharacter(CharacterRenameRequest packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);
            
            using (var ctx = new UchuContext())
            {
                if (ctx.Characters.Any(c => c.Name == packet.Name || c.CustomName == packet.Name))
                {
                    Server.Send(new CharacterRenameResponse
                        {ResponseId = CharacterRenamingResponse.NameAlreadyInUse}, endPoint);
                    
                    return;
                }
                
                var chr = await ctx.Characters.FindAsync(packet.CharacterId);

                chr.CustomName = packet.Name;
                chr.NameRejected = false;
                chr.LastActivity = DateTimeOffset.Now.ToUnixTimeSeconds();

                await ctx.SaveChangesAsync();

                Server.Send(new CharacterRenameResponse
                    {ResponseId = CharacterRenamingResponse.Success}, endPoint);

                await SendCharacterList(endPoint, session.UserId);
            }
        }

        [PacketHandler]
        public void JoinWorld(JoinWorldRequest packet, IPEndPoint endPoint)
        {
            Server.SessionCache.SetCharacter(endPoint, packet.CharacterId);

            var addresses = NetworkInterface.GetAllNetworkInterfaces()
                .Where(i => (i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                             i.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
                            i.OperationalStatus == OperationalStatus.Up)
                .SelectMany(i => i.GetIPProperties().UnicastAddresses).Select(a => a.Address)
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray();

            var address = endPoint.Address.ToString() == "127.0.0.1" ? "localhost" : addresses[0].ToString();

            Server.Send(new ServerRedirectionPacket
            {
                Address = address,
                Port = 2003
            }, endPoint);
        }
    }
}