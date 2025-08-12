using UnityEngine;
using Cells;
using ExEvent;

public class ProjectorSource : EventBehaviour {

	private ProjectorBehaviour projector;
	private bool _isFriend = false;
	private bool _isEnable;
	private bool _isVisible;
	private bool _isMain;
	
	public bool isFriend {
		set {
			_isFriend = value;
			projector.projector.gameObject.SetActive(_isEnable && _isVisible && !_isMain);
			DrawnGride(transform.position);
		}
		get { return _isFriend; }
	}

	public bool isEnable {
		set {
			_isEnable = value;
			projector.projector.gameObject.SetActive(_isEnable && _isVisible && !_isMain);
		}
	}

	public bool isVisible {

		set {
			_isVisible = value;
			projector.projector.gameObject.SetActive(_isEnable && _isVisible && !_isMain);
		}
	}

	public bool isMain {

		set {
			_isMain = value;
			projector.projector.gameObject.SetActive(_isEnable && _isVisible && !_isMain);
		}
	}

	private void OnEnable() {
		GetInstances();
		OnChangeEnemyLight(new BattleEvents.OnChangeEnemyLight());
		OnChangeFriendLight(new BattleEvents.OnChangeFriendLight());
		//DrawnGride();
	}

	public void GetInstances() {

		try {
			GameObject pr = Instantiate(ProjectorManager.Instance.prefabProjector.gameObject);
		projector = pr.GetComponent<ProjectorBehaviour>();
		projector.SetScale(MapManager.Instance.map.cellSize);
		pr.SetActive(true);
		} catch { }
	}

	public void DrawnGride(Vector3 position) {
		try {
			if (projector == null)
				GetInstances();

			projector.SetCell(CellDrawner.Instance.GetGrideByPoint(position).position,
				isFriend ? CellType.friendLight : CellType.enemyLight);
		} catch { }
	}

	[ExEvent.ExEventHandler(typeof(BattleEvents.OnChangeEnemyLight))]
	private void OnChangeEnemyLight(BattleEvents.OnChangeEnemyLight cl) {
		if (!isFriend) {
			isEnable = PlayersManager.Instance.isEnemyLight;
		}
	}

	[ExEvent.ExEventHandler(typeof(BattleEvents.OnChangeFriendLight))]
	private void OnChangeFriendLight(BattleEvents.OnChangeFriendLight cl) {
		if (isFriend) {
			isEnable = PlayersManager.Instance.isFriendLight;
		}
	}

}
