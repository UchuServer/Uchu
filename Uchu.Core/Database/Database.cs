using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Uchu.Core
{
    public static class Database
    {
        public static async Task<User> CreateUserAsync(string username, string password)
        {
            var hashed = BCrypt.Net.BCrypt.EnhancedHashPassword(username);
            var user = new User
            {
                Username = username,
                Password = hashed,
                CharacterIndex = 0
            };

            using (var ctx = new UchuContext())
            {
                await ctx.Users.AddAsync(user);

                await ctx.SaveChangesAsync();
            }

            return user;
        }

        public static async Task<User> GetUserAsync(string username)
        {
            using (var ctx = new UchuContext())
            {
                return await ctx.Users.Include(u => u.Characters).SingleOrDefaultAsync(user => user.Username == username);
            }
        }

        public static async Task<User> GetUserAsync(long id)
        {
            using (var ctx = new UchuContext())
            {
                return await ctx.Users.Include(u => u.Characters).SingleOrDefaultAsync(user => user.UserId == id);
            }
        }

        public static async Task CreateCharacterAsync(Character character, long userId)
        {
            character.CharacterId = Utils.GenerateObjectId();

            using (var ctx = new UchuContext())
            {
                var user = await ctx.Users.Include(u => u.Characters).SingleAsync(u => u.UserId == userId);

                user.Characters.Add(character);

                await ctx.SaveChangesAsync();
            }
        }

        public static async Task<Character> GetCharacterAsync(long id)
        {
            using (var ctx = new UchuContext())
            {
                return await ctx.Characters.Include(c => c.Items).Include(c => c.User).SingleOrDefaultAsync(ch => ch.CharacterId == id);
            }
        }

        public static async Task<Character> GetCharacterAsync(string name)
        {
            using (var ctx = new UchuContext())
            {
                return await ctx.Characters.Include(c => c.Items).Include(c => c.User).SingleOrDefaultAsync(ch => ch.Name == name);
            }
        }

        public static async Task RenameCharacterAsync(long id, string name)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(id);

                chr.CustomName = name;

                await ctx.SaveChangesAsync();
            }
        }

        public static async Task DeleteCharacterAsync(long id)
        {
            using (var ctx = new UchuContext())
            {
                var chr = await ctx.Characters.FindAsync(id);

                ctx.Characters.Remove(chr);

                await ctx.SaveChangesAsync();
            }
        }
    }
}