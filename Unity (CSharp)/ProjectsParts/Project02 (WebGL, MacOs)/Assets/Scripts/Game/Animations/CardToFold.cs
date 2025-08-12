using it.Network.Rest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Garilla.Games;
using it.UI;
using DG.Tweening;
using static PlaceController;
using com.ootii.Geometry;

namespace Garilla.Games.Animations
{
	[TableAnimation(TableAnimationsType.CardToFold, false, false)]
	public class CardToFold : GameAnimationsBase
	{
		private PlayerGameIcone _gameIcone;
		private RectTransform _dropCard;
		private RectTransform _moveRect;
		public override void Init(Hashtable hashtable)
		{
			base.Init(hashtable);
			try
			{
				if (AnimationManager.BasePanel.CurrentGameUIManager.Players == null || AnimationManager.BasePanel.CurrentGameUIManager.Players.Count <= 0)
					throw new System.Exception("Отсутствуют пользователи");
				if (AnimationManager.BasePanel.CurrentGameUIManager.CardDrop == null || AnimationManager.BasePanel.CurrentGameUIManager.CardDrop.Count <= 0)
					throw new System.Exception("Отсутствуют карты для сброса");

				_gameIcone = AnimationManager.BasePanel.CurrentGameUIManager.Players[0];

				if (_gameIcone == null)
					throw new System.Exception("Нет игрока");
				if (_gameIcone.Cards == null)
					throw new System.Exception("У игрока нет карт");

				var item = AnimationManager.BasePanel.CurrentGameUIManager.CardDrop[0].Items.Find(x => x.Count == _gameIcone.Cards.Count);

				if (item == null)
					throw new System.Exception("Нет карт для сброcа по количеству " + _gameIcone.Cards.Count);

				_dropCard = item.Item;
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				OnComplete?.Invoke();
			}
		}

		public override void Play()
		{
			base.Play();

			for (int i = 0; i < _gameIcone.Cards.Count; i++)
				_gameIcone.Cards[i].SetVisible(false);

			float time = 0.2f;

			if (_dropCard == null)
			{
				OnComplete?.Invoke();
				return;
			}
			try
			{
				//GameObject dropCards = GameObject.Instantiate(_dropCard.gameObject, _dropCard.parent);
				GameObject dropCards = PoolerManager.Spawn(_dropCard.gameObject.name);
				if (dropCards == null)
				{
					PoolerManager.AddPool(_dropCard.gameObject, 1, 1);
					dropCards = PoolerManager.Spawn(_dropCard.gameObject.name);
				}
				dropCards.transform.SetParent(_dropCard.parent);
				dropCards.transform.localScale = _dropCard.gameObject.transform.localScale;

				_moveRect = dropCards.GetComponent<RectTransform>();
				_moveRect.SetParent(_gameIcone.transform);
				_moveRect.anchoredPosition = Vector2.zero;
				CanvasGroup cg = _moveRect.gameObject.GetOrAddComponent<CanvasGroup>();
				cg.alpha = 0;
				_moveRect.SetParent(_dropCard.parent);
				dropCards.SetActive(true);
				_moveRect.DOAnchorPos(_dropCard.anchoredPosition, time).OnComplete(() =>
				{
					//GameObject.Destroy(dropCards);
					//_dropCards.gameObject.SetActive(true);
					OnComplete?.Invoke();

					DOVirtual.DelayedCall(1, () =>
					{
						OutCards();
					});

				});
				DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 1f, time * 0.3f);
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				OnComplete?.Invoke();
			}
		}
		private void OutCards()
		{
			try
			{
				if (!AppConfig.DisableAudio)
					DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_GAME_CARDFOLD, 1, null, 0);
				float time = 0.5f;

				var manageer = AnimationManager.BasePanel.CurrentGameUIManager as GameUIManager;
				var center = manageer.PlaceManager.TableImage.rectTransform.anchoredPosition;

				_moveRect.transform.SetParent(AnimationManager.BasePanel.CurrentGameUIManager.transform);

				var cg = _moveRect.GetComponent<CanvasGroup>();
				if (cg == null)
					cg = _moveRect.gameObject.AddComponent<CanvasGroup>();
				cg.alpha = 1;

				_moveRect.DOAnchorPos(center, time).OnComplete(() =>
				{
					//GameObject.Destroy(_moveRect.gameObject);
					cg.alpha = 1;
					PoolerManager.Return(_dropCard.gameObject.name, _moveRect.gameObject);
					OnComplete?.Invoke();
				});


				DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 0f, time * time);
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				OnComplete?.Invoke();
			}

		}

	}
}