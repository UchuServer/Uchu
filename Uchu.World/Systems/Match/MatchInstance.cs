using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace Uchu.World.Systems.Match
{
    public class MatchInstance
    {
        /// <summary>
        /// Players that are required to start the match.
        /// </summary>
        private int _requiredPlayers;
        
        /// <summary>
        /// Max players in the match.
        /// </summary>
        private int _maxPlayers;
        
        /// <summary>
        /// Players currently in the match.
        /// </summary>
        private List<Player> _players = new List<Player>();
        
        /// <summary>
        /// Players that were sent the time.
        /// Sending the time multiple times causes
        /// problems on the client.
        /// </summary>
        private List<Player> _playersSentTime = new List<Player>();

        /// <summary>
        /// Timer for starting the round.
        /// </summary>
        private Timer _countdown = new Timer()
        {
            Interval = 60000,
        };

        /// <summary>
        /// Stopwatch for the elapsed time.
        /// </summary>
        private Stopwatch _countdownStopwatch = new Stopwatch();
        
        /// <summary>
        /// Creates an instance of a match.
        /// </summary>
        /// <param name="type">Activity id of the match.</param>
        public MatchInstance(int type)
        {
            _requiredPlayers = 1; // TODO: Figure out from type and database.
            _maxPlayers = 4; // TODO: Figure out from type and database.
        }

        /// <summary>
        /// Updates the state of the timer.
        /// </summary>
        private void UpdateTimer()
        {
            // Update the countdown.
            if (_players.Count >= _requiredPlayers)
            {
                if (!_countdown.Enabled)
                {
                    _countdown.Start();
                    _countdownStopwatch.Start();
                }
            }
            else
            {
                _countdown.Stop();
            }
            
            // Send the players the updated time.
            if (_countdown.Enabled)
            {
                foreach (var player in _players)
                {
                    if (_playersSentTime.Contains(player)) continue;
                    _playersSentTime.Add(player);
                    player.Message(new MatchUpdate()
                    {
                        Associate = player,
                        Data = "time=3:" + ((_countdown.Interval - _countdownStopwatch.ElapsedMilliseconds)/1000.0),
                        Type = 3,
                    });
                }
            }
            else
            {
                // TODO: What is used to disable the time?
            }
        }

        /// <summary>
        /// Returns if a player can be added to the round.
        /// </summary>
        /// <param name="player">Player to add.</param>
        /// <returns>If the player can be added.</returns>
        public bool CanAddPlayer(Player player)
        {
            return _players.Count < _maxPlayers;
        }

        /// <summary>
        /// Adds a player to the match.
        /// </summary>
        /// <param name="player">Player to add.</param>
        public void AddPlayer(Player player)
        {
            // Send the response.
            player.Message(new MatchResponse()
            {
                Associate = player,
                Response = 0,
            });
            
            // Store the player and send the player.
            // TODO: The existing players see the new player, but the new player does not see the existing player.
            _players.Add(player);
            foreach (var otherPlayer in _players)
            {
                player.Message(new MatchUpdate()
                {
                    Associate = player,
                    Data = "player=9:" + otherPlayer.Id + "\nplayerName=0:" + otherPlayer.Name,
                    Type = 0,
                });
                otherPlayer.Message(new MatchUpdate()
                {
                    Associate = otherPlayer,
                    Data = "player=9:" + player.Id + "\nplayerName=0:" + player.Name,
                    Type = 0,
                });
            }
            
            // Update the timer.
            UpdateTimer();
        }
    }
}