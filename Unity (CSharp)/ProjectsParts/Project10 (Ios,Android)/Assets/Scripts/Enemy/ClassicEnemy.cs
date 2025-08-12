using UnityEngine;
using Spine.Unity;

/// <summary>
/// Структура горения
/// </summary>
[System.Serializable]
public struct BurningParametrs {
	[HideInInspector]
	public GameObject fireObj;        // Объект горения
	[HideInInspector]
	public bool isBurning;            // Флаг горения
	[HideInInspector]
	public float timeBurning;         // Время горения
	[HideInInspector]
	public float lastDamageBurning;   // Время последнего повреждения при горении
	public float damage;                // повреждения наносимые огнем
	public float damagePeriod;          // Время между наничением повреждения
}

/// <summary>
/// Контроллер врагов в классическом забене
/// </summary>
[RequireComponent(typeof(EnemyMove))]
public abstract class ClassicEnemy : GeneralEnemy {

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string burnAnim = "";                      // Анимация горения
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string damageAnim = "";                    // Анимация получения урона

	/// <summary>
	/// Количество монет выдаваемых за смерть
	/// <para>Назначается контроллером</para>
	/// </summary>
	[HideInInspector]
	public int deadCoins;

	/// <summary>
	/// Флаг получения урона от камня
	/// </summary>
	public bool damageFromStone;

	public override void OnEnable() {
		base.OnEnable();
		liveNow = maxLive;
		livelineInst.SetActive(false);
	}

	public override void OnDisable() {
		base.OnDisable();
		livelineInst.SetActive(false);
	}

	public override void Update() {
		base.Update();

		if (burning.fireObj) {
			burning.fireObj.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, -0.1f);
			AddAnimation(2, burnAnim, true, 0);
		}

		// Отключение анимации горения
		if (burning.isBurning && burning.timeBurning <= Time.time) {
			burning.fireObj.SetActive(false);
			burning.fireObj.transform.parent = Pooler.Instance.gameObject.transform;
			burning.fireObj = null;
			burning.isBurning = false;
			ResetAnimation();
		}
		// Повреждения наносимые огнем
		if (burning.isBurning && burning.lastDamageBurning + burning.damagePeriod < Time.time) {
			burning.lastDamageBurning = Time.time;
			Damage(WeaponTypes.fire, burning.damage, Vector3.zero, DamagePowen.level1);
		}
	}

	public override void ShootNow() {
		if (!burning.isBurning)
			base.ShootNow();
	}

	public GameObject livelineInst;           // Ссылка на созданный экземпляр префаба линии жизней

	/// <summary>
	/// Смерть врага
	/// </summary>
	public override void DeadEnemy(bool generateCoins = true) {
		base.DeadEnemy(generateCoins);
		PlayDeadAudio();

		if (burning.fireObj) {
			burning.fireObj.SetActive(false);
			burning.fireObj.transform.parent = Pooler.Instance.gameObject.transform;
			burning.fireObj = null;
		}

		if (generateCoins && deadCoins > 0) {
			CoinsSpawner.GenOneMonetToPlayer(transform.position, deadCoins);
		}
	}

	/// <summary>
	/// Нанесение урона врагу
	/// </summary>
	/// <param name="weaponType">Тип оружия</param>
	/// <param name="power">Сила оружя</param>
	/// <param name="damagePosition">Точка получения урона</param>
	/// <param name="powerDamage">Сила взрыва</param>
	/// <param name="fire">Вероятность возгорания</param>
	/// <param name="stoneDam">Флаг камня</param>
	public override void Damage(WeaponTypes weaponType, float power, Vector3 damagePosition, DamagePowen powerDamage, float fire = 0, bool stoneDam = false) {
		float newPower = power;
		Damage(weaponType, ref newPower, damagePosition, DamagePowen.level1, fire, stoneDam);
	}

	public override void SetNewPhase(RunnerPhase newPhase) {
		base.SetNewPhase(newPhase);

		if (runnerPhase == RunnerPhase.endRun) {
			GenBones(DamagePowen.level2);
			gameObject.SetActive(false);
			DeadEnemy(false);
		}
	}

	public override void Damage(WeaponTypes weaponType, ref float power, Vector3 damagePosition, DamagePowen powerDamage, float fire = 0, bool stoneDam = false) {
		base.Damage(weaponType, ref power, damagePosition, powerDamage, fire, stoneDam);
		// Запрет повреждать врага при туториале саблей
		if (runnerPhase == RunnerPhase.tutorial && weaponType == WeaponTypes.sablePlayer)
			return;

		// не получаем повреждения от камня
		if (stoneDam & !damageFromStone) {
			if (runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial)
				PlayDamageAudio();
			if ((runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial) & Random.value <= 0.3f)
				PlayIdleAudio();
			return;
		}

		// Определяем дамаг, который получит враг
		float damageSum = (liveNow > power ? power : liveNow);
		liveNow -= damageSum;
		power -= damageSum;

		// Генерируем эффект получения урона
		Vector3 boomPos = Vector3.zero;
		if (damagePosition != Vector3.zero) {

			boomPos = Vector3.Lerp(new Vector3(transform.position.x, damagePosition.y, -0.1f),
																		 new Vector3(damagePosition.x, damagePosition.y, -0.1f),
																		 -0.3f);
		} else {
			boomPos = new Vector3(transform.position.x, transform.position.y + 1.5f, -0.3f);
		}
		GameObject bonesBum = Pooler.GetPooledObject("BoneBoom");
		bonesBum.transform.position = boomPos;
		bonesBum.SetActive(true);

		if (liveNow > 0) {
			if (runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial)
				PlayDamageAudio();

			if ((runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial) & Random.value <= 0.3f)
				PlayIdleAudio();

			livelineInst.SetActive(true);
			livelineInst.GetComponent<LiveLineController>().SetSize(maxLive, liveNow);

			AddAnimation(1, damageAnim, false, 0);

			if (fire > Random.value && burning.fireObj == null)
				EnemyFire();
		} else {

			bool breaks = true;

			RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + Vector3.up, Vector3.down, 10, groundMask);

			for (int i = 0; i < hits.Length; i++) {
				if (LayerMask.LayerToName(hits[i].transform.gameObject.layer) == "Ground") {
					breaks = false;
				}
			}

			Questions.QuestionManager.AddEnemyDead(enemyType, weaponType, transform.position, !isGround, breaks);
			GenBones(powerDamage);

			if (!stoneDam)
				RunnerController.Instance.enemiesDeadCount[enemyNum]++;

			EnemySpawner.Instance.EnemyDead(gameObject);
			gameObject.SetActive(false);
			DeadEnemy();
		}
	}


	/// <summary>
	/// Частицы разлета при сметри
	/// </summary>
	public GameObject[] bodyElements;

	/// <summary>
	/// Коеффициент силы разлета костей
	/// </summary>
	protected float koefY;

	public virtual void GenBones(DamagePowen powerDamage, bool group = false) {

		switch (powerDamage) {
			case DamagePowen.level2:
				koefY = 30f;
				break;
			case DamagePowen.level3:
				koefY = 20f;
				break;
			default:
				koefY = 10f;
				break;
		}

		// Генерируем разлетающиеся частицы
		if (bodyElements.Length > 0) {
			foreach (GameObject bodyElem in bodyElements) {
				GameObject inst = Instantiate(bodyElem, bodyElem.transform.position, Quaternion.identity) as GameObject;
				//inst.GetComponent<BodyParticle>().SetStartCoef(koefY);
				inst.GetComponent<Rigidbody2D>().AddForce(new Vector2(1 * koefY * 0.9f, koefY * Random.Range(0.3f,0.8f)), ForceMode2D.Impulse);
			}
		}
	}

	public override void OnTriggerEnter2D(Collider2D oth) {

		base.OnTriggerEnter2D(oth);

		if (oth.tag == "Player") {
			if (runnerPhase == RunnerPhase.boost) {
				Damage(WeaponTypes.none, 100000, Vector3.zero, 0);
			}

			if ((runnerPhase & (RunnerPhase.dead | RunnerPhase.lowEnergy)) != 0 ) {
				EnemySpawner.Instance.CreateDeadCloud();
				gameObject.SetActive(false);
			}
		}

		if (LayerMask.LayerToName(oth.gameObject.layer) == "Barrier") {
			if (oth.tag == "RollingStone") {
				Damage(WeaponTypes.none, (int)RunnerController.barrierDamage(oth.tag, false), transform.position, DamagePowen.level1, 0, true);
				oth.GetComponent<BarrierController>().DestroyThis();
			}
		}

		if (runnerPhase == RunnerPhase.dead && oth.tag == "Player") {
			EnemySpawner.Instance.CreateDeadCloud();
			gameObject.SetActive(false);
		}
	}

	#region Audio
	/// <summary>
	/// Звук получения урона
	/// </summary>
	public AudioClip[] enemyDamageAudio;
	/// <summary>
	/// Звук смерти
	/// </summary>
	public AudioClip[] deadClip;

	/// <summary>
	/// Воспроизведение звуков получения урона
	/// </summary>
	void PlayDamageAudio() {
		if (enemyDamageAudio.Length > 0) {
			AudioManager.PlayEffect(enemyDamageAudio[Random.Range(0, enemyDamageAudio.Length)], AudioMixerTypes.runnerEffect);
		}
	}
	/// <summary>
	/// Воспроизведение звуков смерти
	/// </summary>
	void PlayDeadAudio() {
		if (deadClip.Length > 0) {
			AudioManager.PlayEffect(deadClip[Random.Range(0, deadClip.Length)], AudioMixerTypes.runnerEffect, 0.75f);
		}
	}

	#endregion

	#region Горение

	public BurningParametrs burning;

	/// <summary>
	/// Горение врага
	/// </summary>
	void EnemyFire() {
		//fireObj = Instantiate(fireObject , transform.position , Quaternion.identity) as GameObject;
		burning.fireObj = Pooler.GetPooledObject("EnemyFire");
		burning.fireObj.transform.parent = transform;
		/*
    if (weaponType == EnenyWeaponEnum.enemy)
        fireObj.transform.position = new Vector3(0.3f , 0.3f , 0.3f);
    else if (weaponType == EnenyWeaponEnum.body)
        fireObj.transform.position = new Vector3(0.2f , 0.2f , 0.2f);
    else*/
		burning.fireObj.transform.localPosition = new Vector3(0.15f, 0.15f, 0.15f);
		burning.fireObj.SetActive(true);

		burning.isBurning = true;
		burning.timeBurning = Time.time + Random.Range(3, 7);
	}
	#endregion
}
