using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


namespace it.Popups
{
	public class CashierTransactionItem : MonoBehaviour, ITransactionItem
	{
		[SerializeField] private CanvasGroup _body;
		[SerializeField] private it.Inputs.CurrencyLabel _valueLabel;
		[SerializeField] private RectTransform _requestIdBlock;
		[SerializeField] private RectTransform _cardBlock;
		[SerializeField] private RectTransform _dateBlock;
		[SerializeField] private TextMeshProUGUI _transactionLabel;
		[SerializeField] private TextMeshProUGUI _requestIdLabel;
		[SerializeField] private TextMeshProUGUI _statusLabel;
		[SerializeField] private TextMeshProUGUI _cardLabel;
		[SerializeField] private TextMeshProUGUI _dateLabel;
		[SerializeField] private Color _depositeArrowColor;
		[SerializeField] private Color _withdrawalArrowColor;
		[SerializeField] private Color _doneStateColor;
		[SerializeField] private Color _rejectedStateColor;
		[SerializeField] private it.UI.Elements.GraphicButtonUI _copyTransactionIdButton;
		[SerializeField] private it.UI.Elements.GraphicButtonUI _copyRequestIdButton;
		[SerializeField] private RectTransform _copyTransactionLabel;
		[SerializeField] private RectTransform _copyRequestLabel;

		private TypeRecord _type;
		private UserWalletTransaction _record;
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

		public enum TypeRecord
		{
			deposite, withdrawal
		}

		public void SetRenderer(bool isVisible)
		{
			if (_body == null) return;
			_body.alpha = isVisible ? 1 : 0;
		}

		private void OnEnable()
		{
			if (_copyTransactionLabel)
				_copyTransactionLabel.gameObject.SetActive(false);
			if (_copyRequestLabel)
				_copyRequestLabel.gameObject.SetActive(false);
		}

		public void SetData(UserWalletTransaction record)
		{
			_record = record;
			RectTransform tTr = _transactionLabel.GetComponent<RectTransform>();
			tTr.sizeDelta = new Vector2(400, tTr.sizeDelta.y);
			_transactionLabel.text = $"{"popup.cashier.transactions.transactionId".Localized()}: {_record.id}";
			tTr.sizeDelta = new Vector2(_transactionLabel.preferredWidth, tTr.sizeDelta.y);

			// Для пополнения устанавливаем статус успешной операции
			if (_statusLabel != null)
			{
				if (_record.user_wallet_transaction_type.slug == "replenishment" || _record.user_wallet_transaction_type.slug == "transfer_in")
				{
					_statusLabel.gameObject.SetActive(true);
					_statusLabel.text = $"{"popup.cashier.transactions.deposit".Localized()}: <color=#57A53C>ACCEPTED</color>";
				}
				else
				{
					_statusLabel.gameObject.SetActive(true);
					_statusLabel.text = $"{"popup.cashier.transactions.withdrawal".Localized()}: <color=#57A53C>ACCEPTED</color>";
				}
			}

			if (!string.IsNullOrEmpty(record.request_id))
			{
				_requestIdBlock.gameObject.SetActive(true);
				_requestIdLabel.text = record.request_id;
			}
			else
			{
				_requestIdBlock.gameObject.SetActive(false);
			}

			if (_copyTransactionIdButton != null)
				_copyTransactionIdButton.OnClickPointer.RemoveAllListeners();
			_copyTransactionIdButton.OnClickPointer.AddListener(() =>
			{
				UniClipboard.SetText(_record.id);
			});

			if (_copyRequestIdButton != null)
				_copyRequestIdButton.OnClickPointer.RemoveAllListeners();
			_copyRequestIdButton.OnClickPointer.AddListener(() =>
			{
				UniClipboard.SetText(_record.request_id);
			});

			_type = _record.user_wallet_transaction_type.slug == "replenishment" ? TypeRecord.deposite : TypeRecord.withdrawal;

			_valueLabel.SetValue(Mathf.Abs((float)_record.amount));
			_cardLabel.text = _record.wallet_transactionable.card_number;
			//_descriptionLabel.text = "";
			_dateLabel.text = _record.CreateAt.ToString("dd.MM.yyyy");
			//_statusLabel.text = "DONE";
			//_statusLabel.color = _doneStateColor;
			//_typeLabel.text = _type == TypeRecord.deposite ? "DEPOSIT" : "WITHDRAWAL";
			//_arrow.transform.localScale = _type == TypeRecord.deposite ? Vector3.one : new Vector3(1, -1, 1);
			//_arrow.color = _type == TypeRecord.deposite ? _depositeArrowColor : _withdrawalArrowColor;
		}

		public void TransactionIdCopyTouch()
		{
			UniClipboard.SetText(_record.id);
			StopAllCoroutines();
			StartCoroutine(CopyTransactionLabel());
		}

		private IEnumerator CopyTransactionLabel()
		{
			if (_copyTransactionLabel)
				_copyTransactionLabel.gameObject.SetActive(true);
			yield return new WaitForSeconds(1.5f);
			if (_copyTransactionLabel)
				_copyTransactionLabel.gameObject.SetActive(false);
		}
		public void RequestIdCopyTouch()
		{
			UniClipboard.SetText(_record.id);
			StopAllCoroutines();
			StartCoroutine(CopyRequestLabel());
		}

		private IEnumerator CopyRequestLabel()
		{
			if (_copyRequestLabel)
				_copyRequestLabel.gameObject.SetActive(true);
			yield return new WaitForSeconds(1.5f);
			if (_copyRequestLabel)
				_copyRequestLabel.gameObject.SetActive(false);
		}

	}
}