using System.Collections;
using UnityEngine;

namespace it.Popups
{
	public class ExitPopup : PopupBase
	{
		public UnityEngine.Events.UnityAction OnOk;

		public void OkTouch(){
			OnOk?.Invoke();
			Hide();
		}

	}
}