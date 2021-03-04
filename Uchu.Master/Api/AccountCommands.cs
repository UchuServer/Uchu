using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Core;

namespace Uchu.Master.Api
{
    public class AccountCommands
    {
        [ApiCommand("account/new")]
        public async Task<object> CreateAccount(string accountName, string accountPassword)
        {
            var response = new AccountCreationResponse();

            if (string.IsNullOrWhiteSpace(accountName))
            {
                response.FailedReason = "username null";

                return response;
            }

            if (string.IsNullOrWhiteSpace(accountPassword))
            {
                response.FailedReason = "password null";

                return response;
            }

            await using (var ctx = new UchuContext())
            {
                var duplicate = await ctx.Users.Where(u => !u.Sso).AnyAsync(u => string.Equals(u.Username.ToUpper(), accountName.ToUpper()));

                if (duplicate)
                {
                    response.FailedReason = "duplicate username";

                    return response;
                }

                var password = BCrypt.Net.BCrypt.EnhancedHashPassword(accountPassword);

                await ctx.Users.AddAsync(new User
                {
                    Username = accountName,
                    Password = password
                });

                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new UchuContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(u => string.Equals(u.Username.ToUpper(), accountName.ToUpper()));

                if (user == default) return response;

                response.Success = true;
                response.Id = user.Id;
                response.Username = user.Username;
                response.Hash = user.Password;
            }

            return response;
        }

        [ApiCommand("account/delete")]
        public async Task<object> DeleteAccount(string accountName)
        {
            var response = new AccountDeleteResponse();

            if (string.IsNullOrWhiteSpace(accountName))
            {
                response.FailedReason = "username null";

                return response;
            }

            await using (var ctx = new UchuContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(u => string.Equals(u.Username.ToUpper(), accountName.ToUpper()));

                if (user == default)
                {
                    response.FailedReason = "not found";

                    return response;
                }

                ctx.Users.Remove(user);

                await ctx.SaveChangesAsync();

                response.Success = true;
                response.Username = user.Username;
            }

            return response;
        }

        [ApiCommand("account/level")]
        public async Task<object> AdminAccount(string accountName, string level)
        {
            var response = new AccountAdminResponse();

            if (string.IsNullOrWhiteSpace(accountName))
            {
                response.FailedReason = "username null";

                return response;
            }

            if (string.IsNullOrWhiteSpace(level))
            {
                response.FailedReason = "level null";

                return response;
            }

            await using (var ctx = new UchuContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(u => string.Equals(u.Username.ToUpper(), accountName.ToUpper()));

                if (user == default)
                {
                    response.FailedReason = "not found";

                    return response;
                }

                if (!Enum.TryParse<GameMasterLevel>(level, out var gameMasterLevel) ||
                    !Enum.IsDefined(typeof(GameMasterLevel), gameMasterLevel))
                {
                    response.FailedReason = "invalid level";

                    return response;
                }

                user.GameMasterLevel = (int) gameMasterLevel;

                await ctx.SaveChangesAsync();

                response.Success = true;
                response.Username = user.Username;
                response.Level = (int) gameMasterLevel;
            }

            return response;
        }

        [ApiCommand("account/ban")]
        public async Task<object> BanAccount(string accountName, string reason)
        {
            var response = new AccountBanResponse();

            if (string.IsNullOrWhiteSpace(accountName))
            {
                response.FailedReason = "username null";

                return response;
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                response.FailedReason = "reason null";

                return response;
            }

            await using (var ctx = new UchuContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(u => string.Equals(u.Username.ToUpper(), accountName.ToUpper()));

                if (user == default)
                {
                    response.FailedReason = "not found";

                    return response;
                }

                user.Banned = true;

                user.BannedReason = reason;

                await ctx.SaveChangesAsync();

                response.Success = true;
                response.Username = user.Username;
                response.BannedReason = user.BannedReason;
            }

            return response;
        }

        [ApiCommand("account/pardon")]
        public async Task<object> PardonAccount(string accountName)
        {
            var response = new AccountPardonResponse();

            if (string.IsNullOrWhiteSpace(accountName))
            {
                response.FailedReason = "username null";

                return response;
            }

            await using (var ctx = new UchuContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(u => string.Equals(u.Username.ToUpper(), accountName.ToUpper()));

                if (user == default)
                {
                    response.FailedReason = "not found";

                    return response;
                }

                user.Banned = false;
                user.BannedReason = null;

                await ctx.SaveChangesAsync();

                response.Success = true;
                response.Username = user.Username;
            }

            return response;
        }

        [ApiCommand("account/info")]
        public async Task<object> AccountInfo(string accountName)
        {
            var response = new AccountInfoResponse();

            if (string.IsNullOrWhiteSpace(accountName))
            {
                response.FailedReason = "username null";

                return response;
            }

            await using (var ctx = new UchuContext())
            {
                var user = await ctx.Users.FirstOrDefaultAsync(u => string.Equals(u.Username.ToUpper(), accountName.ToUpper()));

                if (user == default)
                {
                    response.FailedReason = "not found";

                    return response;
                }

                response.Success = true;
                response.Username = user.Username;
                response.Hash = user.Password;
                response.Id = user.Id;
                response.Banned = user.Banned;
                response.BannedReason = user.BannedReason;
                response.Level = user.GameMasterLevel;
            }

            return response;
        }

        [ApiCommand("account/list")]
        public async Task<object> Accounts()
        {
            var response = new AccountListResponse();

            await using var ctx = new UchuContext();

            response.Success = true;

            response.Accounts = await ctx.Users.Select(u => u.Id).ToListAsync();

            return response;
        }
    }
}
