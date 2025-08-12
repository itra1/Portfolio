using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using it.Managers;
using it.UI.Elements;

namespace it.Popups
{
	public class PasswordChangeSuccessfully : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnConfirm;

		public void OkButtonTouch(){
			OnConfirm.Invoke();
		}

	}
}