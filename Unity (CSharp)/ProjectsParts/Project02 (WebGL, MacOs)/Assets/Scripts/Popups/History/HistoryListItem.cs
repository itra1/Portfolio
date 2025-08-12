using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Network.Rest;
namespace Garilla.Games.UI
{
	public class HistoryListItem : MonoBehaviour
	{
		[SerializeField] private GameCardUI[] CardsMe;
		//[SerializeField] private TextMeshProUGUI Winer;
		[SerializeField] private TextMeshProUGUI _winLabel;
		[SerializeField] private TextMeshProUGUI _deltaLabel;
		[SerializeField] private RectTransform _cardParent;
		[SerializeField] private Color _incrementColor;
		[SerializeField] private Color _decrementColor;
		[SerializeField] private Color _neitralColor;

		[Space]
		[SerializeField] private Image _backSelect;

		private HandHistoryPopup HistoryPopup;
		private DistributionHistoryDataResponse DataResponse;
		private decimal _winValue;
		private decimal _winSumValue;
		private decimal _playerChipSum;
		private bool _isMe;

		public void Init(DistributionHistoryDataResponse dataResponse, HandHistoryPopup historyPopup)
		{
			DistributionSharedData sharedData = dataResponse.shared_data;
			SocketEventDistributionUserData dataPlayer = dataResponse.user_data;

			//SetSize(CardsMe.Length);
			int cardCount = 0;
			_winValue = 0;
			_winSumValue = 0;

			for (int i = 0; i < CardsMe.Length; i++)
			{
				if (dataPlayer == null || dataPlayer.cards == null || i >= dataPlayer.cards.Count)
				{
					CardsMe[i].gameObject.SetActive(false);
				}
				else
				{
					CardsMe[i].gameObject.SetActive(true);
					cardCount++;
					CardsMe[i].SetCloseState();
				}
			}


			if (dataPlayer != null && dataPlayer.cards != null)
			{
				for (int i = 0; i < dataPlayer.cards.Count; i++)
				{
					CardsMe[i].InitOnlyVisual(dataPlayer.cards[i]);
					CardsMe[i].SetOpenState();
				}
			}
#if UNITY_STANDALONE
			float w = Mathf.Min(cardCount * CardsMe[0].GetComponent<RectTransform>().rect.width + ((cardCount - 1) * 2.4f),175);
#else
			float w = Mathf.Min(cardCount * CardsMe[0].GetComponent<RectTransform>().rect.width + ((cardCount - 1) * 2.4f), 680);
#endif
			//679.4
			HorizontalLayoutGroup hlg = _cardParent.GetComponent<HorizontalLayoutGroup>();

			if (cardCount == 7)
				hlg.spacing = -2.4f;
			else
				hlg.spacing = 2.4f;


			_cardParent.sizeDelta = new Vector2(w, _cardParent.sizeDelta.y);

			if (sharedData.IsFinish)
			{
				//Winer.text = $"Not_Found";
				for (int i = sharedData.events.Count - 1; i >= 0; i--)
				{
					if (sharedData.events[i].distribution_event_type.slug == "transfer-of-winnings")
					{
						it.Logger.Log("here");
						if (sharedData.TryGetPlayer((ulong)sharedData.events[i].user_id, out DistributionSharedDataPlayer player))
						{
							if (sharedData.events[i].user_id == UserController.User.id)
							{
								_isMe = true;
								_winValue = System.Math.Abs(sharedData.events[i].BankAmountDelta);
								//_winLabel.text = it.Helpers.Currency.String(_winValue);
							}
							//Winer.text = player.user.Nickname;
						}
						//break;
						
							_winSumValue += System.Math.Abs(sharedData.events[i].BankAmountDelta);
						_winLabel.text = it.Helpers.Currency.String(_winSumValue);
					}
				}
				//_winValue = sharedData.Banks[0].amount;
				//_winLabel.text = it.Helpers.Currency.String(sharedData.Banks[0].amount);
			}
			else
			{
				//Winer.text = "Not Finished";
				_winLabel.text = "Not Finished";
			}
			CalcData(sharedData);
			WriteDelta();


			HistoryPopup = historyPopup;
			DataResponse = dataResponse;
		}

		private void WriteDelta()
		{
			if (_playerChipSum == 0)
			{
				_deltaLabel.text = it.Helpers.Currency.String(0m, true);
				_deltaLabel.color = _neitralColor;
				return;
			}
			if (_playerChipSum != 0)
			{
				if (!_isMe)
				{
					_deltaLabel.text = $"-{(it.Helpers.Currency.String(_playerChipSum, true))}";
					//_deltaLabel.text = $"{(it.Helpers.Currency.String(0, true))}";
					_deltaLabel.color = _decrementColor;
				}
				else
				{
					decimal delta = _winValue - _playerChipSum;
					if (delta == 0)
					{
						_deltaLabel.text = $"{(it.Helpers.Currency.String(delta, true))}";
						_deltaLabel.color = _neitralColor;
					}
					else if (delta < 0)
					{
						_deltaLabel.text = $"-{(it.Helpers.Currency.String(delta, true))}";
						_deltaLabel.color = _decrementColor;
					}
					else
					{
						_deltaLabel.text = $"+{(it.Helpers.Currency.String(delta, true))}";
						_deltaLabel.color = _incrementColor;

					}
				}

			}
		}

		private void CalcData(DistributionSharedData sharedData)
		{
			_playerChipSum = 0;
			bool exists = false;
			for (int i = 0; i < sharedData.players.Count; i++)
			{
				if (sharedData.players[i].user.id == UserController.User.id)
					exists = true;
			}
			if (!exists)
			{
				_playerChipSum = 0;
				return;
			}

			for (int i = 0; i < sharedData.events.Count; i++)
			{
				var eve = sharedData.events[i];

				if (eve.user_id == UserController.User.id && eve.distribution_event_type.slug == "refund-of-funds")
					_playerChipSum -= System.Math.Abs(sharedData.events[i].BankAmountDelta);

				if (eve.user_id == UserController.User.id && eve.bank_amount_delta > 0)
					_playerChipSum += (decimal)eve.bank_amount_delta;
			}

		}


		public void SetSize(int cardCount)
		{
			RectTransform cardRect = CardsMe[0].GetComponent<RectTransform>();
			RectTransform cardParent = cardRect.parent.GetComponent<RectTransform>();
			HorizontalLayoutGroup cardParentLayout = cardParent.GetComponent<HorizontalLayoutGroup>();
			RectTransform cardBlock = cardParent.parent.GetComponent<RectTransform>();

			cardParent.sizeDelta = new Vector2(cardRect.rect.width * cardCount, cardParent.sizeDelta.y);

			if (cardCount <= 3)
			{
				cardParent.sizeDelta = new Vector2(cardRect.rect.width * cardCount, cardParent.sizeDelta.y);
				cardParentLayout.spacing = 0;
			}
			else
			{
				cardParent.sizeDelta = new Vector2(cardRect.rect.width * 3, cardParent.sizeDelta.y);
				cardParentLayout.spacing = ((cardParent.sizeDelta.x - (cardRect.rect.width * cardCount))) / (cardCount - 1);
			}

		}

		public void ClickMe()
		{
			//Color c = Color.white;
			//c.a = 0.1f;
			//_backSelect.color = c;
			if(_backSelect != null)
			_backSelect.gameObject.SetActive(true);
			HistoryPopup.SelectHistoryItems(this, DataResponse);
		}
		public void Close()
		{
			//Color c = Color.white;
			//c.a = 0f;
			//_backSelect.color = c;
			if (_backSelect != null)
				_backSelect.gameObject.SetActive(false);
		}

	}
}