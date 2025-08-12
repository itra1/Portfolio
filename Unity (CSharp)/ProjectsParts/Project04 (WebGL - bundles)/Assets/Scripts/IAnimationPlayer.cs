using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimationPlayer {

	void SetWeapon(WeaponType wt, Hand hand);
	void SetHorse(bool isHorse, HorseBehaviour horse = null);
	void SetDead();
	void PlayerAttack();
	void SetAnimEnable(bool isActive);
	void SetMove(bool isMove);

}
