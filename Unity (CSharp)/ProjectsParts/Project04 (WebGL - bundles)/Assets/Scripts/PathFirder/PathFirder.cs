using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Cells;

namespace PathFinder {

	public class PathFirder : MonoBehaviour {

		private void Start() { }

		public class PathNode {
			// Координаты точки на карте.
			public Cell cell { get; set; }
			// Длина пути от старта (G).
			public float PathLengthFromStart { get; set; }
			// Точка, из которой пришли в эту точку.
			public PathNode CameFrom { get; set; }
			// Примерное расстояние до цели (H).
			public float HeuristicEstimatePathLength { get; set; }
			// Ожидаемое полное расстояние до цели (F).
			public float EstimateFullPathLength {
				get { return this.PathLengthFromStart + this.HeuristicEstimatePathLength; }
			}
		}


		public void FindPathInst(List<Cell> cells, Cell start, Cell goal) {
			StartCoroutine(ShowPathFinding(cells, start, goal));
		}

		public static List<Cell> FindPath(List<Cell> cells, Cell start, Cell goal) {

			//cells.Add(start);
			cells.Add(goal);

			// Шаг 1.
			var closedSet = new Collection<PathNode>();
			var openSet = new Collection<PathNode>();
			// Шаг 2.
			PathNode startNode = new PathNode() {
				cell = start,
				CameFrom = null,
				PathLengthFromStart = 0,
				HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
			};
			openSet.Add(startNode);
			while (openSet.Count > 0) {
				// Шаг 3.
				var currentNode = openSet.OrderBy(node => node.EstimateFullPathLength).First();
				// Шаг 4.
				if (currentNode.cell.EqualsRound(goal))
					return GetPathForNode(currentNode);
				// Шаг 5.
				openSet.Remove(currentNode);
				closedSet.Add(currentNode);
				// Шаг 6.
				foreach (var neighbourNode in GetNeighbours(currentNode, goal, cells)) {
					// Шаг 7.
					if (closedSet.Count(node => node.cell.EqualsRound(neighbourNode.cell)) > 0)
						continue;
					var openNode = openSet.FirstOrDefault(node => node.cell.position == neighbourNode.cell.position);
					// Шаг 8.
					if (openNode == null)
						openSet.Add(neighbourNode);
					else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart) {
						// Шаг 9.
						openNode.CameFrom = currentNode;
						openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
					}
				}
			}
			// Шаг 10.
			return null;
		}

		public static IEnumerator ShowPathFinding(List<Cell> cells, Cell start, Cell goal) {
			var closedSet = new Collection<PathNode>();
			var openSet = new List<PathNode>();
			// Шаг 2.
			PathNode startNode = new PathNode() {
				cell = start,
				CameFrom = null,
				PathLengthFromStart = 0,
				HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
			};
			openSet.Add(startNode);
			while (openSet.Count > 0) {
				// Шаг 3.
				var currentNode = openSet.OrderBy(node => node.EstimateFullPathLength).First();
				// Шаг 4.
				if (currentNode.cell.EqualsRound(goal)) {
					GetPathForNode(currentNode);
					yield break;
				}
				// Шаг 5.
				openSet.Remove(currentNode);
				closedSet.Add(currentNode);
				yield return null;
				// Шаг 6.
				foreach (var neighbourNode in GetNeighbours(currentNode, goal, cells)) {
					// Шаг 7.
					if (closedSet.Count(node => node.cell.EqualsRound(neighbourNode.cell)) > 0)
						continue;
					var openNode = openSet.FirstOrDefault(node => node.cell.position == neighbourNode.cell.position);
					Debug.Log(neighbourNode.cell.position);
					// Шаг 8.
					if (openNode == null)
						openSet.Add(neighbourNode);
					else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart) {
						// Шаг 9.
						openNode.CameFrom = currentNode;
						openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
					}
				}
			}
			// Шаг 10.
			yield return null;
		}

		private static int GetDistanceBetweenNeighbours() {
			return 1;
		}

		private static float GetHeuristicPathLength(Cell from, Cell to) {
			//return (int)(Mathf.Abs(from.position.x - to.position.y) + Mathf.Abs(from.position.y - to.position.y));
			return (to.position - from.position).magnitude;
		}

		private static Collection<PathNode> GetNeighbours(PathNode pathNode, Cell goal, List<Cell> cells) {
			var result = new Collection<PathNode>();

			Vector3 positRound = CellDrawner.Instance.RoundGrideByPosion(pathNode.cell.position);

			// Соседними точками являются соседние по стороне клетки.
			Cell[] neighbourPoints = new Cell[6];

			Cell cell1 = CellDrawner.Instance.GetCellByGride(new Vector2(pathNode.cell.gridX + 1, pathNode.cell.gridZ));
			neighbourPoints[0] = cell1;

			Cell cell2 = CellDrawner.Instance.GetCellByGride(new Vector2(pathNode.cell.gridX - 1, pathNode.cell.gridZ));
			neighbourPoints[1] = cell2;

			Cell cell3 = CellDrawner.Instance.GetCellByGride(new Vector2(pathNode.cell.gridX, pathNode.cell.gridZ + 1));
			neighbourPoints[2] = cell3;

			Cell cell4 = CellDrawner.Instance.GetCellByGride(new Vector2(pathNode.cell.gridX, pathNode.cell.gridZ - 1));
			neighbourPoints[3] = cell4;

			if (pathNode.cell.gridZ % 2 == 1) {

				Cell cell5 = CellDrawner.Instance.GetCellByGride(new Vector2(pathNode.cell.gridX + 1, pathNode.cell.gridZ - 1));
				neighbourPoints[4] = cell5;

				Cell cell6 = CellDrawner.Instance.GetCellByGride(new Vector2(pathNode.cell.gridX + 1, pathNode.cell.gridZ + 1));
				neighbourPoints[5] = cell6;

			} else {

				Cell cell5 = CellDrawner.Instance.GetCellByGride(new Vector2(pathNode.cell.gridX - 1, pathNode.cell.gridZ - 1));
				neighbourPoints[4] = cell5;

				Cell cell6 = CellDrawner.Instance.GetCellByGride(new Vector2(pathNode.cell.gridX - 1, pathNode.cell.gridZ + 1));
				neighbourPoints[5] = cell6;

			}

			//neighbourPoints[0] = new Cell(pathNode.cell.position.x + CellDrawner.Instance.cellSizeX, pathNode.cell.position.z);
			//neighbourPoints[1] = new Cell(pathNode.cell.position.x - CellDrawner.Instance.cellSizeX, pathNode.cell.position.z);
			//neighbourPoints[2] = new Cell(pathNode.cell.position.x, pathNode.cell.position.z + CellDrawner.Instance.cellSizeZ);
			//neighbourPoints[3] = new Cell(pathNode.cell.position.x, pathNode.cell.position.z - CellDrawner.Instance.cellSizeZ);

			foreach (var point in neighbourPoints) {

				if (point == null) continue;
				if(!cells.Exists(x=>x.gridX == point.gridX && x.gridZ == point.gridZ)) continue;

				//if (point.instant != null) {
				//	point.instant.sp.color = Color.cyan;
				//}

				// Проверяем, что не вышли за границы карты.
				//if (point.X < 0 || point.X >= cells.GetLength(0))
				//	continue;
				//if (point.Y < 0 || point.Y >= cells.GetLength(1))
				//	continue;
				// Проверяем, что по клетке можно ходить.
				//if (cells[(int)point.X, (int)point.Y].type != CellType.open)
				//	continue;
				// Заполняем данные для точки маршрута.
				var neighbourNode = new PathNode() {
					cell = point,
					CameFrom = pathNode,
					PathLengthFromStart = pathNode.PathLengthFromStart + (float)Math.Round((pathNode.cell.position - point.position).magnitude,3),
					HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
				};
				result.Add(neighbourNode);
			}
			return result;
		}

		private static List<Cell> GetPathForNode(PathNode pathNode) {
			var result = new List<Cell>();
			var currentNode = pathNode;
			while (currentNode != null) {
				result.Add(currentNode.cell);
				currentNode = currentNode.CameFrom;
			}
			result.Reverse();
			return result;
		}

	}
}