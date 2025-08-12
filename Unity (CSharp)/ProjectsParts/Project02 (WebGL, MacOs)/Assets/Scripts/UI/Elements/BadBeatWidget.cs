using DG.Tweening;

using Garilla.BadBeat;

using it.Api;
using it.Network.Rest;

using System.Collections;

using TMPro;

using UnityEngine;

namespace it.UI.Elements
{
	public class BadBeatWidget : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _textLabel;
		//[SerializeField] private bool _requests;

		private static decimal _value;
		private static decimal _oldValue;
		private double _timePeriaoUpdate = 60;
		private double _lastTimeGet = -15;
		private RectTransform _rt;

		private void OnEnable()
		{
			BadBeatController.Instance.OnUpdate -= UpdateData;
			BadBeatController.Instance.OnUpdate += UpdateData;
			if (Garilla.BadBeat.BadBeatController.Instance.Data != null)
			{
				SetData(Garilla.BadBeat.BadBeatController.Instance.Data.amount);
			}

			if (!ServerManager.ExistsServers)
			{
				ServerManager.OnServersSetComplete += ServerSet;
				return;
			}
			GetData();
		}
		private void OnDisable()
		{
			BadBeatController.Instance.OnUpdate -= UpdateData;
		}

		private void UpdateData(JackpotInfoResponse data)
		{
			SetData(Garilla.BadBeat.BadBeatController.Instance.Data.amount);
		}


		private void ServerSet()
		{
			ServerManager.OnServersSetComplete -= ServerSet;
			GetData();
		}

		public void Clear()
		{
			_value = 0;
			Print(_value);
		}

		public void GetData()
		{
			Garilla.BadBeat.BadBeatController.Instance.GetData((result) =>
			{
				SetData(result.amount);
			});
		}

		private void SetData(decimal data)
		{
			_lastTimeGet = Time.realtimeSinceStartupAsDouble;

			if (data != _value)
			{
				_oldValue = _value;
				_value = data;
				if (!gameObject.activeSelf)
					Print(data);
				else
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