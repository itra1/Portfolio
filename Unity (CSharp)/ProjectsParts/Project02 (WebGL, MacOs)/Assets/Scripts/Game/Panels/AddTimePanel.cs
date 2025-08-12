using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Network.Rest;
using System;

namespace it.Game.Panels
{
	public class AddTimePanel : MonoBehaviour
	{
		[SerializeField] private GameObject _addTimeButton;
		[SerializeField] private TextMeshProUGUI _valueLabel;

		private DistributionSharedData _sharedData;

		private Coroutine _corTimer;
		private Table _tableId;

		private void OnEnable()
		{
			_addTimeButton.gameObject.SetActive(false);
		}

		public void SetData(bool isMe, Table tableId, DistributionSharedData distrib, DistributionEvent action)
		{
			_valueLabel.text = "+" + tableId.action_time;
			_sharedData = distrib;
			_tableId = tableId;

			if (!isMe)
			{
				if (_corTimer != null)
					StopCoroutine(_corTimer);
				_addTimeButton.gameObject.SetActive(false);
				return;
			}

			if (action != null)
			{
				if (!string.IsNullOrEmpty(action.calltime_at))
				{
					var endTime = DateTime.Parse(distrib.active_event.calltime_at);
					var diff = (endTime - GameHelper.NowTime).TotalSeconds;
					if (diff > 0)
					{
						if (_corTimer != null)
							StopCoroutine(_corTimer);
						_corTimer = StartCoroutine(WaitCor(endTime));
					}
					else
					{
						_addTimeButton.gameObject.SetActive(false);
					}
				}
				else
				{
					it.Logger.Log("active_event.calltime_at == null");
				}
			}
			else
			{
				if (_corTimer != null)
					StopCoroutine(_corTimer);
				_addTimeButton.gameObject.SetActive(false);
			}
		}


		public void AddButtonTouch()
		{
			_addTimeButton.gameObject.SetActive(false);
			if (_corTimer != null)
				StopCoroutine(_corTimer);
			TableApi.AddActionTime(_tableId.id, (result) =>
			{
				_addTimeButton.gameObject.SetActive(false);
			});
		}

		public void Clear()
		{
			StopAllCoroutines();
			_addTimeButton.gameObject.SetActive(false);
		}

		IEnumerator WaitCor(DateTime targetTime)
		{
			while (true)
			{
				var diff = (targetTime - GameHelper.NowTime).TotalSeconds;

				if (diff <= 10)
				{
					_addTimeButton.gameObject.SetActive(true);
				}
				else
					_addTimeButton.gameObject.SetActive(false);

				yield return new WaitForSeconds(1);

			}

		}
	}
}