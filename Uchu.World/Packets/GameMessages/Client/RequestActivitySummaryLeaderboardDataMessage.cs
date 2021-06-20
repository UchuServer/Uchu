using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RequestActivitySummaryLeaderboardDataMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RequestActivitySummaryLeaderboardData;
		[Default]
		public int GameId { get; set; }
		[Default(QueryType.TopCharacter)]
		public QueryType QueryType { get; set; }
		[Default(10)]
		public int ResultsEnd { get; set; }
		[Default]
		public int ResultsStart { get; set; }
		public GameObject Target { get; set; }
		public bool Weekly { get; set; }

		public RequestActivitySummaryLeaderboardDataMessage(GameObject associate = default, int gameId = default, QueryType queryType = QueryType.TopCharacter, int resultsEnd = 10, int resultsStart = default, GameObject target = default, bool weekly = default)
		{
			this.Associate = associate;
			this.GameId = gameId;
			this.QueryType = queryType;
			this.ResultsEnd = resultsEnd;
			this.ResultsStart = resultsStart;
			this.Target = target;
			this.Weekly = weekly;
		}
	}
}