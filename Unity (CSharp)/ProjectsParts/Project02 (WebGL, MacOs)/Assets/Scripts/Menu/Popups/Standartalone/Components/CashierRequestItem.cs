using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using System;
using System.Windows;
using I2.Loc;
using it.Api;

namespace it.Popups
{
	public class CashierRequestItem : MonoBehaviour, ITransactionItem
	{
		public UnityEngine.Events.UnityAction OnCanceled;

		[SerializeField] private CanvasGroup _body;
		[SerializeField] private List<StatusTypes> _statusTypes;
		[SerializeField] private it.Inputs.CurrencyLabel _valueLabel;
		[SerializeField] private TextMeshProUGUI _transactionLabel;
		[SerializeField] private TextMeshProUGUI _statusTypeLabel;
		[SerializeField] private TextMeshProUGUI _statusLabel;
		[SerializeField] private TextMeshProUGUI _descriptionLabel;
		[SerializeField] private RectTransform _cardBlock;
		[SerializeField] private TextMeshProUGUI _cardLabel;
		[SerializeField] private RectTransform _dateBlock;
		[SerializeField] private TextMeshProUGUI _dateLabel;
		[SerializeField] private it.UI.Elements.GraphicButtonUI _cancelButton;
		[SerializeField] private it.UI.Elements.GraphicButtonUI _copyRequestIdButton;
		[SerializeField] private RectTransform _copyLabel;

		private UserRequestTransaction _record;

		private RectTransform _rt;
		public RectTransform RectRt
		{
			get
			{
				if (_rt == null)
					_rt = GetComponent<RectTransform>();
				return _rt;
			}
		}

		[System.Serializable]
		public class StatusTypes
		{
			public string Slug;
			public Color Color;
			public string Value;
			public string Description;
			public bool IsCanceled;
		}

		private void OnEnable()
		{
			if (_copyLabel)
				_copyLabel.gameObject.SetActive(false);
		}

		public void SetRenderer(bool isVisible)
		{
			if (_body == null) return;
			_body.alpha = isVisible ? 1 : 0;
		}

		public void SetData(UserRequestTransaction record)
		{
			_record = record;

			RectTransform tTr = _transactionLabel.GetComponent<RectTransform>();
			//tTr.sizeDelta = new Vector2(400, tTr.sizeDelta.y);
			_transactionLabel.text = $"{"popup.cashier.requests.requestId".Localized()}: {_record.id}";
			tTr.sizeDelta = new Vector2(_transactionLabel.preferredWidth, tTr.sizeDelta.y);


			_valueLabel.SetValue(Mathf.Abs((float)_record.euro_amount));
			_dateLabel.text = _record.CreateAt.ToString("dd.MM.yyyy");
			_cardLabel.text = _record.card_number;

			var status = _statusTypes.Find(x => x.Slug == record.status);

			if (status == null)
			{
				it.Logger.LogError("Server error");
				return;
			}

			if (_statusTypeLabel != null)
			{
				_statusTypeLabel.gameObject.SetActive(true);
				if (_record.status == "wait")
				{
					_statusTypeLabel.text = $"{"popup.cashier.transactions.status".Localized()}:";
				}
				else
				{
					_statusTypeLabel.text = $"{"popup.cashier.transactions.withdrawal".Localized()}:";
				}
				var statusTitleRect = _statusTypeLabel.GetComponent<RectTransform>();
				statusTitleRect.sizeDelta = new Vector2(_statusTypeLabel.preferredWidth, statusTitleRect.sizeDelta.y);
			}

			_statusLabel.text = I2.Loc.LocalizationManager.GetTermTranslation(status.Value).ToUpper();
			_statusLabel.color = status.Color;
			RectTransform dTr = _descriptionLabel.GetComponent<RectTransform>();
			//dTr.sizeDelta = new Vector2(400, dTr.sizeDelta.y);
			_descriptionLabel.text = I2.Loc.LocalizationManager.GetTermTranslation(status.Description);
			dTr.sizeDelta = new Vector2(_descriptionLabel.preferredWidth, tTr.sizeDelta.y);
			bool isActiveCancel = status.IsCanceled;
			_cancelButton.gameObject.SetActive(isActiveCancel);

			if (isActiveCancel)
			{
				_cancelButton.OnClick.RemoveAllListeners();
				_cancelButton.OnClick.AddListener(() =>
				{
					OnCanceled?.Invoke();
				});
			}
		}

		public void CanchelButtonTouch()
		{
			OnCanceled?.Invoke();
		}

		public void TransactionIdCopyTouch()
		{
			UniClipboard.SetText(_record.id);
			StopAllCoroutines();
			StartCoroutine(CopyLabel());
		}

		private IEnumerator CopyLabel()
		{
			if (_copyLabel)
				_copyLabel.gameObject.SetActive(true);
			yield return new WaitForSeconds(1.5f);
			if (_copyLabel)
				_copyLabel.gameObject.SetActive(false);
		}

		public void CancelTouch()
		{
			//OnCanceled?.Invoke();
		}

	}
}