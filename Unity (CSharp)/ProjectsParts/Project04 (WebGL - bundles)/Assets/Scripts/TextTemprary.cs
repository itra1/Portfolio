using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTemprary : MonoBehaviour {

	private TextMeshProUGUI _textGui;

	private Queue<string> queueString = new Queue<string>();

	private bool isShow = false;

	private TextMeshProUGUI textGui {
		get {
			if (_textGui == null)
				_textGui = GetComponent<TextMeshProUGUI>();
			return _textGui;
		}
		set {
			if (_textGui == null)
				_textGui = GetComponent<TextMeshProUGUI>();
			_textGui = value;
		}
	}

	public void ShowText(string text) {
		queueString.Enqueue(text);
		NextShow();
	}

	private void NextShow() {
		if (isShow || queueString.Count == 0) return;

		string data = queueString.Dequeue();
		StartCoroutine(HideProcess(data));
	}


	IEnumerator HideProcess(string data) {
		isShow = true;
		textGui.color = new Color(textGui.color.r, textGui.color.g, textGui.color.b, 1);
		yield return new WaitForSeconds(1);

		while (textGui.color.a > 0) {
			textGui.color = new Color(textGui.color.r, textGui.color.g, textGui.color.b, textGui.color.a - 1 * Time.deltaTime);
			yield return null;
		}
		textGui.color = new Color(textGui.color.r, textGui.color.g, textGui.color.b, 0);
		isShow = false;
		NextShow();
	}

}
