namespace Uchu.World
{
    public class TeamPlayerComponent : Component
    {
        public void MessageSetLeader(Player player)
        {
            As<Player>().Message(new TeamSetLeaderMessage
            {
                Associate = GameObject,
                NewLeader = player
            });
        }

        public void MessageAddPlayer(Player player)
        {
            As<Player>().Message(new TeamAddPlayerMessage
            {
                Associate = GameObject,
                Player = player
            });
        }
    }
}