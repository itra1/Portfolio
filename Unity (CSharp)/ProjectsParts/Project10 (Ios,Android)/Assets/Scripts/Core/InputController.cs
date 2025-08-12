
using UnityEngine;
using System.Collections;

/// <summary>
/// Контроллер ввода
/// </summary>
public class InputController : MonoBehaviour {

	Vector3 dis;
	Vector2 touchDeltaPosition;

	void Update() {
		if (Input.touchCount > 0) {
			for (int i = 0; i < Input.touchCount; i++) {
				dis = Camera.main.ScreenToViewportPoint(Input.GetTouch(i).position);
				if (dis.y > 0.25) {
					touchDeltaPosition = Input.GetTouch(i).deltaPosition;
					if (Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y) && Mathf.Abs(touchDeltaPosition.x) > 1f)
            Player.Jack.PlayerController.Instance.FireKey(true);
				}
			}
		}
	}
}
