using UnityEngine;
using TMPro;

namespace it.Widgets
{
	public class PromotionWidget : MonoBehaviour
	{

	public void OnClick(){
			it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
	}
	}
}