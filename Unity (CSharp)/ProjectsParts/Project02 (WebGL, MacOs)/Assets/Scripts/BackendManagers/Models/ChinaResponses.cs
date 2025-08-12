using System;
using System.Collections.Generic;
using System.Linq;
using it.Network.Rest;

[Serializable]
public class ChinaChangeCardResponse
{
	
	public DistributionCard[] data;
}

[Serializable]
public class ChinaDistributionSharedData
{
	
	public int id;
	
	public bool is_active;
	
	public bool is_alive;
	
	public string created_at;
	
	public string updated_at;
	
	public ChinaDistributionSharedDataPlayer[] players;
	
	public DistributionStage[] stages;
	
	public DistributionEvent[] active_events;

	public DistributionEvent GetActiveEventByPlayer(ChinaDistributionSharedDataPlayer player)
	{
		return GetActiveEventByPlayerId(player.user.id);
	}

	public DistributionEvent GetActiveEventByPlayerId(ulong idPlayer)
	{
		if (active_events == null) return null;

		return Array.Find(
				active_events,
				it => (bool)it.is_active && it.user_id == idPlayer
		);
	}

	public bool IsActivePlayer(ulong idPlayer)
	{
		if (active_events == null) return false;

		return Array.Find(
				active_events,
				it => (bool)it.is_active && it.user_id == idPlayer
		) != null;
	}

	public bool IsWaitDistribution
	{
		get
		{
			var index = Array.FindIndex(
					players,
					delegate (ChinaDistributionSharedDataPlayer it)
					{
						return it.user.id == GameHelper.UserInfo.id;
					}
			);

			return index == -1;
		}
	}

	public bool IsAvailableEndTurn => AvailableForMoveCards.Count <= (IsPreFlop ? 0 : 1);


	public List<DistributionCard> AvailableForMoveCards
	{
		get
		{
			List<DistributionCard> cards = new List<DistributionCard>();

			foreach (var player in players)
			{
				if (player.user.id == GameHelper.UserInfo.id)
				{
					cards.AddRange(player.cards.Where(card => card.IsFreeDistribution));
					break;
				}
			}

			return cards;
		}
	}


	public DistributionStage GetActiveStage()
	{
		foreach (var item in stages)
		{
			if (item.is_active) return item;
		}

		return null;
	}

	public bool IsPostflop => GetActiveStage() != null && GetActiveStage().distribution_stage_type.slug != "initial_distribution";

	public bool IsPreFlop => !IsPostflop;

	public bool IsMeActive => IsActivePlayer(GameHelper.UserInfo.id);

	public decimal GetTotalBank()
	{
		return 0;
	}

	private ChinaDistributionSharedDataPlayer mePlayer = null;
	public ChinaDistributionSharedDataPlayer MePlayer
	{
		get
		{
			if (mePlayer == null)
			{
				foreach (var item in players)
				{
					if (item.user.id == GameHelper.UserInfo.id)
					{
						mePlayer = item;
						break;
					}
				}
			}

			return mePlayer;
		}
	}
}

[Serializable]
public class ChinaDistributionSharedDataPlayer
{
	public const string RoleBigBlind = "big-blind";
	public const string RoleSmallBlind = "small-blind";
	public const string RoleDealer = "dealer";
	public const string RoleDealerSb = "dealer+sb";
	public const string RoleDealerBb = "dealer+bb";
	public const string RolePlayerBb = "bb-payer";
	public const string RolePlayerAnte = "ante-payer";
	public const string RoleDealerBbp = "dealer+bbp";

	
	public ulong? distribution_session_id;
	
	public int? place;
	
	public UserLimited user;
	
	public decimal amount;
	
	public string role;
	
	public decimal? winning_amount;
	
	public bool? is_fantasy;
	
	public bool? is_fantasy_next_distribution;
	
	public DistributionSessionState state;
	
	public DistributionSessionState active_stage_state;
	
	public List<DistributionCard> cards;
	
	public List<DistributionPlayerCombination> combinations;

	public string ActiveStageStateName => active_stage_state != null ? active_stage_state.slug : "";
	public bool IsWin => winning_amount != null && winning_amount > 0;
	public bool IsFantasy => is_fantasy ?? false;
}

[Serializable]
public class ChinaCardRequestBody
{
	
	public ulong player_card_id;
	
	public int row;
	
	public int position;
	public ChinaCardRequestBody(ulong player_card_id, int row, int position)
	{
		this.player_card_id = player_card_id;
		this.row = row;
		this.position = position;
	}
}