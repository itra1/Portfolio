using it.Api;
using it.Network.Rest;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Popups;

public class CashierController : MonoBehaviour
{
	public CashierMethods CashierMethods { get => _cashierMethods; set => _cashierMethods = value; }
	public List<UserRequestTransaction> RequestTransactions { get => _requestTransactions; set => _requestTransactions = value; }
	public int RequestNoVisible { get; set; } = 0;

	private CashierMethods _cashierMethods;
	private List<UserRequestTransaction> _requestTransactions;


	public void GetCashierMethods()
	{
		UserApi.CashierMethods((result) =>
		{
			if (result.IsSuccess)
				_cashierMethods = result.Result;
		});
	}

	private Tween _requestsRepeatTween;
	public void GetPaymentRequest(UnityEngine.Events.UnityAction onComplete = null)
	{
		if (_requestsRepeatTween != null)
			_requestsRepeatTween.Kill();

		UserApi.GetPaymentRequests((result) =>
		{
			_requestTransactions = result;

			RequestNoVisible = 0;

			for (int i = 0; i < _requestTransactions.Count; i++)
				if (_requestTransactions[i].has_unseen_change)
					RequestNoVisible++;

			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.UserRequestUpdate);

			_requestsRepeatTween = DOVirtual.DelayedCall(60 * 5, () =>
			{
				GetPaymentRequest(null);
			});
			onComplete?.Invoke();
		},
			(error) =>
			{
			});
	}

	public void KillRequestsProcess()
	{
		if (_requestsRepeatTween != null)
			_requestsRepeatTween.Kill();
	}

	public void RequestDeposite(Replenishment replenishment, UnityEngine.Events.UnityAction<string> onComplete)
	{

		UserApi.Deposite(replenishment, (result) =>
		{
			onComplete?.Invoke(result);
		},
		 (error) =>
		 {
			 ProcessError(error);
		 });
	}



	public void RequestWithdrawal(Replenishment replenishment, UnityEngine.Events.UnityAction onComplete)
	{
		UserApi.Withdrawal(replenishment, (result) =>
		{
			if (result.IsSuccess)
				onComplete?.Invoke();
			else
				ProcessError(result.ErrorMessage);
		});
	}


	private void ProcessError(string error)
	{
		try
		{
			var errors = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(error);

			if (errors.errors.Length <= 0) return;

			if (errors.errors[0].id == "max_active_requests_limit_reached")
			{
				it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("errors.forms.maxActiveRequestsLimitReached".Localized());
				return;
			}

			if (errors.errors[0].id == "amount" && errors.errors[0].title == "The amount must be at least 1.")
			{
				it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("errors.forms.theAmountMustBeAtLeast1".Localized());
				return;
			}

			if (errors.errors[0].id == "amount" && errors.errors[0].title == "The amount must not be greater than 10000.")
			{
				it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("errors.forms.theAmountMustBeGreaterAt10000".Localized());
				return;
			}

			if (errors.errors[0].id == "invalid_requisites" && errors.errors[0].title == "errors.invalid_requisites")
			{
				it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("errors.forms.fullFill".Localized());
				return;
			}

			if (errors.errors[0].status >= 500)
			{
				it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("errors.data.processDataError".Localized());
				return;
			}

		}
		catch { }
	}

}