using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsadaAnimation : MonoBehaviour, IAnimationPlayer {
	private bool _isMove;
	public Animator animComponent;

	public void PlayerAttack() {
		animComponent.SetTrigger("attack");
	}

	public void SetAnimEnable(bool isActive) {
		return;
	}

	public void SetDead() {
		animComponent.SetBool("isDead", true);
	}

	public void SetHorse(bool isHorse, HorseBehaviour horse = null) {
		return;
	}

	public void SetMove(bool isMove) {
		this._isMove = isMove;

		animComponent.SetBool("move", _isMove);
	}

	public void SetWeapon(WeaponType wt, Hand hand) {
		return;
	}
}
