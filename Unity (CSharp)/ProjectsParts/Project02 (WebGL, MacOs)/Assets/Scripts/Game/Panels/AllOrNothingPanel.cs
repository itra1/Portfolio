using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using it.Network.Rest;
 

namespace Garilla.Games.UI
{
	public class AllOrNothingPanel : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _handLeftLabel;
		[SerializeField] private TextMeshProUGUI _winLinesLabel;
		[SerializeField] private GameObject _bingoGameObject;
		[SerializeField] private it.UI.Elements.GraphicButtonUI _restartButton;
		[SerializeField] private List<CardAllOrNothing> _cards = new List<CardAllOrNothing>();

		private GameUIManager _gameManager;
		private it.Network.Rest.BingoGame _bGame;
		private bool _isVisibleCards;
		private bool _isVisibleBingo;
		private bool _checkCompare;
		private bool _waitUpdate;
		private bool _waitFirst;
		private bool _lockedUpdate;

		public GameUIManager GameManager { get => _gameManager; set => _gameManager = value; }

		private void Awake()
		{
			_waitFirst = true;
			SetInteractableButton(false);
		}

		public void DistributionStart()
		{
			if (_waitUpdate)
			{
				_waitUpdate = false;
				GetUpdate();
			}
		}

		private void OnEnable()
		{
			_bingoGameObject.gameObject.SetActive(false);
		}

		public void Init()
		{
			if (_gameManager.SharedData == null) return;

			for (int i = 0; i < _gameManager.SharedData.distribution.players.Count; i++)
			{
				if (_gameManager.SharedData.distribution.players[i].user.id == UserController.User.id)
				{
					SetBingoGame(_gameManager.SharedData.distribution.players[i].bingo_game, null);
				}
			}
		}

		public void SetBaseData(IBingoInfo baseData)
		{
			_title.text = string.Format("game.panels.allOrNothing.winUpTo".Localized(), it.Helpers.Currency.String(baseData.SpinMax));
			_handLeftLabel.text = string.Format("game.panels.allOrNothing.handLeft".Localized(), baseData.BingoMaxHand);
		}

		private void SetBingoGame(it.Network.Rest.BingoGame newBingoGame, UnityEngine.Events.UnityAction onComplete = null)
		{
			if (_lockedUpdate) return;
			_bGame = newBingoGame;
			_handLeftLabel.text = string.Format("game.panels.allOrNothing.handLeft".Localized(), (_bGame.hands_max - _bGame.hands_left));
			_winLinesLabel.text = string.Format("game.panels.allOrNothing.Line".Localized(), (_bGame.WinningAmounts.Count));
			_title.text = string.Format("game.panels.allOrNothing.winUpTo".Localized(), it.Helpers.Currency.String(_bGame.spin_variants[0]));
			ConfirmGame(() =>
			{
				if (_waitFirst)
				{
					_waitFirst = false;
					SetInteractableButton(true);
				}
				onComplete?.Invoke();

			});

			if (_bGame.is_win != null && (bool)_bGame.is_win)
			{
				float time = 0;

				foreach (var amount in _bGame.WinningAmounts)
				{
					time += 1;
					DOVirtual.DelayedCall(time, () => ShowBingoLabel());
					time += 1;
					DOVirtual.DelayedCall(time, () =>
					{
						GameManager.PrisePanel.SetValue(_bGame.spin_variants, (decimal)_bGame.winning_amount);
						GameManager.PrisePanel.gameObject.SetActive(true);
					});
					time += 6;
				}
			}
			else
			{
				if (_checkCompare)
				{
					ClearCompare();
					CheckCompare();
				}
			}
		}

		public void CheckCompare()
		{
			_checkCompare = true;

			foreach (var player in _gameManager.Players.Values)
			{
				if (player.UserId != UserController.User.id)
					continue;

				bool isCompare = false;
				foreach (var c in _cards)
				{
					foreach (var pc in player.Cards)
					{
						isCompare = pc.Card.card.card_type_id == c.Card.card_type_id && !c.StarImage.gameObject.activeSelf;
						if (isCompare)
						{
							c.BingoLight.SetActive(isCompare);
							pc.BingoLight.gameObject.SetActive(isCompare);
						}
					}

				}
			}
		}

		public void ClearCompare()
		{
			_checkCompare = false;
			foreach (var c in _cards)
				c.BingoLight.SetActive(false);
			foreach (var player in _gameManager.Players.Values)
				foreach (var pc in player.Cards)
					pc.BingoLight.gameObject.SetActive(false);
		}

		private void ConfirmGame(UnityEngine.Events.UnityAction onComplete)
		{
			StartCoroutine(VisibleCards(onComplete));
		}

		public IEnumerator VisibleCards(UnityEngine.Events.UnityAction onComplete)
		{
			for (int i = 0; i < _cards.Count; i++)
			{
				var itm = _bGame.matrix[(int)(i / 4)][i % 4];
				_cards[i].Init(itm.card, itm.is_matched);
				yield return new WaitForFixedUpdate();
			}
			onComplete?.Invoke();
		}

		/// <summary>
		/// Зфпрос на обновление Desk
		/// </summary>
		public void RestartClickButton()
		{
			SetInteractableButton(false);
			_waitUpdate = true;
		}

		public void GetUpdate()
		{
			bool checkcompare = _checkCompare;

			ClearCompare();
			TableApi.ResetBingoGame(_gameManager.SelectTable.id, (success) =>
			{
				if (success.IsSuccess)
				{
					SetBingoGame(success.Result, () =>
					{
						if (checkcompare)
							CheckCompare();
						SetInteractableButton(true);
						DOVirtual.DelayedCall(5, () =>
						{
							_lockedUpdate = false;
						});
					});
					_lockedUpdate = true;
				}
			});
		}

		private void SetInteractableButton(bool value)
		{
			_restartButton.interactable = value;
			_restartButton.Image.color = value ? Color.white : new Color(.5f, .5f, .5f);
			_restartButton.GetComponentInChildren<TextMeshProUGUI>().color = value ? Color.white : new Color(.5f, .5f, .5f);
		}

		/// <summary>
		/// Анимированное отображение бинго лейьла
		/// </summary>
		public void ShowBingoLabel()
		{
			Animator bingoAnimator = _bingoGameObject.GetComponent<Animator>();
			_bingoGameObject.gameObject.SetActive(true);
			bingoAnimator.SetTrigger("visible");
		}

		/// <summary>
		/// Анимированное сокрытие бинго лейбла
		/// </summary>
		public void HideBingoLabel()
		{
			var cg = _bingoGameObject.GetComponent<CanvasGroup>();
			DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 0, 0.4f);
		}

	}
}