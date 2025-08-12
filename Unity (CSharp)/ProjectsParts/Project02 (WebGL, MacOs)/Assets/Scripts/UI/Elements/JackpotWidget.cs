using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using it.Api;
using Garilla.Jackpot;

namespace it.UI.Elements
{
	public class JackpotWidget : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _textLabel;
		[SerializeField] private bool _requests;

		private static decimal _value;
		private static decimal _oldValue;
		private double _timePeriaoUpdate = 60;
		private double _lastTimeGet = -15;
		private RectTransform _rt;

		private void OnEnable()
		{
			GetJackpot();
			//com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, UserLogin);
			Print(_value);
		}

		private void OnDisable()
		{
			//com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserLogin, UserLogin);
		}

		public void ClickTouch()
		{
			PlayerPrefs.SetString(StringConstants.BUTTON_JACKPOT, "");
#if UNITY_STANDALONE
			StandaloneController.Instance.FocusMain();
#endif
		}

		//private void UserLogin(com.ootii.Messages.IMessage handle)
		//{

		//}

		private void Update()
		{
			if (!UserController.IsLogin) return;

			if (_requests)
			{
				if (_lastTimeGet + _timePeriaoUpdate > Time.realtimeSinceStartupAsDouble) return;
				GetJackpot();
			}
			else
			{
				if (_lastTimeGet + 2 > Time.realtimeSinceStartupAsDouble) return;
				GetFromPrefs();
			}

		}

		private void GetFromPrefs()
		{

			_lastTimeGet = Time.realtimeSinceStartupAsDouble;
			decimal newValue = decimal.Parse(PlayerPrefs.GetString("JackpotValue", "0"));

			if (newValue != _value)
			{
				SetData(newValue);
			}

		}

		private void GetJackpot()
		{
			_lastTimeGet = Time.realtimeSinceStartupAsDouble;

			if (!ServerManager.ExistsServers) return;

			JackpotController.Instance.RequestData(jackpot =>
			{
				PlayerPrefs.SetString("JackpotValue", jackpot.amount.ToString());
				SetData(jackpot.amount);
			});

			//AppApi.GetJackpot((result) =>
			//{
			//	if (result.IsSuccess)
			//	{
			//		PlayerPrefs.SetString("JackpotValue", result.Result.amount.ToString());
			//		SetData(result.Result.amount);
			//	}
			//	else
			//		_lastTimeGet = Time.realtimeSinceStartupAsDouble + 0.8f;
			//});
		}

		private void SetData(decimal data)
		{
			_lastTimeGet = Time.realtimeSinceStartupAsDouble;

			if (data != _value)
			{
				_oldValue = _value;
				_value = data;
				StartCoroutine(Output());
			}
		}

		private IEnumerator Output()
		{
			if (_rt == null)
				_rt = _textLabel.GetComponent<RectTransform>();


			_rt.DOScale(Vector3.one * 1.3f, 0.2f);

			decimal delta = _value - _oldValue;

			decimal increment = delta / 20;

			float sign = System.Math.Sign(delta);

			while (_oldValue != _value)
			{
				yield return null;

				_oldValue += increment;

				if (sign > 0 && _oldValue > _value)
					_oldValue = _value;
				if (sign < 0 && _oldValue < _value)
					_oldValue = _value;
				Print(_oldValue);
			}
			_rt.DOScale(Vector3.one * 1f, 0.2f);
		}

		private void Print(decimal val)
		{

			decimal cell = System.Math.Floor(val);
			decimal prop = val - cell;

			_textLabel.text = $"{StringConstants.CURRENCY_SYMBOL}{it.Helpers.Currency.String(cell, false)}.<size=50%>{System.Math.Floor(prop * 100)}";

		}

	}
}