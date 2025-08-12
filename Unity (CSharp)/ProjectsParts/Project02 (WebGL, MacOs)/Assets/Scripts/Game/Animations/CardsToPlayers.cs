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
	[TableAnimation(TableAnimationsType.CardsToPlayer)]
	public class CardsToPlayers : GameAnimationsBase
	{

		private List<PlayerCards> _cards;
		private int _spawnPeriaod = 100;
		private List<DistributionCard> _distribCards;

		public override void Init(Hashtable hashtable)
		{
			try
			{
				base.Init(hashtable);
				_cards = new List<PlayerCards>();

				List<DistributionSharedDataPlayer> playersList = (List<DistributionSharedDataPlayer>)hashtable["players"];
				_distribCards = (List<DistributionCard>)hashtable["cards"];

				//var prefab = AnimationManager.BasePanel.CurrentGameUIManager.GameCardUIPrefab;
				Canvas canv = AnimationManager.BasePanel.GetComponentInParent<Canvas>();
				int index = -1;
				foreach (var elem in playersList)
				{
					index++;
					PlayerCards item = new PlayerCards()
					{
						Distribs = elem.cards,
						PlayerId = elem.user.id,
						Instances = new List<GameCardUI>()
					};
					foreach (var key in AnimationManager.BasePanel.CurrentGameUIManager.Players.Keys)
					{
						if (AnimationManager.BasePanel.CurrentGameUIManager.Players[key].UserId == item.PlayerId)
						{
							item.Player = AnimationManager.BasePanel.CurrentGameUIManager.Players[key];
							item.CardBlock = item.Player.CardsPositions.Cards.Find(x => x.CardsCount == item.Distribs.Count);
							item.Player.WaitMovePlayerAnimation = true;
						}
					}
					item.Player.ClearCards();
					for (int i = 0; i < item.Distribs.Count; i++)
					{
						//GameObject inst = GameObject.Instantiate(prefab.gameObject, canv.transform);
						GameObject inst = PoolerManager.Spawn("GameCard");
						inst.transform.SetParent(canv.transform);
						inst.transform.localScale = Vector3.one;
						RectTransform iRect = inst.GetComponent<RectTransform>();
						iRect.anchoredPosition = Vector3.zero;
						iRect.localRotation = Quaternion.identity;
						iRect.localScale = Vector3.zero;
						//iRect.anchorMin = Vector2.zero;
						//iRect.anchorMax = Vector2.one;
						iRect.sizeDelta = Vector2.zero;
						iRect.localPosition = Vector2.zero;
						GameCardUI instComp = inst.GetComponent<GameCardUI>();
						instComp.IsPlayerCard = true;
						bool exists = false;

						if (index == 0)
							if (!AppConfig.DisableAudio)
								DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_GAME_DEAL, 1, null, 0);

						for (int x = 0; x < _distribCards.Count; x++)
						{
							if (_distribCards[x].id == item.Distribs[i].id)
							{
								instComp.Init(_distribCards[x]);
								exists = true;
							}
						}
						if (!exists)
							instComp.Init(item.Distribs[i]);

						item.Instances.Add(instComp);
						inst.gameObject.SetActive(false);
						item.Player.Cards.Add(instComp);
					}
					_cards.Add(item);
				}
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

			try
			{

				for (int i = 0; i < _cards.Count; i++)
				{
					PlayAnim(_cards[i]);
				}
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				OnComplete?.Invoke();
			}
		}

		private async void PlayAnim(PlayerCards cards)
		{

			//var prefab = AnimationManager.BasePanel.CurrentGameUIManager.GameCardUIPrefab;
			Canvas canv = AnimationManager.BasePanel.GetComponentInParent<Canvas>();
			RectTransform centerSpawn = AnimationManager.BasePanel.CurrentGameUIManager.PlaceManager.Center;

			float time = 0.1f;
			Color shasowW = Color.white;
			Color shasowT = shasowW;
			shasowT.a = 0;

			for (int i = 0; i < cards.Instances.Count; i++)
			{
				try
				{
					int index = i;

					Vector2 targetDelta = new Vector2(cards.CardBlock.CardPositions[i].rect.width, cards.CardBlock.CardPositions[i].rect.height);

					GameCardUI instComp = cards.Instances[i];
					instComp.gameObject.SetActive(true);

					//for (int x = 0; x < _distribCards.Length; x++)
					//{
					//	if (_distribCards[x].id == cards.Distribs[i].id)
					//		instComp.Init(_distribCards[x]);
					//}

					instComp.Shadow.color = shasowW;
					instComp.ShodowMove = true;
					instComp.SetCloseState();
					RectTransform instRect = instComp.GetComponent<RectTransform>();
					instRect.rotation = Quaternion.identity;
					instRect.anchoredPosition = Vector3.zero;
					instRect.transform.SetParent(cards.CardBlock.CardPositions[i]);
					instRect.localScale = Vector2.one;
					instRect.sizeDelta = Vector3.zero;
					instRect.anchorMin = Vector2.one / 2;
					instRect.anchorMax = Vector2.one / 2;
					instRect.sizeDelta = Vector3.zero;
					//inst.transform.position = canv.transform.InverseTransformPoint(centerSpawn.TransformPoint(centerSpawn.position));
					var cg = instComp.GetComponent<CanvasGroup>();
					cg.alpha = 0;
					//inst.transform.DOMove(cards.CardBlock.CardPositions[i].position, time);
					//inst.transform.DOScale(Vector2.one, time);
					instComp.Shadow.DOColor(shasowT, time);
					instRect.DOAnchorPos(Vector2.zero, time).OnComplete(() =>
					{
						instComp.ShodowMove = false;

						if (index == cards.Distribs.Count - 1)
							cards.Player.WaitMovePlayerAnimation = false;
					});
					instRect.DOSizeDelta(targetDelta, time);
					instRect.DOLocalRotate(Vector3.zero, time);
					DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 1f, time * 0.4f);
				}
				catch (System.Exception ex)
				{
					it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				}
				await UniTask.Delay(_spawnPeriaod);
			}

			if (cards.Player.UserId == UserController.User.id)
				ShowCards(cards.Player);
		}
		private async void ShowCards(PlayerGameIcone player)
		{
			await UniTask.Delay(50);

			foreach (var elem in player.Cards)
			{
				try
				{
					elem.RotateToOpen();
				}
				catch (System.Exception ex)
				{
					it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				}
				//elem.Show();
				await UniTask.Delay(100);
			}
			await UniTask.Delay(100);
			OnComplete?.Invoke();

		}

		private class PlayerCards
		{
			public PlayerGameIcone Player;
			public PlayerGameIcone.CardGroup.CardBlock CardBlock;
			public ulong PlayerId;
			public List<DistributionCard> Distribs;
			public List<GameCardUI> Instances;
		}

	}
}