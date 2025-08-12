using UnityEngine;
using System.Collections;
using Spine;

public class GiantEnemy : ClassicEnemy {
	
	GameObject myEnemy;           // Ссылка на сгенерированного врага

	public override void OnEnable() {
		base.OnEnable();

		myEnemy = null;
	}


	/// <summary>
	/// Выполнение выстрела
	/// </summary>
	public override void OnShoot() {
		if (myEnemy == null)
			myEnemy = EnemySpawner.Instance.GenEnemyForGigant();
	}

	public override void Update() {
		base.Update();

		if (myEnemy != null && !myEnemy.activeInHierarchy) {
			myEnemy = null;
			//tossConnect = false;
		}
	}

	/// <summary>
	/// Установка основной анимации
	/// </summary>
	/// <param name="anim">Имя анимации</param>
	/// <param name="loop">Флаг зацикливания</param>
	public override void SetAnimation(string anim, bool loop) {
		if (currentAnimation != anim) {

			if (currentAnimation == shoot.distAttackAnim && myEnemy != null) {
				myEnemy.SetActive(false);
				myEnemy = null;
			}
		}
		base.SetAnimation(anim, loop);
	}

	/// <summary>
	/// Повторная инициализация анимации
	/// </summary>
	public override void ResetAnimation() {

		if (myEnemy != null) {
			myEnemy = null;
		}
		//if(shoot != null) shoot.enemyObject = null;
		base.ResetAnimation();
	}

	public override void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
		base.AnimEvent(state, trackIndex, e);

		if (state.GetCurrent(trackIndex).ToString() == shoot.distAttackAnim) {
			myEnemy = null;
			//tossConnect = false;
			if (shoot != null)
				shoot.enemyObject = null;
		}
	}
	
	public override void OnTriggerEnter2D(Collider2D oth) {

		base.OnTriggerEnter2D(oth);

		if (oth.gameObject == myEnemy) {
			myEnemy.GetComponent<AztecEnemy>().thisToss = true;
			shoot.Shoot(weapon.type, myEnemy.gameObject);
			//tossConnect = true;
			myEnemy = null;
		}

	}

}
