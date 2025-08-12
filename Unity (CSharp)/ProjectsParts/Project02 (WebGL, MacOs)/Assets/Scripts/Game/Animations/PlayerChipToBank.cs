using it.Network.Rest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Garilla.Games;
using it.UI;
using DG.Tweening;
using it.UI.Elements;
using static PlaceController;

namespace Garilla.Games.Animations
{
	/// <summary>
	/// Анимация полета фишек играков в банк
	/// </summary>
	[TableAnimation(TableAnimationsType.PlayerChipToBank, false, false)]
	public class PlayerChipToBank : GameAnimationsBase
	{
		private Bets _targetBet;
		private Bets _betSource;
		private float _time = 0.2f;
		public override void Init(Hashtable hashtable)
		{
			try
			{
				base.Init(hashtable);

				_betSource = (Bets)hashtable["bets"];
				//GameObject betInst = GameObject.Instantiate(_betSource.gameObject, _betSource.transform.parent);
				GameObject betInst = PoolerManager.Spawn(_betSource.gameObject.name);
				if (betInst == null)
				{
					PoolerManager.AddPool(_betSource.gameObject, 1, 1);
					betInst = PoolerManager.Spawn(_betSource.gameObject.name);
				}
				betInst.transform.SetParent(_betSource.transform.parent);
				betInst.transform.localScale = _betSource.gameObject.transform.localScale;

				_targetBet = betInst.GetComponent<Bets>();
				_betSource.gameObject.SetActive(false);
				_betSource.SetValue(0);
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				OnComplete.Invoke();
			}
		}

		public override void Play()
		{
			try
			{
				base.Play();


				RectTransform betRect = _targetBet.GetComponent<RectTransform>();
				//betRect.SetParent(AnimationManager.BasePanel.CurrentGameUIManager.BankSplit.transform.parent);
				betRect.SetParent(AnimationManager.BasePanel.CurrentGameUIManager.BankSplit.transform);
				betRect.gameObject.SetActive(true);
				betRect.anchorMax = Vector2.one / 2;
				betRect.anchorMin = Vector2.one / 2;

				//DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_GAME_BET, 1, null, 0);

				if (!AppConfig.DisableAudio)
					DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_GAME_BET, 1);
				betRect.DOAnchorPos(Vector2.zero, _time).OnComplete(() =>
				{
					//GameObject.Destroy(betRect.gameObject);
					PoolerManager.Return(_betSource.gameObject.name, betRect.gameObject);
					OnComplete.Invoke();
				});

				//CanvasGroup cg = betRect.GetComponent<CanvasGroup>();

				//if (cg == null)
				//	cg = betRect.gameObject.AddComponent<CanvasGroup>();

				//DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 0, _time / 2);


			}
			catch (System.Exception ex)
			{
				it.Logger.LogError($"{ex.Message} - {ex.StackTrace}");
				OnComplete.Invoke();
			}

		}
	}
}