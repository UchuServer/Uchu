using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InfectedRose.Core;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;
using Uchu.World.Systems.Match;

namespace Uchu.World.Handlers.GameMessages
{
    public class MatchHandler : HandlerGroup
    {
        [PacketHandler]
        public void MatchRequestHandler(MatchRequestMessage message, Player player)
        {
            if (message.Type == MatchRequestType.Join)
            {
                // Add the player to a match or remove them.
                if (message.Value == 0)
                {
                    Provisioner.PlayerLeft(player);
                }
                else
                {
                    Provisioner.GetProvisioner(message.Value).AddPlayer(player);
                }
            }
            else if (message.Type == MatchRequestType.SetReady)
            {
                // Set the player as ready or not.
                if (message.Value == 1)
                {
                    Provisioner.PlayerReady(player);
                } else if (message.Value == 0)
                {
                    Provisioner.PlayerNotReady(player);
                }
            }
        }

        [PacketHandler]
        public void RequestActivitySummaryLeaderboardDataMessageHandler(RequestActivitySummaryLeaderboardDataMessage message, Player player)
        {
            using var ctx = new UchuContext();
            // Get current year and week number according to ISO 8601
            var yearAndWeek = ISOWeek.GetYear(DateTime.Now) * 100 + ISOWeek.GetWeekOfYear(DateTime.Now);

            // Find leaderboard entries for this activity
            // If weekly leaderboard is requested, only return results from current week
            var leaderboardQueryable = ctx.ActivityScores
                .Where(score => score.Activity == message.GameId
                                && (message.Weekly ? score.Week == yearAndWeek : score.Week == 0));

            // Find leaderboard type
            var activity = ClientCache.Find<Activities>(message.GameId);
            var leaderboardType = (LeaderboardType) (activity?.LeaderboardType ?? -1);

            // For some reason, whoever made this in 2011 gave the the NS and NT footraces the
            // same activity ID. So for footraces, we filter by zone ID to ensure we don't mix
            // the leaderboards. But this check shouldn't be done for other leaderboards, as
            // Survival minigames have their leaderboards accessible from multiple zones.
            if (leaderboardType == LeaderboardType.Footrace)
                leaderboardQueryable = leaderboardQueryable.Where(score => score.Zone == (int) player.Zone.ZoneId);

            // Order either by time ascending or time descending depending on which kind of activity it is
            if (leaderboardType == LeaderboardType.Footrace
                || leaderboardType == LeaderboardType.AvantGardensSurvival
                || leaderboardType == LeaderboardType.BattleOfNimbusStation)
                leaderboardQueryable = leaderboardQueryable.OrderByDescending(score => score.Time);
            else
                leaderboardQueryable = leaderboardQueryable.OrderBy(score => score.Time);

            var leaderboard = leaderboardQueryable.ToList();

            // Dictionary <rank, score>
            // Rank is what the client will show as position on the leaderboard
            var toSend = new Dictionary<int, ActivityScore>();

            switch (message.QueryType)
            {
                case QueryType.TopSocial:
                    // TODO: Friends.
                    break;
                case QueryType.TopAll:
                    // Top 10.
                    for (var i = message.ResultsStart; i < message.ResultsEnd && i < leaderboard.Count; i++)
                    {
                        toSend.Add(i + 1, leaderboard[i]);
                    }
                    break;
                case QueryType.TopCharacter:
                    // Leaderboard around this player's rank.
                    var playerIndex = leaderboard.FindIndex(score => score.CharacterId == player.Id);

                    // If player is not in leaderboard, return (client will show a friendly message telling the player
                    // to first complete the activity)
                    if (playerIndex == -1)
                        break;

                    var availableBefore = playerIndex;
                    var availableAfter = leaderboard.Count - playerIndex;

                    // By default we show 5 scores before this player's, and 4 after (last index isn't included).
                    var includeBefore = 5;
                    var includeAfter = 5;

                    // For every step we can't go before, add one to after
                    includeAfter += Math.Max(0, 5 - availableBefore);

                    // For every step we can't go after, add one to before
                    includeBefore += Math.Max(0, 5 - availableAfter);

                    // Ensure we don't go outside the leaderboard limits
                    var startIndex = Math.Max(0, playerIndex - includeBefore);
                    var stopIndex = Math.Min(leaderboard.Count, playerIndex + includeAfter);

                    for (var i = startIndex; i < stopIndex; i++)
                    {
                        toSend.Add(i + 1, leaderboard[i]);
                    }
                    break;
            }

            // "Properly" implementing this odd nested-dictionaries-and-arrays-inside-LDF didn't seem
            // particularly fun and/or useful; this implementation just does everything needed for leaderboards.
            var data = new LegoDataDictionary
            {
                { "ADO.Result", true },
                { "Result.Count", 1 },
                { "Result[0].Index", "RowNumber" },
                { "Result[0].RowCount", toSend.Count },
            };

            var index = 0;
            foreach (var (rank, activityScore) in toSend)
            {
                var characterName = ctx.Characters.FirstOrDefault(c => c.Id == activityScore.CharacterId)?.Name
                                    ?? "Deleted Character";
                data.Add($"Result[0].Row[{index}].CharacterID", activityScore.CharacterId);
                data.Add($"Result[0].Row[{index}].LastPlayed", activityScore.LastPlayed);
                data.Add($"Result[0].Row[{index}].NumPlayed", activityScore.NumPlayed);
                data.Add($"Result[0].Row[{index}].RowNumber", rank);
                data.Add($"Result[0].Row[{index}].Time", activityScore.Time);
                data.Add($"Result[0].Row[{index}].Points", activityScore.Points);
                data.Add($"Result[0].Row[{index}].name", characterName);
                // TODO: ".Relationship" variable (int).
                // (AGS client script: if not 0, FoundFriendGuild set to true. Teams?)
                // data.Add($"Result[0].Row[{index}].Relationship", 0);
                index++;
            }

            player.Message(new SendActivitySummaryLeaderboardDataMessage
            {
                Associate = player,
                GameId = message.GameId,
                InfoType = (int) message.QueryType,
                LeaderboardData = data,
                Throttled = false,
                Weekly = message.Weekly,
            });
        }
    }
}
