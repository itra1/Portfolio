using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cells;

/// <summary>
/// Контроллер проекта
/// </summary>
public class ProjectorBehaviour : MonoBehaviour {

	public Projector projector;
	private float orthographicSize;

	public Material defaultMaterial;
	public Material openMaterial;
	public Material blockMaterial;
	public Material myMaterial;
	public Material magicMaterial;
	public Material attackMaterial;

	public Material allFriendMaterial;
	public Material allEnemyMaterial;

	public void Init(Cell cellsource) {
		cellsource.OnChangeType = ChangeCelltype;
	}


	void ChangeCelltype(Cell cellsource) {

	}

	public void SetScale(float orthScale) {
		if(orthographicSize == 0)
			orthographicSize = projector.orthographicSize;
		projector.orthographicSize = orthographicSize * orthScale;
	}

	public void SetCell(Cell cell) {
		SetCell(cell.position, cell.type);
	}

	public void SetCell(Vector3 position, CellType cellType) {
		transform.position = position;
		switch (cellType) {
			case CellType.enemySelect:
				projector.material = blockMaterial;
				break;
			case CellType.close:
				projector.material = blockMaterial;
				break;
			case CellType.my:
				projector.material = myMaterial;
				break;
			case CellType.moveReady:
				projector.material = defaultMaterial;
				break;
			case CellType.friendLight:
				projector.material = allFriendMaterial;
				break;
			case CellType.enemyLight:
				projector.material = allEnemyMaterial;
				break;
			case CellType.magic:
				projector.material = magicMaterial;
				break;
			case CellType.attack:
				projector.material = attackMaterial;
				break;
			case CellType.friendSelect:
			default:
				projector.material = openMaterial;
				break;
		}
	}
}
