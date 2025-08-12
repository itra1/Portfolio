using it.Network.Rest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Garilla.Games;
using it.UI;
using DG.Tweening;

namespace Garilla.Games.Animations
{
	[TableAnimation(TableAnimationsType.CardFromTableToOut, false, false)]
	public class CardFromTableToOut : GameAnimationsBase
	{
		List<GameCardUI> _cardsOnTable;
		public override void Init(Hashtable hashtable)
		{
			try
			{
				base.Init(hashtable);
				var panel = (AnimationManager.BasePanel.CurrentGameUIManager as GameUIManager);

				_cardsOnTable = new List<GameCardUI>(panel.CardsOnTable);
				panel.CardsOnTable.Clear();
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

				float time = 0.05f * _cardsOnTable.Count;
				float timeOut = 0.1f;

				if (_cardsOnTable.Count <= 0)
				{
					OnComplete?.Invoke();
					return;
				}

				RectTransform canterAncor = _cardsOnTable[0].transform.parent.parent.GetComponent<RectTransform>();
				RectTransform firstCard = _cardsOnTable[0].GetComponent<RectTransform>();

				for (int i = 0; i < _cardsOnTable.Count; i++)
				{
					var cardItem = _cardsOnTable[i];
					cardItem.NoWin();
					cardItem.RotateToClose();
					cardItem.transform.SetParent(cardItem.transform.parent.parent);
					RectTransform cardRedct = cardItem.GetComponent<RectTransform>();

					var cg = cardItem.gameObject.GetComponent<CanvasGroup>();
					if (cg == null)
						cg = cardItem.gameObject.AddComponent<CanvasGroup>();
					cg.alpha = 1;


					cardRedct.DOAnchorPos(firstCard.anchoredPosition, time).OnComplete(() =>
					{

						cardRedct.DOAnchorPos(canterAncor.anchoredPosition, timeOut).OnComplete(() =>
						{

							PoolerManager.Return("GameCard", cardRedct.gameObject);
							cg.alpha = 1;
							OnComplete?.Invoke();
						});

						DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 0f, time * timeOut);
					});

				}
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
			}
		}

	}

}