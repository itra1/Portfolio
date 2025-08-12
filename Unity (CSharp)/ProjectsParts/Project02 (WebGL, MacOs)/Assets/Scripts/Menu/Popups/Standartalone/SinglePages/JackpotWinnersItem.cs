using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using it.Main.SinglePages;
using UnityEngine;
using TMPro;
using it.Network.Rest;
using UnityEditor;
using UnityEngine.UI;
using Avatar = it.UI.Avatar;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;
using System.Globalization;

namespace it.Main.SinglePages
{
	public class JackpotWinnersItem : MonoBehaviour
	{
		[SerializeField] private it.UI.Avatar _avatar;
		[SerializeField] private TextMeshProUGUI _userNameLabel;
		[SerializeField] private TextMeshProUGUI _dateLabel;
		[SerializeField] private it.Inputs.CurrencyLabel _amountLabel;
		[SerializeField] private CardLibrary _cardLibrary;
		[SerializeField] private GameObject parent;
		[SerializeField] private GameObject parentFor2Cards;

		[SerializeField] private Image prefab;

		private GameObject currentObject;
		private List<Image> _cards = new List<Image>();

		private CultureInfo cultures = CultureInfo.CreateSpecificCulture("en-GB");
		private DateTime dateValue;

		public void UseData(JackpotInfoResponse jackpotResponse, int index)
		{
			_cards.ForEach(x => Destroy(x.gameObject));
			_cards.Clear();

			for (int i = 0; i < jackpotResponse.winners[index].cards.Length; i++)
			{

				if (jackpotResponse.winners[index].cards.Length == 2)
				{
					currentObject = parentFor2Cards;
					currentObject.SetActive(true);
					var item = Instantiate(prefab, parentFor2Cards.transform);
					item.sprite = _cardLibrary.GetCard(jackpotResponse.winners[index].cards[i]);
					_cards.Add(item);
				}
				else
				{
					currentObject = parent;
					currentObject.SetActive(true);
					var item = Instantiate(prefab, parent.transform);
					item.sprite = _cardLibrary.GetCard(jackpotResponse.winners[index].cards[i]);
					_cards.Add(item);
				}
			}

			_amountLabel.SetValue(jackpotResponse.winners[index].amount);
			_avatar.SetAvatar(jackpotResponse.winners[index].avatar_url);
			_userNameLabel.text = jackpotResponse.winners[index].nickname;

			var str = jackpotResponse.winners[index].date;
			dateValue = DateTime.Parse(str);

			_dateLabel.text = dateValue.ToString("MMM dd, hh:mm", cultures);
		}
	}
}

