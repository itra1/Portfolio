using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;
using com.ootii.Geometry;

namespace it.Popups
{
	/// <summary>
	/// ВЫбор игры для Dealer Choise
	/// </summary>
	public class DealerChoicePopup : PopupBase
	{
		[SerializeField] private TextMeshProUGUI _timerLabel;
		//[SerializeField] private it.UI.Elements.GraphicButtonUI[] _buttons;
		[SerializeField] private Game[] _games;

		private Dictionary<PokerTableType, string> _settingsNameMap = new Dictionary<PokerTableType, string>() {
			{ PokerTableType.NLH ,"texas_holdem"},
			{ PokerTableType.PLO4, "omaha_high"},
			{ PokerTableType.PLO4HiLo, "omaha_low"},
			{ PokerTableType.PLO5 ,"omaha_high_5"},
			{ PokerTableType.PLO5HiLo, "omaha_low_5"},
			{ PokerTableType.Montana, "montana" },
			{ PokerTableType.Memphis, "texas_holdem_3" },
			{ PokerTableType.PLO6, "omaha_high_6" },
			{ PokerTableType.PLO7, "omaha_high_7" }
		};

		private string _tableName;
		private Coroutine _timerCor;
		private it.Network.Rest.Table _table;
		private bool _isSelected;

		[System.Serializable]
		public struct Game
		{
			public GameType Type;
			public it.UI.Elements.GraphicButtonUI Button;
			public string Slug;
			[HideInInspector] public Image Icone;
		}

		protected override void Awake()
		{
			base.Awake();
		}

		private void InitButtons()
		{

			for (int i = 0; i < _games.Length; i++)
			{

				int index = i;
				var elem = _games[i];

				_games[i].Icone = elem.Button.transform.Find("Icone").GetComponent<Image>();
				_games[i].Icone.color = Color.white;

				if (_table.available_dealer_choices.Count > 0 && !_table.available_dealer_choices.Contains(elem.Slug))
				{
					elem.Button.gameObject.SetActive(false);
					continue;
				}
				else
					elem.Button.gameObject.SetActive(true);


				_games[i].Icone.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				elem.Button.OnClick.RemoveAllListeners();
				elem.Button.OnClick.AddListener(() =>
				{
					if (_isSelected) return;
					_isSelected = true;
					_tableName = elem.Slug;
					FocusButton(index);
					ComplateTimer();
				});
				var hHendler = elem.Button.GetOrAddComponent<Garilla.Main.HoverHandler>();
				var iRect = _games[i].Icone.GetComponent<RectTransform>();
				hHendler.PointerEnterEvent.RemoveAllListeners();
				hHendler.PointerEnterEvent.AddListener(() => {
					iRect.DOAnchorPos(iRect.anchoredPosition + Vector2.up * 3, 0.15f);
				});
				hHendler.PointerExitEvent.RemoveAllListeners();
				hHendler.PointerExitEvent.AddListener(() => {
					iRect.DOAnchorPos(Vector2.zero, 0.15f);
				});
			}
		}


		public void SetTable(it.Network.Rest.Table table)
		{
			_isSelected = false;
			_table = table;
			InitButtons();
			FocusAllButton();
			StopAllCoroutines();
			_timerCor = StartCoroutine(TimerCoroutine(5));
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			StopAllCoroutines();
		}

		public void FocusButton(int index)
		{
			for (int i = 0; i < _games.Length; i++)
			{
				_games[i].Icone.color = i == index ? Color.white : Color.gray;
				//var iRect = _games[i].Icone.GetComponent<RectTransform>();
				//iRect.DOAnchorPos((i == index ? iRect.anchoredPosition + Vector2.up*3 : iRect.anchoredPosition), 0.15f);
			}
		}

		public void FocusAllButton()
		{
			for (int i = 0; i < _games.Length; i++)
			{
				_games[i].Icone.color = Color.white;
				//_games[i].Icone.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			}
		}

		private IEnumerator TimerCoroutine(int timer)
		{
			while (timer >= 0)
			{
				ConfirmTimer(timer);
				yield return new WaitForSeconds(1);
				timer--;
			}
			ConfirmTimer(0);
			Hide();
			//ComplateTimer();
		}

		public void ConfirmTimer(int second)
		{
			_timerLabel.text = $"{second} <size=40%>{I2.Loc.LocalizationManager.GetTermTranslation("popup.dealerChoise.sec")}";
		}

		private void ComplateTimer()
		{
			TableApi.DealerChoiceSelect(_table.id, _tableName, (result) =>
			{
				Hide();
			});
		}

	}
}