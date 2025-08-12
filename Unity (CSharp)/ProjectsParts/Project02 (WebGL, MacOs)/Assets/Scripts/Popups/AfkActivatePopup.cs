using Garilla;
using it.Network.Rest;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;

namespace it.Popups
{
	/// <summary>
	/// AFK вшфдщпж
	/// </summary>
	public class AfkActivatePopup : PopupBase
	{
		[SerializeField] private RectTransform _startButtonsRect;
		[SerializeField] private RectTransform _activeButtonsRect;
		[SerializeField] private TextMeshProUGUI _minut1Label;
		[SerializeField] private TextMeshProUGUI _minut0Label;
		[SerializeField] private TextMeshProUGUI _second1Label;
		[SerializeField] private TextMeshProUGUI _second0Label;

		private TimerManager.RealTimer _timer;
		private ulong _tableId;

		protected override void EnableInit()
		{
			base.EnableInit();

			_startButtonsRect.gameObject.SetActive(false);
			_activeButtonsRect.gameObject.SetActive(false);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.MyAfkActive, MyAfkActive);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.MyAfkActive, MyAfkActive);
		}

		private void MyAfkActive(com.ootii.Messages.IMessage handle){
			Init(_tableId);
		}

		public void Init(ulong tableId)
		{
			ConfirmTimer("00:00");
			_tableId = tableId;
			_timer = null;

			Lock(true);
			TableApi.GetMyAfk(tableId, (result) =>
			{
				if (gameObject == null) return;
				Lock(false);

				if (!result.IsSuccess) return;
				if (!result.Result.SkipDistributions && !result.Result.SkipDistributionsWellBeSet)
				{
					_startButtonsRect.gameObject.SetActive(true);
					_activeButtonsRect.gameObject.SetActive(false);
					var date = System.DateTime.MinValue.AddSeconds((float)result.Result.skip_distributions_duration);
					ConfirmTimer(date.ToString("mm:ss"));
					return;
				}
				_startButtonsRect.gameObject.SetActive(false);
				_activeButtonsRect.gameObject.SetActive(true);

				if(result.Result.SkipDistributionsWellBeSet)
				{
					var date = System.DateTime.MinValue.AddSeconds((float)result.Result.skip_distributions_duration);
					ConfirmTimer(date.ToString("mm:ss"));
				}

				if(result.Result.SkipDistributions)
					ProcessTimer(result.Result);

			});

		}

		private void ProcessTimer(TablePlayerOptions result)
		{

			string timerName = StringConstants.AfkTimerName(UserController.User.id);

			_timer = (TimerManager.RealTimer)TimerManager.Instance.GetTimer(StringConstants.AfkTimerName(UserController.User.id));

			if (_timer == null)
				_timer = (TimerManager.RealTimer)TimerManager.Instance.AddTimer(timerName, result.SkipDistributionsTime);

			_timer.OnUpdate.RemoveListener(UpdateTimer);
			_timer.OnUpdate.AddListener(UpdateTimer);
			_timer.OnComplete.RemoveListener(TimerComplete);
			_timer.OnComplete.AddListener(TimerComplete);
			//UpdateTimer();

		}

		private void TimerComplete(bool result){
			Hide();
		}

		private void UpdateTimer()
		{
			if (_timer == null || _timer.TimeLeftDouble < 0) return;
			//var date = System.DateTime.FromOADate(_timer.TimeLeftDouble);
			//var date = System.DateTime.MinValue.AddSeconds(_timer.TimeLeftDouble);
			var timespan = System.TimeSpan.FromSeconds(_timer.TimeLeftDouble);
			ConfirmTimer(string.Format("{0:D2}:{1:D2}", (int)_timer.TimeLeftDouble/60, (int)_timer.TimeLeftDouble % 60));
		}

		private void ConfirmTimer(string dateString)
		{
			_minut1Label.text = dateString.Substring(0, 1);
			_minut0Label.text = dateString.Substring(1, 1);
			_second1Label.text = dateString.Substring(3, 1);
			_second0Label.text = dateString.Substring(4, 1);
		}

		public void ApplayButtonTouch()
		{
			Lock(true);
			TableApi.StartMyAfk(_tableId, (result) =>
			{
				Lock(false);
				if (result.IsSuccess)
				{
					_startButtonsRect.gameObject.SetActive(!result.Result.SkipDistributions && !result.Result.SkipDistributionsWellBeSet);
					_activeButtonsRect.gameObject.SetActive(result.Result.SkipDistributions || result.Result.SkipDistributionsWellBeSet);
					if (result.Result.SkipDistributions)
					{
						ProcessTimer(result.Result);
					}
				}
				else
				{
					_startButtonsRect.gameObject.SetActive(true);
					_activeButtonsRect.gameObject.SetActive(false);
				}
				com.ootii.Messages.MessageDispatcher.SendMessage(this,EventsConstants.MyAfkChange, result.Result,0.1f);
			});
		}

		public void CancelButtonTouch()
		{
			Hide();
		}

		public void ImBackButtonTouch()
		{
			Lock(true);
			TableApi.StopMyAfk(_tableId, (result) =>
			{
				Lock(false);
				if (result.IsSuccess)
				{
					Hide();
				}

				com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.MyAfkChange, result.Result, 0.1f);
			});
		}

	}
}