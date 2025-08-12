using UnityEngine;

namespace it.Popups
{
	public class RecoverySuccess : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnConfirm;
		public void LoginButton()
		{
            this.GetComponentInParent<PopupBase>().Hide();

            it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
           
		}
	}
}