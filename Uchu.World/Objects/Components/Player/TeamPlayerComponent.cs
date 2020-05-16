namespace Uchu.World
{
    public class TeamPlayerComponent : Component
    {
        public void MessageSetLeader(Player player)
        {
            if (!(GameObject is Player @this)) return;
            
            @this.Message(new TeamSetLeaderMessage
            {
                Associate = GameObject,
                NewLeader = player
            });
        }

        public void MessageAddPlayer(Player player)
        {
            if (!(GameObject is Player @this)) return;

            @this.Message(new TeamAddPlayerMessage
            {
                Associate = GameObject,
                Player = player
            });
        }
    }
}