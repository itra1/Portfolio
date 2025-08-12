using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;
using Spine;


/// <summary>
/// Шаман
/// </summary>
public class Tusila : Enemy {

	/// <summary>
	/// Анимация выполнения заклинания
	/// </summary>
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string spellAnim = "";
	/// <summary>
	/// Анимация при состоянии покоя
	/// </summary>
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string stopIdle = "";

	private bool isFirst = false;

	List<Enemy> spellList = new List<Enemy>();
	public ParticleSystem boobleParticel;

	protected override void OnEnable() {
		base.OnEnable();

		isFirst = false;

		targetMagicObject = null;
		velocityX = -1;
		SetDirectionVelocity(velocityX);

		spellList.Clear();

		magicZone.TriggerEnter += OnMagicZoneEnter;
		magicZone.TriggerStay += OnMagicZoneStay;
		magicZone.TriggerExit += OnMagicZoneExit;
		magicZone.GetComponent<CircleCollider2D>().radius = magicZoneRadiusChecking;
	}

	public override void OnDisable() {
		StopAllCoroutines();
		base.OnDisable();

		magicZone.TriggerEnter -= OnMagicZoneEnter;
		magicZone.TriggerStay -= OnMagicZoneStay;
		magicZone.TriggerExit -= OnMagicZoneExit;
	}

	public override void Update() {
		base.Update();

		if (phase != Phase.dead && transform.position.x < CameraController.rightPoint.x - 2 && !isFirst) {
			isFirst = true;
			GetTarget();
		}
		ChackVector();

		if (isActiveMagic) {

			if (isMySpell) UseActiveMagic(this);

			if (timeEndMagic <= Time.time)
				DeactiveMagic();
		}
	}

	void ChackVector() {
		if (targetMagicObject != null && transform.position.x > targetMagicObject.transform.position.x && (directionVelocity == 1 || directionVelocity == 0) && (transform.position - targetMagicObject.transform.position).magnitude > 1)
			SetDirectionVelocity(-1);
		if (targetMagicObject != null && transform.position.x < targetMagicObject.transform.position.x && (directionVelocity == -1 || directionVelocity == 0) && (transform.position - targetMagicObject.transform.position).magnitude > 1)
			SetDirectionVelocity(1);
	}

	public override void Move() {
		if (activeMagic != ShamanMagic.none) return;

		velocity.x = Mathf.Abs(speedX) * directionVelocity * (isBaffSpead ? 4 : 1);
		if (isStun) velocity.x *= 1 - stunDelay;

		if (targetMagicObject != null)
			velocity.y = (targetMagicObject.transform.position.y - transform.position.y);

		transform.position += velocity * Time.deltaTime;
	}

	/// <summary>
	/// Реакция на событие анимации
	/// </summary>
	/// <param name="state"></param>
	/// <param name="trackIndex"></param>
	/// <param name="e"></param>
	public override void AnimEvent(Spine.TrackEntry trackEntry, Spine.Event e) {
		base.AnimEvent(trackEntry, e);
		if (e.ToString() == "spell") {
			ActivateMagicFromAnim();
		}
	}
	
	#region Magic

	enum ShamanMagic { none, health, shield }             // Типы магии
	ShamanMagic activeMagic;                              // Активная магия
	float timeEndMagic;                                   // Время окончания заклинания
	bool isMySpell;                                       // Флаг использования магии на себе самом
	bool isActiveMagic;                                   // Флаг использования магии
	public EnemyColliderHelper magicZone;                 // Зона действия магии а так же определения нахождения там врагов
	public float magicZoneRadiusChecking = 1.5f;          // Радиус проверки замби в зоне досигаемости
	public float magicZoneRadiusUse = 2.5f;               // Радиус действия магии
	Enemy targetMagicObject;                              // целевой объект магии
	float velocityX;                                      // Направление движения
	float magicWaitTime;                                  // Время, до которого использование магии запрещено
	public float timeShield;                              // Время действия щита
	public float hpForSecond;                             // Скорость действия исцеления
	Enemy[] enemyMasToMagic;                              // Массив врагов на сцене
	public GameObject magicHealtSprite;                   // Объет визуализацтт магии лечения
	public GameObject magicShieldSprite;                  // Объект визуализации магии щита
	Enemy oneEnemy;                                       // Временная переменная для работы магии
																												/// <summary>
																												/// Получаем информацию о цели
																												/// </summary>
	void GetTarget() {

		if (phase == Phase.dead) return;

		if (isActiveMagic || activeMagic != ShamanMagic.none || phase == Phase.damage) {
			//Invoke("GetTarget", 1);
			return;
		}
		targetMagicObject = null;

		/// Получаем список врагов на сцене
		enemyMasToMagic = EnemysSpawn.GetAllEnemy;

		float? distanse = null;
		/// Ищем ближайшего с шаману, у которого либо отсутствует щит, либо не полный запас здоровья
		for (int i = 0; i < enemyMasToMagic.Length; i++) {
			if (enemyMasToMagic[i].enemyType != EnemyType.Tusila && enemyMasToMagic[i].transform.position.x < CameraController.rightPoint.x && enemyMasToMagic[i].liveNow > 0 && (enemyMasToMagic[i].startLive > enemyMasToMagic[i].liveNow || !enemyMasToMagic[i].isShield)) {
				if (distanse == null || distanse > Vector3.Distance(transform.position, enemyMasToMagic[i].transform.position)) {
					distanse = Vector3.Distance(transform.position, enemyMasToMagic[i].transform.position);
					targetMagicObject = enemyMasToMagic[i];
				}
			}
		}
		/// Подписывыемся на смерть объекта, на случай если не успеем дойти
		if (targetMagicObject != null) targetMagicObject.SubscribeDead(DieTargetMagicObject);

		/// Если не нашли никого, выполняем самолечение или накладываем щит, если нужно
		if (targetMagicObject == null) {
			SetDirectionVelocity(0);
			if (magicWaitTime < Time.time) {
				if (!isShield) {
					isMySpell = true;
					ActivateMagic(ShamanMagic.shield);
				}else if (startLive > liveNow) {
					isMySpell = true;
					ActivateMagic(ShamanMagic.shield);
				} else {
					SetAnimation(stopIdle, false);
					//SetRunAnimation();
				}
			} else {
				SetAnimation(stopIdle, false);
				//SetRunAnimation();
			}

			return;
		}

		if (targetMagicObject != null) {
			ChackVector();
			SetRunAnimation();
		}

	}

	/// <summary>
	/// Реакция на вхождение в зону дейтсвия магии
	/// </summary>
	/// <param name="col"></param>
	void OnMagicZoneEnter(Collider2D col) {
		if (phase == Phase.dead || LayerMask.LayerToName(col.gameObject.layer) != "Enemy") return;
		oneEnemy = col.GetComponent<Enemy>();
		if (oneEnemy.enemyType == EnemyType.Tusila) return;

		if (targetMagicObject == oneEnemy) {
			targetMagicObject = null;
		}

		spellList.Add(oneEnemy);

		TriggetActivateMagic(oneEnemy);
	}

	/// <summary>
	/// Реакция на нахождения в зоне действия магии
	/// </summary
	/// <param name="col"></param>
	void OnMagicZoneStay(Collider2D col) {
		if (phase == Phase.dead || LayerMask.LayerToName(col.gameObject.layer) != "Enemy") return;
		oneEnemy = col.GetComponent<Enemy>();
		if (oneEnemy.enemyType == EnemyType.Tusila) return;

		if (isActiveMagic && oneEnemy != null) {
			UseActiveMagic(oneEnemy);
		} else if (!isActiveMagic && activeMagic == ShamanMagic.none)
			TriggetActivateMagic(oneEnemy);
	}

	void OnMagicZoneExit(Collider2D col) {
		if (phase == Phase.dead || LayerMask.LayerToName(col.gameObject.layer) != "Enemy") return;
		oneEnemy = col.GetComponent<Enemy>();
		if (oneEnemy.enemyType == EnemyType.Tusila) return;

		if (targetMagicObject == oneEnemy) {
			targetMagicObject = null;
		}

		spellList.Remove(oneEnemy);

		TriggetActivateMagic(oneEnemy);
	}


	/// <summary>
	/// Активация магии при обнаружении объекта
	/// </summary>
	void TriggetActivateMagic(Enemy oneTarget) {

		if (activeMagic == ShamanMagic.none && magicWaitTime < Time.time) {
			if (CheckNeedsHealth(oneTarget))
				ActivateMagic(ShamanMagic.health);
			else if (CheckNeedsShield(oneTarget))
				ActivateMagic(ShamanMagic.shield);
		}
	}

	/// <summary>
	/// Использование активной магии
	/// </summary>
	/// <param name="enemyTarget"></param>
	void UseActiveMagic(Enemy enemyTarget) {
		if (activeMagic == ShamanMagic.health && enemyTarget.startLive > enemyTarget.liveNow)
			UseHealth(oneEnemy);
		if (activeMagic == ShamanMagic.shield && !enemyTarget.isShield)
			UseShield(oneEnemy);
		enemyTarget = null;
	}

	/// <summary>
	/// Проверка необходимости использования лечения
	/// </summary>
	/// <param name="checkObj">Проверяемый объект</param>
	/// <returns>флаг необходимости использования магии</returns>
	bool CheckNeedsHealth(Enemy checkObj) {
		if (checkObj.startLive > checkObj.liveNow)
			return true;
		else
			return false;
	}

	/// <summary>
	/// Проверка необходимости использования лечения
	/// </summary>
	/// <param name="checkObj">Проверяемый объект</param>
	/// <returns>флаг необходимости использования магии</returns>
	bool CheckNeedsHealth(GameObject checkObj) {
		if (!checkObj.GetComponent<Enemy>()) return false;
		return CheckNeedsHealth(checkObj.GetComponent<Enemy>());
	}

	/// <summary>
	/// Проверка на необходимость использования магии щита
	/// </summary>
	/// <param name="checkObj"></param>
	/// <returns></returns>
	bool CheckNeedsShield(Enemy checkObj) {
		return !checkObj.isShield;
	}

	/// <summary>
	/// Запуск магии
	/// </summary>
	/// <param name="magicType"></param>
	void ActivateMagic(ShamanMagic magicType) {
		if (activeMagic != ShamanMagic.none || magicWaitTime > Time.time) return;
		PlayBafAudio();
		activeMagic = magicType;

		magicZone.GetComponent<CircleCollider2D>().radius = magicZoneRadiusUse;
		SetAnimation(spellAnim, false);
	}

	/// <summary>
	/// Активация магии по событии в анимации
	/// </summary>
	void ActivateMagicFromAnim() {
		isActiveMagic = true;
		MagicTimeIncrement();

		boobleParticel.Play();

		switch (activeMagic) {
			case ShamanMagic.health:
				//magicShieldSprite.SetActive(true);
				UseHealth(this);
				spellList.ForEach(x => UseHealth(x));
				break;
			case ShamanMagic.shield:
				//magicHealtSprite.SetActive(true);
				UseShield(this);
				spellList.ForEach(x => UseShield(x));
				break;
		}

	}

	/// <summary>
	/// Деактивации магии
	/// </summary>
	void DeactiveMagic() {
		isMySpell = false;
		isActiveMagic = false;
		magicWaitTime = Time.time + 2;
		magicZone.GetComponent<CircleCollider2D>().radius = magicZoneRadiusChecking;
		activeMagic = ShamanMagic.none;
		magicShieldSprite.SetActive(false);
		magicHealtSprite.SetActive(false);
		GetTarget();
	}

	protected override void OnDamage() {
		base.OnDamage();
		targetMagicObject = null;
	}

	protected override void EndDamage() {
		targetMagicObject = null;
		if (IsInvoking("GetTarget")) CancelInvoke("GetTarget");
		velocityX = 0;
		directionVelocity = velocityX;
		base.EndDamage();
		DeactiveMagic();
	}


	/// <summary>
	/// Событие смерти целевого объекта
	/// </summary>
	void DieTargetMagicObject(Enemy enemy) {
		Invoke("GetTarget", 1);
	}

	/// <summary>
	/// Применение щита
	/// </summary>
	/// <param name="targetObject"></param>
	void UseShield(Enemy targetObject) {
		MagicTimeIncrement();
		targetObject.SetShield(timeShield);
	}

	/// <summary>
	/// Инкремент использования магии
	/// </summary>
	void MagicTimeIncrement() {
		timeEndMagic = Time.time + 2;
	}

	/// <summary>
	/// Применение исцеления
	/// </summary>
	/// <param name="targetObject">Объект, на котором применяется заклинание</param>
	void UseHealth(Enemy targetObject) {

		MagicTimeIncrement();
		targetObject.HealthIncrement(hpForSecond * Time.deltaTime);

	}

	#endregion
	
	public override void AnimComplited(TrackEntry trackEntry) {
		base.AnimComplited(trackEntry);
		if (trackEntry.ToString() == stopIdle) {
			GetTarget();
		}
	}
	

	#region Звуки

	public AudioBlock bafAudioBlock;

	protected virtual void PlayBafAudio() {
		bafAudioBlock.PlayRandom(this);
	}


	#endregion

}
