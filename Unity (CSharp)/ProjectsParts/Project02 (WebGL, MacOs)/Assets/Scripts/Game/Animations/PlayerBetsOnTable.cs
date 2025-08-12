using it.Network.Rest;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using it.UI;
using static PlaceController;

namespace Garilla.Games.Animations
{
	/// <summary>
	/// Анимация полета фишек от пользователя на стол
	/// </summary>
	[TableAnimation(TableAnimationsType.PlayerBetsOnTable, false,false)]
	public class PlayerBetsOnTable : GameAnimationsBase
	{
		private int place;
		private it.UI.Elements.Bets _bet;
		private RectTransform _playerPosition;
		private DistributionEvent _evnt;
		private Table _table;
		private PlayerGameIcone _player;
		private float _time = 0.15f;

		public override void Init(Hashtable hashtable)
		{
			try
			{
				base.Init(hashtable);
				_evnt = (DistributionEvent)hashtable["event"];
				_table = (Table)hashtable["table"];

				// получаем позицию игрока
				foreach (var key in AnimationManager.BasePanel.CurrentGameUIManager.Players.Keys)
				{
					if (AnimationManager.BasePanel.CurrentGameUIManager.Players[key].UserId == (ulong)_evnt.user_id)
					{
						_player = AnimationManager.BasePanel.CurrentGameUIManager.Players[key];
						_playerPosition = AnimationManager.BasePanel.CurrentGameUIManager.Players[key].GetComponent<RectTransform>();
						place = key;
					}
				}

				// получаем целевой бит
				_bet = AnimationManager.BasePanel.CurrentGameUIManager.Bets[place];
				_bet.WaitChange = true;
				_bet.SetTable(_table);
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

				_time = 0.2f;

				//GameObject betItem = GameObject.Instantiate(_bet.gameObject, _bet.transform.parent);
				GameObject betItem = PoolerManager.Spawn(_bet.gameObject.name);
				if (betItem == null)
				{
					PoolerManager.AddPool(_bet.gameObject, 1, 1);
					betItem = PoolerManager.Spawn(_bet.gameObject.name);
				}
				betItem.transform.SetParent(_bet.transform.parent);
				betItem.transform.localScale = _bet.gameObject.transform.localScale;
				RectTransform sourceRect = _bet.gameObject.GetComponent<RectTransform>();
				RectTransform targetRect = betItem.gameObject.GetComponent<RectTransform>();
				targetRect.anchorMin = sourceRect.anchorMin;
				targetRect.anchorMax = sourceRect.anchorMax;
				targetRect.pivot = sourceRect.pivot;
				targetRect.anchoredPosition = sourceRect.anchoredPosition;
				targetRect.sizeDelta = sourceRect.sizeDelta;

				betItem.gameObject.SetActive(true);
				var bt = betItem.GetComponent<it.UI.Elements.Bets>();
				bt.VisibleValue = false;
				bt.SetValue(_table, (decimal)_evnt.bank_amount_delta);

				RectTransform betRect = betItem.GetComponent<RectTransform>();
				betRect.SetParent(_playerPosition.parent);
				betRect.anchoredPosition = Vector2.zero;
				betRect.SetParent(_bet.transform.parent);
				if (!AppConfig.DisableAudio)
					DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_GAME_BET, 1);
				betRect.DOAnchorPos(_bet.GetComponent<RectTransform>().anchoredPosition, _time).OnComplete(() =>
				{
					_bet.gameObject.SetActive(true);
					_bet.SetValue(AnimationManager.BasePanel.CurrentGameUIManager._currentSharedData.StageDistribToPreflop() ? _player.ShredDataPlayer.bet : _player.ShredDataPlayer.BetInRound);
					_bet.WaitChange = false;
					//GameObject.Destroy(betItem);
					PoolerManager.Return(_bet.gameObject.name, betItem.gameObject);
					OnComplete?.Invoke();
				});
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				OnComplete?.Invoke();
			}

		}

	}
}