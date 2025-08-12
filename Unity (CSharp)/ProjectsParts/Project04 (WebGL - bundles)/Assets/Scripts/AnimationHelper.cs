using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour {
	public Action OnStartAttack;
	public Action OnEndAttack;

	public void StartAttack() {
		if (OnStartAttack != null) OnStartAttack();
	}

	public void EndAttack() {
		if (OnEndAttack != null) OnEndAttack();
	}



}
