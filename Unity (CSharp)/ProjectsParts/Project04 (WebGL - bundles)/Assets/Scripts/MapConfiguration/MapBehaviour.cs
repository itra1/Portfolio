using System.Collections.Generic;
using Cells;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapBehaviour : Singleton<MapBehaviour> {
	
	public MapBehaviour source { get; set; }
	
	[Tooltip("Ширина карты")]
	public int cellWidth;
	[Tooltip("Длина карты")]
	public int cellLength;
	[Tooltip("Размер ячейки")]
	public float cellSize;

	public float playerSize = 5;
	public float fogOfWarRound;

	public MiniMapConfig miniMap;

	public Vector3 startGridePoint;

	public OutLineParam outLineParam;


	public List<Cell> cellList {
		get {
			if (_cellList == null) {
				DrawGride();
			}
			return _cellList;
		}
		set { _cellList = value; }
	}

	public List<Cell> _cellList;

	public void DrawGride() {

		CellDrawner cellDrawner;
		GameManager gm;

		if (Application.isPlaying) {
			cellDrawner = CellDrawner.Instance;
			gm = GameManager.Instance;
		}
		else {
			gm = GameObject.Find("Managers").GetComponent<GameManager>();
			cellDrawner = GameObject.Find("Managers").GetComponent<CellDrawner>();
		}

		cellDrawner.CalcGrid();

		gm.mapClick.GetComponent<BoxCollider>().center = new Vector3(cellDrawner.pointStart.x + ((cellDrawner.pointDiagon.x - cellDrawner.pointStart.x)/2), 0, cellDrawner.pointStart.z - ((cellDrawner.pointStart.z - cellDrawner.pointDiagon.z) / 2));
		gm.mapClick.GetComponent<BoxCollider>().size = new Vector3((cellDrawner.pointDiagon.x - cellDrawner.pointStart.x)+500, 0, (cellDrawner.pointStart.z- cellDrawner.pointDiagon.z) +500);

		if (cellDrawner == null) return;

		CameraController.Instance.leftBorder = cellDrawner.pointStart.x;
		CameraController.Instance.rightBorder = cellDrawner.pointDiagon.x;
		CameraController.Instance.bottomBorder  = cellDrawner.pointDiagon.z;
		CameraController.Instance.topborder = cellDrawner.pointStart.z;

	}

	public void CalcMinimapRegion() {
		//miniMap.regionPointStart.x = startGridePoint.x - (cellSize*0.433f) - (cellSize * 3);
		//miniMap.regionPointStart.y = startGridePoint.z - (cellSize * 0.5f) - (cellSize * 3);
		//miniMap.regionPointEnd.x = startGridePoint.x + (cellSize * cellWidth * 0.866f) + (cellSize * 3);
		//miniMap.regionPointEnd.y = startGridePoint.z + (cellSize * cellLength * 0.75f) - (cellSize*0.25f) + (cellSize * 3);

		miniMap.regionPointStart.x = startGridePoint.x - (cellSize * 0.433f) - (cellSize * 3);
		miniMap.regionPointStart.y = startGridePoint.z - (cellSize * cellLength * 0.75f) - (cellSize * 0.25f) - (cellSize * 3);
		miniMap.regionPointEnd.x = startGridePoint.x + (cellSize * cellWidth * 0.866f) + (cellSize * 3);
		miniMap.regionPointEnd.y = startGridePoint.z + (cellSize * 0.5f) + (cellSize * 3);
	}

#if UNITY_EDITOR
	public void SaveObject() {

		PrefabUtility.ReplacePrefab(gameObject, PrefabUtility.GetPrefabParent(gameObject),
			ReplacePrefabOptions.ConnectToPrefab);

		string oldPath = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(gameObject));
		//AssetDatabase.RenameAsset(oldPath, "Map" + mapId);

	}
#endif

	public void RecalcNet() {


		CellDrawner cellDrawner;
		GameManager gm;


		if (Application.isPlaying) {
			cellDrawner = CellDrawner.Instance;
			gm = GameManager.Instance;
		} else {
			gm = GameObject.Find("Managers").GetComponent<GameManager>();
			cellDrawner = GameObject.Find("Managers").GetComponent<CellDrawner>();
			var mm = GameObject.Find("Managers").GetComponent<MapManager>();
			mm.map = this;
		}
		//startGridePoint.z += ((cellLength-1) * cellDrawner.cellDistanceZ);

		cellDrawner.CalcGridInverce();
	}

	[System.Serializable]
	public struct OutLineParam {
		public bool isDrawn;
		public Color color;
		public float lineWeight;
	}

}
