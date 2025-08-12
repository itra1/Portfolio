using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Spine;

/// <summary>
/// Контроллер врага закованного в броне
/// </summary>
public class YaLegenda : Enemy {
	/// <summary>
	/// список бафаемых объектов
	/// </summary>
	//List<GameObject> bafList;

	float rightStop = CameraController.rightPoint.x - (CameraController.distanseX / 10f);
	float distanseWalk = CameraController.distanseX / 5f;
	//float timeStopBaffSpeed;
	//float timeNextBaffSpeed;

	float targetPosition;

	private bool _isFirst;

	private int _step;

	/*
	 * step == 0 двигается и запускаем баф призыва
	 * step == 1 ждет выполнения анимации призыва
	 * step == 2 запускает анимацию ускорения
	 * step == 3 ждет выполнение анимации
	 * повторяет до подхода к убежищу
	 */


	private int _countAttack;

	private bool _baffSpeed;
	private bool _baffShustric;

	protected override void OnEnable() {

		_baffShustric = false;
		_countAttack = 0;
		SetDirectionVelocity(-1f);
		_step = 0;
		_baffSpeed = false;
		targetPosition = -99999999;
		base.OnEnable();
		targetPosition = rightStop - 0.8f;
		_isFirst = true;
	}

	public override void Update() {
		if (phase == Phase.dead) return;
		if (transform.position.x > rightStop - 0.8f && transform.position.x < rightStop - 0.2f && _step != 0) {
			_step = 0;
			SetDirectionVelocity(-1);
			targetPosition = 0;
			_countAttack = 0;
		}

		if (phase == Phase.run && transform.position.x <= targetPosition && _step == 0) {

			if (_isFirst) {
				_isFirst = false;
				_step = 2;
				SetStep(_step);
			} else {
				_step++;
				SetStep(_step);
			}
		}
		if (_baffShustric || _baffSpeed) return;

		//if (phase != Phase.attack && damageList.Count > 0) {
		//	SetPhase(Phase.attack);
		//}

		base.Update();

	}


	public void SetStep(int step) {
		
		switch (step) {
			case 0:
				//CheckPhase();
				if (damageList.Count <= 0)
					ChangeTarget();
				else {
					_step++;
					SetStep(_step);
				}
				break;
			case 1:
				SetPhase(Phase.run);
				StartCreateBeast();
				break;
			case 2:
				StartBaffSpeed();
				break;
			case 3:
				if (damageList.Count > 0) {
					SetPhase(Phase.attack);
					_countAttack = 0;
				} else {
					_step = 0;
					SetStep(_step);
				}
				break;
		}

	}

	protected override void SetRunPhase() {
		base.SetRunPhase();
		LeftGraphic();
	}

	public override void Move() {
		velocity.x = Mathf.Abs(speedX) * directionVelocity;
		if (isStun) velocity.x *= 1 - stunDelay;
		transform.position += velocity * Time.deltaTime;
	}

	int countBeast = 5;

	IEnumerator GenerateShustric() {
		for (int i = 0; i < countBeast; i++) {

			GameObject instBest = EnemysSpawn.Instance.GetEnemy("Shustrik");
			instBest.transform.position = new Vector3(CameraController.rightPoint.x + 1f, Random.Range(transform.position.y - 0.5f, transform.position.y + 0.5f), transform.position.y);
			instBest.SetActive(true);
			yield return new WaitForSeconds(0.5f);
		}
	}


	/// <summary>
	/// Генерация нового элемента
	/// </summary>
	void GenerateNewBeast() {
		StartCoroutine(GenerateShustric());
	}

	void ChangeTarget() {
		SetAnimation(runAnim, true);
		targetPosition = transform.position.x - distanseWalk;
	}

	#region Spell

	void StartCreateBeast() {
		_baffShustric = true;
		PlaySpellAudio();
		RightGraphic();
		SetAnimation(spellAnim, false);
	}

	void SpellAttak() {
		GenerateNewBeast();
	}

	public void StartBaffSpeed() {
		//_step++;
		//SetStep(_step);
		SetAnimation(attackAnim[0], false);
		_baffSpeed = true;
	}
	public override void Attack() {
		//if (_countAttack == 0) {

		//	ChangeTarget();
		//	SetDirectionVelocity(1f);

		//	SetPhase(Phase.run);
		//	SetAnimation(runAnim, true);

		//	return;
		//}
		base.Attack();
	}
	public override void AttackEvent() {
		base.AttackEvent();
		//_countAttack--;
	}

	void BafSpeed() {
		EnemysSpawn.Instance.UseBaffSpead(true);
	}

	#endregion

	#region animation
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string spellAnim = ""; // Анимация магии
	#endregion

	#region Anim
	public override void AnimEvent(TrackEntry trackEntry, Spine.Event e) {
		base.AnimEvent(trackEntry, e);

		if (_baffSpeed && e.data.name == "hit")
			BafSpeed();

		if (_baffShustric && trackEntry.ToString() == "spell" && e.data.name == "spell")
			SpellAttak();
	}

	public override void AnimStart(Spine.TrackEntry trackEntry) {
		base.AnimStart(trackEntry);
	}

	public override void AnimComplited(Spine.TrackEntry trackEntry) {
		base.AnimComplited(trackEntry);

		if (trackEntry.ToString() == attackAnim[0] && !_baffSpeed) {
			_countAttack++;

			if (_countAttack == 5) {
				_step = 1;
				SetStep(_step);
			}
		}

		if (trackEntry.ToString() == attackAnim[0] && _baffSpeed && phase != Phase.stun) {
			_baffSpeed = false;
			_step = 3;
			SetStep(_step);
		}

		if (trackEntry.ToString() == "spell" && _baffShustric) {
			_baffShustric = false;
			LeftGraphic();
			_step++;
			SetStep(_step);
		}

	}
	#endregion


	protected override void EndDamage() {
		base.EndDamage();
		//if (_baffSpeed) SetAnimation(stunnAnim, true);
		_baffShustric = false;
		_baffSpeed = false;
	}

	protected override void GetConfig() {
		base.GetConfig();

	}

	#region Звуки


	public List<AudioClipData> spellAudio;
	public AudioBlock spellAudioBlock;

	protected virtual void PlaySpellAudio() {
		spellAudioBlock.PlayRandom(this);
	}


	#endregion

}
