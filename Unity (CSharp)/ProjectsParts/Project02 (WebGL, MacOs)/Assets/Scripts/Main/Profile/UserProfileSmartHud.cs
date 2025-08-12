using System.Collections;
using UnityEngine;
using TMPro;

namespace it.UI
{
	public class UserProfileSmartHud : MonoBehaviour
	{
		public void MainTouch()
		{
			it.Main.SinglePageController.Instance.Show(SinglePagesType.SmartHud);
		}
	}
}