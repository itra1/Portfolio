using System;
using System.Collections.Generic;
using System.Globalization;

using it.Network.Rest;
using it.UI.Elements;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace it.Main.SinglePages
{
	public class JackpotLastWinner : MonoBehaviour
	{
		[SerializeField] private it.UI.Avatar Avatar;
		[SerializeField] public TextMeshProUGUI beTheFirst;
		[SerializeField] public it.Inputs.CurrencyLabel jackpot;
		[SerializeField] public TextMeshProUGUI nickName;
		[SerializeField] public TextMeshProUGUI data;
		[SerializeField] public GameObject nameContainer;
		[SerializeField] private string _defaultNickname;

		[SerializeField] private Image image;
		[SerializeField] private GameObject parent;
		[SerializeField] private GameObject parentFor2Cards;
		[SerializeField] private CardLibrary _cardLibrary;

		[SerializeField] private GameObject defaultCard;

		CultureInfo cultures = CultureInfo.CreateSpecificCulture("en-GB");
		DateTime dateValue;

		private List<Image> cards = new List<Image>();

		private GameObject currentActiveParent;

		public void SetData(JackpotInfoResponse jackpotResponse)
		{
			ClearCards();
			if (jackpotResponse != null && jackpotResponse.winners.Length > 0)
			{
				Avatar.SetAvatar(jackpotResponse.winners[0].avatar_url);
				beTheFirst.gameObject.SetActive(false);
				jackpot.gameObject.SetActive(true);
				jackpot.SetValue(jackpotResponse.winners[0].amount);

				var str = jackpotResponse.winners[0].date;
				dateValue = DateTime.Parse(str, cultures);

				data.text = dateValue.ToString("MMM dd, hh:mm", cultures);

				nameContainer.gameObject.SetActive(true);
				nickName.text = jackpotResponse.winners[0].nickname;
				ShowCardOnBoard(jackpotResponse);
			}
			else
			{
				if (!string.IsNullOrEmpty(_defaultNickname))
					nickName.text = _defaultNickname;

				currentActiveParent = parentFor2Cards;
				currentActiveParent.SetActive(true);
				if (defaultCard != null) defaultCard.SetActive(true);
			}
		}

		private void ShowCardOnBoard(JackpotInfoResponse jackpotResponse)
		{
			cards.ForEach(x => Destroy(x.gameObject));
			cards.Clear();

			for (int i = 0; i < jackpotResponse.winners[0].cards.Length; i++)
			{
				if (defaultCard != null) defaultCard.SetActive(false);
				if (jackpotResponse.winners[0].cards.Length == 2)
				{
					currentActiveParent = parentFor2Cards;
					currentActiveParent.SetActive(true);
					SpawnCard(jackpotResponse, parentFor2Cards, i);
				}
				else
				{
					currentActiveParent = parent;
					currentActiveParent.SetActive(true);
					SpawnCard(jackpotResponse, parent, i);
				}
			}
		}

		private void SpawnCard(JackpotInfoResponse jackpotResponse, GameObject parent, int index)
		{
			var cardImage = Instantiate(image, parent.transform);
			var a = parent.GetComponent<HorizontalLayoutGroup>();
			cards.Add(cardImage);
			cardImage.sprite = _cardLibrary.GetCard(jackpotResponse.winners[0].cards[index]);
		}

		private void OnDisable()
		{
			if (defaultCard != null) defaultCard.SetActive(false);
			currentActiveParent.SetActive(false);
		}

		private void ClearCards()
		{
			for (int i = 0; i < cards.Count; i++)
			{
				Destroy(cards[i].gameObject);
			}

			cards.Clear();
		}
	}
}