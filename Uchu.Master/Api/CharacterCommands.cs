using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Core;

namespace Uchu.Master.Api
{
    public class CharacterCommands
    {
        [ApiCommand("character/list")]
        public async Task<object> CharacterList(string id)
        {
            var response = new CharacterListResponse();

            if (!long.TryParse(id, out var userId))
            {
                response.FailedReason = "invalid id";

                return response;
            }

            await using var ctx = new UchuContext();

            var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == default)
            {
                response.FailedReason = "not found";

                return response;
            }

            var characters = await ctx.Characters.Where(c => c.UserId == user.Id).ToArrayAsync();

            response.Success = true;

            response.UserId = userId;

            response.Characters = characters.Select(c => c.Id.ToString()).ToList();

            return response;
        }

        [ApiCommand("character/details")]
        public async Task<object> CharacterDetails(string id)
        {
            var response = new CharacterDetailsResponse();

            if (!long.TryParse(id, out var characterId))
            {
                response.FailedReason = "invalid id";

                return response;
            }

            await using var ctx = new UchuContext();

            var character = await ctx.Characters
                .Include(c => c.Items)
                .Include(c => c.Missions)
                .ThenInclude(m => m.Tasks).ThenInclude(t => t.Values)
                .FirstOrDefaultAsync(c => c.Id == characterId);
            
            Console.WriteLine($"Details request: {characterId} -> {character}");

            if (character == default)
            {
                response.FailedReason = "not found";

                return response;
            }
            
            response.Success = true;
            
            response.UserId = character.UserId;
            
            response.CharacterId = response.CharacterId;

            response.Details = character;

            return response;
        }
    }
}