using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using it.Network.Rest;

public class HistoryItemPlayer : MonoBehaviour
{
	[SerializeField] private GameObject InfoPanel;
	[SerializeField] private TextMeshProUGUI Nickname;
	[SerializeField] private TextMeshProUGUI TitleEvent;
	[SerializeField] private TextMeshProUGUI Count;

	[Space, Header("CardsInHand")]
	[SerializeField] private GameObject CardsInHandPanel;
	[SerializeField] private GameCardUI[] CardsInHand;

	[Space, Header("Cards")]
	[SerializeField] private GameObject CardsPanel;
	[SerializeField] private GameCardUI[] Cards;
	[SerializeField] private Vector3 WinOffect = new Vector3(0, 5, 0);

	public void Init(DistributionSharedDataPlayer player, DistributionEvent distributionEvent, bool cards, List<DistributionCard> distributionCards, string SpecialSlug = null,
			decimal SpecialBank = -1)
	{
		if (SpecialSlug == null) SpecialSlug = distributionEvent.distribution_event_type.slug;
		if (SpecialBank == -1) SpecialBank = distributionEvent.BankAmountDelta;
		TitleEvent.text = SpecialSlug;
		Nickname.text = player.user.nickname;
		if (SpecialBank != 0) Count.text = it.Helpers.Currency.String(SpecialBank);
		else Count.text = "";

		InfoPanel.SetActive(cards == false);
		CardsInHandPanel.SetActive(cards);
		CardsPanel.SetActive(distributionCards != null);

		var mainrect = gameObject.GetComponent<RectTransform>();

		if (distributionCards != null)
		{
			mainrect.sizeDelta = new Vector2(mainrect.sizeDelta.x, 83);
		}
		else
		{
			mainrect.sizeDelta = new Vector2(mainrect.sizeDelta.x, 43);
		}

		if (cards)
		{
			for (int i = 0; i < CardsInHand.Length; i++)
			{
				if (i < player.cards.Count)
				{
					CardsInHand[i].gameObject.SetActive(true);
					CardsInHand[i].Init(player.cards[i], player.combinations, player.combinations, true, false);
				}
				else
				{
					CardsInHand[i].gameObject.SetActive(false);
				}
			}
		}

		if (distributionCards != null)
		{
			for (int i = 0; i < distributionCards.Count; i++)
			{
				Cards[i].Init(distributionCards[i], player.combinations, player.combinations, true, false);
				Cards[i].transform.localPosition = Vector3.zero;
			}
			if (player.cards != null || player.cards.Count == 0)
			{
				List<DistributionCard> CardsInside = new List<DistributionCard>();
				for (int i = 0; i < player.cards.Count && player.combinations.Count > 0; i++)
				{
					if (player.combinations[0].IsContainsCard(player.cards[i].CardId))
					{
						CardsInside.Add(player.cards[i]);
					}
				}
				int n = 0;
				for (int i = 0; i < Cards.Length; i++)
				{
					if (Cards[i].isShowWin == false)
					{
						if (CardsInside.Count > n) Cards[i].Init(CardsInside[n], player.combinations, player.combinations, true, false);
						else if (player.cards.Count > n) Cards[i].Init(player.cards[n], player.combinations, player.combinations, true, false);
						else Cards[i].Init(null, player.combinations, true, false);

						Cards[i].transform.localPosition = WinOffect;
						n++;
					}
				}
			}
			else
			{
				for (int i = 0; i < Cards.Length; i++)
				{
					Cards[i].Init(null, player.combinations, true, false);
				}
			}
		}
	}
}
