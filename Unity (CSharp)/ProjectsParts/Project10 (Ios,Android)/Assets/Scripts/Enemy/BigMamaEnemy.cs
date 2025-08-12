using UnityEngine;
using System.Collections;

public class BigMamaEnemy : ShipEnemy {

	/// <summary>
	/// Дамаг принапрыгивании играком, если лежит на земле
	/// </summary>
	public float playerJumpDamage;

	public GameObject spriteObject;

	public override void OnTriggerEnter2D(Collider2D oth) {

		base.OnTriggerEnter2D(oth);

		if (oth.tag == "Player") {
			if (shoot.bodyAttack && shoot.bodyStep != 3) {
        Player.Jack.PlayerController playerCont = oth.GetComponent<Player.Jack.PlayerController>();
				if (playerCont)
					playerCont.ThisDamage(WeaponTypes.none, playerDamage.type, playerDamage.damagePower, transform.position);
			} else if (shoot.bodyStep == 3) {
				AddAnimation(1, shoot.failAnim, false, 0);
				shoot.PlayBodyAttackAudio();
				Damage(WeaponTypes.none, playerJumpDamage, oth.transform.position, DamagePowen.level1, 0, false);
				oth.GetComponent<Player.Jack.PlayerMove>().MinJump();
			}
		}
	}

	public override void SetGraphicLocalAngle(Vector3 newLocalAngle) {
		spriteObject.transform.localEulerAngles = newLocalAngle;
	}
	public override void SetGraphicLocalPosition(Vector3 newLocalPosition) {
		spriteObject.transform.localPosition = newLocalPosition;


	}
}
