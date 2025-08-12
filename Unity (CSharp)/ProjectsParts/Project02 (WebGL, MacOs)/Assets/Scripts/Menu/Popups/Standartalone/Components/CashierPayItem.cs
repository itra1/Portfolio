using DG.Tweening;
using UnityEngine;
using TMPro;
using it.Network.Rest;
using it.Api;

namespace it.Popups
{
	public abstract class CashierPayItem : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnResize;

		public virtual string ProvideSlug { get; }

		[SerializeField] protected RectTransform _header;
		[SerializeField] protected RectTransform _body;
		[SerializeField] protected RectTransform _detailsPanel;
		[SerializeField] protected TextMeshProUGUI _amountSymbol;
		[SerializeField] protected TextMeshProUGUI _amountValue;
		[SerializeField] protected TMP_InputField _cardHolderValue;
		[SerializeField] protected TMP_InputField _cardNumberValue;
		[SerializeField] protected TMP_InputField _mobileValue;
		[SerializeField] protected TMP_Dropdown _mounthValue;
		[SerializeField] protected TMP_InputField _yearValue;
		[SerializeField] protected TMP_InputField _cvvValue;
		[SerializeField] protected TextMeshProUGUI _amountErrorValue;

		private RectTransform _rt;
		private bool _isExpand;
		private bool _isExpandDetails;

		protected decimal _amount;

		public bool IsExpand { get => _isExpand; set => _isExpand = value; }

		private void OnEnable()
		{
			//com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserBalanceUpdate, BalanceUpdate);

			_amountErrorValue.gameObject.SetActive(false);
				_amount = 0;
			ConfirmValue();
		}


		private void OnDisable()
		{
			//com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserBalanceUpdate, BalanceUpdate);
		}

		//private void BalanceUpdate(com.ootii.Messages.IMessage handler){
		//	_amount = 0;
		//	ConfirmValue();
		//}

		public void AddAmount(decimal increment)
		{
			_amount += increment;
			ConfirmValue();
		}

		private void ConfirmValue()
		{
			_amountValue.text = _amount.ToString();
		}

		public void ExpandButton()
		{
			if (_rt == null)
				_rt = GetComponent<RectTransform>();

			_isExpand = !_isExpand;

			if (_isExpand)
			{
				_rt.DOSizeDelta(new Vector2(_rt.sizeDelta.x, _header.rect.height + _body.rect.height), 0.3f).OnUpdate(() =>
				{
					OnResize?.Invoke();
				}).OnComplete(() =>
				{
					OnResize?.Invoke();
				});
			}
			else
			{
				_rt.DOSizeDelta(new Vector2(_rt.sizeDelta.x, _header.rect.height), 0.3f).OnUpdate(() =>
				{
					OnResize?.Invoke();
				}).OnComplete(() =>
				{
					OnResize?.Invoke();
				});
			}

		}

		public void ExpandDetailsButton()
		{
			_isExpandDetails = !_isExpandDetails;

			if (_isExpandDetails)
			{
				_detailsPanel.DOSizeDelta(new Vector2(_detailsPanel.sizeDelta.x, 85.6f), 0.3f);
			}
			else
			{
				_detailsPanel.DOSizeDelta(new Vector2(_detailsPanel.sizeDelta.x, 39f), 0.3f);
			}

		}

		public virtual void DepositeTouch()
		{

			Replenishment replenishment = new Replenishment();
			replenishment.provider = ProvideSlug;
			replenishment.amount = _amount;
			replenishment.requisites = new Requisites();
			replenishment.requisites.cardHolder = _cardHolderValue.text;
			replenishment.requisites.cardNumber = _cardNumberValue.text;
			//replenishment.Requisites.ExpireMonth = _mounthValue.value + 1;
			//replenishment.Requisites.ExpireYear = int.Parse(_yearValue.text);
			replenishment.requisites.cvv = int.Parse(_cvvValue.text);
			replenishment.requisites.phone = _mobileValue.text;

			UserApi.Deposite(replenishment, (result) =>
			{
			},
			 (error) =>
			 {
			 });

		}
	}
}