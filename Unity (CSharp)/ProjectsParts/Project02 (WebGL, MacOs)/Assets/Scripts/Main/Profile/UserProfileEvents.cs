using System.Collections;
using UnityEngine;
using TMPro;

namespace it.UI
{
	public class UserProfileEvents : MonoBehaviour
	{
		//public UnityEngine.Events.UnityAction OnExpandAction;


		public void ExpandButtonTouch()
		{
			it.Main.SinglePageController.Instance.Show(SinglePagesType.Events);
		}

		public void FilterButtonTouch(){


		}

	}
}