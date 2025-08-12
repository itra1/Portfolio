//#define LOCAL_TEST
#define BATTLE_TEST
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cells;
using ExEvent;
using Network.Input;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PlayersManager))]
public class PlayersManagerEditir : Editor {
	private int classId = 0;
	private int race = 0;
	private int model = 0;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		classId = EditorGUILayout.IntField("Class", classId);
		race = EditorGUILayout.IntField("Расса", race);
		model = EditorGUILayout.IntField("Модель", model);

		if (GUILayout.Button("Create")) {
			((PlayersManager)target).CreatePlayerInEditro(classId, race, model);
		}

	}
}

#endif

public class PlayersManager : Singleton<PlayersManager> {

#if UNITY_EDITOR || LOCAL_TEST
	public GameObject playerPref;
#endif

	private bool roundChange;

	public List<PlayerPrefab> prefabList = new List<PlayerPrefab>();
	public Dictionary<string, PlayerBehaviour> instancePlayers = new Dictionary<string, PlayerBehaviour>();

	public MyPlayer myPlayer;

	public Queue<PlayerCreateOrder> queueCreate = new Queue<PlayerCreateOrder>();

	public PlayerBehaviour selectEnemyPlayer;
	public PlayerBehaviour mainPlayer;

	private bool _isOsada;
	public bool isOsada {
		get { return _isOsada; }
		set {
			if (_isOsada == value) return;
			_isOsada = value;
			BattleEvents.OnChangeIsOsada.Call();
		}
	}

	private bool _isAttackMode;
	public bool isAttackMode {
		get { return _isAttackMode; }
		set {
			if (_isAttackMode == value) return;
			_isAttackMode = value;
			DrawActiveCells();
			BattleEvents.OnChangeActionMode.Call();
		}
	}

	public MapManager mapManager;


	[ExEvent.ExEventHandler(typeof(BattleEvents.StartBattle))]
	void StartBattle(BattleEvents.StartBattle battleStart) {
		 
		if(battleStart.battleStart == null) return;

		myPlayer = battleStart.battleStart.my_player;

		foreach (var key in battleStart.battleStart.data.playersDict.Keys) {
			PlayerInfo pif = battleStart.battleStart.data.playersDict[key];
			pif.playerName = key;

			if(pif.killed > 0) continue;

			CreatePlayer(pif);
		}
	}
	
	public void CreateTechnic(Technic tech) {
		PlayerCreateOrder pco = new PlayerCreateOrder();
		pco.info = new PlayerInfo();
		pco.info.class_id = 9009;
		pco.info.race = 99;
		pco.info.x = int.Parse(tech.x);
		pco.info.y = int.Parse(tech.y);
		pco.info.model = Int32.Parse(tech.technic_id);
		pco.info.playerName = tech.tbid.ToString();
		pco.info.login = tech.name;

		pco.prefab = new PlayerPrefab();
		queueCreate.Enqueue(pco);

		if (!createCorWork)
			StartCoroutine(CreateProcess());
	}

	public void CreatePlayer(PlayerInfo player) {


#if (UNITY_EDITOR || LOCAL_TEST) && !BATTLE_TEST

		PlayerPrefab pp = new PlayerPrefab();
		pp.prefab = playerPref;
		pp.model = player.model;
		pp.classId = player.class_id;
		pp.race = player.race;
		GenerateInstance(pp, player);

		return;

#endif

		PlayerCreateOrder pco = new PlayerCreateOrder();
		pco.info = player;
		pco.prefab = new PlayerPrefab();
		queueCreate.Enqueue(pco);

		if (!createCorWork)
			StartCoroutine(CreateProcess());

	}

	public void CreatePlayerInEditro(int classId, int race, int model) {

		PlayerInfo newPlayer = new PlayerInfo();
		newPlayer.class_id = classId;
		newPlayer.race = race;
		newPlayer.model = model;
		CreatePlayer(newPlayer);

	}

	private bool createPlayerProcess;
	private bool createCorWork;

	IEnumerator CreateProcess() {
		createCorWork = true;
		while (queueCreate.Count > 0) {
			PlayerCreateOrder newPlayer = queueCreate.Dequeue();

			string playerKey = String.Format("player_high_{0}_{1}_{2}", newPlayer.info.class_id, newPlayer.info.race, newPlayer.info.model);

			//string playerKey = String.Format("player_{0}_{1}_{2}", newPlayer.info.race, newPlayer.info.class_id, newPlayer.info.model);

			if (instancePlayers.ContainsKey(newPlayer.info.pid.ToString())) continue;

			PlayerPrefab playerPrefab = prefabList.Find(x => x.classId == newPlayer.info.class_id && x.race == newPlayer.info.race && x.model == newPlayer.info.model);

			if (playerPrefab == null) {
				createPlayerProcess = true;

				BundleManager.GetBundle(
					String.Format("players/player_high.{0}_{1}_{2}", newPlayer.info.class_id, newPlayer.info.race, newPlayer.info.model),
					String.Format(playerKey),
					(pr) => {
						PlayerPrefab pref = newPlayer.prefab;
						pref.model = newPlayer.info.model;
						pref.classId = newPlayer.info.class_id;
						pref.race = newPlayer.info.race;
						pref.prefab = (GameObject)pr;
						prefabList.Add(pref);
						GenerateInstance(pref, newPlayer.info);
						createPlayerProcess = false;
					});

				while (createPlayerProcess) yield return null;
			} else {
				GenerateInstance(playerPrefab, newPlayer.info);
			}

		}
		ExEvent.BattleEvents.LoadAllModels.Call();
		createCorWork = false;
	}

	[ExEventHandler(typeof(BattleEvents.BattleRoundChange))]
	public void RoundChange(BattleEvents.BattleRoundChange round) {
		roundChange = true;
		ClearSelect();
	}


	[ExEvent.ExEventHandler(typeof(BattleEvents.BattleUpdate))]
	public void ChangeStatePlayer(BattleEvents.BattleUpdate battleUpdate) {

		isMainPlayerChange = true;
		List<string> userKeys = instancePlayers.Keys.ToList();

		foreach (var technic in battleUpdate.battleStart.data.technics) {
			ProcessUpdateTechtic(ref userKeys, technic);
		}

		foreach (var key in battleUpdate.battleStart.data.playersDict.Keys) {
			PlayerInfo pif = battleUpdate.battleStart.data.playersDict[key];
			pif.playerName = key;

			//if (battleUpdate.battleStart.data.players[key].technic != null) {
			//	if (userKeys.Contains(battleUpdate.battleStart.data.players[key].technic.tbid)) {
			//		ProcessUpdateTechtic(ref userKeys, battleUpdate.battleStart.data.players[key].technic);
			//	}
			//}

			if (!instancePlayers.ContainsKey(key) && pif.killed <= 0) {
				CreatePlayer(pif);
				return;
			}


			userKeys.Remove(key);
			
			PlayerBehaviour playerInst = instancePlayers[key];
			playerInst.EnemyDress(pif.GetDress());

			if(pif.class_id <= 4 && !string.IsNullOrEmpty(pif.owner_id) && pif.owner_id != "0")
				playerInst.team = pif.army;
			else
				playerInst.team = -1;

      if (myPlayer != null && pif.pid != myPlayer.pid)
        playerInst.team = -1;


      if (pif.technic != null) {
				if (playerInst.isVisiblePlayer) {
					playerInst.isVisiblePlayer = false;

					if (mainPlayer != null && key == mainPlayer.playerInfo.pid.ToString()) {
						isOsada = true;
						isAttackMode = false;
					}
				}
			}
			else {
				if (!playerInst.isVisiblePlayer) {
					playerInst.isVisiblePlayer = true;
					if (mainPlayer != null && key == mainPlayer.playerInfo.pid.ToString()) {
						isOsada = false;
						isAttackMode = false;
					}
				}
			}

			try {
				if ((mainPlayer != null && playerInst.playerInfo.pid == mainPlayer.playerInfo.pid) && !playerInst.playerInfo.attacked && pif.attacked && playerInst.playerInfo.complete == 0 && pif.complete == 1) {
					playerInst.AttackPlayer(null);
				}
			} catch (Exception ex) {
				Debug.Log(ex.Message);
			}

			playerInst.playerInfo.complete = pif.complete;
			playerInst.playerInfo.attacked = pif.attacked;
			playerInst.playerInfo.hp = pif.hp;
			playerInst.playerInfo.army = pif.army;

			if (/*roundChange &&*/ key == myPlayer.pid.ToString() && (int)pif.x == mainPlayer.actualCell.gridX && (int)pif.y == mainPlayer.actualCell.gridZ) {
				DrawActiveCells();
				//ProjectorManager.Instance.ShowCellReady(mainPlayer.actualCell, battleUpdate.battleStart.poss_move, CellType.moveReady);
				roundChange = false;
			}

			if (battleUpdate.battleStart.magic_cell != null && battleUpdate.battleStart.magic_cell.Length > 0) {
				ProjectorManager.Instance.ShowCellReady(mainPlayer.actualCell, battleUpdate.battleStart.magic_cell, CellType.magic);
			}


			if (pif.killed > 0) {
				if (instancePlayers[key] == selectEnemyPlayer) {
					ProjectorManager.Instance.HideEnemyCell();
				}

				instancePlayers[key].Dead();
				//MiniMapBehaviour.Instance.RemovePlayer(instancePlayers[key]);
				//Destroy(instancePlayers[VARIABLE].gameObject);
				instancePlayers.Remove(key);
			}
			else {
				playerInst.MoveToCell(pif.posX, pif.posY);
			}

		}

		if (userKeys.Count > 0) {
			foreach (var VARIABLE in userKeys) {

				if (!instancePlayers[VARIABLE].isVisible) return;

				if (instancePlayers[VARIABLE] == selectEnemyPlayer) {
					ProjectorManager.Instance.HideEnemyCell();
				}

				if (selectEnemyPlayer == instancePlayers[VARIABLE])
					SelectPlayer(mainPlayer);
				
				instancePlayers[VARIABLE].Dead(false);
				
				MiniMapBehaviour.Instance.RemovePlayer(instancePlayers[VARIABLE]);
				//Destroy(instancePlayers[VARIABLE].gameObject);
				instancePlayers.Remove(VARIABLE);
			}
		}

		//if (userKeys.Count > 0) {
		//	foreach (var VARIABLE in userKeys) {

		//		if(!instancePlayers[VARIABLE].isVisible) return;

		//		if (instancePlayers[VARIABLE] == selectEnemyPlayer) {
		//			ProjectorManager.Instance.HideEnemyCell();
		//		}

		//		instancePlayers[VARIABLE].Dead();
		//		MiniMapBehaviour.Instance.RemovePlayer(instancePlayers[VARIABLE]);
		//		//Destroy(instancePlayers[VARIABLE].gameObject);
		//		instancePlayers.Remove(VARIABLE);
		//	}
		//}

	}

	private void ProcessUpdateTechtic(ref List<string> userKeys, Technic technic) {
		Technic tech = technic;
		tech.playerName = technic.tbid;

		if (!instancePlayers.ContainsKey(technic.tbid)) {
			CreateTechnic(technic);
			return;
		}

		userKeys.Remove(technic.tbid);

		tech.x = tech.x ?? "0";
		tech.y = tech.y ?? "0";

		PlayerBehaviour playerInst = instancePlayers[technic.tbid];
		
		playerInst.MoveToCell(Int32.Parse(tech.x), Int32.Parse(tech.y));
	}


	public void GenerateInstance(PlayerPrefab playerPrefs, PlayerInfo playerInfo) {

		if (instancePlayers.ContainsKey(playerInfo.playerName)) return;

		GameObject instancePlayer = Instantiate(playerPrefs.prefab);
		PlayerBehaviour playerBehaviour = instancePlayer.GetComponent<PlayerBehaviour>();
		
		playerBehaviour.playerInfo = playerInfo;
		instancePlayers.Add(playerInfo.playerName, playerBehaviour);
		
		//MiniMapBehaviour.Instance.AddPlayer(playerBehaviour);

#if UNITY_EDITOR
		instancePlayer.name = playerInfo.playerName;
#endif

		playerInfo.x = playerInfo.x ?? 0;
		playerInfo.y = playerInfo.y ?? 0;

		playerBehaviour.SetPosition((int)playerInfo.x, (int)playerInfo.y);
		instancePlayer.SetActive(true);

		playerBehaviour.EnemyDress(playerInfo.GetDress());
		if (playerInfo.class_id <= 4 && !string.IsNullOrEmpty(playerInfo.owner_id) && playerInfo.owner_id != "0")
			playerBehaviour.team = playerInfo.army;
		else
			playerBehaviour.team = -1;

		if (playerInfo.pid == myPlayer.pid) {
			//CameraController.Instance.transform.position = instancePlayer.transform.position + new Vector3(0, 30, -20);
			CameraController.Instance.GetComponent<FoW.FogOfWar>().team = playerInfo.army;
			mainPlayer = playerBehaviour;
			mainPlayer.isMain = true;
			MainPlayerToCentrCamera();
			mainPlayer.mover.OnMoveStart += () => {
				ProjectorManager.instance.DeactiveMoveCells();
			};
			MiniMapBehaviour.Instance.SetParentPlayer(playerBehaviour);
			ExEvent.BattleEvents.MainPlayerCreate.Call();
		} else {
      playerBehaviour.team = -1;
      playerBehaviour.isMain = false;
			if (mainPlayer != null) {
				playerBehaviour.LockTo(mainPlayer.tr.position);
			}
		}

	}

	public void MainPlayerToCentrCamera() {
		CameraController.Instance.transform.eulerAngles = new Vector3(50,0,0);
		CameraController.Instance.transform.position = mainPlayer.transform.position + new Vector3(0, 30, -20);
	}

	public void MovePlayer(Cell targetCell, NetworkManager.ClickType type) {
		
		NetworkManager.Instance.PlayerMoveTo(new Vector2(targetCell.gridX, targetCell.gridZ), type,  (battleInfo) => {
			BattleManager.Instance.BattleUpdate(battleInfo);
			isMainPlayerChange = true;
		});

	}

	public void ClickCell(Cell clickCell) {

    if (Tutorial.Tutorial.Instance.isActive) {
      if (!Tutorial.Tutorial.Instance.ClickReady(clickCell))
        return;
    }
		
		NetworkManager.ClickType type = NetworkManager.ClickType.none;
		
		if (type == NetworkManager.ClickType.none)
			type = (BattleManager.Instance.CheckMagicCoord(clickCell)
			? NetworkManager.ClickType.magic_cell
			: NetworkManager.ClickType.none);

		if (isOsada && isAttackMode) {

			if (type == NetworkManager.ClickType.none)
				type = (BattleManager.Instance.CheckAttackCoord(clickCell)
					? NetworkManager.ClickType.attack_cell
					: NetworkManager.ClickType.none);
		}
		else {
			if (type == NetworkManager.ClickType.none)
				type = (BattleManager.Instance.CheckMoveCoord(clickCell)
				? NetworkManager.ClickType.move
				: NetworkManager.ClickType.none);
		}
		
		if (type != NetworkManager.ClickType.none) {
			MovePlayer(clickCell, type);
		}

		// Проверяем на выбор оппонента
		PlayerBehaviour selectPlayerNew = GetSelectEnemy(clickCell);
		if (selectPlayerNew == null || selectPlayerNew == selectEnemyPlayer) return;
		SelectPlayer(selectPlayerNew);
	}

	private void DrawActiveCells() {
		
		if (isAttackMode) {
			ProjectorManager.Instance.ShowCellReady(mainPlayer.actualCell, BattleManager.instance.attackPositionList.ToArray(), CellType.attack);
		} else {
			ProjectorManager.Instance.ShowCellReady(mainPlayer.actualCell, BattleManager.instance.movePositionList.ToArray(), CellType.moveReady);
		}

	}

	public void SelectPlayer(PlayerBehaviour selectPlayerNew) {
#if !UNITY_EDITOR
		if (mainPlayer != null && selectPlayerNew == mainPlayer) return;
#endif
		if (selectEnemyPlayer != null && selectEnemyPlayer == selectPlayerNew) return;

		if (selectEnemyPlayer != null) {
			selectEnemyPlayer.mover.OnMoveStart -= SelectPlayerStartMove;
			selectEnemyPlayer.mover.OnMoveEnd -= SelectPlayerEndMove;
			ProjectorManager.Instance.HideEnemyCell();
		}

		selectEnemyPlayer = selectPlayerNew;

		selectEnemyPlayer.mover.OnMoveStart += SelectPlayerStartMove;
		selectEnemyPlayer.mover.OnMoveEnd += SelectPlayerEndMove;

		mainPlayer.LockTo(selectEnemyPlayer.tr.position);

		SelectPlayerEndMove();

		ExEvent.GameEvents.PlayerSelect.Call(selectEnemyPlayer);
	}

	public void PlayerAttack() {
		mainPlayer.AttackPlayer(selectEnemyPlayer);
	}

	private PlayerBehaviour GetSelectEnemy(Cell selectCell) {
		foreach (var key in instancePlayers.Keys) {
			if (instancePlayers[key].actualCell.gridX == selectCell.gridX &&
					instancePlayers[key].actualCell.gridZ == selectCell.gridZ)
				return instancePlayers[key];
		}
		return null;
	}

	void SelectPlayerStartMove() {
		ProjectorManager.Instance.HideEnemyCell();
	}

	void SelectPlayerEndMove() {
		ProjectorManager.Instance.ShowEnemyCell(selectEnemyPlayer.actualCell, selectEnemyPlayer.playerInfo.army == mainPlayer.playerInfo.army);
	}


	#region Атака и защита


	public BodyElement attackLeft { get; set; }
	public BodyElement attackRight { get; set; }
	public BodyElement attackHand2 { get; set; }
	public BodyElement shieldRight1 { get; set; }
	public BodyElement shieldRight2 { get; set; }
	public BodyElement shieldLeft1 { get; set; }
	public BodyElement shieldLeft2 { get; set; }

	// Сброс защиты
	public void ClearProtect() {
		shieldRight1 = BodyElement.none;
		shieldRight2 = BodyElement.none;
		shieldLeft1 = BodyElement.none;
		shieldLeft2 = BodyElement.none;
	}

	// Установка защиты
	public void SetProtected(List<List<int>> data) {
		shieldRight1 = (BodyElement)data[1][0];
		shieldRight2 = (BodyElement)data[1][1];
		shieldLeft1 = (BodyElement)data[2][0];
		shieldLeft2 = (BodyElement)data[2][1];

		if (shieldRight1 != BodyElement.none || shieldRight2 != BodyElement.none)
			SetAttack(BodyElement.none, AttackHand.right, false);

		if (shieldLeft1 != BodyElement.none || shieldLeft2 != BodyElement.none)
			SetAttack(BodyElement.none, AttackHand.left, false);
	}

	public void SetAttack(BodyElement bodyElement, AttackHand hand, bool sendBrowser = true) {

		if (mainPlayer == null) return;
		if (attackReadyArr == null && !isMainPlayerChange) return;

		if (isMainPlayerChange) {
			isMainPlayerChange = false;
			AttackButton(true, (result) => {
				try {
					attackReadyArr = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckAttackArray>(result);
				} catch {
					attackReadyArr = null;
				}

				SetAttackReady(bodyElement, hand, sendBrowser);

			});
		} else {
			SetAttackReady(bodyElement, hand, sendBrowser);
		}

	}
	
	public void SetAttackReady(BodyElement bodyElement, AttackHand hand, bool sendBrowser = true) {

		if (attackReadyArr == null || attackReadyArr.attacks == null) return;



    if (hand == AttackHand.right && attackReadyArr.attacks["1"][0] <= 0 && attackReadyArr.attacks["3"][0] <= 0) return;
		if (hand == AttackHand.left && attackReadyArr.attacks["2"][0] <= 0 && attackReadyArr.attacks["3"][0] <= 0) return;

		if (attackReadyArr.attacks["3"][0] > 0)
			hand = AttackHand.all;

		int[] arraySend = new int[4];

		switch (hand) {
			case AttackHand.left:
				attackLeft = bodyElement == attackLeft ? BodyElement.none : bodyElement;
				shieldLeft1 = BodyElement.none;
				shieldLeft2 = BodyElement.none;
				break;
			case AttackHand.right:
				attackRight = bodyElement == attackRight ? BodyElement.none : bodyElement;
				shieldRight1 = BodyElement.none;
				shieldRight2 = BodyElement.none;
				break;
			case AttackHand.all:
			default:
				attackRight = BodyElement.none;
				attackLeft = BodyElement.none;
				attackHand2 = bodyElement == attackRight ? BodyElement.none : bodyElement;
				shieldLeft1 = BodyElement.none;
				shieldLeft2 = BodyElement.none;
				shieldRight1 = BodyElement.none;
				shieldRight2 = BodyElement.none;
				break;
		}

		arraySend[1] = (int)attackLeft;
		arraySend[2] = (int)attackRight;
		arraySend[3] = (int)attackHand2;


    if (Tutorial.Tutorial.Instance.isActive)
      Tutorial.Tutorial.Instance.SetAttack(bodyElement, (int)hand);

    if (attackReadyArr != null) {
			if (sendBrowser)
				BrowserContact.Instance.OnBattleSetAttack(Newtonsoft.Json.JsonConvert.SerializeObject(arraySend));

			ExEvent.GameEvents.ChangeAttackTargets.Call(attackLeft, attackRight, shieldLeft1, shieldLeft2, shieldRight1,
				shieldRight2);
		}

	}


	private bool isMainPlayerChange = true;   // Какое либо изменение состояния игрока
	private CheckAttackArray attackReadyArr;

	public void AttackButton(bool isCheck, Action<string> complitedCallback, Action OnFalse = null) {

		Debug.Log("AttackButton");

		string queryComb = isCheck ? "&id_query=get_comb" : "&id_query=complete";

		string queryKicks = String.Format("&data[kicks][1][0]={0}&data[kicks][2][0]={1}&data[kicks][3][0]={2}",
			(int)attackRight, (int)attackLeft, (int)attackHand2);
		string queryBlock =
			String.Format(
				"&data[blocks][1][0]={0}&data[blocks][1][1]={1}&data[blocks][2][0]={2}&data[blocks][2][1]={3}&data[blocks][3][0]={4}&data[blocks][3][1]={5}&data[blocks][3][2]={6}&data[blocks][3][3]={7}",
				attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["1"][0] > 0 ? (int)shieldRight1 : 0,
				attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["1"][0] > 0 ? (int)shieldRight2 : 0,
				attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["2"][0] > 0 ? (int)shieldLeft1 : 0,
				attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["2"][0] > 0 ? (int)shieldLeft2 : 0,
				attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["3"][0] > 0 ? (int)shieldRight1 : 0,
				attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["3"][0] > 0 ? (int)shieldRight2 : 0,
				attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["3"][0] > 0 ? (int)shieldLeft1 : 0,
				attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["3"][0] > 0 ? (int)shieldRight2 : 0);
		string queryType = String.Format("&data[type]={0}", 1);

		NetworkManager.Instance.Attack(queryComb + queryKicks + queryBlock + queryType, (data) => {

			CheckAttack result = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckAttack>(data);

			if (result.result != "complete_ok") {
				if (OnFalse != null) OnFalse();
			}


      if (Tutorial.Tutorial.Instance.isActive)
        Tutorial.Tutorial.Instance.AttackButton();

      if (!isCheck)
				PlayersManager.Instance.PlayerAttack();

			complitedCallback(data);
			if (!isCheck) {
				isMainPlayerChange = true;
			}
			
		});
		
	}

	/// <summary>
	/// Завершение раунда
	/// </summary>
	public void EndRoundButton(Action complitedCallback, Action OnFalse = null) {

		string queryKicks = String.Format("&id_query=complete&data[kicks][1][0]={0}&data[kicks][2][0]={1}&data[kicks][3][0]={2}", (int)attackRight, (int)attackLeft, (int)attackHand2);
		string queryBlock = String.Format("&data[blocks][1][0]={0}&data[blocks][1][1]={1}&data[blocks][2][0]={2}&data[blocks][2][1]={3}&data[blocks][3][0]={4}&data[blocks][3][1]={5}&data[blocks][3][2]={6}&data[blocks][3][3]={7}",
																			attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["1"][0] > 0 ? (int)shieldRight1 : 0,
																			attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["1"][0] > 0 ? (int)shieldRight2 : 0,
																			attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["2"][0] > 0 ? (int)shieldLeft1 : 0,
																			attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["2"][0] > 0 ? (int)shieldLeft2 : 0,
																			attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["3"][0] > 0 ? (int)shieldRight1 : 0,
																			attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["3"][0] > 0 ? (int)shieldRight2 : 0,
																			attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["3"][0] > 0 ? (int)shieldLeft1 : 0,
																			attackReadyArr != null && attackReadyArr.blocks != null && attackReadyArr.blocks["3"][0] > 0 ? (int)shieldRight2 : 0);
		string queryType = String.Format("&data[type]={0}", 2);

		NetworkManager.Instance.EndRound(queryKicks + queryBlock + queryType, (data) => {


			CheckAttack result = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckAttack>(data);

			if (result.result != "complete_ok") {
				if (OnFalse != null) OnFalse();
			}
			
			complitedCallback();
			isMainPlayerChange = true;
		});
	}

	public void ClearSelect(bool sendBrowser = false) {
		attackLeft = BodyElement.none;
		attackRight = BodyElement.none;

		SetAttack(attackLeft, AttackHand.left, sendBrowser);
		SetAttack(attackRight, AttackHand.right, sendBrowser);
	}

	#endregion

	public void PlayerChangeDress() {
		mainPlayer.LoadItemsInfo();
	}


	public bool isFriendLight = true;
	public bool isEnemyLight = true;
	public void SetFriendLight() {
		isFriendLight = !isFriendLight;

		BattleEvents.OnChangeFriendLight.Call();

	}

	public void SetEnemyLight() {
		isEnemyLight = !isEnemyLight;

		BattleEvents.OnChangeEnemyLight.Call();
	}
	
}

// 10 - голова
// 20 - туловища
// 30 - правая рука
// 31 - левая рука
// 40 - ноги
// Тип 1 - атака, 2 - защита
public enum BodyElement {
	none = 0,
	head = 10,
	body = 20,
	rightHand = 30,
	leftHand = 31,
	leg = 40
}

public enum AttackHand {
	none = 0,
	right = 1,      // правая
	left = 2,     // левая
	all = 3     // двуруник (копье, лук)
}

[System.Serializable]
public class PlayerPrefab {
	public int race;
	public int classId;
	public int model;
	public GameObject prefab;
}

[System.Serializable]
public class PlayerCreateOrder {
	public PlayerPrefab prefab;
	public PlayerInfo info;
}

[System.Serializable]
public class CheckAttack {
	public string result;
}

[System.Serializable]
public class CheckAttackArray {
	public Dictionary<string, int[]> attacks;
	public Dictionary<string, int[]> blocks;
}