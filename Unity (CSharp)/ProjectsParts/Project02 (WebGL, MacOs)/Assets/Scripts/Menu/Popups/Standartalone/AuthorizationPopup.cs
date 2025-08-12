using UnityEngine;

namespace it.Popups
{
	public class AuthorizationPopup : PopupBase
	{
		[SerializeField] private AuthorizationForm _authForm;


		protected override void EnableInit()
		{
			base.EnableInit();
			_authForm.gameObject.SetActive(true);

			_authForm.OnLoginConfirm = () =>
			{
				Hide();
				//it.Main.PopupController.Instance.ShowPopup(PopupType.Welcome);
			};

			_authForm.OnSign = () =>
			{
				Hide();
				it.Main.PopupController.Instance.ShowPopup(PopupType.Registration);
			};

			_authForm.OnForgot = () =>
			{
				Hide();
				it.Main.PopupController.Instance.ShowPopup(PopupType.PasswordRecovery);
			};

		}

	}

}