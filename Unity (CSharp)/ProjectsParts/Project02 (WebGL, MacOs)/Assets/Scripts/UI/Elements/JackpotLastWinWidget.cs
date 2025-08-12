using System.Collections;
using UnityEngine;
using TMPro;
using System;
using it.Api;

namespace Garilla.Widgets
{
	public class JackpotLastWinWidget : MonoBehaviour
	{
		[SerializeField] private it.UI.Avatar _avatar;
		[SerializeField] private it.Inputs.CurrencyLabel _valueLabel;
		[SerializeField] private TextMeshProUGUI _dateLabel;
		[SerializeField] private TextMeshProUGUI _winnerNameLabel;
		[SerializeField] private RectTransform _winnerNamePanel;
		[SerializeField] private TextMeshProUGUI _beTheFirst;

		private DateTime _lastRequest;

		private void Awake()
		{
			ClearData();
			GetLastWinner();
		}

		private void GetLastWinner()
		{
			if (!ServerManager.ExistsServers) return;
			_lastRequest = DateTime.Now.AddSeconds(30);

			AppApi.GetJackpot((result) =>
			{

				if (result.IsSuccess && result.Result.winners.Length > 0)
				{
					_beTheFirst.gameObject.SetActive(false);
					_valueLabel.gameObject.SetActive(true);
					_winnerNamePanel.gameObject.SetActive(true);
					_dateLabel.gameObject.SetActive(true);

					var itm = result.Result.winners[0];

					if (!string.IsNullOrEmpty(itm.avatar_url))
						_avatar.SetAvatar(itm.avatar_url);

					_valueLabel.SetValue("{0}", itm.amount);
					_winnerNameLabel.text = itm.nickname;
					_dateLabel.text = itm.Date.ToString("MMM dd, hh:mm");
				}
				else
					ClearData();

			});

		}

		private void ClearData()
		{
			_beTheFirst.gameObject.SetActive(true);
			_valueLabel.gameObject.SetActive(false);
			_winnerNamePanel.gameObject.SetActive(false);
			_dateLabel.gameObject.SetActive(false);
			_avatar.SetDefaultAvatar();
		}


		private void Update()
		{
			if (_lastRequest < DateTime.Now)
				GetLastWinner();
		}


	}
}