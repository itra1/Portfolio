using UGui.Screens.Elements;

using UnityEngine;

namespace UGui.Screens.Pages
{
	public class RegistrationErrorPage: MonoBehaviour
	{

		public void OnButtonTouch()
		{
			GetComponentInParent<AuthorizationScreen>().OnRegistrationErrorComplete();
		}
	}
}
