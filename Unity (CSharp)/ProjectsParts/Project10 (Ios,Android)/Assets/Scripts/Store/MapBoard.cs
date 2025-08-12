using UnityEngine;
using System.Collections;

public class MapBoard : MonoBehaviour {

	bool _isTapDown;

	void Start() {
		//MapController.OnMoved += MovedCheck;
	}

	void OnDestroy() {
		//MapController.OnMoved -= MovedCheck;
	}

	void MovedCheck() {
		_isTapDown = false;
	}

	void OnMouseDown() {
		_isTapDown = true;
		UiController.ClickButtonAudio();
	}

	void OnMouseUp() {
		if (!_isTapDown) return;
		//UiController.ActiveRatingPanel();
		ShowRatingPanel();
	}

	void ShowRatingPanel() {
		RatingMap panel = UiController.ShowUi<RatingMap>();
		panel.gameObject.SetActive(true);
		//RatingMap ratingMap = inst.GetComponent<RatingMap>();
	}


}
