using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Api.Models;
using Uchu.Core.Resources;

namespace Uchu.Core.Handlers.Commands
{
    public class ServerStatusCommandHandler : HandlerGroup
    {
        [CommandHandler(Signature = "shutdown", Help = "Shuts down the server")]
        public void ShutdownServer(string[] arguments)
        {
            int delayMinutes = 0;
            if (arguments.Length != 0 && int.TryParse(arguments[0], out var arg))
            {
                delayMinutes = arg;
                Logger.Warning($"Shutting down in {delayMinutes} minute(s).");
            }

            Task.Run(async () =>
            {
                string[] args = {"Server shutting down", $"The server will be shutting down in {delayMinutes} minute(s)."};
                await Broadcast(args).ConfigureAwait(false);
                await Task.Delay(delayMinutes * 60 * 1000).ConfigureAwait(false);
                await UchuServer.Api.RunCommandAsync<BaseResponse>(
                    UchuServer.MasterApi, $"master/decommission?message=Shut down server."
                ).ConfigureAwait(false);
            });
        }

        [CommandHandler(Signature = "adduser", Help = "Add a user")]
        public static string AddUser(string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments),
                    ResourceStrings.StandardCommandHandler_AddUser_ArgumentsNullException);

            if (arguments.Length != 1)
            {
                return "adduser <name>";
            }

            var name = arguments[0];

            if (name.Length > 33)
            {
                return "Usernames with more than 33 characters is not supported";
            }

            using var ctx = new UchuContext();
            if (ctx.Users.Where(u => !u.Sso).Any(u => string.Equals(u.Username.ToUpper(), name.ToUpper())))
            {
                return "A user with that username already exists";
            }

            Console.Write(ResourceStrings.StandardCommandHandler_AddUser_PasswordPrompt);
            var password = GetPassword();

            if (password.Length > 42)
            {
                return "Passwords with more than 42 characters is not supported";
            }

            ctx.Users.Add(new User
            {
                Username = name,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password),
                CharacterIndex = 0
            });

            ctx.SaveChanges();

            return $"\nSuccessfully added user: {name}!";
        }

        [CommandHandler(Signature = "removeuser", Help = "Remove a user")]
        public static string RemoveUser(string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments),
                    ResourceStrings.StandardCommandHandler_RemoveUser_ArgumentsNullException);

            if (arguments.Length != 1)
            {
                return "removeuser <name>";
            }

            var name = arguments[0];

            using var ctx = new UchuContext();
            var user = ctx.Users.FirstOrDefault(u => String.Equals(u.Username.ToUpper(), name.ToUpper()));

            if (user == null)
            {
                return $"No user with the username of: {name}";
            }

            Console.Write(ResourceStrings.StandardCommandHandler_RemoveUser_ConfirmationMessage);
            if (Console.ReadLine() != name) return "Deletion aborted";

            ctx.Users.Remove(user);
            ctx.SaveChanges();

            return $"Successfully deleted user: {name}";
        }

        [CommandHandler(Signature = "ban", Help = "Ban a user", GameMasterLevel = GameMasterLevel.Mythran)]
        [SuppressMessage("ReSharper", "CA2000")]
        public static async Task<string> BanUser(string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments),
                    ResourceStrings.StandardCommandHandler_BanUser_ArgumentsNullException);

            if (arguments.Length != 2)
            {
                return $"{arguments[0]} <name> <reason>";
            }

            var name = arguments[0];
            var reason = arguments[1];

            await using var ctx = new UchuContext();
            var user = await ctx.Users.FirstOrDefaultAsync(u => String.Equals(u.Username.ToUpper(), name.ToUpper()))
                .ConfigureAwait(false);

            if (user == null)
            {
                return $"No user with the username of: {name}";
            }

            user.Banned = true;
            user.BannedReason = reason;

            await ctx.SaveChangesAsync().ConfigureAwait(false);

            return $"Successfully banned {name}!";
        }

        [CommandHandler(Signature = "pardon", Help = "Pardon a user", GameMasterLevel = GameMasterLevel.Mythran)]
        [SuppressMessage("ReSharper", "CA2000")]
        public static async Task<string> PardonUser(string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments),
                    ResourceStrings.StandardCommandHandler_PardonUser_ArgumentsNullException);

            if (arguments.Length != 1)
            {
                return $"{arguments[0]} <name>";
            }

            var name = arguments[0];

            await using var ctx = new UchuContext();
            var user = await ctx.Users.FirstOrDefaultAsync(u => string.Equals(u.Username.ToUpper(), name.ToUpper()))
                .ConfigureAwait(false);

            if (user == null)
            {
                return $"No user with the username of: {name}";
            }

            user.Banned = false;
            user.BannedReason = null;

            await ctx.SaveChangesAsync().ConfigureAwait(false);

            return $"Successfully pardoned {name}!";
        }

        [CommandHandler(Signature = "users", Help = "List all users", GameMasterLevel = GameMasterLevel.Admin)]
        public static string GetUsers()
        {
            using var ctx = new UchuContext();
            var users = ctx.Users;
            return !users.Any()
                ? "No registered users"
                : string.Join("\n", users.Select(s => s.Username));
        }

        [CommandHandler(Signature = "approve", Help = "Approve usernames", GameMasterLevel = GameMasterLevel.Mythran)]
        [SuppressMessage("ReSharper", "CA1304")]
        [SuppressMessage("ReSharper", "CA2000")]
        public static async Task<string> ApproveUsernames(string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments),
                    ResourceStrings.StandardCommandHandler_ApproveUsername_ArgumentsNullException);

            await using var ctx = new UchuContext();
            if (arguments.Length == 0 || arguments[0].ToLower() == "*" || string.IsNullOrEmpty(arguments[0]))
            {
                var unApproved = ctx.Characters.Where(c => !c.NameRejected && c.Name != c.CustomName && !string.IsNullOrEmpty(c.CustomName));

                if (arguments.Length != 1 || arguments[0] != "*")
                {
                    return string.Join("\n",
                                unApproved.Select(s => s.CustomName)
                            ) + "\napprove <name> / *";
                }

                foreach (var character in unApproved)
                {
                    character.Name = character.CustomName;
                    character.CustomName = "";
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);

                return "Successfully approved all names!";
            }

            var selectedCharacter = await ctx.Characters.FirstOrDefaultAsync(
                c => c.CustomName == arguments[0] && !c.NameRejected
            ).ConfigureAwait(false);

            if (selectedCharacter == null)
            {
                return $"No unapproved character with name: \"{arguments[0]}\"";
            }

            selectedCharacter.Name = selectedCharacter.CustomName;
            selectedCharacter.CustomName = "";

            await ctx.SaveChangesAsync().ConfigureAwait(false);

            return $"Successfully approved \"{selectedCharacter.Name}\"!";
        }

        [CommandHandler(Signature = "reject", Help = "Reject usernames", GameMasterLevel = GameMasterLevel.Mythran)]
        [SuppressMessage("ReSharper", "CA1304")]
        [SuppressMessage("ReSharper", "CA2000")]
        public static async Task<string> RejectUserNames(string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments),
                    ResourceStrings.StandardCommandHandler_RejectUserNames_ArgumentsNullException);

            await using var ctx = new UchuContext();
            if (arguments.Length == 0 || arguments[0].ToLower() == "all")
            {
                var unApproved = ctx.Characters.Where(c => !c.NameRejected && c.Name != c.CustomName && !string.IsNullOrEmpty(c.CustomName));

                if (arguments.Length != 1 || arguments[0] != "all")
                    return string.Join("\n",
                               unApproved.Select(s => s.CustomName)
                           ) + "\nreject <name> / all";

                foreach (var character in unApproved)
                {
                    character.NameRejected = true;
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
                return "Successfully rejected all names!";
            }

            var selectedCharacter = await ctx.Characters.FirstOrDefaultAsync(
                c => c.CustomName == arguments[1] && !c.NameRejected)
                .ConfigureAwait(false);

            if (selectedCharacter == null)
            {
                return $"No unapproved character with name: \"{arguments[1]}\"";
            }

            selectedCharacter.NameRejected = true;

            await ctx.SaveChangesAsync().ConfigureAwait(false);
            return $"Successfully rejected \"{selectedCharacter.CustomName}\"!";
        }

        [CommandHandler(Signature = "gamemaster", Help = "Set Game Master level for user", GameMasterLevel = GameMasterLevel.Admin)]
        [SuppressMessage("ReSharper", "CA2000")]
        public static async Task<string> SetGameMasterLevel(string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments),
                    ResourceStrings.StandardCommandHandler_SetGameMasterLevel_ArgumentsNullException);

            if (arguments.Length != 2)
            {
                return "gamemaster <username> <level>";
            }

            var username = arguments[0];

            await using var ctx = new UchuContext();
            var user = await ctx.Users.FirstOrDefaultAsync(u => string.Equals(u.Username.ToUpper(), username.ToUpper()))
                .ConfigureAwait(false);

            if (user == default)
            {
                return $"No user with the username of: {username}";
            }

            if (!Enum.TryParse<GameMasterLevel>(arguments[1], out var level) ||
                !Enum.IsDefined(typeof(GameMasterLevel), level))
            {
                return "Invalid <level>";
            }

            user.GameMasterLevel = (int) level;

            await ctx.SaveChangesAsync().ConfigureAwait(false);

            return$"Successfully set {user.Username}'s Game Master " +
                  $"level to {(GameMasterLevel) user.GameMasterLevel}";
        }
        [CommandHandler(Signature = "broadcast", Help = "Send an announcement to all players in all worlds",
            GameMasterLevel = GameMasterLevel.Admin)]
        public async Task<string> Broadcast(string[] arguments)
        {
            if (arguments.Length < 2) return "/broadcast <title> <message>";

            var args = arguments.ToList();

            var title = args[0];

            args.RemoveAt(0);

            var message = string.Join(" ", args);

            var instanceListResponse = await UchuServer.Api.RunCommandAsync<InstanceListResponse>(
                UchuServer.MasterApi, $"instance/list"
            ).ConfigureAwait(false);

            foreach (var instance in instanceListResponse.Instances.Where(i => i.Type == (int) ServerType.World))
            {
                await UchuServer.Api.RunCommandAsync<BaseResponse>(instance.ApiPort,
                    $"world/announce?title={title}&message={message}"
                    ).ConfigureAwait(false);
            }

            return $"Sent announcement with title \"{title}\" and message \"{message}\"";
        }

        private static string GetPassword()
        {
            var pwd = new StringBuilder();
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length <= 0) continue;
                    pwd.Length--;
                    Console.Write(ResourceStrings.ServerStatusCommandHandler_GetPassword_Backspace);
                }
                else if (i.KeyChar != '\u0000')
                {
                    pwd.Append(i.KeyChar);
                    Console.Write(ResourceStrings.ServerStatusCommandHandler_GetPassword_Star);
                }
            }

            return pwd.ToString();
        }
    }
}
