using System.Collections;
using UnityEngine;
using TMPro;
 

namespace it.UI
{
	public class UserProfilePasswordChange : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _lastChangeLabel;


		private void OnEnable()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
			ConfirmData();
		}

		public void ChangePasswordTouch()
		{
			//#if UNITY_ANDROID || UNITY_WEBGL
			//			it.Main.PopupController.Instance.ShowPopup(PopupType.PasswordRecovery);
			//#endif
			//#if UNITY_STANDALONE
			//            it.Main.PopupController.Instance.ShowPopup(PopupType.PasswordChange);
			//#endif

			it.Main.PopupController.Instance.ShowPopup(PopupType.PasswordChange);
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, UserProfileUpdate);
		}

		private void UserProfileUpdate(com.ootii.Messages.IMessage hendle)
		{
			ConfirmData();
		}

		private void ConfirmData()
		{
			_lastChangeLabel.text = $"{"profile.passwordChange.lastChange".Localized()}\n" + (!string.IsNullOrEmpty(UserController.User.password_change_at)
			? System.DateTime.Parse(UserController.User.password_change_at).ToString("dd.MM.yy")
			: System.DateTime.Parse(UserController.User.created_at).ToString("dd.MM.yy"));
		}
	}
}