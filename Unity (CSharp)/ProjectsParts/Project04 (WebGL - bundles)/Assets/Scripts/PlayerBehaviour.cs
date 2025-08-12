using System;
using Cells;
using ExEvent;
using UnityEngine;
using Network.Input;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(PlayerBehaviour))]
public class PlayerBehaviourEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("SetHorse")) {
			((PlayerBehaviour)target).SetHorse(true);
		}

		if (GUILayout.Button("Dead")) {
			((PlayerBehaviour)target).Dead();
		}

		if (GUILayout.Button("Ataka")) {
			((PlayerBehaviour)target).AttackPlayer(null);
		}

	}
}

#endif

public class PlayerBehaviour : ExEvent.EventBehaviour {

	public string title;
	public Action OnChangeInfo;

	public event Action OnStartAttack;
	public event Action OnEndAttack;

	public event Action<bool> OnChangeVisible;

	public IAnimationPlayer _playerAnimator;
	public PlayerWeapon playerWeapon;

	public PlayerType type;

	public AnimationHelper animHeplper;

	private int _team = 0;
	private FoW.FogOfWarUnit _fogOfWarUnit;
	public int team {
		get {
			return _team;
		}
		set {
			_team = value;
			if(_fogOfWarUnit == null) {
				_fogOfWarUnit = GetComponent<FoW.FogOfWarUnit>();

				if(_fogOfWarUnit == null) {
					_fogOfWarUnit = gameObject.AddComponent<FoW.FogOfWarUnit>();
					_fogOfWarUnit.radius = (MapManager.Instance.map.cellSize > 10 ? 76 : 39);
				}
			}
			_fogOfWarUnit.team = _team;
		}
	}

	private bool _isMain;

	public bool isMain {
		set {
			_isMain = value;

			projectorSource.isMain = _isMain;

		}
		get { return _isMain; }
	}

	public IAnimationPlayer playerAnimator {
		get {
			if (_playerAnimator == null)
				_playerAnimator = GetComponent<IAnimationPlayer>();

			return _playerAnimator;
		}
	}

	public Transform _mainPlayer;

	public Transform tr {
		get { return transform; }
	}

	public bool isVisibleFog = true;
	private bool _isVisiblePlayer = true;

	public bool isVisiblePlayer {
		get { return _isVisiblePlayer; }
		set {
			_isVisiblePlayer = value;
			projectorSource.isVisible = isVisible;
			modelBehaviour.SetVisibleFog(isVisible);

			if (_horse != null)
				_horse.SetVisibleFog(isVisible);
			if (OnChangeVisible != null) OnChangeVisible(isVisible);
		}
	}

	public bool isVisible {
		get { return isVisibleFog && isVisiblePlayer; }
	}

	private ModelBehaviour _modelBehaviour;

	private ModelBehaviour modelBehaviour {
		get {
			if (_modelBehaviour == null)
				_modelBehaviour = GetComponent<ModelBehaviour>();
			return _modelBehaviour;
		}
	}

	public Transform mainPlayer {
		get {
			if (_mainPlayer == null && PlayersManager.Instance.mainPlayer != null) {
				_mainPlayer = PlayersManager.Instance.mainPlayer.tr;
			}
			return _mainPlayer;
		}
	}

	private PlayerInfo _playerInfo;
	public PlayerInfo playerInfo {
		get { return _playerInfo; }
		set {
			_playerInfo = value;
			if (OnChangeInfo != null) OnChangeInfo();

			if (PlayersManager.Instance.mainPlayer != null)
				projectorSource.isFriend = PlayersManager.Instance.mainPlayer.playerInfo.army == playerInfo.army;

			ExEvent.GameEvents.PlayerInfoChange.Call(this);

			LoadItemsInfo();
		}
	}

	[ExEvent.ExEventHandler(typeof(BattleEvents.MainPlayerCreate))]
	private void MainPlayerCreate(BattleEvents.MainPlayerCreate eventData) {

		if (PlayersManager.Instance.mainPlayer != null)
			projectorSource.isFriend = PlayersManager.Instance.mainPlayer.playerInfo.army == playerInfo.army;
	}


	private EnemyData _playerData;

	private void OnEnable() {

		moveDown = false;
		SetActiveRegdol(false);
		playerAnimator.SetAnimEnable(true);
		isDead = false;

		if (modelBehaviour != null) {
			modelBehaviour.SetData(Сlothes.cuirass, false);
			modelBehaviour.SetData(Сlothes.helmet, false);
			modelBehaviour.SetData(Сlothes.pants, false);
			modelBehaviour.SetData(Сlothes.shoulders, false);
	  }

	  float scl = MapManager.Instance.map.playerSize;
	  transform.localScale = new Vector3(scl, scl, scl);
	  distanceMax = scl == 5 ? 95 : 45;
  }

	public void EnemyDress(CharacterDress.CharacterDressElement[] dressArray) {

		ModelBehaviour mb = GetComponent<ModelBehaviour>();
		mb.SetData(Сlothes.cuirass, false);
		mb.SetData(Сlothes.helmet, false);
		mb.SetData(Сlothes.pants, false);
		mb.SetData(Сlothes.shoulders, false);
		playerWeapon.SetWeapon(WeaponType.none, Hand.left);
		playerWeapon.SetWeapon(WeaponType.none, Hand.right);

		bool existHorse = false;
		foreach (var item in dressArray) {

			int typeId = item.type;

			ItemLibrary il = ItemManager.Instance.itemLibrary.Find(x => x.typeBegin <= typeId && x.typeEnd >= typeId);

			if (il.type == ItemType.weapon)
				playerWeapon.SetWeapon(WeaponType.sword, (Hand)item.status);

			if (il.type == ItemType.shield)
				playerWeapon.SetWeapon(WeaponType.shield, (Hand)item.status);

			if (il.type == ItemType.smallArms)
				playerWeapon.SetWeapon(WeaponType.bow, (Hand)item.status);

			if (il.type == ItemType.armor)
				mb.SetData(Сlothes.cuirass, true);

			if (il.type == ItemType.shoulder)
				mb.SetData(Сlothes.shoulders, true);

			if (il.type == ItemType.helmet)
				mb.SetData(Сlothes.helmet, true);

			if (il.type == ItemType.pants)
				mb.SetData(Сlothes.pants, true);

			if (il.type == ItemType.horses) {
				existHorse = true;
				SetHorse(true);
			}
		}

		modelBehaviour.SetVisibleFog(isVisible);
		projectorSource.isVisible = isVisible;

		if (!existHorse)
			SetHorse(false);

	}

	public EnemyData playerData {
		get { return _playerData; }
		set {
			_playerData = value;

			ModelBehaviour mb = GetComponent<ModelBehaviour>();
			mb.SetData(Сlothes.cuirass, false);
			mb.SetData(Сlothes.helmet, false);
			mb.SetData(Сlothes.pants, false);
			mb.SetData(Сlothes.shoulders, false);
			playerWeapon.SetWeapon(WeaponType.none, Hand.left);
			playerWeapon.SetWeapon(WeaponType.none, Hand.right);


			bool existHorse = false;
			if (_playerData != null && _playerData.enemy_items != null) {
				foreach (var item in _playerData.enemy_items) {

					int typeId = int.Parse(item.type);

					ItemLibrary il = ItemManager.Instance.itemLibrary.Find(x => x.typeBegin <= typeId && x.typeEnd >= typeId);

					if (il.type == ItemType.weapon)
						playerWeapon.SetWeapon(WeaponType.sword, item.status == "2" ? Hand.left : Hand.right);

					if (il.type == ItemType.shield)
						playerWeapon.SetWeapon(WeaponType.shield, item.status == "2" ? Hand.left : Hand.right);

					if (il.type == ItemType.smallArms)
						playerWeapon.SetWeapon(WeaponType.bow, item.status == "2" ? Hand.left : Hand.right);

					if (il.type == ItemType.armor)
						mb.SetData(Сlothes.cuirass, true);

					if (il.type == ItemType.shoulder)
						mb.SetData(Сlothes.shoulders, true);

					if (il.type == ItemType.helmet)
						mb.SetData(Сlothes.helmet, true);

					if (il.type == ItemType.pants)
						mb.SetData(Сlothes.pants, true);

					if (il.type == ItemType.horses) {
						existHorse = true;
						SetHorse(true);
					}
				}
			}

			modelBehaviour.SetVisibleFog(isVisible);
			projectorSource.isVisible = isVisible;

			if (!existHorse)
				SetHorse(false);
		}
	}


	private bool _isHorse;
	private HorseBehaviour _horse;
	public void SetHorse(bool isHorse) {

		if (isHorse & _isHorse) return;

		_isHorse = isHorse;

		if (_isHorse) {
			_horse = ItemManager.Instance.GetHorse(playerInfo != null && playerInfo.army == 0);
			_horse.gameObject.transform.position = transform.position;
			transform.position = _horse.parent.position;
			_horse.SetVisibleFog(isVisible);
		} else {

			if (_horse != null)
				_horse.gameObject.SetActive(false);

			_horse = null;
		}
		playerAnimator.SetHorse(_isHorse, _horse);
		moveBehaviour.SetHorse(_isHorse, _horse);

	}

	public MoveBehaviour moveBehaviour;
	public Cell _actualCell;

	public Cell actualCell {
		get { return _actualCell; }
		set { _actualCell = value; }
	}
	public Cell targetCell;
	public PlayerPhase phase;

	ProjectorSource _projectorSource;

	private ProjectorSource projectorSource {
		get {

			if (_projectorSource == null)
				_projectorSource = GetComponent<ProjectorSource>();

			return _projectorSource;
		}
	}

	public MoveBehaviour mover {
		get { return GetComponent<MoveBehaviour>(); }
	}

	private float distanceMain;
	private float distanceMax;
	private int cadrCount;

	private void Update() {

		if (_isHorse && _horse != null)
			transform.position = _horse.parent.position;
		
		if (moveDown) {
			transform.position += Vector3.down * 1 * Time.deltaTime;
			if (transform.position.y <= -15) {
				gameObject.SetActive(false);
			}
			return;
		}

		cadrCount++;
		if (cadrCount % 30 != 0) return;

		if (!isMain) {
			if (mainPlayer != null) {
				//isVisibleFog
				if (modelBehaviour != null) {
					distanceMain = (_mainPlayer.position - transform.position).magnitude;
					if (isVisibleFog && distanceMain > distanceMax) {
						isVisibleFog = false;
						modelBehaviour.SetVisibleFog(isVisible);
						projectorSource.isVisible = isVisible;
						if (_horse != null)
							_horse.SetVisibleFog(isVisible);

						if (OnChangeVisible != null) OnChangeVisible(isVisible);
					} else if (!isVisibleFog && distanceMain < distanceMax) {
						isVisibleFog = true;
						projectorSource.isVisible = isVisible;
						modelBehaviour.SetVisibleFog(isVisible);

						if (_horse != null)
							_horse.SetVisibleFog(isVisible);
						if (OnChangeVisible != null) OnChangeVisible(isVisible);
					}
				}
			}
		}
	}

	private void OnMouseDown() {
		PlayersManager.Instance.SelectPlayer(this);
	}

	private void FixedUpdate() {
		if (isMain)
			MapManager.Instance.SetMapFog(transform.position);
	}

	public void LoadItemsInfo() {
		/*
		NetworkManager.Instance.GetEnemyInfo(playerInfo.pid, (enemy) => {
			playerData = enemy;
		});
		*/
	}

	public void SetPosition(int x, int y) {
		Cell tmpCell = CellDrawner.Instance.GetCellByGride(new Vector2(x, y));

		if (tmpCell == null) return;

		actualCell = tmpCell;

		transform.position = actualCell.position;

		if (!isMain)
			projectorSource.DrawnGride(transform.position);

	}

	public void MoveToCell(int x, int y) {

		if (actualCell != null && actualCell.gridX == x && actualCell.gridZ == y) return;
		if (targetCell != null && targetCell.gridX == x && targetCell.gridZ == y) return;

		targetCell = CellDrawner.Instance.GetCellByGride(new Vector2(x, y));
		MoveToCell(targetCell);
	}

	public void MoveToCell(Cell targetCell) {
		if (isDead) return;
		if (targetCell == null || actualCell == targetCell) return;
		moveBehaviour.MoveToCell(targetCell);
		if (!isMain)
			projectorSource.DrawnGride(targetCell.position);
	}

	public void AttackPlayer(PlayerBehaviour attackPlayer) {
		if (isDead) return;
		if (attackPlayer != null)
			moveBehaviour.RotationTo(attackPlayer.tr.position);
		playerAnimator.PlayerAttack();
	}

	public void LockTo(Vector3 lookPos) {
		moveBehaviour.RotationTo(lookPos);
	}

	private bool moveDown = false;
	private bool isDead = false;

	public void Dead(bool isAnimate = true) {

		if (!isVisible || !isAnimate) {
			gameObject.SetActive(false);
			isDead = true;
			if (_horse) {
				_horse.isDead = true;
				_horse.gameObject.SetActive(false);
			}

			isVisiblePlayer = false;

			return;
		}

		//playerAnimator.SetDead();
		isDead = true;
		SetActiveRegdol(true);
		if (_horse)
			_horse.Dead();
		playerAnimator.SetAnimEnable(false);
		Invoke("SetMoveDown", 10);
	}


	private StatePlayer _statePlayer;
	private void OnMouseEnter() {
		_statePlayer = PlayerStatManager.Instance.GetStatPlayer();
		_statePlayer.gameObject.SetActive(true);
		_statePlayer.Init(playerInfo.army == PlayersManager.Instance.mainPlayer.playerInfo.army, this);
	}

	private void OnMouseExit() {
		if (_statePlayer != null) {
			_statePlayer.gameObject.SetActive(false);
			_statePlayer = null;
		}
	}

	private void SetMoveDown() {
		moveDown = true;
		SetActiveRegdol(false);
	}

	private void SetActiveRegdol(bool setActive) {
		Rigidbody[] rigLias = animHeplper.GetComponentsInChildren<Rigidbody>();

		for (int i = 0; i < rigLias.Length; i++) {
			rigLias[i].useGravity = setActive;
			rigLias[i].isKinematic = !setActive;
		}
		Collider[] collLias = animHeplper.GetComponentsInChildren<Collider>();

		for (int i = 0; i < collLias.Length; i++) {
			collLias[i].enabled = setActive;
		}
	}

}


public enum PlayerPhase {
	idle = 0,
	move = 1,
	attack = 2,
	block = 3
}

public enum PlayerType {
	none,
	gall,
	rome,
	bandit,
	hell,
	god,
	skeleton
}