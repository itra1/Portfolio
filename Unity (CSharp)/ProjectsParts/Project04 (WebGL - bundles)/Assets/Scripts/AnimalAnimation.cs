using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimalAnimation : MonoBehaviour, IAnimationPlayer {

	public Animation animComponent;
	public AnimationHelper animhelper;

	private void OnEnable() {
		animComponent.Play("Idle");
		animhelper.OnEndAttack = () => {
			animComponent.Play("Idle");
		};
	}

	[ContextMenu("Attack")]
	public void PlayerAttack() {
		animComponent.Play("Attack");
	}

	[ContextMenu("Dead")]
	public void SetDead() {
		animComponent.Play("Death");
	}

	public void SetHorse(bool isHorse, HorseBehaviour horse = null) {
		return;
	}

	public void SetWeapon(WeaponType wt, Hand hand) {
		return;
	}

	public void SetAnimEnable(bool isActive) {

	}

	[ContextMenu("StartMove")]
	public void StartMove() {
		animComponent.Play("Walk");
	}

	[ContextMenu("StopMove")]
	public void StopMove() {
		animComponent.Stop();
	}

	public void SetMove(bool isMove) {
		if(isMove)
			animComponent.Play("Walk");
		else {
			animComponent.Stop();
			animComponent.Play("Idle");
		}

	}
}
