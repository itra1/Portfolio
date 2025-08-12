using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZbCatScene;

public class CatSceneHelper1 : ExEvent.EventBehaviour {

	public ClickMacker cm;

	public Image _image;
	private Material _material;
	private Vector4 scaleRound;
	private Enemy _targetEnemy;
	private int _actualStep;
	private bool _readyStart;
	private bool _readyEnd;

	private void Start() {

		if (Game.User.UserManager.Instance.CheckLevelComplited(1, 1) || !CatSceneManager.Instance.IsOn || CatSceneManager.Instance.library.catSceneList[0].isShow || Game.User.UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival) {
			scaleRound = new Vector4(-10.8f, 0, -4.5f, 250);
			Destroy(gameObject);
			return;
		}


		cm.OnClick = ClickPointer;

		//_image = GetComponent<Image>();
		//_image.enabled = true;
		_material = _image.material;
		_material.SetVector("_Point", scaleRound);
	}

	private void Update() {

		if (_targetEnemy != null) {
			if (_actualStep == 2 && _readyStart) {
				if (_targetEnemy.transform.position.x <= 6.5f) {
					_readyStart = false;
					_targetEnemy.SetStun(1, 99999999999);
					_targetEnemy.SetPhase(Enemy.Phase.stun);
					CatSceneManager.Instance.NextScene(true);
				}
			}

			if (_actualStep == 7) {
				/*
				Vector3 koeff = new Vector3(960f / Camera.main.pixelWidth, 600f / Camera.main.pixelHeight, 0);

				Vector3 startpoint = new Vector3(-6.5f, _targetEnemy.transform.position.y + 1, 0);
				Vector3 target = startpoint + (((_targetEnemy.transform.position + Vector3.up) - startpoint) * 0.9f);
				Vector3 readyVector = Camera.main.WorldToScreenPoint(target) - new Vector3(15, 25);
				cm.GetComponent<RectTransform>().anchoredPosition = new Vector3(readyVector.x* koeff.x, readyVector.y* koeff.y, readyVector.z);
				*/
				/*
				Vector3 readyVector = Camera.main.WorldToScreenPoint(_targetEnemy.transform.position + Vector3.up);
				readyVector = new Vector3(960f / Camera.main.pixelWidth * (readyVector.x - 15), 600f / Camera.main.pixelHeight * (readyVector.y - 25), 0);
				//cm.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(_targetEnemy.transform.position + Vector3.up) - new Vector3(15,25);
				cm.GetComponent<RectTransform>().anchoredPosition = readyVector;
				*/

				Vector3 startpoint = new Vector3(-6.5f, _targetEnemy.transform.position.y + 1, 0);
				Vector3 target = startpoint + (((_targetEnemy.transform.position + Vector3.up) - startpoint) * 0.9f);
				cm.transform.position = target;
				/*
				Vector3 readyVector = Camera.main.WorldToScreenPoint(target) - new Vector3(15, 25);
				cm.GetComponent<RectTransform>().anchoredPosition = readyVector;
				*/

			}

			if (_targetEnemy.transform.position.x <= -6.5f) {
				_targetEnemy.SetStun(1, 99999999999);
				_targetEnemy.SetPhase(Enemy.Phase.stun);
			}
		}


	}

	void ClickPointer() {
		if (_actualStep == 5) {
			cm.gameObject.SetActive(false);
			PlayerController.Instance.FakeShoot((Vector2)Input.mousePosition);
		}
		if (_actualStep == 7) {
			PlayerController.Instance.FakeShoot((Vector2)Input.mousePosition);
		}
	}

	void DamageEnemy(Enemy enm, GameObject damager, float value) {
		if (_actualStep == 5) {
			CatSceneManager.Instance.NextScene(true);
		}
	}

	void DeadEnemy(Enemy enm) {
		cm.gameObject.SetActive(false);
		_targetEnemy.OnDamageEvnt -= DamageEnemy;
		_targetEnemy.OnDead -= DeadEnemy;
		CatSceneManager.Instance.NextScene(true);
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.StartCatScene))]
	void StartCastScene(ExEvent.CatSceneEvent.StartCatScene cs) {

		if (cs.id != "1") return;

		_activeCoroutine = StartCoroutine(FirstShow());

		scaleRound = new Vector4(PlayerController.Instance.transform.position.x, PlayerController.Instance.transform.position.y + 1, -4.5f, 0);

	  Game.Weapon.WeaponGenerator.Instance.SetActiveWeaponManager(Game.Weapon.WeaponType.tomato);
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.StartCatFrame))]
	void StartCatFrame(ExEvent.CatSceneEvent.StartCatFrame cs) {
		_actualStep = cs.pageNum;

		if (cs.pageNum == 2) {
			if (_targetEnemy == null) {
				_readyStart = true;
				_targetEnemy = EnemysSpawn.Instance.GenerateEnemyBuName("DremuchiyRisovod",Vector3.zero).GetComponent<Enemy>();
				_targetEnemy.OnDamageEvnt += DamageEnemy;
				_targetEnemy.OnDead += DeadEnemy;
			}
		}
		if (cs.pageNum == 5) {

		  Game.Weapon.WeaponGenerator.Instance.CreateWeaponsControllers();
			_readyStart = true;
			cm.gameObject.SetActive(true);
			cm.transform.position = _targetEnemy.transform.position + Vector3.up;
		}
		if (cs.pageNum == 7) {
			_readyStart = true;
			cm.gameObject.SetActive(true);
			cm.transform.position = _targetEnemy.transform.position + Vector3.up;
		}

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.EndCatFrame))]
	void EndCatFrame(ExEvent.CatSceneEvent.EndCatFrame cs) {

		if (cs.pageNum == 1) {
			if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
			_activeCoroutine = StartCoroutine(SecondShow());
		}

		if (cs.pageNum == 6) {
			_targetEnemy.SetStun(1, 0.1f);
			_targetEnemy.SetPhase(Enemy.Phase.run);
		}

	}
	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.EndCatScene))]
	void EndCastScene(ExEvent.CatSceneEvent.EndCatScene cs) {

		Destroy(gameObject);
		return;

	}

	private Coroutine _activeCoroutine;

	IEnumerator FirstShow() {
		while (scaleRound.w < 2.55f) {
			scaleRound.w += Time.deltaTime * 1.6f;
			_material.SetVector("_Point", scaleRound);
			yield return null;
		}
	}
	IEnumerator SecondShow() {
		while (scaleRound.w < 250f) {
			scaleRound.w += Time.deltaTime * 50;
			_material.SetVector("_Point", scaleRound);
			yield return null;
		}
	}

}
