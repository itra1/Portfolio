using UnityEngine;
using UnityEngine.UI;

public class AchiveLocationIcon : MonoBehaviour {

	public Image image;

	private int _actuvelNum;

	public void SetAchiveNum(int achiveNum) {

		if (_actuvelNum == achiveNum) return;
		_actuvelNum = achiveNum;

		//var gp = GraphicManager.Instance.link.achiveList[achiveNum];
		//image.sprite = gp.mini;
		//image.GetComponent<RectTransform>().sizeDelta = new Vector2(gp.mini.textureRect.width, gp.mini.textureRect.height);
	}

}
