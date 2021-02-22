using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
        /// Players currently ready for the match.
        /// </summary>
        private List<Player> _readyPlayers = new List<Player>();
        
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
                var remainingTimeChanged = false;
                if (!_countdown.Enabled)
                {
                    // Send the initial time.
                    remainingTimeChanged = true;
                    _countdown.Start();
                    _countdownStopwatch.Start();
                } else if (_players.Count == _readyPlayers.Count && _countdown.Interval - _countdownStopwatch.ElapsedMilliseconds > 5000)
                {
                    // Set the time to 5 seconds to prepare the round.
                    remainingTimeChanged = true;
                    _countdown.Stop();
                    _countdownStopwatch.Stop();
                    _countdown.Interval = 5000;
                    _countdown.Start();
                    _countdownStopwatch.Start();
                }
                
                // Send the clients the updated time.
                foreach (var player in _players)
                {
                    if (!_playersSentTime.Contains(player))
                    {
                        _playersSentTime.Add(player);
                        player.Message(new MatchUpdate()
                        {
                            Associate = player,
                            Data = "time=3:" + ((_countdown.Interval - _countdownStopwatch.ElapsedMilliseconds)/1000.0),
                            Type = MatchUpdateType.SetInitialTime,
                        });
                    }
                    else if (remainingTimeChanged)
                    {
                        player.Message(new MatchUpdate()
                        {
                            Associate = player,
                            Data = "time=3:" + ((_countdown.Interval - _countdownStopwatch.ElapsedMilliseconds)/1000.0),
                            Type = MatchUpdateType.SetTime,
                        });
                    }
                }
            }
            else
            {
                // Stop the time.
                _countdown.Stop();
                _countdownStopwatch.Stop();
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
        /// Returns a player is in the match.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns>If the player is in the round.</returns>
        public bool HasPlayer(Player player)
        {
            return _players.Contains(player);
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
                    Type = MatchUpdateType.PlayerAdded,
                });
                otherPlayer.Message(new MatchUpdate()
                {
                    Associate = otherPlayer,
                    Data = "player=9:" + player.Id + "\nplayerName=0:" + player.Name,
                    Type = MatchUpdateType.PlayerAdded,
                });
            }
            
            // Update the timer.
            UpdateTimer();
        }

        /// <summary>
        /// Set a player as ready in their current match.
        /// </summary>
        /// <param name="player">Player to mark as ready.</param>
        public void SetPlayerReady(Player player)
        {
            // Send to all players that the player is ready.
            _readyPlayers.Add(player);
            player.Message(new MatchResponse()
            {
                Associate = player,
                Response = 0,
            });
            foreach (var otherPlayer in _players)
            {
                otherPlayer.Message(new MatchUpdate()
                {
                    Associate = otherPlayer,
                    Data = "player=9:" + player.Id,
                    Type = MatchUpdateType.PlayerReady,
                });
            }
            
            // Update the time.
            UpdateTimer();
        }

        /// <summary>
        /// Set a player as not ready in their current match.
        /// </summary>
        /// <param name="player">Player to mark as ready.</param>
        public void SetPlayerNotReady(Player player)
        {
            // Send to all players that the player is no longer ready.
            _readyPlayers.Remove(player);
            player.Message(new MatchResponse()
            {
                Associate = player,
                Response = 0,
            });
            foreach (var otherPlayer in _players)
            {
                otherPlayer.Message(new MatchUpdate()
                {
                    Associate = otherPlayer,
                    Data = "player=9:" + player.Id,
                    Type = MatchUpdateType.PlayerNotReady,
                });
            }
        }
    }
}