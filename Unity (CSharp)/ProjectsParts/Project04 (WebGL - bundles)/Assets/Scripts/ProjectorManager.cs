using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cells;
using ExEvent;
using Network.Input;

public class ProjectorManager : Singleton<ProjectorManager> {

	private List<ProjectorBehaviour> instantList = new List<ProjectorBehaviour>();
	private ProjectorBehaviour instanceSelectPlayer;

	public ProjectorBehaviour prefabProjector;

	public void DrawAllGreed(List<Cell> cellList) {

		foreach (var element in cellList) {
			ProjectorBehaviour pb = GetProjector(false);
			pb.transform.position = element.position;
		}
	}

	public void ShowEnemyCell(Cell cellPos, bool isFriend) {
		if (instanceSelectPlayer == null)
			instanceSelectPlayer = CreateInstance(false);
		instanceSelectPlayer.gameObject.SetActive(true);
		instanceSelectPlayer.SetCell(cellPos.position, isFriend ? CellType.friendSelect : CellType.enemySelect);
	}


	public void HideEnemyCell() {
		if (instanceSelectPlayer != null) instanceSelectPlayer.gameObject.SetActive(false);
	}

	public void DeactiveMoveCells() {
		instantList.ForEach(x => x.gameObject.SetActive(false));
	}

	public void ShowCellReady(Cell myPosition, MovePosition[] movePositions, CellType type) {

		DeactiveMoveCells();

		CreateMyPrijector(myPosition);
		
		foreach (var movePositionElem in movePositions) {

			ProjectorBehaviour useMb = instantList.Find(x => !x.isActiveAndEnabled);

			if (useMb == null)
				useMb = CreateInstance(false);

			Cell useCell = CellDrawner.Instance.GetCellByGride(new Vector2(movePositionElem.x, movePositionElem.y));

			if (useCell == null) continue;

			useMb.SetCell(useCell.position, type);
			useMb.gameObject.SetActive(true);
			instantList.Add(useMb);
		}
    ExEvent.BattleEvents.LoadNewCells.Call();
	}

	void CreateMyPrijector(Cell myPosition) {
		ProjectorBehaviour useMb = instantList.Find(x => !x.isActiveAndEnabled);

		if (useMb == null)
			useMb = CreateInstance(false);

		useMb.SetCell(myPosition.position, CellType.my);
		useMb.gameObject.SetActive(true);
		instantList.Add(useMb);
	}

	ProjectorBehaviour CreateInstance(bool allList = true) {
		GameObject inst = Instantiate(prefabProjector.gameObject);
		inst.transform.SetParent(transform);
		inst.SetActive(false);

		ProjectorBehaviour pb = inst.GetComponent<ProjectorBehaviour>();
		pb.SetScale(MapManager.Instance.map.cellSize);
		if (allList)
			instantList.Add(pb);

		return pb;
	}

	public ProjectorBehaviour GetProjector(bool reUse = false) {

		ProjectorBehaviour pb = null;

		if (reUse) {
			pb = instantList.Find(x => !x.gameObject.activeInHierarchy);
		}

		if (pb == null || reUse) {
			GameObject inst = Instantiate(prefabProjector.gameObject);
			inst.transform.SetParent(transform);
			inst.SetActive(false);
			pb = inst.GetComponent<ProjectorBehaviour>();
			instantList.Add(inst.GetComponent<ProjectorBehaviour>());
		}

		return pb;
	}

}
