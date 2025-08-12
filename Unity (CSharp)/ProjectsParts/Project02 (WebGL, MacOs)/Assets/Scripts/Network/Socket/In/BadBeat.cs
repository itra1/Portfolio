using Garilla.BadBeat;

using it.Network.Rest;

using static it.Network.Socket.DistributonEvents;

namespace it.Network.Socket
{
	[SocketAction("bad_beat_jackpot_win_as_loser", "")]
	public class BadBeatJackpotWinAsLoser : SocketIn
	{
		public decimal Award;
		public ulong TableId;
		public ulong UserId;

		public CardType[] cards;

		public class IncomingPachage
		{
			public Level1 shared_data;

			public class Level1
			{
				public CardType[] cards;
			}
		}
		public override void Parse()
		{
			Award = JSource.GetJSON("shared_data").GetJNumber("award").AsDecimal();
			TableId = JSource.GetJSON("shared_data").GetJNumber("table_id").AsULong();
			UserId = JSource.GetJSON("shared_data").GetJNumber("user_id").AsULong();

			var p = Newtonsoft.Json.JsonConvert.DeserializeObject<IncomingPachage>(sSource);
			cards = p.shared_data.cards;
			p = null;
		}

		public override void Process()
		{
			//PromotionController.Instance.SetAwawrd(PromotionInfoCategory.AoN_Race, Award);
			BadBeatController.Instance.SetAward(UserId, false, TableId, Award, cards);
		}

	}
	[SocketAction("bad_beat_jackpot_win_as_winner", "")]
	public class BadBeatJackpotWinAsWinner : SocketIn
	{
		public decimal Award;
		public ulong TableId;
		public ulong UserId;

		public CardType[] cards;
		public class IncomingPachage
		{
			public Level1 shared_data;

			public class Level1
			{
				public CardType[] cards;
			}
		}
		public override void Parse()
		{
			Award = JSource.GetJSON("shared_data").GetJNumber("award").AsDecimal();
			TableId = JSource.GetJSON("shared_data").GetJNumber("table_id").AsULong();
			UserId = JSource.GetJSON("shared_data").GetJNumber("user_id").AsULong();

			var p = Newtonsoft.Json.JsonConvert.DeserializeObject<IncomingPachage>(sSource);
			cards = p.shared_data.cards;
			p = null;
		}

		public override void Process()
		{
			//PromotionController.Instance.SetAwawrd(PromotionInfoCategory.AoN_Race, Award);
			BadBeatController.Instance.SetAward(UserId, false, TableId, Award, cards);
		}

	}
}