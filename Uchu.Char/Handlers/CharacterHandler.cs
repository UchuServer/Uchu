using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.Char.Handlers
{
    public class CharacterHandler : HandlerGroup
    {
        private static async Task SendCharacterList(IRakConnection connection, long userId)
        {
            await using var ctx = new UchuContext();

            var user = await ctx.Users.Include(u => u.Characters).ThenInclude(c => c.Items)
                .SingleAsync(u => u.Id == userId);

            var charCount = user.Characters.Count;
            var chars = new CharacterListResponse.Character[charCount];

            user.Characters.Sort((u2, u1) =>
                DateTimeOffset.Compare(DateTimeOffset.FromUnixTimeSeconds(u1.LastActivity),
                    DateTimeOffset.FromUnixTimeSeconds(u2.LastActivity)));

            for (var i = 0; i < charCount; i++)
            {
                var chr = user.Characters[i];

                var items = chr.Items.Where(itm => itm.IsEquipped).Select(itm => (uint) itm.Lot).ToArray();
                chars[i] = new CharacterListResponse.Character
                {
                    CharacterId = chr.Id,
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

            connection.Send(new CharacterListResponse
            {
                CharacterCount = (byte) charCount,
                CharacterIndex = (byte) user.CharacterIndex,
                Characters = chars
            });

            Logger.Debug($"Sent character list to {connection}");
        }

        [PacketHandler]
        public async Task CharacterList(CharacterListRequest packet, IRakConnection connection)
        {
            var session = UchuServer.SessionCache.GetSession(connection.EndPoint);
            await SendCharacterList(connection, session.UserId);
        }

        [PacketHandler]
        public async Task CharacterCreate(CharacterCreateRequest packet, IRakConnection connection)
        {
            var session = UchuServer.SessionCache.GetSession(connection.EndPoint);

            uint shirtLot;
            uint pantsLot;

            await using (var ctx = new CdClientContext())
            {
                // Shirt
                var shirtColor = await ctx.BrickColorsTable.FirstOrDefaultAsync(c => c.Id == packet.ShirtColor);
                var shirtName =
                    $"{(shirtColor != null ? shirtColor.Description : "Bright Red")} Shirt {packet.ShirtStyle}";
                var shirt = ctx.ObjectsTable.ToArray().FirstOrDefault(o =>
                    string.Equals(o.Name, shirtName, StringComparison.CurrentCultureIgnoreCase));
                shirtLot = (uint) (shirt != null ? shirt.Id : 4049); // Select 'Bright Red Shirt 1' if not found.
                
                // Pants
                var pantsColor = await ctx.BrickColorsTable.FirstOrDefaultAsync(c => c.Id == packet.PantsColor);
                var pantsName = $"{(pantsColor != null ? pantsColor.Description : "Bright Red")} Pants";
                var pants = ctx.ObjectsTable.ToArray().FirstOrDefault(o =>
                    string.Equals(o.Name, pantsName, StringComparison.CurrentCultureIgnoreCase));
                pantsLot = (uint) (pants != null ? pants.Id : 2508); // Select 'Bright Red Pants' if not found.
            }

            var first = (await UchuServer.Resources.ReadTextAsync("names/minifigname_first.txt")).Split('\n');
            var middle = (await UchuServer.Resources.ReadTextAsync("names/minifigname_middle.txt")).Split('\n');
            var last = (await UchuServer.Resources.ReadTextAsync("names/minifigname_last.txt")).Split('\n');

            var name = (
                first[packet.Predefined.First] +
                middle[packet.Predefined.Middle] +
                last[packet.Predefined.Last]
            ).Replace("\r", "");

            await using (var ctx = new UchuContext())
            {
                if (ctx.Characters.Any(c => c.Name == packet.CharacterName))
                {
                    Logger.Debug($"{connection} character create rejected due to duplicate name");
                    connection.Send(new CharacterCreateResponse
                        {ResponseId = CharacterCreationResponse.CustomNameInUse}
                    );

                    return;
                }

                if (ctx.Characters.Any(c => c.Name == name))
                {
                    Logger.Debug($"{connection} character create rejected due to duplicate pre-made name");
                    connection.Send(new CharacterCreateResponse
                        {ResponseId = CharacterCreationResponse.PredefinedNameInUse}
                    );

                    return;
                }

                var user = await ctx.Users.Include(u => u.Characters).SingleAsync(u => u.Id == session.UserId);

                user.Characters.Add(new Character
                {
                    Id = ObjectId.Standalone,
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
                    LastZone = 0,
                    LastInstance = 0,
                    LastClone = 0,
                    InventorySize = 20,
                    LastActivity = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    Items = new List<InventoryItem>
                    {
                        new InventoryItem
                        {
                            Id = ObjectId.Standalone,
                            Lot = (int) shirtLot,
                            Slot = 0,
                            Count = 1,
                            InventoryType = (int) InventoryType.Items,
                            IsEquipped = true
                        },
                        new InventoryItem
                        {
                            Id = ObjectId.Standalone,
                            Lot = (int) pantsLot,
                            Slot = 1,
                            Count = 1,
                            InventoryType = (int) InventoryType.Items,
                            IsEquipped = true
                        }
                    },
                    CurrentImagination = 0,
                    MaximumImagination = 0
                });

                Logger.Debug(
                    $"{user.Username} created character \"{packet.CharacterName}\" with the pre-made name of \"{name}\""
                );

                await ctx.SaveChangesAsync();

                connection.Send(new CharacterCreateResponse
                    {ResponseId = CharacterCreationResponse.Success}
                );

                await SendCharacterList(connection, session.UserId);
            }
        }

        [PacketHandler]
        public async Task DeleteCharacter(CharacterDeleteRequest packet, IRakConnection connection)
        {
            await using var ctx = new UchuContext();

            try
            {
                ctx.Characters.Remove(await ctx.Characters.FindAsync(packet.CharacterId));
                await ctx.SaveChangesAsync();
                connection.Send(new CharacterDeleteResponse());
            }
            catch (Exception e)
            {
                Logger.Error($"Character deletion failed for {connection}'s character {packet.CharacterId}\n{e}");
                connection.Send(new CharacterDeleteResponse {Success = false});
            }
        }

        [PacketHandler]
        public async Task RenameCharacter(CharacterRenameRequest packet, IRakConnection connection)
        {
            var session = UchuServer.SessionCache.GetSession(connection.EndPoint);

            await using var ctx = new UchuContext();
            
            // Check if the name already exists and return proper response if so
            if (ctx.Characters.Any(c => c.Name == packet.Name || c.CustomName == packet.Name))
            {
                connection.Send(new CharacterRenameResponse
                    {ResponseId = CharacterRenamingResponse.NameAlreadyInUse}
                );

                return;
            }

            // If the name is free, update accordingly and notify the client
            var chr = await ctx.Characters.FindAsync(packet.CharacterId);

            chr.CustomName = packet.Name;
            chr.NameRejected = false;
            chr.LastActivity = DateTimeOffset.Now.ToUnixTimeSeconds();

            await ctx.SaveChangesAsync();

            connection.Send(new CharacterRenameResponse
                {ResponseId = CharacterRenamingResponse.Success}
            );

            await SendCharacterList(connection, session.UserId);
        }

        [PacketHandler]
        public async Task JoinWorld(JoinWorldRequest packet, IRakConnection connection)
        {
            UchuServer.SessionCache.SetCharacter(connection.EndPoint, packet.CharacterId);

            await using var ctx = new UchuContext();
            var character = await ctx.Characters.FirstAsync(c => c.Id == packet.CharacterId);
            character.LastActivity = DateTimeOffset.Now.ToUnixTimeSeconds();
            await ctx.SaveChangesAsync();

            var zone = (ZoneId) character.LastZone;
            var requestZone = (ZoneId) (zone == 0 ? 1000 : zone);
            
            // We don't want to lock up the server on a world server request, as it may take time.
            var _ = Task.Run(async () =>
            {
                // Request world server.
                var server = await ServerHelper.RequestWorldServerAsync(UchuServer, requestZone);
                if (server == default)
                {
                    // If there's no server available, error
                    var session = UchuServer.SessionCache.GetSession(connection.EndPoint);
                    await SendCharacterList(connection, session.UserId);

                    return;
                }
                
                // Send to world server.
                connection.Send(new ServerRedirectionPacket
                {
                    Address = UchuServer.Host,
                    Port = (ushort) server.Port
                });
            });
        }
    }
}