using System.Collections;
using UnityEngine;

namespace it.Popups
{
	public class ConfirmForgotPassword : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnLogin;

		public void LoginButton()
		{
			OnLogin?.Invoke();
		}

		public void CloseButton()
		{
			GetComponentInParent<AuthorizationPopup>().Hide();
		}
	}
}