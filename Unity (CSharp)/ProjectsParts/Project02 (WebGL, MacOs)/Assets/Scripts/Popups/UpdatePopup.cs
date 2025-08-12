using System.Collections;

using UnityEngine;
using Garilla.Update;

namespace it.Popups
{
	public class UpdatePopup : PopupBase
	{
		/// <summary>
		/// Событие кнопки выхода
		/// </summary>
		public void ExitButtonTouch()
		{
			UpdateController.Instance.ExitApp();
		}

		/// <summary>
		/// событие кнопки обновлени
		/// </summary>
		public void UpdateButtonTouch()
		{
			UpdateController.Instance.UpdateApp();
		}

	}
}