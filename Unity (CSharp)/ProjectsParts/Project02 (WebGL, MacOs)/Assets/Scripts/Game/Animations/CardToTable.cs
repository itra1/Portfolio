using it.Network.Rest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Garilla.Games;
using it.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Garilla.Games.Animations
{
	[TableAnimation(TableAnimationsType.CardToTable)]
	public class CardToTable : GameAnimationsBase
	{
		private int _spawnPeriaod = 100;
		private GameCardUI cardInst;
		private int _targetIndex;
		private float _time = 0.4f;

		private List<DistributionCard> _cards;
		private DistributionEvent _event;

		public override void Init(Hashtable hashtable)
		{
			bool isSetCardType = false;
			GameObject cardInstGO = null;
			try
			{
				_cards = (List<DistributionCard>)hashtable["cards"];
				_event = (DistributionEvent)hashtable["event"];

				//var prefab = AnimationManager.BasePanel.CurrentGameUIManager.GameCardUIPrefab;
				AnimationManager.BasePanel.CurrentGameUIManager.AnimationToTableWait++;
				RectTransform _parentSpawn = AnimationManager.BasePanel.CurrentGameUIManager.CardTableParent.parent.GetComponent<RectTransform>();
				//GameObject inst = GameObject.Instantiate(prefab.gameObject, _parentSpawn);
				cardInstGO = PoolerManager.Spawn("GameCard");
				var instrect = cardInstGO.GetComponent<RectTransform>();
				instrect.SetParent(_parentSpawn);
				instrect.localScale = Vector3.one;
				instrect.sizeDelta = Vector2.zero;
				cardInst = cardInstGO.GetComponent<GameCardUI>();
				cardInst.ClearState();
				cardInst.IsOnTable = true;
				cardInst.Shadow.color = Color.white;
				cardInst.ShodowMove = true;
				isSetCardType = SetCardtype(cardInst);

				var cg = cardInst.gameObject.GetComponent<CanvasGroup>();
				if (cg != null) cg.alpha = 0;

				AnimationManager.BasePanel.CurrentGameUIManager.CardsOnTable.Add(cardInst);
				_targetIndex = AnimationManager.BasePanel.CurrentGameUIManager.CardsOnTable.Count - 1;

			}
			catch (System.Exception ex)
			{
				if (!isSetCardType && cardInstGO != null)
				{
					if (cardInst == null)
					{
						cardInst = cardInstGO.GetComponent<GameCardUI>();
						SetCardtype(cardInst);
					}
				}

				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				OnComplete?.Invoke();
			}
		}

		private bool SetCardtype(GameCardUI cardInst)
		{
			for (int i = 0; i < _cards.Count; i++)
			{
				if (_cards[i].id == _event.eventable_id)
				{
					cardInst.Init(_cards[i]);
					cardInst.SetCloseState();
					return true;
				}
			}
			return false;
		}

		public override void Play()
		{
			base.Play();

			//if (_cards.Count <= 0)
			//{
			//	OnComplete?.Invoke();
			//	return;
			//}

			if(_targetIndex < 0)
			{
				OnComplete?.Invoke();
				try
				{
					it.Logger.LogError($"Попытка запустить анимацию с отрицательным индексом карт. Количество карт на столе {AnimationManager.BasePanel.CurrentGameUIManager.CardsOnTable.Count}");
				}
				catch
				{
					it.Logger.LogError($"Попытка запустить анимацию с отрицательным индексом карт. Количество карт на столе неизвестно");
				}
				return;
			}

			PlayAnimation();

		}

		private async void PlayAnimation()
		{
			try
			{
				it.Logger.Log("TargetIndex " + _targetIndex);
				int targetPlace = Mathf.Min(_targetIndex, AnimationManager.BasePanel.CurrentGameUIManager.CardPlaces.Count - 1);

				if (AnimationManager.BasePanel.CurrentGameUIManager.CardPlaces.Count < _targetIndex)
				{
					OnComplete?.Invoke();
					return;
				}
				if (!AppConfig.DisableAudio)
					DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_GAME_CARDOPEN, 1);

				Vector2 spawnPosition = AnimationManager.BasePanel.CurrentGameUIManager.CardTableParent.anchoredPosition;
				spawnPosition += Vector2.down * 150;
				_time = 0.1f;

				Color shasowW = Color.white;
				Color shasowT = shasowW;
				shasowT.a = 0;

				Vector2 targetDelta = new Vector2(AnimationManager.BasePanel.CurrentGameUIManager.CardPlaces[targetPlace].rect.width, AnimationManager.BasePanel.CurrentGameUIManager.CardPlaces[targetPlace].rect.height);
				RectTransform instRect = cardInst.GetComponent<RectTransform>();
				instRect.rotation = Quaternion.identity;
				instRect.anchoredPosition = spawnPosition;
				instRect.transform.SetParent(AnimationManager.BasePanel.CurrentGameUIManager.CardPlaces[targetPlace]);
				instRect.localScale = Vector2.one;
				instRect.anchorMin = Vector2.one * 0.5f;
				instRect.anchorMax = Vector2.one * 0.5f;
				instRect.sizeDelta = Vector3.zero;
				cardInst.Shadow.color = shasowT;
				cardInst.Shadow.DOColor(shasowW, _time);
				var cg = cardInst.GetComponent<CanvasGroup>();
				cg.alpha = 0;
				DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 1f, _time * 0.4f);
				instRect.DOSizeDelta(targetDelta, _time);
				instRect.DOLocalRotate(Vector3.zero, _time);
				cardInst.Shadow.color = shasowT;
				instRect.DOAnchorPos(Vector2.zero, _time).OnComplete(() =>
				{
					cardInst.ShodowMove = false;
					//cardInst.MoveShadow();
					DOVirtual.DelayedCall(0.05f, () =>
					{
						cardInst.RotateToOpen();
					});

				});

				AnimationManager.BasePanel.CurrentGameUIManager.AnimationToTableWait--;
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
			}
			await UniTask.Delay(_spawnPeriaod);

			OnComplete?.Invoke();

		}

	}
}