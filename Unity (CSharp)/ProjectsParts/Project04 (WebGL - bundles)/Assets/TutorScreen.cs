using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Tutorial {
	
	public class TutorScreen : Singleton<TutorScreen> {

		public TutorDialog dialog;
		
		public void ShowDialogWorldPosition(Vector3 position, Vector2 viewPivot, string text) {
			dialog.GetComponent<RectTransform>().pivot = viewPivot;
			dialog.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(position);
			dialog.text.text = text;
			dialog.gameObject.SetActive(true);
		}
		public void ShowDialogScreenPosition(Vector2 position, Vector2 viewPivot, string text) {
			dialog.GetComponent<RectTransform>().pivot = viewPivot;
			dialog.GetComponent<RectTransform>().anchoredPosition = position;
			dialog.text.text = text;
			dialog.gameObject.SetActive(true);
		}

		public void UpdatePositionWord(Vector3 position) {
			dialog.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(position);
		}

		public void HideDialog() {
			dialog.gameObject.SetActive(false);
		}

	}
	
}