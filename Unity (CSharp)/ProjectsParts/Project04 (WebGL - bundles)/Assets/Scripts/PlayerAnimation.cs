using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour, IAnimationPlayer {

	public PlayerWeapon pw;
	private Animator _animatorComponent;

	public RuntimeAnimatorController defaultAnimator;
	public RuntimeAnimatorController horseAnimator;

	private bool _isWeapon;
	private bool _isLeftSword;
	private bool _isPike;
	private bool _isSword;
	private bool _isBow;
	private bool _isMove;
	
	private void OnEnable() {
		animatorComponent.Rebind();
		SetHorse(false, null);
	}
	
	public void SetWeapon(WeaponType wt, Hand hand) {

		switch (wt) {
			case WeaponType.sword: {
					_isPike = false;
					_isBow = false;
					_isSword = true;
					_isLeftSword = pw.activeRightWeapon == null || pw.activeRightWeapon.type != WeaponType.sword;
					break;
				}
			case WeaponType.bow: {
					_isLeftSword = false;
					_isPike = false;
					_isSword = false;
					_isBow = true;
					break;
				}
			case WeaponType.pike: {
					_isLeftSword = false;
					_isBow = false;
					_isSword = false;
					_isPike = true;
					break;
				}
			case WeaponType.shield: {
					_isPike = false;
					_isBow = false;
					break;
				}
			case WeaponType.none: {
					_isPike = false;
					_isBow = false;
					_isWeapon = (pw.activeRightWeapon != null && pw.activeRightWeapon.type != WeaponType.none && pw.activeRightWeapon.type != WeaponType.shield)
											|| (pw.activeLeftWeapon != null && pw.activeLeftWeapon.type != WeaponType.none && pw.activeLeftWeapon.type != WeaponType.shield);
					_isLeftSword = pw.activeLeftWeapon != null && pw.activeRightWeapon != null && pw.activeRightWeapon.type != WeaponType.sword && pw.activeLeftWeapon.type == WeaponType.sword;
					_isSword = (pw.activeLeftWeapon != null && pw.activeRightWeapon != null) && (pw.activeRightWeapon.type == WeaponType.sword || pw.activeLeftWeapon.type == WeaponType.sword);

					break;
				}

		}

		ApplyParametrs();

	}

	private bool _isHorse;
	private HorseBehaviour _horse;

	public Animator animatorComponent {
		get {
			if (_animatorComponent == null)
				_animatorComponent = GetComponentInChildren<Animator>();
			return _animatorComponent;
		}
	}

	public void SetHorse(bool isHorse, HorseBehaviour horse = null) {
		_isHorse = isHorse;
		_horse = horse;

		animatorComponent.runtimeAnimatorController = _isHorse ? horseAnimator : defaultAnimator;
		ApplyParametrs();
	}

	public void ApplyParametrs() {

		animatorComponent.SetBool("isLeftWeapon", _isLeftSword);
		animatorComponent.SetBool("isSword", _isSword);
		animatorComponent.SetBool("isPike", _isPike);
		animatorComponent.SetBool("isBow", _isBow);
		animatorComponent.SetBool("isWeapon", _isWeapon);
	}

	public void SetWeapon(bool isWeapon) {
		this._isWeapon = isWeapon;
	}

	public void SetMove(bool isMove) {
		this._isMove = isMove;

		if (_isHorse) {
			_horse.SetMove(_isMove);
		}
		else {
			animatorComponent.SetBool("move", _isMove);
		}

	}

	public void SetDead() {
		animatorComponent.SetBool("isDead", true);
	}

	public void PlayerAttack() {
		animatorComponent.SetTrigger("attack");
	}

	public void SetAnimEnable(bool isActive) {
		animatorComponent.enabled = isActive;
	}
}
