//#define LOCAL_TEST
//#define BATTLE_TEST
#define RELEASE
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cells;
using ExEvent;
using UnityEngine;
using Network.Input;
using UnityEngine.SceneManagement;


public class BattleManager : Singleton<BattleManager> {
	private BattleInfo battleInfo;

	public BattlePhase phase;

	private int battleRound;

	public List<MovePosition> movePositionList = new List<MovePosition>();
	public List<MovePosition> lastmovePositionList = new List<MovePosition>();
	public List<MovePosition> magicPositionList = new List<MovePosition>();
	public List<MovePosition> attackPositionList = new List<MovePosition>();

	private bool _mapLoaded;

#if UNITY_EDITOR || LOCAL_TEST
	public GameObject mapBeh;
#endif

	public enum BattlePhase {
		load = 0,
		battle = 1
	}

	protected override void Awake() {
		phase = BattlePhase.load;
		_mapLoaded = false;
	}

	private void Start() { }

	/// <summary>
	/// Старт бой
	/// </summary>
	/// <param name=""></param>
	public void SetBattleInfo(BattleUpdate battleStart) {
		this.battleStart = battleStart;
		// Обработка боя
		this.battleInfo = battleStart.battle_info;

#if RELEASE
		StartLoadMap(this.battleInfo.map_id);
#endif

#if BATTLE_TEST
		BrowserContact.Instance.OnGetMapNum();
		//StartLoadMap(100);
#endif

#if (UNITY_EDITOR || LOCAL_TEST) && !BATTLE_TEST && !RELEASE

		GameObject obj = Instantiate(mapBeh);
		MapManager.Instance.SetMap(obj.GetComponent<MapBehaviour>());

		MiniMapBehaviour.Instance.SetMap(obj.GetComponent<MapBehaviour>().miniMap);


		SceneManager.LoadScene("map100",LoadSceneMode.Additive);
		phase = BattlePhase.battle;
		_mapLoaded = true;
		StartCoroutine(BattleStateUpdate());

		if (battleStart.poss_move.Length > 0) ParceMovePosition(battleStart.poss_move);

		ExEvent.BattleEvents.StartBattle.Call(battleStart);

		return;
#endif

#if UNITY_EDITOR && BATTLE_TEST
#endif



	}

	private BattleUpdate battleStart;

	public void StartLoadMap(int map) {

		//TODO Удалить нафиг перед публикацией
		battleInfo.map_id = map;

		MapManager.Instance.SceneLoader(battleInfo.map_id, () => {

			phase = BattlePhase.battle;
			_mapLoaded = true;
			StartCoroutine(BattleStateUpdate());

			ParceMovePosition(battleStart.poss_move);
			ParceAttackCell(battleStart.attack_cell);
			ParceMagicCell(battleStart.magic_cell);

			ExEvent.BattleEvents.StartBattle.Call(battleStart);
		});
	}

	public void OpenBattleLog() {
		BrowserContact.Instance.OnShowBattleLog(battleInfo.battle_id);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	IEnumerator BattleStateUpdate() {
		while (phase == BattlePhase.battle) {
			yield return new WaitForSeconds(3);

			NetworkManager.Instance.BattleUpdate(this.battleInfo.battle_id, BattleUpdate);
		}
	}

	public void BattleUpdate(BattleUpdate dataString) {

		if (!_mapLoaded) return;

		if (string.IsNullOrEmpty(dataString.id_answer))
			return;

		if (dataString.id_answer == "map_close") {
			Debug.Log("Бой окончен");
			StopAllCoroutines();
			BattleEvents.BattleEnd.Call();
			return;
		}

		if (battleRound != dataString.battle_info.round) {
			this.battleRound = dataString.battle_info.round;
			BattleEvents.BattleRoundChange.Call(this.battleRound);
		}
		
		ParceMovePosition(dataString.poss_move);
		ParceAttackCell(dataString.attack_cell);
		ParceMagicCell(dataString.magic_cell);

		BattleEvents.BattleUpdate.Call(dataString);
	}

	void ParceMovePosition(MovePosition[] positionList) {
		lastmovePositionList = movePositionList;
		movePositionList = positionList.ToList();
	}

	void ParceMagicCell(MovePosition[] positionList) {
		magicPositionList = positionList.ToList();
	}

	void ParceAttackCell(MovePosition[] positionList) {
		attackPositionList = positionList.ToList();
	}

	public bool CheckMoveCoord(Cell check) {
		Debug.Log("movePositionList =" + movePositionList.Count);
		return movePositionList.Exists(x => x.x == check.gridX && x.y == check.gridZ);
	}

	public bool CheckAttackCoord(Cell check) {
		Debug.Log("attackPositionList =" + attackPositionList.Count);
		return attackPositionList.Exists(x => x.x == check.gridX && x.y == check.gridZ);
	}

	public bool CheckMagicCoord(Cell check) {
		Debug.Log("magicPositionList =" + magicPositionList.Count);
		return magicPositionList.Exists(x => x.x == check.gridX && x.y == check.gridZ);
	}


}
