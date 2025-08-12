using System.Collections;
using UnityEngine;

namespace it.Popups
{
	/// <summary>
	/// Окно подтверждения окна Фолд
	/// </summary>
	public class FoldActionPopup : PopupBase
	{
		public UnityEngine.Events.UnityAction OnOk;
		public UnityEngine.Events.UnityAction OnCancel;

		public void CloseButton()
		{
			OnCancel?.Invoke();
			Hide();
		}

		public void OkButton()
		{
			OnOk?.Invoke();
			Hide();
		}
	}
}