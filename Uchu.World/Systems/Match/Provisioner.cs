using System.Collections.Generic;

namespace Uchu.World.Systems.Match
{
    public class Provisioner
    {
        /// <summary>
        /// Static dictionary of match provisioners.
        /// </summary>
        private static readonly Dictionary<int, Provisioner> _staticProvisioners = new Dictionary<int, Provisioner>();

        /// <summary>
        /// Match type of the provisioner.
        /// </summary>
        private int _type;

        /// <summary>
        /// List of active round instances.
        /// </summary>
        private List<MatchInstance> _matches = new List<MatchInstance>();
        
        /// <summary>
        /// Zones that have been connected for events.
        /// </summary>
        private static readonly List<Zone> _connectedZones = new List<Zone>();

        /// <summary>
        /// Creates a provisioner.
        /// </summary>
        private Provisioner(int type)
        {
            _type = type;
        }
        
        /// <summary>
        /// Returns a static provisioner instance for
        /// the given match type.
        /// </summary>
        /// <returns>Provisioner for the given match type.</returns>
        public static Provisioner GetProvisioner(int type)
        {
            // Create a provisioner if it doesn't exist.
            if (!_staticProvisioners.ContainsKey(type))
            {
                _staticProvisioners[type] = new Provisioner(type);
            }

            return _staticProvisioners[type];
        }

        /// <summary>
        /// Marks a player as ready in their current match.
        /// </summary>
        /// <param name="player">Player to mark as ready.</param>
        public static void PlayerReady(Player player)
        {
            foreach (var provisioner in _staticProvisioners.Values)
            {
                provisioner.SetPlayerReady(player);
            } 
        }

        /// <summary>
        /// Invoked when a player leaves a match.
        /// </summary>
        /// <param name="player">Player that left.</param>
        public static void PlayerLeft(Player player)
        {
            foreach (var provisioner in _staticProvisioners.Values)
            {
                provisioner.RemovePlayer(player);
            }
        }

        /// <summary>
        /// Marks a player as not ready in their current match.
        /// </summary>
        /// <param name="player">Player to mark as ready.</param>
        public static void PlayerNotReady(Player player)
        {
            foreach (var provisioner in _staticProvisioners.Values)
            {
                provisioner.SetPlayerNotReady(player);
            } 
        }

        /// <summary>
        /// Adds a player to a match.
        /// </summary>
        /// <param name="player">Player to add.</param>
        public void AddPlayer(Player player)
        {
            // Get the round to use.
            // TODO: Support teams.
            MatchInstance match = default;
            foreach (var otherMatch in _matches)
            {
                if (!otherMatch.CanAddPlayer(player)) continue;
                match = otherMatch;
                break;
            }
            if (match == default)
            {
                match = new MatchInstance(_type, player.Zone);
                match.TimeEnded.AddListener(() =>
                {
                    _matches.Remove(match);
                });
                _matches.Add(match);
            }
            
            // Add the player.
            match.AddPlayer(player);
            
            // Connect players leaving the zone.
            if (_connectedZones.Contains(player.Zone)) return;
            player.Zone.OnPlayerLeave.AddListener(RemovePlayer);
            _connectedZones.Add(player.Zone);
        }
        
        /// <summary>
        /// Removes a player from their current match.
        /// </summary>
        /// <param name="player">Player to remove.</param>
        public void RemovePlayer(Player player)
        {
            foreach (var match in _matches)
            {
                if (!match.HasPlayer(player)) continue;
                match.RemovePlayer(player);
                return;
            }
        }
        
        /// <summary>
        /// Set a player as ready in their current match.
        /// </summary>
        /// <param name="player">Player to mark as ready.</param>
        public void SetPlayerReady(Player player)
        {
            foreach (var match in _matches)
            {
                if (!match.HasPlayer(player)) continue;
                match.SetPlayerReady(player);
                return;
            } 
        }
        
        /// <summary>
        /// Set a player as not ready in their current match.
        /// </summary>
        /// <param name="player">Player to mark as ready.</param>
        public void SetPlayerNotReady(Player player)
        {
            foreach (var match in _matches)
            {
                if (!match.HasPlayer(player)) continue;
                match.SetPlayerNotReady(player);
                return;
            } 
        }
    }
}