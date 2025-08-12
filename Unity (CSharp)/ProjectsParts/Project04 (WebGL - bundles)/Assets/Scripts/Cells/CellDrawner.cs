using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cells {

#if UNITY_EDITOR
	using UnityEditor;

	[CustomEditor(typeof(CellDrawner))]
	public class CellDrawnerEditor : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (GUILayout.Button("Рассчитать сетку")) {
				((CellDrawner)target).CalcGrid();

				//EditorUtility.SetDirty((CellDrawner)target);
			}

			if (GUILayout.Button("Отрисовать сетку")) {
				((CellDrawner)target).DrawGrid();

				//EditorUtility.SetDirty((CellDrawner)target);
			}

			if (GUILayout.Button("Уничтожить сетку")) {
				((CellDrawner)target).DestroyGrid();

				//EditorUtility.SetDirty((CellDrawner)target);
			}

			if (GUILayout.Button("Уничтожить сетку")) {
				((CellDrawner)target).DestroyGrid();

				//EditorUtility.SetDirty((CellDrawner)target);
			}

			if (GUILayout.Button("Отрисовать границу сетки")) {
				((CellDrawner)target).DrawOutLine();

				//EditorUtility.SetDirty((CellDrawner)target);
			}
			

			if (GUILayout.Button("Отрисовать сетку")) {
				((CellDrawner)target).GrideRenderer();

				//EditorUtility.SetDirty((CellDrawner)target);
			}

		}
	}

#endif

	public class CellDrawner : Singleton<CellDrawner> {

		public Color gridColor;
		public LineRenderer lineRenderer;

		public MapManager mapManager;
		public CellRendererManager cellRenderer;

		public float cellDistanceX { get { return mapManager.map.cellSize * 0.866f; } }
		public float cellDistanceZ { get { return mapManager.map.cellSize * 0.75f; } }

		public int grideCountX { get { return mapManager.map.cellWidth; } }
		public int grideCountZ { get { return mapManager.map.cellLength; } }

		public float mapWidth { get { return grideCountX * cellDistanceX; } }
		public float mapLength { get { return grideCountZ * cellDistanceZ; } }

		public Vector3 pointStart {
			get { return new Vector3(mapManager.map.startGridePoint.x - (cellDistanceX / 2), 0, mapManager.map.startGridePoint.z + (mapManager.map.cellSize / 2)); }
		}

		public Vector3 pointDiagon {
			get { return new Vector3(mapManager.map.startGridePoint.x + cellDistanceX * grideCountX, 0, mapManager.map.startGridePoint.z - (cellDistanceZ * (grideCountZ - 1)) - (mapManager.map.cellSize / 2)); }
		}

		public Transform cellParent;
		public CellBehaviour cellPrefab;

		protected override void Awake() {
			base.Awake();
		}
		private void Start() { }

		private void OnDrawGizmos() {

			if (mapManager == null || mapManager.map == null) return;

			for (int i = 0; i < mapManager.map.cellList.Count; i++) {
				GizmoDrawCell(mapManager.map.cellList[i]);
			}

			DrawGizmosMapBorder();

			if (mapManager != null && mapManager.map != null) {
				Gizmos.color = Color.green;
#if UNITY_EDITOR
				mapManager.map.miniMap.DrawRegion();
#endif
			}

		}

		private void Update() {
			if (!isDrawn && mapManager.map != null) {
				DrawOutLine();
			}
		}

		private bool isDrawn;
		public void DrawOutLine() {
			//if (mapManager == null) return;
			isDrawn = true;

			List<Vector3> positionList = new List<Vector3>();

			for (int i = 0; i < mapManager.map.cellList.Count; i++) {
				if (mapManager.map.cellList[i].gridZ == 0) {
					positionList.Add(mapManager.map.cellList[i].position + new Vector3(mapManager.map.cellSize * -0.433f, 0, mapManager.map.cellSize * 0.25f));
					positionList.Add(mapManager.map.cellList[i].position + new Vector3(0, 0, mapManager.map.cellSize / 2));
				}
			}
			for (int i = 0; i < mapManager.map.cellList.Count; i++) {
				if (mapManager.map.cellList[i].gridX == grideCountX - 1) {
					positionList.Add(mapManager.map.cellList[i].position + new Vector3(mapManager.map.cellSize * 0.433f, 0, mapManager.map.cellSize * 0.25f));
					positionList.Add(mapManager.map.cellList[i].position + new Vector3(mapManager.map.cellSize * 0.433f, 0, mapManager.map.cellSize * -0.25f));
				}
			}
			for (int i = mapManager.map.cellList.Count - 1; i >= 0; i--) {
				if (mapManager.map.cellList[i].gridZ == grideCountZ - 1) {
					positionList.Add(mapManager.map.cellList[i].position + new Vector3(0, 0, -mapManager.map.cellSize / 2));
					positionList.Add(mapManager.map.cellList[i].position + new Vector3(mapManager.map.cellSize * -0.433f, 0, -mapManager.map.cellSize * 0.25f));
				}
			}
			for (int i = mapManager.map.cellList.Count - 1; i >= 0; i--) {
				if (mapManager.map.cellList[i].gridX == 0) {
					positionList.Add(mapManager.map.cellList[i].position + new Vector3(mapManager.map.cellSize * -0.433f, 0, mapManager.map.cellSize * -0.25f));
					positionList.Add(mapManager.map.cellList[i].position + new Vector3(mapManager.map.cellSize * -0.433f, 0, mapManager.map.cellSize * 0.25f));
				}
			}
			//positionList.Add(mapManager.map.cellList[0].position + new Vector3(0, 0, mapManager.map.cellSize / 2));

			lineRenderer.positionCount = positionList.Count;
			lineRenderer.SetPositions(positionList.ToArray());
		}

		public void GrideRenderer() {
			cellRenderer.RendererGride(mapManager.map.cellList, mapManager.map.cellSize);
		}

		void GizmoDrawCell(Cell cell) {
			Debug.DrawLine(cell.position + new Vector3(0, 0, mapManager.map.cellSize / 2), cell.position + new Vector3(mapManager.map.cellSize * 0.433f, 0, mapManager.map.cellSize * 0.25f), gridColor);
			Debug.DrawLine(cell.position + new Vector3(0, 0, mapManager.map.cellSize / 2), cell.position + new Vector3(mapManager.map.cellSize * -0.433f, 0, mapManager.map.cellSize * 0.25f), gridColor);

			Debug.DrawLine(cell.position + new Vector3(0, 0, -mapManager.map.cellSize / 2), cell.position + new Vector3(mapManager.map.cellSize * 0.433f, 0, mapManager.map.cellSize * -0.25f), gridColor);
			Debug.DrawLine(cell.position + new Vector3(0, 0, -mapManager.map.cellSize / 2), cell.position + new Vector3(mapManager.map.cellSize * -0.433f, 0, mapManager.map.cellSize * -0.25f), gridColor);

			Debug.DrawLine(cell.position + new Vector3(mapManager.map.cellSize * 0.433f, 0, mapManager.map.cellSize * 0.25f), cell.position + new Vector3(mapManager.map.cellSize * 0.433f, 0, mapManager.map.cellSize * -0.25f), gridColor);
			Debug.DrawLine(cell.position + new Vector3(mapManager.map.cellSize * -0.433f, 0, mapManager.map.cellSize * 0.25f), cell.position + new Vector3(mapManager.map.cellSize * -0.433f, 0, mapManager.map.cellSize * -0.25f), gridColor);
		}

		private void DrawGizmosMapBorder() {

			Gizmos.color = Color.blue;
			Gizmos.DrawLine(pointStart, new Vector3(pointDiagon.x, 0, pointStart.z));
			Gizmos.DrawLine(new Vector3(pointStart.x, 0, pointDiagon.z), pointDiagon);

			Gizmos.DrawLine(pointStart, new Vector3(pointStart.x, 0, pointDiagon.z));
			Gizmos.DrawLine(new Vector3(pointDiagon.x, 0, pointStart.z), pointDiagon);
			
			//Gizmos.DrawLine(new Vector3(mapManager.map.startGridePoint.x, 0, mapLength / 2), new Vector3(mapWidth / 2, 0, mapLength / 2));
			//Gizmos.DrawLine(new Vector3(mapManager.map.startGridePoint.x, 0, -mapLength / 2), new Vector3(mapManager.map.startGridePoint.x, 0, mapLength / 2));
			//Gizmos.DrawLine(new Vector3(mapWidth / 2, 0, -mapLength / 2), new Vector3(mapWidth / 2, 0, mapLength / 2));
		}


		public void ClearGrid() {
			while (cellParent.childCount > 0) {
				DestroyImmediate(cellParent.GetChild(0).gameObject);
			}
		}

		public void DestroyGrid() {
			while (cellParent.childCount > 0) {
				if (!Application.isPlaying) {
					DestroyImmediate(cellParent.GetChild(0).gameObject);
				} else {
					Destroy(cellParent.GetChild(0).gameObject);
				}
			}
			mapManager.map.cellList.Clear();
		}

		public Vector3 RoundGrideByPosion(Vector3 point) {
			//Vector3 position = new Vector3(mapWidth / 2 + point.x, 0, mapLength / 2 + point.z);
			Vector3 position = new Vector3(point.x - mapManager.map.startGridePoint.x, 0, mapManager.map.startGridePoint.z - point.z);
			return new Vector3(Mathf.Abs(Mathf.RoundToInt(position.x / cellDistanceX)), 0, Math.Abs(Mathf.RoundToInt(position.z / cellDistanceZ)));
		}

		public Cell GetGrideByPoint(Vector3 point) {

			//if (cellGrid == null || cellGrid.GetLength(0) == 0 || cellGrid.GetLength(1) == 0) return null;
			if (mapManager.map.cellList.Count <= 0) return null;


			Vector3 targetPoint = RoundGrideByPosion(point);

			float distance = cellDistanceX;
			Cell cell = null;

			for (int i = (int)targetPoint.x - 1; i <= (int)targetPoint.x + 1; i++) {
				for (int j = (int)targetPoint.z - 1; j <= (int)targetPoint.z + 1; j++) {
					//if (i < 0 || j < 0 || cellGrid.GetLength(0) - 1 < i || cellGrid.GetLength(1) - 1 < j) continue;
					if (i < 0 || j < 0 || i >= grideCountX || j >= grideCountZ || (i == grideCountX && j > 0)) continue;
					//Cell trg = cellGrid[i, j];
					Cell trg = mapManager.map.cellList[i * grideCountZ + j];
					float magn = (trg.position - new Vector3(point.x, 0, point.z)).magnitude;
					if (magn < distance) {
						distance = magn;
						cell = trg;
					}
				}
			}

			return cell;

		}

		public Cell GetCellByGride(Vector2 grydePos) {
			if (grydePos.x < 0 || grydePos.x > grideCountX || grydePos.y < 0 || grydePos.y > grideCountZ) return null;

			return mapManager.map.cellList.Find(x => x.gridX == (int)grydePos.x && x.gridZ == (int)grydePos.y);

			//return mapManager.map.cellList[(int)grydePos.x * grideCountZ + (int)grydePos.y];
		}

		public void CalcGrid() {

			CalcGridInverce();
			return;


			mapManager.map.cellList.Clear();
			//ClearGrid();

			for (int i = 0; i < grideCountX; i++) {
				for (int j = 0; j < grideCountZ; j++) {
					//GameObject inst = Instantiate(cellPrefab.gameObject);
					//inst.transform.SetParent(cellParent);
					//inst.transform.position = GetGridPosition(i, j);

					Cell cellElement = new Cell();
					cellElement.position = GetGridPosition(i, j);
					cellElement.gridX = i;
					cellElement.gridZ = mapManager.map.cellLength - j;
					//cellElement.instant = inst.GetComponent<CellBehaviour>();
					cellElement.type = CellType.moveReady;
#if UNITY_EDITOR
					//inst.name = inst.name + "_" + i + "_" + j;
#endif
					//cellGrid[i, j] = cellElement;
					mapManager.map.cellList.Add(cellElement);
				}
			}
		}

		public void CalcGridInverce() {
			mapManager.map.cellList.Clear();

			for (int i = 0; i < grideCountX; i++) {
				for (int j = 0; j < grideCountZ; j++) {
					//GameObject inst = Instantiate(cellPrefab.gameObject);
					//inst.transform.SetParent(cellParent);
					//inst.transform.position = GetGridPosition(i, j);

					Cell cellElement = new Cell();
					cellElement.position = GetGridPositionInvert(i, j);
					cellElement.gridX = i;
					cellElement.gridZ = j;
					//cellElement.instant = inst.GetComponent<CellBehaviour>();
					cellElement.type = CellType.moveReady;
#if UNITY_EDITOR
					//inst.name = inst.name + "_" + i + "_" + j;
#endif
					//cellGrid[i, j] = cellElement;
					mapManager.map.cellList.Add(cellElement);
				}
			}

		}
		public Vector3 GetGridPositionInvert(int indexX, int indexZ) {
			return new Vector3((float)Math.Round(mapManager.map.startGridePoint.x + indexX * cellDistanceX + (indexZ % 2 > 0 ? cellDistanceX / 2 : 0), 3),
													0,
													(float)Math.Round(mapManager.map.startGridePoint.z - (indexZ * cellDistanceZ), 3));
		}

		public void DrawGrid() {
			ProjectorManager.Instance.DrawAllGreed(mapManager.map.cellList);
		}

		public Vector3 GetGridPosition(int indexX, int indexZ) {
			return new Vector3((float)Math.Round(mapManager.map.startGridePoint.x + indexX * cellDistanceX + (indexZ % 2 > 0 ? cellDistanceX / 2 : 0), 3),
													0,
													(float)Math.Round(mapManager.map.startGridePoint.z + (indexZ * cellDistanceZ), 3));
		}

	}

}