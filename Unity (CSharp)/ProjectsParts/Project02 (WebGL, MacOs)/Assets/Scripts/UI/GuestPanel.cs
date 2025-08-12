using System.Collections;
using UnityEngine;

namespace it.Main
{
	public class GuestPanel : MonoBehaviour
	{
		[SerializeField] private it.UI.Avatar _avatar;

		private void Awake()
		{
			_avatar.SetDefaultAvatar();
		}

		public void LoginButton()
		{
			PopupController.Instance.ShowPopup(PopupType.Authorization);
		}

		public void RegisterButton()
		{
			PopupController.Instance.ShowPopup(PopupType.Registration);
		}

	}
}