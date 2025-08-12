using System.Collections;
using UnityEngine;
using TMPro;
using it.Network.Rest;
using it.Api;
using Leguar.TotalJSON;
using UnityEngine.UI;
using Garilla;
using it.Helpers;

namespace it.Popups
{
	public class CashierDetailsForm : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnBack;

		[SerializeField] private it.UI.Elements.GraphicButtonUI _mainButton;
		[SerializeField] private TypeCashier _type;
		[SerializeField] private RawImage _logo;
		[SerializeField] protected CreditCardForm _creditCardForm;
		[SerializeField] protected PhoneForm _phoneForm;
		[SerializeField] protected CashierValueForm _valueForm;
		[SerializeField] protected TextMeshProUGUI _minimumValueLabel;
		[SerializeField] protected TextMeshProUGUI _maximumCalueLabel;
		[SerializeField] private TMP_InputField _cardNumberInput;
		[SerializeField] private TMP_InputField _cardMonthInput;
		[SerializeField] private TMP_InputField _cardYearInput;
		[SerializeField] private TMP_InputField _cardHolderInput;
		[SerializeField] private TMP_InputField _phoneInput;
		[SerializeField] protected TMP_InputField _walletInput;

		protected ICashierMethod _method;
		protected SelectMethod _selectedMethod;
		private Texture _textureDefault;
		private string _currency;

		protected virtual void OnEnable()
		{
			if (_mainButton != null) _mainButton.interactable = true;

			if (_cardNumberInput != null)
			{
				_cardNumberInput.text = "";
				_cardNumberInput.caretPosition = -1;
			}
			if (_cardMonthInput != null)
			{
				_cardMonthInput.text = "";
				_cardMonthInput.caretPosition = -1;
			}
			if (_cardYearInput != null)
			{
				_cardYearInput.text = "";
				_cardYearInput.caretPosition = -1;
			}
			if (_cardHolderInput != null)
			{
				_cardHolderInput.text = "";
				_cardHolderInput.caretPosition = -1;
			}
			if (_phoneInput != null)
			{
				_phoneInput.text = "";
				_phoneInput.caretPosition = -1;
			}
			if (_walletInput != null)
			{
				_walletInput.text = "";
				_walletInput.caretPosition = -1;
			}
		}

		public void PastleButtonTouch()
		{
#if UNITY_WEBGL

			Garilla.Platform.WebGL.WebGLCopyPastle.PastText(text => {
				if (_walletInput != null)
					_walletInput.text = text;
			});

#else
			if (_walletInput != null)
				_walletInput.text = UniClipboard.GetText();
#endif
		}

		public virtual void Set(SelectMethod method)
		{
			_selectedMethod = method;
			_method = method.Method;

			_currency = method.Currency;

			if (_valueForm != null)
			{
				_valueForm.MinValue = _method.MinLimit ?? 0;
				_valueForm.MaxValue = _method.MaxLimit ?? decimal.MaxValue;
				_valueForm.SetValue((double)(_method.MinLimit ?? 0));
			}
			if (_minimumValueLabel != null)
				_minimumValueLabel.gameObject.SetActive(_method.MinLimit != null);
			if (_maximumCalueLabel != null)
				_maximumCalueLabel.gameObject.SetActive(_method.MaxLimit != null);

			if (_minimumValueLabel != null && _method.MinLimit != null)
				_minimumValueLabel.text = _type == TypeCashier.Deposit
			? string.Format("popup.cashier.deposite.minimum".Localized(), (_method.MinLimit ?? 0).CurrencyString())
			: string.Format("popup.cashier.withdrawal.minimum".Localized(), (_method.MinLimit ?? decimal.MaxValue).CurrencyString());
			if (_maximumCalueLabel != null && _method.MaxLimit != null)
				_maximumCalueLabel.text = _type == TypeCashier.Deposit
			? string.Format("popup.cashier.deposite.maximum".Localized(), (_method.MaxLimit ?? 0).CurrencyString())
			: string.Format("popup.cashier.withdrawal.maximum".Localized(), (_method.MaxLimit ?? decimal.MaxValue).CurrencyString());

			if (!string.IsNullOrEmpty(method.ImageUrl))
			{
				if (_textureDefault == null)
					_textureDefault = _logo.texture;

				it.Managers.NetworkManager.Instance.RequestTexture(method.ImageUrl, (s, b) =>
				{
					_logo.texture = s;
					//_logo.GetComponent<AspectRatioFitter>().aspectRatio = (float)s.width / (float)s.height;

				}, (err) =>
				{
					_logo.texture = _textureDefault;
				});
			}
		}

		public void BackTouch()
		{
			OnBack?.Invoke();
		}

		private bool ValidateAmount()
		{
			if (_valueForm.Value <= 0)
			{
				it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("errors.forms.theAmountMustBeAtLeast1".Localized());
				return false;
			}

			if (_method != null && _method.MinLimit != null && _method.MinLimit > _valueForm.Value)
			{

				it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("errors.forms.amountLessLimit".Localized());
				return false;
			}
			if (_method != null && _method.MaxLimit != null && _method.MaxLimit < _valueForm.Value)
			{
				it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("errors.forms.amountOverLimit".Localized());
				return false;
			}

			return true;
		}

		protected Replenishment GetReplenishmentDeposite()
		{

			Replenishment replenishment = new Replenishment();
			replenishment.provider = _method.Slug;
			replenishment.amount = _valueForm.Value;
			replenishment.requisites = new Requisites();
			replenishment.IsQR = _selectedMethod.IsQR;

			if (_method.Slug == "kauri")
				replenishment.requisites.currency = _currency;

			if (_mainButton != null) _mainButton.interactable = false;

			return replenishment;
		}

		public virtual void DepositeTouch()
		{
			if (!ValidateAmount())
			{
				return;
			}
			ProcessDepositeRequest();
		}
		protected virtual void ProcessDepositeRequest()
		{
			var replenishment = GetReplenishmentDeposite();
			RequestDeposite(replenishment);
		}


		protected void RequestDeposite(Replenishment replenishment)
		{
			UserController.Instance.Cashier.RequestDeposite(replenishment, (result) =>
			{
				if (_mainButton != null) _mainButton.interactable = true;
				if (replenishment.IsQR)
				{
					try
					{
						ParceQR(result);
						return;
					}
					catch { };
				}

				string url = JSON.ParseString(result).GetString("data");

#if UNITY_WEBGL
				ConfirmPopup panel = Main.PopupController.Instance.ShowPopup<ConfirmPopup>(PopupType.Confirm);
				panel.SetDescriptionString("message.info.openUrlToProcess".Localized());
				panel.OnConfirm = () =>
				{
					Application.OpenURL(url);
				};
#else
				Application.OpenURL(url);
#endif
			});

//			UserApi.Deposite(replenishment, (result) =>
//			{
//				if (_mainButton != null) _mainButton.interactable = true;
//				try
//				{
//					ParceQR(result);
//					return;
//				}
//				catch { };

//				string url = JSON.ParseString(result).GetString("data");
//				//#if UNITY_STANDALONE
//				//				var popup = it.Main.PopupController.Instance.ShowPopup<it.Popups.BrowserPopup>(PopupType.Browser);
//				//				popup.SetUrl(url);

//				//#else

//#if UNITY_WEBGL
//				ConfirmPopup panel = Main.PopupController.Instance.ShowPopup<ConfirmPopup>(PopupType.Confirm);
//				panel.SetDescriptionString("message.info.openUrlToProcess".Localized());
//				panel.OnConfirm = () =>
//				{
//					Application.OpenURL(url);
//				};
//#else
//				Application.OpenURL(url);
//#endif

//				//#endif

//			},
//			 (error) =>
//			 {
//				 ProcessError(error);
//			 });
		}

		protected virtual void ProcessDepositeResult()
		{

		}

		protected virtual void ParceQR(string input)
		{
			throw new System.Exception("no parce qr");
			//var data = it.Helpers.ParserHelper.Parse<CashierDeposite>(JSON.ParseString(input).GetJSON("data"));
		}

		protected Replenishment GetReplenishmentWithdrawal()
		{

			Replenishment replenishment = new Replenishment();
			replenishment.provider = _method.Slug;
			replenishment.amount = _valueForm.Value;
			replenishment.IsQR = _selectedMethod.IsQR;
			replenishment.requisites = new Requisites();

			if (_method.Slug == "kauri")
				replenishment.requisites.currencyOut = _currency;

			if (_cardNumberInput != null)
				replenishment.requisites.cardNumber = _cardNumberInput.text;
			if (_cardMonthInput != null)
				replenishment.requisites.cardExpires = $"{_cardMonthInput.text}/{_cardYearInput.text}";
			if (_cardHolderInput != null)
				replenishment.requisites.cardHolder = $"{_cardHolderInput.text}";
			if (_phoneInput != null)
				replenishment.requisites.phone = $"+{_phoneInput.text}";
			if (_walletInput != null)
			{
				replenishment.requisites.walletNumber = $"{_walletInput.text}";
			}
			return replenishment;
		}

		public virtual void WithdrawalTouch()
		{
			if (!ValidateAmount())
			{
				return;
			}
			ProcessWithdrawalRequest();
		}

		protected virtual void ProcessWithdrawalRequest()
		{
			RequestWithdrawal(GetReplenishmentWithdrawal());
		}


		private void RequestWithdrawal(Replenishment replenishment)
		{
			UserController.Instance.Cashier.RequestWithdrawal(replenishment, () =>
			{
				ShowCompleteWithdrawalDialog();
			});

			//UserApi.Withdrawal(replenishment, (result) =>
			//{
			//	ShowCompleteWithdrawalDialog();
			//},
			// (error) =>
			// {
			//	 ProcessError(error);
			// });
		}




		protected void ShowCompleteWithdrawalDialog()
		{
			var popup = it.Main.PopupController.Instance.ShowPopup<it.Popups.InfoPopup>(PopupType.Info);
			popup.SetDescriptionString("message.info.withdrawalProcess".Localized());
		}

		public void TelegramTouch()
		{
			LinkManager.OpenUrl("telegramGarilaPoker");
		}

	}
}