using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

using it.Helpers;

namespace it.UI.Elements
{
	public class BalanceWidget : MonoBehaviour
	{
		[SerializeField] private RectTransform _addIcone;
		[SerializeField] private TextMeshProUGUI _valueLabel;

		private RectTransform _rt;
		private RectTransform _rtLabel;
		private float _xIncrement = 150;

		private void Awake()
		{
			_rt = GetComponent<RectTransform>();
			CashierVisibleChange(null);

		}

		private void Start()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserBalanceUpdate, UserBalanceUpdate);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, UserBalanceUpdate);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.CashierVisibleChange, CashierVisibleChange);
		}

		private void OnDestroy()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserBalanceUpdate, UserBalanceUpdate);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserLogin, UserBalanceUpdate);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.CashierVisibleChange, CashierVisibleChange);
		}

		private void OnEnable()
		{
			ConfirmValue();
		}
		private void CashierVisibleChange(com.ootii.Messages.IMessage handler)
		{
#if UNITY_IOS
			_xIncrement = AppConfig.ActiveCashier ? 150 : 70;

			_addIcone?.gameObject.SetActive(AppConfig.ActiveCashier);
#endif
		}

		private void UserBalanceUpdate(com.ootii.Messages.IMessage handler)
		{
			ConfirmValue();
		}

		public void OpenCashierTouch()
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.Cashier);
		}

		private void ConfirmValue()
		{
			_valueLabel.text = StringConstants.CURRENCY_SYMBOL_GOLD + UserController.Balance.CurrencyString(false);
			if (_rtLabel == null)
				_rtLabel = _valueLabel.GetComponent<RectTransform>();
			_rtLabel.sizeDelta = new Vector2(_valueLabel.preferredWidth, _rtLabel.sizeDelta.y);
			_rt.sizeDelta = new Vector2(_rtLabel.sizeDelta.x + _xIncrement, _rt.sizeDelta.y);
		}

	}
}