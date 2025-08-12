using System.Collections.Generic;
using UnityEngine;
using Cells;
using ExEvent;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class MapManagerEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		
	}
}

#endif


public class GameManager : Singleton<GameManager> {
	
	public ClickHelper mapClick;
	public CellDrawner cellDrawner;

	public LayerMask groundMask;

	public ProjectorManager projectorManager;

	public Transform world;

	protected override void Awake() {
		base.Awake();

	}

	public void SetParam(string data) {
		
	}

	private void Start() {

    // Выполняем запрос
#if !UNITY_EDITOR && UNITY_WEBGL

      BrowserContact.Instance.OnGetCookie("tok", (data) => {
        Debug.Log("Browser contact GetCookie " + data);
        NetworkManager.Instance.tok = data;
        StartBattle();
      });
    
#else

		NetworkManager.Instance.Authorization(StartBattle);
		
#endif
	}

	protected override void OnDestroy() {
		base.OnDestroy();
	}
	
	/// <summary>
	/// Запускаем бой
	/// </summary>
	private void StartBattle() {

		//NetworkManager.instance.CustomRequest("/game/battles/battle_test.php", (el) => { });
		

		NetworkManager.Instance.BattleStart((res) => {

			if (res == null) {
				Debug.Log("Пустой ответ от сервера");
				Invoke("StartBattle", 0.1f);
			}
			
			if (res.id_answer == "map_close") {
				Debug.Log("Бой не запущен");
				return;
			}

			// Выполняем инифиализацию боя
			BattleManager.Instance.SetBattleInfo(res);

		});
		
	}

	[ExEventHandler(typeof(GameEvents.MapClick))]
	private void OnClick(GameEvents.MapClick mapClick) {
		
		Cell cell = cellDrawner.GetGrideByPoint(GetMapPosition(mapClick.position));
		if (cell == null) return;
		
		PlayersManager.Instance.ClickCell(cell);
	}

	public Vector3 GetMapPosition(Vector2 mouseDisplayPosition) {
		RaycastHit hitinfo;
		Physics.Raycast(CameraController.Instance.transform.position, Camera.main.ScreenToWorldPoint(new Vector3(mouseDisplayPosition.x, mouseDisplayPosition.y, 1)) - CameraController.Instance.transform.position, out hitinfo, 1000, groundMask);
		return hitinfo.point;
	}
	
	/// <summary>
	/// Отрисовать сетку
	/// </summary>
	public void DrawGride() {
		cellDrawner.CalcGrid();
		ShowGride();
	}

	/// <summary>
	/// Показать сетку
	/// </summary>
	public void ShowGride() {
		projectorManager.DrawAllGreed(MapManager.Instance.map.cellList);
	}


}
