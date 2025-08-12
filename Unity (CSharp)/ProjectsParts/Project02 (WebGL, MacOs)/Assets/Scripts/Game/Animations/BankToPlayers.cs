using it.Network.Rest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Garilla.Games;
using it.UI;
using DG.Tweening;
using System.Linq;
using it.UI.Elements;

namespace Garilla.Games.Animations
{
	[TableAnimation(TableAnimationsType.BankToPlayers, false, false)]
	public class BankToPlayers : GameAnimationsBase
	{
		public List<Item> _items = new List<Item>();
		public class Item
		{
			public ulong Id;
			public decimal Value;
		}

		public override void Init(Hashtable hashtable)
		{
			try
			{
				base.Init(hashtable);
				List<DistributionEvent> events = (List<DistributionEvent>)hashtable["events"];

				for (int i = 0; i < events.Count; i++)
				{
					if (events[i].distribution_event_type.slug == "transfer-of-winnings" || events[i].distribution_event_type.slug == "rake-payout")
					{
						if (!_items.Exists(x => x.Id == (ulong)events[i].user_id))
							_items.Add(new Item() { Id = (ulong)events[i].user_id, Value = 0 });

						var itm = _items.Find(x => x.Id == (ulong)events[i].user_id);

						itm.Value += System.Math.Abs((decimal)events[i].bank_amount_delta);
					}
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

			float time = 0.2f;
			AnimationManager.BasePanel.CurrentGameUIManager.BankSplit.Clear();

			for (int i = 0; i < _items.Count; i++)
			{
				try
				{
					var itn = _items[i];
					int place = 0;

					foreach (var key in AnimationManager.BasePanel.CurrentGameUIManager.Players.Keys)
						if (AnimationManager.BasePanel.CurrentGameUIManager.Players[key].UserId == itn.Id)
							place = key;

					var btSource = AnimationManager.BasePanel.CurrentGameUIManager.Bets[place];
					if(btSource == null){
						continue;
					}
					//GameObject inst = GameObject.Instantiate(btSource.gameObject, btSource.transform.parent);

					GameObject inst = PoolerManager.Spawn(btSource.gameObject.name);
					if (inst == null)
					{
						PoolerManager.AddPool(btSource.gameObject, 1, 1);
						inst = PoolerManager.Spawn(btSource.gameObject.name);
					}
					inst.transform.SetParent(btSource.transform.parent);
					inst.transform.localScale = btSource.gameObject.transform.localScale;
					RectTransform sourceRect = btSource.gameObject.GetComponent<RectTransform>();
					RectTransform targetRect = inst.gameObject.GetComponent<RectTransform>();
					targetRect.anchorMin = sourceRect.anchorMin;
					targetRect.anchorMax = sourceRect.anchorMax;
					targetRect.pivot = sourceRect.pivot;
					targetRect.anchoredPosition = sourceRect.anchoredPosition;
					targetRect.sizeDelta = sourceRect.sizeDelta;

					inst.gameObject.SetActive(true);
					Bets instBet = inst.GetComponent<Bets>();
					instBet.VisibleValue = false;
					RectTransform rt = inst.GetComponent<RectTransform>();
					rt.SetParent(AnimationManager.BasePanel.CurrentGameUIManager.BankSplit.transform);
					rt.anchoredPosition = Vector2.zero;
					rt.SetParent(btSource.transform.parent);

					if(!AppConfig.DisableAudio)
					DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_GAME_WIN, 1, null, 0);

					rt.DOAnchorPos(btSource.GetComponent<RectTransform>().anchoredPosition, time).OnComplete(() =>
					{
						//GameObject.Destroy(rt.gameObject);
						PoolerManager.Return(btSource.gameObject.name, rt.gameObject);
						btSource.SetValue(itn.Value);
						btSource.gameObject.SetActive(true);

						DOVirtual.DelayedCall(1, () =>
						{
							btSource.gameObject.SetActive(false);
						});
					});
				}
				catch (System.Exception ex)
				{
					it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				}
			}

		}

	}
}