using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.World.Social;

namespace Uchu.World.Handlers.Commands
{
    public class GuildCommandHandler : HandlerGroup
    {
        [CommandHandler(Signature = "guild", Help = "Guild commands", GameMasterLevel = 0)]
        public async Task<string> Guild(string[] arguments, Player player)
        {
            if (player.GuildGuiState == GuildGuiState.Suppress)
            {
                player.GuildGuiState = GuildGuiState.Creating;
                
                return "Executed guild command.";
            }
            
            if (arguments.Length == default)
            {
                await SendGuildInformationAsync(player);
                
                return "Executed guild command.";
            }
            
            var args = arguments.ToList();

            var action = args[0];

            args.RemoveAt(0);
            
            await using var ctx = new UchuContext();

            var character = await ctx.Characters.FirstAsync(c => c.Id == player.Id);

            Guild guild;
            
            switch (action.ToLower())
            {
                case "new":
                    if (character.GuildId != 0)
                    {
                        player.CentralNoticeGui("You are already a member of a guild!");
                        
                        break;
                    }
                    
                    await player.GuildCreateGuiAsync(true);

                    await player.SingleArgumentGuiAsync(
                        "SetGuildCreateHeader",
                        "text",
                        "Create Guild"
                    );
                    
                    await player.SingleArgumentGuiAsync(
                        "SetGuildCreateDescription",
                        "text",
                        "Guilds are an amazing tool for creating a community!"
                    );

                    await player.SingleArgumentGuiAsync(
                        "SetGuildCreateName",
                        "text",
                        "Select a name and click the button to create your guild."
                    );

                    await player.MessageGuiAsync("EnableGuildCreateInput");
                    
                    break;
                case "create":
                    if (player.GuildInviteName != default) return "Suppressed";
                    
                    if (character.GuildId != 0)
                    {
                        player.CentralNoticeGui("You are already a member of a guild!");
                        
                        break;
                    }
                    
                    var name = string.Join(" ", args);

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        player.CentralNoticeGui("Your guild needs a name!");
                        
                        break;
                    }

                    if (await ctx.Guilds.AnyAsync(g => g.Name == name))
                    {
                        player.CentralNoticeGui("This guild name is taken!");
                        
                        break;
                    }
                    
                    player.CentralNoticeGui($"You created your guild \"{name}\"!");

                    var id = ObjectId.Standalone;
                    
                    guild = new Guild
                    {
                        Id = id,
                        CreatorId = character.Id,
                        Name = name
                    };

                    character.GuildId = id;

                    await ctx.Guilds.AddAsync(guild);

                    await ctx.SaveChangesAsync();

                    await SendGuildInformationAsync(player);
                    
                    break;
                case "leave":
                    if (character.GuildId != 0)
                    {
                        player.CentralNoticeGui("You are not a member of a guild!");
                        
                        break;
                    }

                    guild = await ctx.Guilds.FirstAsync(g => g.Id == character.GuildId);

                    if (guild.CreatorId == character.Id)
                    {
                        await DismantleGuildAsync(player);
                    }

                    character.GuildId = 0;

                    await ctx.SaveChangesAsync();

                    player.CentralNoticeGui("You left your guild!");
                    break;
                case "invite":
                    if (character.GuildId == 0)
                    {
                        player.CentralNoticeGui("You are not a member of a guild!");
                        
                        break;
                    }
                    
                    var playerName = string.Join(" ", args);

                    var invited = await ctx.Characters.FirstOrDefaultAsync(c => c.Name == playerName);

                    if (invited == null)
                    {
                        player.CentralNoticeGui($"No player named \"{playerName}\" found, please try again.");
                        
                        break;
                    }

                    if (invited.GuildId != 0)
                    {
                        player.CentralNoticeGui($"{playerName} is already in a guild!");
                        
                        break;
                    }
                    
                    guild = await ctx.Guilds.Include(g => g.Invites).FirstAsync(
                        g => g.Id == character.GuildId
                    );

                    if (guild.Invites.Any(i => i.RecipientId == invited.Id))
                    {
                        player.CentralNoticeGui($"There is already a pending invite to {playerName}!");
                        
                        break;
                    }
                    
                    var invite = new GuildInvite
                    {
                        GuildId = guild.Id,
                        RecipientId = invited.Id,
                        SenderId = character.Id
                    };

                    await ctx.GuildInvites.AddAsync(invite);

                    await ctx.SaveChangesAsync();

                    var invitedPlayer = player.Zone.Players.FirstOrDefault(p => p.Id == invited.Id);

                    if (invitedPlayer != default)
                    {
                        await DisplayGuildInviteAsync(invitedPlayer);
                    }
                    
                    player.CentralNoticeGui($"Send a guild invite to {playerName}!");
                    
                    break;
                case "invites":
                    await DisplayGuildInviteAsync(player);
                    
                    break;
                case "accept":
                    if (player.GuildInviteName == default) return "Suppressed";
                    
                    player.GuildGuiState = GuildGuiState.Suppress;
                    
                    guild = await ctx.Guilds.Include(g => g.Invites).FirstOrDefaultAsync(
                        g => g.Name == player.GuildInviteName
                    );

                    if (guild == default)
                    {
                        player.CentralNoticeGui($"No guild named \"{player.GuildInviteName}\" found, please try again.");
                        
                        break;
                    }

                    var guildInvite = guild.Invites.FirstOrDefault(i => i.RecipientId == player.Id);

                    if (guildInvite == default)
                    {
                        player.CentralNoticeGui($"You have no pending invite to {guild}!");
                        
                        break;
                    }

                    guild.Invites.Remove(guildInvite);

                    character.GuildId = guild.Id;

                    player.GuildInviteName = default;
                    
                    await ctx.SaveChangesAsync();
                    
                    player.CentralNoticeGui($"You accepted the invite to {guild.Name}!");
                    
                    break;
                case "decline":
                    if (player.GuildInviteName == default) return "Suppressed";
                    
                    player.GuildGuiState = GuildGuiState.Suppress;
                    
                    guild = await ctx.Guilds.Include(g => g.Invites).FirstOrDefaultAsync(
                        g => g.Name == player.GuildInviteName
                    );

                    if (guild == default)
                    {
                        player.CentralNoticeGui($"No guild named \"{player.GuildInviteName}\" found, please try again.");
                        
                        break;
                    }

                    var declineInvite = guild.Invites.FirstOrDefault(i => i.RecipientId == player.Id);

                    if (declineInvite == default)
                    {
                        player.CentralNoticeGui($"You have no pending invite to {guild}!");
                        
                        break;
                    }

                    guild.Invites.Remove(declineInvite);
                    
                    await ctx.SaveChangesAsync();
                    
                    player.GuildInviteName = default;
                    
                    player.CentralNoticeGui($"You declined the invite to {guild.Name}!");
                    
                    break;
            }

            return "Executed guild command.";
        }

        public static async Task SendGuildInformationAsync(Player player)
        {
            await using var ctx = new UchuContext();

            var character = await ctx.Characters.FirstAsync(c => c.Id == player.Id);

            var guild = await ctx.Guilds.FirstOrDefaultAsync(g => g.Id == character.GuildId);

            if (guild == default)
            {
                player.CentralNoticeGui("You are not a member of a guild!");
                
                return;
            }
            
            await UiHelper.SetGuildNameAsync(player, guild.Name);

            await player.MessageGuiAsync("ClearGuildMembers");

            var members = await ctx.Characters.Where(c => c.GuildId == guild.Id).ToArrayAsync();

            var index = 0;
            
            foreach (var member in members)
            {
                var memberPlayer = player.Zone.Players.FirstOrDefault(p => p.Id ==  member.Id);

                await UiHelper.AddGuildMemberAsync(player, index++, new GuildMember
                {
                    Name = member.Name,
                    Online = memberPlayer != default,
                    Rank = guild.CreatorId == member.Id ? "Owner" : "Member",
                    Zone = ((ZoneId) member.LastZone).ToString()
                });
            }
            
            await player.GuildMenuGuiAsync(true);
        }

        public static async Task DismantleGuildAsync(Player player)
        {
            await using var ctx = new UchuContext();

            var character = await ctx.Characters.FirstAsync(c => c.Id == player.Id);

            var guild = await ctx.Guilds.FirstAsync(g => g.Id == character.GuildId);

            var members = await ctx.Characters.Where(c => c.GuildId == guild.Id).ToArrayAsync();

            foreach (var member in members)
            {
                member.GuildId = 0;
                
                var memberPlayer = player.Zone.Players.FirstOrDefault(p => p.Id ==  member.Id);

                memberPlayer?.CentralNoticeGui($"{guild.Name} was dismantled, you are no longer a member of a guild.");
            }

            ctx.Guilds.Remove(guild);

            await ctx.SaveChangesAsync();
        }

        public static async Task DisplayGuildInviteAsync(Player player, GuildInvite invite = default)
        {
            await using var ctx = new UchuContext();

            if (invite == default)
            {
                invite = await ctx.GuildInvites.FirstOrDefaultAsync(
                    g => g.RecipientId == player.Id
                );
            }
            else
            {
                invite = await ctx.GuildInvites.FirstOrDefaultAsync(
                    g => g.Id == invite.Id
                );
            }
            
            if (invite == default) return;

            var guild = await ctx.Guilds.FirstOrDefaultAsync(g => g.Id == invite.GuildId);

            if (guild == default)
            {
                ctx.GuildInvites.Remove(invite);

                await ctx.SaveChangesAsync();
                
                return;
            }

            await player.SingleArgumentGuiAsync(
                "SetGuildCreateHeader",
                "text",
                "Guild Invite"
            );
                    
            await player.SingleArgumentGuiAsync(
                "SetGuildCreateDescription",
                "text",
                $"You are invited to join {guild.Name}!"
            );

            await player.SingleArgumentGuiAsync(
                "SetGuildCreateName",
                "text",
                "Click the button to accept the invite"
            );

            await player.MessageGuiAsync("DisableGuildCreateInput");

            await player.GuildCreateGuiAsync(true);

            player.GuildGuiState = GuildGuiState.Invite;

            player.GuildInviteName = guild.Name;
            
            await ctx.SaveChangesAsync();
        }
    }
}