using UnityEngine;
using System.Collections;

public class HeadlessEnemy : ClassicEnemy {

	/// <summary>
	/// Флаг наличия головы
	/// </summary>
	bool head = true;                              // Флаг наличия головы

	public override void OnEnable() {
		base.OnEnable();
		head = true;
	}

	/// <summary>
	/// Набор специфических частей тела при разлетании
	/// </summary>
	public GameObject[] specialBodyElements;
	/// <summary>
	/// Разлет костей при смерти
	/// </summary>
	/// <param name="powerDamage"></param>
	/// <param name="group"></param>
	public override void GenBones(DamagePowen powerDamage, bool group = false) {

		base.GenBones(powerDamage, group);

		//Генерируем выпадание спецефичных частей
		if (specialBodyElements.Length > 0 & head) {
			foreach (GameObject bodyElem in specialBodyElements) {
				GameObject inst = Instantiate(bodyElem , bodyElem.transform.position , Quaternion.identity) as GameObject;
				inst.GetComponent<Rigidbody2D>().AddForce(new Vector2(1 * koefY * 0.9f, koefY * Random.Range(0.3f, 0.8f)), ForceMode2D.Impulse);
			}
		}
	}

	/// <summary>
	/// Событие окончание анимации спайна
	/// </summary>
	/// <param name="state">Название анимации</param>
	/// <param name="trackIndex">Номер трека</param>
	public override void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
		base.AnimEvent(state, trackIndex, e);

		if (state.GetCurrent(trackIndex).ToString() == shoot.distAttackAnim)
			head = false;

	}
	/// <summary>
	/// Выполнение выстрела
	/// </summary>
	public override void OnShoot() {
		if (head)
			base.OnShoot();
	}

	public override bool JumpReady() {
		return head ? true : false;
	}

}
