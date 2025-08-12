using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class GraphicText : MonoBehaviour {

	public Image icon;
	public float iconDiff;

	public Image prefab;
	public float offsetSybmol;
	private string _text;
	private List<Image> _prefabList = new List<Image>();
	//public GameDesign.AlphabetType alphabetType;
	public Aling align;

	public enum Aling {
		left, center, right
	}

	private void Awake() {
		if(_prefabList.Count == 0)
			_prefabList.Add(prefab);
	}

	public void SetValue(int textValue) {
		SetValue(textValue.ToString());
	}

	public void SetValue(string textValue) {


		if (_prefabList.Count == 0)
			_prefabList.Add(prefab);


		_text = textValue;

		if (_prefabList.Count < _text.Length)
			AddInstances(_text.Length - _prefabList.Count);

		ConfirmGraphic();

		if (align == Aling.center) Centralize();
	}

	void AddInstances(int count) {

		for (int i = 0; i < count; i++) {
			Image newInst = Instantiate(prefab);
			newInst.transform.SetParent(prefab.transform.parent);
			newInst.transform.localScale = prefab.transform.localScale;
			newInst.rectTransform.anchoredPosition = prefab.rectTransform.anchoredPosition;
			_prefabList.Add(newInst);
		}

	}

	void ConfirmGraphic() {
		_prefabList.ForEach(x=>x.gameObject.SetActive(false));

		float offset = -GetComponent<RectTransform>().rect.width / 2;

		if (icon != null) {
			float iconWidth = icon.rectTransform.rect.width;
			icon.rectTransform.anchoredPosition = new Vector2(offset + iconWidth/2, icon.rectTransform.anchoredPosition.y);
			offset += iconWidth + iconDiff;
		}
		
		for (int i = 0; i < _text.Length; i++) {
			Image newInst = _prefabList[i];
			//newInst.sprite = GameDesign.Instance.GetAlphaSprite(_text[i].ToString(), alphabetType);
			newInst.rectTransform.sizeDelta = new Vector2(newInst.rectTransform.sizeDelta.y / newInst.sprite.rect.height * newInst.sprite.rect.width, newInst.rectTransform.sizeDelta.y);

			offset += (newInst.rectTransform.sizeDelta.x / 2 + offsetSybmol) * (align  == Aling.right? -1:1) ;
			newInst.rectTransform.anchoredPosition = new Vector2(offset, _prefabList[0].rectTransform.anchoredPosition.y);
			newInst.gameObject.SetActive(true);

			offset += (newInst.rectTransform.sizeDelta.x / 2) * (align == Aling.right ? -1 : 1);
		}


	}
	
	void Centralize() {

		float allWhight = 0;
		Image last = _prefabList.FindLast(x => x.gameObject.activeInHierarchy);

		if (last == null) return;
		
		allWhight = last.rectTransform.anchoredPosition.x - _prefabList[0].rectTransform.anchoredPosition.x;

		if (icon != null) {
			allWhight += icon.rectTransform.rect.width + iconDiff;
		}

		float diff = -allWhight / 2 - _prefabList[0].rectTransform.anchoredPosition.x;

		if (diff != 0) {
			if (icon != null) {
				icon.rectTransform.anchoredPosition = new Vector2(icon.rectTransform.anchoredPosition.x + diff, icon.rectTransform.anchoredPosition.y);
			}
			_prefabList.ForEach(x => x.rectTransform.anchoredPosition = new Vector2(x.rectTransform.anchoredPosition.x + diff, x.rectTransform.anchoredPosition.y));
		}

	}

}
