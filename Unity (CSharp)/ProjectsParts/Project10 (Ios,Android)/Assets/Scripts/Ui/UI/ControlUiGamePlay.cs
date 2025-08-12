using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUiGamePlay : MonoBehaviour {


	public void ButtonLeft(bool isDown) {
		ControllerManager.SetLeftKey(isDown);
	}

	public void ButtonRight(bool isDown) {
		//ControllerManager.OnHorizontalButton((flag ? 1 : 0));
		ControllerManager.SetRightKey(isDown);
	}

	public void ButtonJump(bool isDown) {
		ControllerManager.OnJumpButton(isDown);
	}

	public void ButtonAttack(bool isDown) {
		ControllerManager.OnFireButton(isDown);
	}

	//public void ButtonMagic(bool flag) {
	//	animComp.SetBool("ButtonMagicPress", flag);
	//}


}
