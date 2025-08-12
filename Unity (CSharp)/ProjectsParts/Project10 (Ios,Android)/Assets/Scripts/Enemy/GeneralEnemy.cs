using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;

/// <summary>
/// Общий контроллер для врагов в забеге
/// </summary>
public class GeneralEnemy : Enemy {

	[HideInInspector]
	public int maxLive;                                 // Максимальное значение жизней
	protected float liveNow;                            // Текущее значение жизней
	protected EnemyMove play;                           // Ссылка на компонент
	protected EnemyShoot shoot;                         // Компонент атаки
	[SerializeField]
	protected PlayerDamage playerDamage;                // Настройки дамага по плееру
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string attackAnim = "";                      // Анимация ближней атаки
	public LayerMask playerLayer;                       // Слой с положением игрока
	public float playerRadius;                          // Радиус определения игрока
	public WeaponParametrs weapon;                      // Настройки работы с оружием
	public int enemyNum;                                // Номер врага
	[HideInInspector]
	public float lastDistanceShoot;                     // Время последней дистанционной атаки
	float nextReadyShortAttack;                         // Время разрешения выполнения новой ближайшей атаки

	float shootDistance;

	void Awake() {
		GetConfig();
	}

	public override void Start() {
		base.Start();
	}

	public override void OnEnable() {
		base.OnEnable();
		play = GetComponent<EnemyMove>();
		shoot = GetComponent<EnemyShoot>();
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();
		CheckPlayer();
	}

	public override void Update() {
		base.Update();

		if (runnerPhase == RunnerPhase.run && play.inBoundary) {

			if (lastDistanceShoot + shootDistance <= RunnerController.playerDistantion) {
				lastDistanceShoot = RunnerController.playerDistantion;

				if (weapon.probability >= Random.value) {
					ShootNow();
				}
			}
		}

		if (transform.position.y <= 0
				|| transform.position.x <= CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 2f
				|| transform.position.y > CameraController.displayDiff.transform.position.y + CameraController.displayDiff.top * 2f
				|| transform.position.x > CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 2f)
			gameObject.SetActive(false);
	}

	public virtual void ShootNow() {
		OnShoot();
	}

	float hitBreackProbability;
	float hitBreackTimeWait;

	public bool JumpBreackReady() {
		if (hitBreackTimeWait > Time.time) return false;

		hitBreackTimeWait = Time.time + 1;
		if (hitBreackProbability >= Random.value)
			return false;

		return true;
	}

	float hitBarrierProbability;
	float hitBarrierTimeWait;

	public bool JumpBarrierReady() {
		if (hitBarrierTimeWait > Time.time) return false;

		hitBarrierTimeWait = Time.time + 1;
		if (hitBarrierProbability >= Random.value)
			return false;

		return true;
	}

	public virtual bool JumpReady() {
		return true;
	}

	/// <summary>
	/// Совершение выстрела
	/// </summary>
	public virtual void OnShoot() {
		shoot.Shoot(weapon.type);
	}


	/// <summary>
	/// Определение приближения плеера
	/// </summary>
	/// <returns>Статус определения плеера</returns>
	public virtual bool CheckPlayer() {

		if (Physics2D.OverlapBox(transform.position + Vector3.up * 2.5f, new Vector2(1, 5), 0, playerLayer) && nextReadyShortAttack <= Time.time) {
			// Если не началась телесная атака выполняем анимацию и отменяем телесную
			if (shoot.bodyStep <= 1 && runnerPhase == RunnerPhase.run) {
				AddAnimation(2, attackAnim, false, 0);
				PlayAttackAudio();
				// Если выполняется атака с забегом вперед, отключаем
				if (shoot.bodyAttack)
					shoot.bodyAttack = false;
				shoot.bodyStep = 0;
				nextReadyShortAttack = Time.time + Random.Range(playerDamage.damageTime.min, playerDamage.damageTime.max);
				return true;
			}
		}
		return false;
	}

	public virtual void SetGraphicLocalAngle(Vector3 newLocalAngle) { }
	public virtual void SetGraphicLocalPosition(Vector3 newLocalPosition) { }

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

	public override void Damage(WeaponTypes weaponType, ref float power, Vector3 damagePosition, DamagePowen powerDamage, float fire = 0, bool stoneDam = false) {
	}

	#region Audio

	public AudioClip[] enemyIdleAudio;              // Звук в забеге
	public AudioClip[] shotAttack;                  // Звуки атаки
	public AudioClip[] attackClip;                  // Звуки ближней аттаки

	/// <summary>
	/// Воспроизведение обычных звуков
	/// </summary>
	public void PlayIdleAudio() {
		if (enemyIdleAudio.Length > 0 & Time.time > EnemySpawner.Instance.lastEnemyIdlePlay) {
			audioComp.PlayOneShot(enemyIdleAudio[Random.Range(0, enemyIdleAudio.Length)], 1);
			EnemySpawner.Instance.lastEnemyIdlePlay = Time.time + 1f;
		}
	}
	/// <summary>
	/// Воспроизведение звуков дистанционной атаки
	/// </summary>
	public void PlayDistAttackAudio() {
		if (shotAttack.Length > 0) {
			audioComp.PlayOneShot(shotAttack[Random.Range(0, shotAttack.Length)], 1);
		}
	}

	public void PlayAttackAudio() {
		if (attackClip.Length > 0) {
			AudioManager.PlayEffect(attackClip[Random.Range(0, attackClip.Length)], AudioMixerTypes.runnerEffect, 1);
		}
	}


	#endregion

	/// <summary>
	/// Событие окончание анимации спайна
	/// </summary>
	/// <param name="state">Название анимации</param>
	/// <param name="trackIndex">Номер трека</param>
	public override void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
		base.AnimEnd(state, trackIndex);

		// После анимации дистанционной атаки выполняем сброс
		if (state.GetCurrent(trackIndex).ToString() == shoot.distAttackAnim)
			ResetAnimation();

		// При ручной атаке...
		if (state.GetCurrent(trackIndex).ToString() == attackAnim) {
			bool isPlayer = Physics2D.OverlapCircle(transform.position, playerRadius, playerLayer);
			if (isPlayer) {
        Player.Jack.PlayerController.Instance.ThisDamage(WeaponTypes.none, Player.Jack.DamagType.live, playerDamage.damagePower, transform.position);
			} else {
				if (runnerPhase == RunnerPhase.run)
					Questions.QuestionManager.ConfirmQuestion(Quest.enemyShotButMiss, 1, transform.position + Vector3.up);
			}
		}

		// При дистанционной атаке...
		if (state.GetCurrent(trackIndex).ToString() == shoot.distAttackAnim) {

			if (Random.value <= 0.9f && shotAttack.Length > 0)
				PlayDistAttackAudio();
			else
				AudioManager.PlayEffect(enemyIdleAudio[Random.Range(0, enemyIdleAudio.Length)], AudioMixerTypes.runnerEffect, 1);

			shoot.SpawnWeapon(weapon.type);
		}

	}

	#region Миньен

	/// <summary>
	/// Установка босса
	/// </summary>
	/// <param name="boss"></param>
	public void SetBoss(EnemyBoss boss) {

		EnemyMinion minion = GetComponent<EnemyMinion>();

		if (minion != null)
			minion.SetBoss(boss);
		//MoveFunction = null;
	}
  #endregion

  #region Настройки
  Configuration.EnemyParam enemyParam;

  public void GetConfig() {

    EnemyTypes enemType = enemyType == EnemyTypes.aztecForward ? EnemyTypes.aztec : enemyType;

    enemyParam = Config.Instance.config.enemy.Find(x => x.type == (int)enemType);

		hitBarrierProbability = enemyParam.takeBarrier;
		hitBreackProbability = enemyParam.pitDown;
		maxLive = (int)enemyParam.health;
		shootDistance = enemyParam.distanceShoot;
		weapon.probability = enemyParam.distanceShoot;
		liveNow = maxLive;
	}

	#endregion

}
