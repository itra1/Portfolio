using it.Network.Rest;
using System.Collections;
using UnityEngine;

namespace it.Network.Socket
{
	[SocketAction("promo_aon_race_award", "Промо AoN")]
	public class PromoAonRaceAward : SocketIn
	{
		public decimal Award;
		public string Level;
		public ulong TableId;
		public ulong UserId;

		public override void Parse()
		{
			Award = JSource.GetJSON("shared_data").GetJNumber("award").AsDecimal();
			Level = JSource.GetJSON("shared_data").GetString("level");
			TableId = JSource.GetJSON("shared_data").GetJNumber("table_id").AsULong();
			UserId = JSource.GetJSON("shared_data").GetJNumber("user_id").AsULong();
		}

		public override void Process()
		{
			PromotionController.Instance.SetAwawrd(PromotionInfoCategory.AoN_Race, UserId, TableId, Level, Award);
		}
	}

	[SocketAction("promo_game_manager_award", "Промо Game Manager")]
	public class PromoGameManagerAward : SocketIn
	{
		public decimal Award;
		public ulong UserId;
		public ulong TableId;

		public override void Parse()
		{
			Award = JSource.GetJSON("shared_data").GetJNumber("award").AsDecimal();
			UserId = JSource.GetJSON("shared_data").GetJNumber("user_id").AsULong();
			TableId = JSource.GetJSON("shared_data").GetJNumber("table_id").AsULong();
		}

		public override void Process()
		{
			PromotionController.Instance.SetAwawrd(PromotionInfoCategory.Game_Manager, UserId, TableId, "", Award);
		}
	}

	[SocketAction("promo_poker_hands_award", "Промо Poker Hands")]
	public class PromoPokerHandsAward : SocketIn
	{
		public decimal Award;
		public ulong UserId;
		public ulong TableId;

		public override void Parse()
		{
			Award = JSource.GetJSON("shared_data").GetJNumber("award").AsDecimal();
			UserId = JSource.GetJSON("shared_data").GetJNumber("user_id").AsULong();
			TableId = JSource.GetJSON("shared_data").GetJNumber("table_id").AsULong();
		}

		public override void Process()
		{
			PromotionController.Instance.SetAwawrd(PromotionInfoCategory.Poker_Hands, UserId, TableId, "", Award);
		}
	}

	[SocketAction("promo_three_bet_race_award", "Промо TreeBat")]
	public class PromoThreeBatAward : SocketIn
	{
		public decimal Award;
		public string Level;
		public ulong TableId;
		public ulong UserId;

		public override void Parse()
		{
			Award = JSource.GetJSON("shared_data").GetJNumber("award").AsDecimal();
			Level = JSource.GetJSON("shared_data").GetString("level");
			TableId = JSource.GetJSON("shared_data").GetJNumber("table_id").AsULong();
			UserId = JSource.GetJSON("shared_data").GetJNumber("user_id").AsULong();
		}

		public override void Process()
		{
			PromotionController.Instance.SetAwawrd(PromotionInfoCategory.Bet_Race, UserId, TableId, Level, Award);
		}
	}

	[SocketAction("promo_wtsd_race_award", "Промо WTSD Race")]
	public class PromoWTSDBatAward : SocketIn
	{
		public decimal Award;
		public string Level;
		public ulong TableId;
		public ulong UserId;

		public override void Parse()
		{
			Award = JSource.GetJSON("shared_data").GetJNumber("award").AsDecimal();
			Level = JSource.GetJSON("shared_data").GetString("level");
			TableId = JSource.GetJSON("shared_data").GetJNumber("table_id").AsULong();
			UserId = JSource.GetJSON("shared_data").GetJNumber("user_id").AsULong();
		}

		public override void Process()
		{
			PromotionController.Instance.SetAwawrd(PromotionInfoCategory.WT_Race, UserId, TableId, Level, Award);
		}
	}

	[SocketAction("promo_aon_race_increment", "Промо AoN Race increment")]
	public class PromoAonRaceIncrement : SocketIn
	{
		public int Counter;
		public string Level;
		public ulong TableId;

		public override void Parse()
		{
			Counter = JSource.GetJSON("shared_data").GetJNumber("counter").AsInt();
			Level = JSource.GetJSON("shared_data").GetString("level");
			TableId = JSource.GetJSON("shared_data").GetJNumber("table_id").AsULong();
		}

		public override void Process()
		{
			PromotionController.Instance.SetIncrement(PromotionInfoCategory.AoN_Race, Level, Counter, TableId);
		}
	}

	[SocketAction("promo_three_bet_race_increment", "Промо Tree Bet increment")]
	public class PromoThreeBetIncrement : SocketIn
	{
		public int Counter;
		public string Level;
		public ulong TableId;

		public override void Parse()
		{
			Counter = JSource.GetJSON("shared_data").GetJNumber("counter").AsInt();
			Level = JSource.GetJSON("shared_data").GetString("level");
			TableId = JSource.GetJSON("shared_data").GetJNumber("table_id").AsULong();
		}

		public override void Process()
		{
			PromotionController.Instance.SetIncrement(PromotionInfoCategory.Bet_Race, Level, Counter, TableId);
		}
	}

	[SocketAction("promo_wtsd_race_increment", "Промо WTSD Race increment")]
	public class PromoWTSDRaceIncrement : SocketIn
	{
		public int Counter;
		public string Level;
		public ulong TableId;

		public override void Parse()
		{
			Counter = JSource.GetJSON("shared_data").GetJNumber("counter").AsInt();
			Level = JSource.GetJSON("shared_data").GetString("level");
			TableId = JSource.GetJSON("shared_data").GetJNumber("table_id").AsULong();
		}

		public override void Process()
		{
			PromotionController.Instance.SetIncrement(PromotionInfoCategory.WT_Race, Level, Counter, TableId);
		}
	}

}