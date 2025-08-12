using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZbCatScene;

public class CatSceneHelper19 : ExEvent.EventBehaviour {
	private Enemy[] _sourceEnemy;

	public ClickMacker cm;
	private bool _isActive = false;
	private List<Enemy> _genEnemy = new List<Enemy>();
	private int _actualStep;
	private bool _readyStart;
	private float _oldHunterDamage;
	private float _oldTimeReload;

	private void Start() {
		/*
		if (User.Instance.activeBattleInfo.group != 2 || User.Instance.activeBattleInfo.group == 6) {
			Destroy(gameObject);
			return;
		}
		*/

		cm.OnClick = ClickPointer;
	}

	void ClickPointer() {
		if (_actualStep == 3) {
			cm.gameObject.SetActive(false);
			PlayerController.Instance.FakeShoot(_genEnemy[3].transform.position + Vector3.up);
		}
	}

	private void Update() {

		if (!_isActive) return;

		if (_actualStep == 0) {
			if (_genEnemy.Count > 0) {
				if (_genEnemy[0].transform.position.x <= 5.5f && _readyStart) {
					_readyStart = false;
					for (int i = 0; i < _genEnemy.Count; i++) {
						_genEnemy[i].SetStun(1, 99999999999);
						_genEnemy[i].SetPhase(Enemy.Phase.stun);
					}
					CatSceneManager.Instance.NextScene(true);
				}
			}
		}
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.StartCatScene))]
	void StartCastScene(ExEvent.CatSceneEvent.StartCatScene cs) {
		if (cs.id == "18") {
			_isActive = true;
			BattleManager.battlePhase = BattlePhase.pause;
			_sourceEnemy = EnemysSpawn.GetAllEnemy;
			for (int i = 0; i < _sourceEnemy.Length; i++) {
				if (_sourceEnemy[i].phase != Enemy.Phase.dead) {
					_sourceEnemy[i].SetStun(1, 99999999999);
					_sourceEnemy[i].SetPhase(Enemy.Phase.stun);
				}
			}

		}
	}

	void DeadEnemy(Enemy enm) {

		if (_actualStep == 1) {
			_genEnemy[0].OnDead -= DeadEnemy;
			CatSceneManager.Instance.NextScene(true);
		}

		if (_actualStep == 2) {

			if (enm == _genEnemy[0]) {
				_genEnemy[3].OnDead += DeadEnemy;
				CatSceneManager.Instance.NextScene(true);
			}

			

		}

		if (enm == _genEnemy[3]) {
			_genEnemy[3].OnDead -= DeadEnemy;
			CatSceneManager.Instance.NextScene(true);
		}

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.StartCatFrame))]
	void StartCatFrame(ExEvent.CatSceneEvent.StartCatFrame cs) {
		if (!_isActive) return;

		Debug.Log(cs.pageNum);

		_actualStep = cs.pageNum;
		_readyStart = true;

		if (cs.pageNum == 0) {

			for (int i = 0; i < 3; i++)
				_genEnemy.Add(EnemysSpawn.Instance.GenerateEnemyBuName("Diversant", new Vector3(13 + i * .5f, DecorationManager.Instance.loaderLocation.roadSize.max, 0)).GetComponent<Enemy>());
			
			for (int i = 0; i < 4; i++)
				_genEnemy.Add(EnemysSpawn.Instance.GenerateEnemyBuName("Diversant", new Vector3(15 + i * .5f, DecorationManager.Instance.loaderLocation.roadSize.min, 0)).GetComponent<Enemy>());
			
		}

		if (cs.pageNum == 2) {
			PlayerController.Instance.FakeShoot(_genEnemy[0].transform.position + Vector3.up);
			_genEnemy[0].OnDead += DeadEnemy;

		}
		if (cs.pageNum == 3) {
			cm.gameObject.SetActive(true);
			cm.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(_genEnemy[3].transform.position + Vector3.up);
		}
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.EndCatFrame))]
	void EndCatFrame(ExEvent.CatSceneEvent.EndCatFrame cs) {
		if (!_isActive) return;

		if (cs.pageNum == 0) {

		  Game.User.UserWeapon.Instance.AddBullet(Game.Weapon.WeaponType.hunter, 1);
		  Game.User.UserWeapon.Instance.selectCompany.Add(Game.Weapon.WeaponType.hunter);
		  Game.Weapon.WeaponGenerator.Instance.CreateWeaponsControllers();
		  Game.Weapon.WeaponGenerator.Instance.SetActiveWeaponManager(Game.Weapon.WeaponType.hunter);
			_oldHunterDamage = (PlayerController.Instance.ActiveWeapon as Game.Weapon.ManagerHunter).damage;
			(PlayerController.Instance.ActiveWeapon as Game.Weapon.ManagerHunter).damage = 60;
			_oldTimeReload = PlayerController.Instance.ActiveWeapon._timeReload;
			PlayerController.Instance.ActiveWeapon._timeReload = 0;
		}
	}
	[ExEvent.ExEventHandler(typeof(ExEvent.CatSceneEvent.EndCatScene))]
	void EndCastScene(ExEvent.CatSceneEvent.EndCatScene cs) {
		if (!_isActive) return;
		BattleManager.battlePhase = BattlePhase.battle;
		for (int i = 0; i < _sourceEnemy.Length; i++) {
			if (_sourceEnemy[i].phase != Enemy.Phase.dead) {
				_sourceEnemy[i].SetStun(1, 0.1f);
			}
		}

			(PlayerController.Instance.ActiveWeapon as Game.Weapon.ManagerHunter).damage = _oldHunterDamage;

		PlayerController.Instance.ActiveWeapon._timeReload = _oldTimeReload;
	}

}
