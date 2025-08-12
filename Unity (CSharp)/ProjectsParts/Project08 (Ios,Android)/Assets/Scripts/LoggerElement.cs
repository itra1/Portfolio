using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoggerElement : MonoBehaviour {

	public TextMeshProUGUI title;
	public TextMeshProUGUI stack;
	public RectTransform me;

	private string conditionData;
	private string stackData;


	public float SetData(string condition, string stack) {
		this.conditionData = condition;
		this.stackData = stack;
		StartCoroutine(SetDataCor());
		return 150;
	}

	IEnumerator SetDataCor() {
		yield return null;
		yield return null;
		this.title.text = conditionData.Length > 400 ? conditionData.Substring(0, 400) : conditionData;
		yield return null;
		this.stack.text = stackData.Length > 400 ? stackData.Substring(0, 400) : stackData;
		yield return null;

		this.title.rectTransform.anchoredPosition = Vector2.zero;
			this.stack.rectTransform.anchoredPosition = new Vector2(0, -this.title.preferredHeight);
			me.sizeDelta = new Vector2(0, this.title.preferredHeight + this.stack.preferredHeight);
	}

}
