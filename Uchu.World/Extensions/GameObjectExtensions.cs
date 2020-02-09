using System.Threading.Tasks;

namespace Uchu.World
{
    public static class GameObjectExtensions
    {
        /// <summary>
        ///     Accounts for network delay, e.g, player ping.
        /// </summary>
        /// <param name="this">Networked GameObject</param>
        /// <returns>Task to be awaited to account for ping</returns>
        public static Task NetFavorAsync(this GameObject @this)
        {
            if (@this is Player player)
            {
                return Task.Delay(player.Ping);
            }
            
            return Task.CompletedTask;
        }
    }
}