using System;
using System.Collections;
using System.Collections.Generic;
using Cells;
using PathFinder;
using UnityEngine;

public class MoveBehaviour : MonoBehaviourBase {

	public event Action OnMoveStart;
	
	public event Action OnMoveEnd;
	

	public float deffSpeed = 3;
	public float moveSpeed;
	public PlayerBehaviour _playerBehaviour;
	private IAnimationPlayer _playerAnimation;
	public IAnimationPlayer playerAnimation {
		get {
			if (_playerAnimation == null)
				_playerAnimation = GetComponent<IAnimationPlayer>();
			return _playerAnimation;
		}
	}

	private List<Cell> targetListCell = new List<Cell>();
	private Cell moveCell;

	private Transform moveTransform {
		get { return _ishorse ? _horse.transform : transform; }
	}
	
	private void Awake() {
		_playerBehaviour = GetComponent<PlayerBehaviour>();
	}

	private void OnEnable() {
		moveSpeed = deffSpeed * transform.localScale.y;
	}

	List<Cell> GetActualCells() {
		List<Cell> readyCells = new List<Cell>();

		if (BattleManager.Instance.lastmovePositionList.Count > 0) {
			for (int i = 0; i < BattleManager.Instance.lastmovePositionList.Count; i++)
				readyCells.Add(
					MapManager.Instance.map.cellList.Find(
						x =>
							x.gridX == BattleManager.Instance.lastmovePositionList[i].x &&
							x.gridZ == BattleManager.Instance.lastmovePositionList[i].y));
		}

		//foreach (var key in PlayersManager.Instance.instancePlayers.Keys) {
		//		readyCells.Add(PlayersManager.Instance.instancePlayers[key].moveBehaviour.moveCell);
		//}
		
		return readyCells;
	}
	
	public void MoveToCell(Cell targetCell) {

		List<Cell> cellListMove = GetActualCells();

		if (cellListMove.Count > 0) {
			targetListCell = PathFirder.FindPath(GetActualCells(), _playerBehaviour.actualCell, targetCell);
		}
		else {
			targetListCell = null;
		}

		if (targetListCell == null) {
			targetListCell = PathFirder.FindPath(MapManager.Instance.map.cellList, _playerBehaviour.actualCell, targetCell);
		}

		targetListCell.RemoveAt(0);
		Vector3[] vl = new Vector3[targetListCell.Count];

		for (int i = 0; i < targetListCell.Count; i++)
			vl[i] = targetListCell[i].position;

		playerAnimation.SetMove(true);
		moveCell = targetListCell[0];
		_playerBehaviour.phase = PlayerPhase.move;
		if (OnMoveStart != null) OnMoveStart();
	}

	private void Update() {
		if (_playerBehaviour.phase == PlayerPhase.move) Move();

		if (_playerBehaviour.phase == PlayerPhase.move) {
			if (moveCell != null) {
				
			}

		}
		else {
			moveTransform.rotation = Quaternion.Slerp(moveTransform.rotation, _lookDirection, Time.deltaTime * 8);
			moveTransform.localEulerAngles = new Vector3(0, moveTransform.localEulerAngles.y, 0);
			if (_ishorse) {
				transform.rotation = moveTransform.rotation;
			}
		}
		//}
		//else {
		//	if(_lookDirection != null)
		//		transform.rotation = Quaternion.Slerp(transform.rotation, _lookDirection, Time.deltaTime * 8);
		//}
	}

	private Vector3 velocity = Vector3.zero;
	public Vector3 newPoint;

	void Move() {

		//if (moveCell == null) moveCell = targetListCell[0];

		velocity = (moveCell.position - new Vector3(moveTransform.position.x, 0, moveTransform.position.z)).normalized;
		newPoint = moveTransform.position + velocity * moveSpeed * Time.deltaTime;
		
		_lookDirection = Quaternion.Lerp(moveTransform.rotation,
					Quaternion.FromToRotation(Vector3.forward, velocity),20f * Time.deltaTime);
		moveTransform.rotation = Quaternion.Slerp(moveTransform.rotation, _lookDirection, Time.deltaTime * 8);
		moveTransform.localEulerAngles = new Vector3(0, moveTransform.localEulerAngles.y, 0);
		if (_ishorse) {
			transform.rotation = moveTransform.rotation;
		}

		if ((newPoint - moveTransform.position).magnitude > (moveCell.position - moveTransform.position).magnitude) {
			targetListCell.Remove(moveCell);
			_playerBehaviour.actualCell = moveCell;
			moveCell = null;

			if (targetListCell.Count == 0) {
				_playerBehaviour.phase = PlayerPhase.idle;
				playerAnimation.SetMove(false);
				if (OnMoveEnd != null) OnMoveEnd();
				return;
			}
			else {
				if (targetListCell.Count > 0)
					moveCell = targetListCell[0];
				else
					moveCell = null;
			}
		}
		else {
			moveTransform.position = newPoint;
		}


		//if ((moveCell.position - new Vector3(moveTransform.position.x, 0, moveTransform.position.z)).magnitude <= 0.15) {
		//	targetListCell.Remove(moveCell);
		//	_playerBehaviour.actualCell = moveCell;
		//	moveCell = null;

		//	if (targetListCell.Count == 0) {
		//		_playerBehaviour.phase = PlayerPhase.idle;
		//		playerAnimation.SetMove(false);
		//		if (OnMoveEnd != null) OnMoveEnd();
		//		return;
		//	}

		//}
	}

	private bool _ishorse;
	private HorseBehaviour _horse;
	public void SetHorse(bool isHorse, HorseBehaviour horse = null) {
		_ishorse = isHorse;
		_horse = horse;
	}
	

	private Quaternion _lookDirection;

	public void RotationTo(Vector3 lookDirection) {
		if (_playerBehaviour.phase == PlayerPhase.move) return;
		_lookDirection = Quaternion.LookRotation(lookDirection - moveTransform.position);
		//transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, velocity), 20f * Time.deltaTime);
		//transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 8);
	}

}

[System.Serializable]
public struct MovePosition {
	public int x;
	public int y;
}