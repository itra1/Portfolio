using System.Collections;
using UnityEngine;

namespace it.UI
{
	public class UserProfilePokerStatistic : MonoBehaviour
	{
		public void ExpandButtonTouch()
		{
			it.Main.SinglePageController.Instance.Show(SinglePagesType.PokerStatistic);
		}

		public void FilterButtonTouch()
		{


		}
	}
}