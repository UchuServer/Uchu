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
            }

            return user;
        }

        public static async Task<User> GetUserAsync(string username)
        {
            using (var ctx = new UchuContext())
            {
                return await ctx.Users.SingleOrDefaultAsync(user => user.Username == username);
            }
        }

        public static async Task<User> GetUserAsync(long id)
        {
            using (var ctx = new UchuContext())
            {
                return await ctx.Users.SingleOrDefaultAsync(user => user.UserId == id);
            }
        }
    }
}