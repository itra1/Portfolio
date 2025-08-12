using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Popups
{
    public class RegistrationSuccess : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnConfirm;
		public void LoginButton()
		{
			//Hide();

			OnConfirm?.Invoke();
			//it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
			//it.Main.PopupController.Instance.HidePopUps(PopupType.PasswordRecovery);
           
		}


	}
}