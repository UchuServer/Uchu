namespace Uchu.World
{
    [Essential]
    public class TeamPlayer : Component
    {
        public void MessageSetLeader(Player player)
        {
            Player.Message(new TeamSetLeaderMessage
            {
                Associate = Player,
                NewLeader = player
            });
        }

        public void MessageAddPlayer(Player player)
        {
            Player.Message(new TeamAddPlayerMessage
            {
                Associate = Player,
                Player = player
            });
        }
    }
}