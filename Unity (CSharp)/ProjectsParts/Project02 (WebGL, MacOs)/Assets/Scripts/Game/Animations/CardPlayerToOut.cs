using it.Network.Rest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Garilla.Games;
using it.UI;
using DG.Tweening;

namespace Garilla.Games.Animations
{
	[TableAnimation(TableAnimationsType.CardPlayerToOut, false, false)]
	public class CardPlayerToOut : GameAnimationsBase
	{
		private List<GameCardUI> _gameCards;
		//private RectTransform _dropCard;
		private PlayerGameIcone _player;
		public override void Init(Hashtable hashtable)
		{
			try
			{
				base.Init(hashtable);

				_gameCards = new List<GameCardUI>();

				PlayerGameIcone player = (PlayerGameIcone)hashtable["player"];

				foreach (var elem in player.Cards)
					_gameCards.Add(elem);
				player.Cards.Clear();
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				OnComplete?.Invoke();
			}
		}

		public override void Play()
		{
			try
			{
				base.Play();
				float time = 0.3f;

				var manageer = AnimationManager.BasePanel.CurrentGameUIManager as GameUIManager;
				var center = manageer.PlaceManager.TableImage.rectTransform.anchoredPosition;

				if (!AppConfig.DisableAudio)
					DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_GAME_CARDFOLD, 1, null, 0);

				foreach (var card in _gameCards)
				{
					card.transform.SetParent(AnimationManager.BasePanel.CurrentGameUIManager.transform);

					RectTransform cardRedct = card.GetComponent<RectTransform>();

					var cg = card.GetComponent<CanvasGroup>();
					if (cg == null)
						cg = card.gameObject.AddComponent<CanvasGroup>();
					cg.alpha = 1;

					cardRedct.DOAnchorPos(center, time).OnComplete(() =>
					{
						//GameObject.Destroy(cardRedct.gameObject);
						PoolerManager.Return("GameCard", cardRedct.gameObject);
						cg.alpha = 1;
						OnComplete?.Invoke();
					});


					DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 0f, time * time);

				}
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				OnComplete?.Invoke();
			}

		}

	}
}