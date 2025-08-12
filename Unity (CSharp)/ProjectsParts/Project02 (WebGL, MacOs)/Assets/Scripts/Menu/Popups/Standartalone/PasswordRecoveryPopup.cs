using UnityEngine;

namespace it.Popups
{
	public class PasswordRecoveryPopup : PopupBase
	{
		[SerializeField] private ForgotPasswordForm _forgotPasswordForm;
		[SerializeField] private RecoveryConfirm _recoveryConfirm;
		[SerializeField] private RestorePassword _passwordChange;
		[SerializeField] private RecoverySuccess _recoverySuccess;

		protected override void EnableInit()
		{
			base.EnableInit();
			_forgotPasswordForm.gameObject.SetActive(true);
			_recoveryConfirm.gameObject.SetActive(false);
			_passwordChange.gameObject.SetActive(false);
			_recoverySuccess.gameObject.SetActive(false);

			_forgotPasswordForm.OnRecovery = (email) =>
			{
				//  _forgotPasswordForm.Set
				_forgotPasswordForm.gameObject.SetActive(false);
				_recoveryConfirm.gameObject.SetActive(true);
				_recoveryConfirm.SetEmail(email);
			};
			_recoveryConfirm.OnRecovery = (email, code) =>
			{
				_recoveryConfirm.gameObject.SetActive(false);
				_passwordChange.gameObject.SetActive(true);
				_passwordChange.SetData(email, code);

			};
			_passwordChange.OnConfirm = () =>
			{
				_passwordChange.gameObject.SetActive(false);
				_recoverySuccess.gameObject.SetActive(true);
			};
			_recoverySuccess.OnConfirm = () =>
			{
				// _forgotPasswordForm.gameObject.SetActive(true);
				_recoverySuccess.gameObject.SetActive(false);
			};

		}

		public void BackToAuth()
		{
			Hide();

            it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
		

		}

	}
}