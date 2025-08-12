using System.Collections;
using System.Collections.Generic;
using GameCompany;
using UnityEngine;
using ExEvent;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LocationsWorld))]
public class LocationsWorldEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Recalc"))
			((LocationsWorld)target).LoadLocations(true);

	}
}

#endif

public class LocationsWorld : WorldAbstract {

	public Transform islandsParent;
	public GameObject locationPrefab;
	private List<IslandLocationsWorld> instIslandList = new List<IslandLocationsWorld>();
	public List<IslandLocationsWorld> islandList = new List<IslandLocationsWorld>();

	private float _diff;

	private Vector3 _targetPosition;

	private int _actualPage = 0;

	private bool _uiStatusTitle;
	private bool _uiStatusArrow;

	private bool uiStatusArrow {
		set {
			_uiStatusArrow = value;
		}
	}

	private bool uiStatusTitle {
		set {
			_uiStatusTitle = value;
		}
	}

	private void Start() {

		_diff = Camera.main.ViewportToWorldPoint(Vector3.one).x * 2;

		if (PlayerManager.Instance.company.CheckFullInit())
			LoadLocations();
	}

	private LocationsGamePlay _locationsGamePlay;
	private void OnEnable() {
		_locationsGamePlay = UIManager.Instance.GetPanel(UiType.locations) as LocationsGamePlay;
		if (PlayerManager.Instance.company.CheckFullInit()) {
			LoadLocations();
		}
	}

	public void LoadLocations(bool force = false) {

		if (!force && islandList.Count == PlayerManager.Instance.company.GetActualCompany().locations.Count + 1) return;

		StartCoroutine(SpawnIsland());

	}

	private bool isSpawnLocation = false;
	private bool readySpawn = false;

	IEnumerator SpawnIsland() {

		if (isSpawnLocation) {
			readySpawn = true;
			yield break;
		}

		isSpawnLocation = true;

		HideExists();

		//if (PlayerManager.Instance.company.companies.Count == 0) yield break;

		GameCompany.Company useCompany = PlayerManager.Instance.company.GetActualCompany();

		for (int i = 0; i < useCompany.locations.Count; i++) {

			IslandLocationsWorld com = GetInstance();
			com.transform.localPosition = new Vector3(_diff * i, 0, 0);
			com.gameObject.SetActive(true);

			com.location = PlayerManager.Instance.company.GetActualCompany().locations[i];

			com.SetData(i, useCompany.locations[i]);

			com.OnComplited = () => {
			};

			com.OnDownIsland = DownIsland;
			com.OnUpIsland = UpIsland;

			islandList.Add(com);
			yield return null;
		}
		AddShareIsland();
		//_actualPage = 0;

		_locationsGamePlay.ChangeLocation();

		isSpawnLocation = false;

		if (readySpawn) {
			StartCoroutine(SpawnIsland());
			readySpawn = false;
		}

		OnLoadPlayer();

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.OnAddDecor))]
	public void ChengeAchive(ExEvent.PlayerEvents.OnAddDecor decor) {
		StartCoroutine(SpawnIsland());
	}

	[ExEventHandler(typeof(ExEvent.PlayerEvents.OnChangeCompany))]
	void OnChangeCompany(PlayerEvents.OnChangeCompany cmp) {
		LoadLocations(cmp.force);
	}


	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.OnByeLocation))]
	void OnByeLocation(ExEvent.PlayerEvents.OnByeLocation onLoad) {
		var actualPage = _actualPage;
		LoadLocations(onLoad.force);
		_actualPage = actualPage;
		_locationsGamePlay.ChangeLocation();
	}

	/// <summary>
	/// Добавляем исланд шаринг
	/// </summary>
	private void AddShareIsland() {

		int numPos = islandList.Count;

		IslandLocationsWorld com = GetInstance();
		com.transform.localPosition = new Vector3(_diff * numPos, 0, 0);
		com.gameObject.SetActive(true);

		//com.location = PlayerManager.Instance.GetActualCompany().locations[numPos];

		com.SetIsShare(numPos);

		com.OnComplited = () => {
		};

		//com.OnDownIsland = DownIsland;
		//com.OnUpIsland = UpIsland;

		islandList.Add(com);
	}


	private void HideExists() {
		instIslandList.ForEach(x => x.gameObject.SetActive(false));
		islandList.Clear();
	}

	private IslandLocationsWorld GetInstance() {
		IslandLocationsWorld ilw = instIslandList.Find(x => !x.gameObject.activeInHierarchy);

		if (ilw == null) {
			GameObject inst = Instantiate(locationPrefab);
			inst.transform.SetParent(islandsParent);
			ilw = inst.GetComponent<IslandLocationsWorld>();
			instIslandList.Add(ilw);
		}
		return ilw;
	}

	public void SelectFocusIsland() {
		IslandLocationsWorld loc = islandList[_actualPage];
		SelectIsland(loc);
	}

	public IslandLocationsWorld GetFocusIsland() {
		if (_actualPage == -1 || islandList.Count < _actualPage + 1)
			return new IslandLocationsWorld();
		return islandList[_actualPage];
	}

	private bool downIsland = false;
	private void DownIsland(IslandLocationsWorld slnd) {
		downIsland = true;
	}

	private void UpIsland(IslandLocationsWorld slnd) {

		if (downIsland)
			SelectIsland(slnd);
		downIsland = false;
	}

	public void SelectIsland(IslandLocationsWorld slnd) {
		if (_isMove) return;

		// Пробуе загрузить если возможно
		//if (slnd.location.levels.Count != slnd.location.levelsCount) {

		//	if (slnd.isBye) {
		//		//if (!PlayerManager.Instance.company.isLoadProcess) return;

		//		//Locker.Instance.SetLocker(true);
		//		//PlayerManager.Instance.company.AllDownload(() => {
		//		//	Locker.Instance.SetLocker(false);
		//		//});
		//		var actualPage = _actualPage;
		//		LoadLocations(true);
		//		_actualPage = actualPage;
		//		_locationsGamePlay.ChangeLocation();
		//	}

		//	return;
		//}

		// Разрешваем вход, если уровень пройден не полностью
		//if (slnd.isComplete) return;

		if (!slnd.isOpen) {
			AudioManager.Instance.library.PlayClickInactiveAudio();
			slnd.PlayLocked();
			return;
		}
		AudioManager.Instance.library.PlayClickAudio();

		//if (PlayerManager.Instance.company.actualLocationNum > 0) {
		//	// Запускаем видео
		//	GoogleAdsMobile.Instance.ShowRewardVideo( GoogleAdsMobile.VideoType.video,  () => {
		//		OpenLevel(slnd);
		//	});
		//	return;
		//}
		OpenLevel(slnd);

	}

	private void OpenLevel(IslandLocationsWorld slnd) {

		PlayerManager.Instance.company.actualLocationNum = slnd.num;

		GameManager.Instance.ToLevels(() => {
		});
		GameManager.gamePhase = GamePhase.levels;
	}


	void OnLoadPlayer() {
		var actualPage = _actualPage;
		//LoadLocations();

		if (GameManager.gamePhase != GamePhase.locations) {
			bool isPosition = false;

			for (int i = 0; i < islandList.Count; i++) {
				if (i == islandList.Count - 1 || isPosition) continue;
				if (!PlayerManager.Instance.company.CheckCompleteLocation(i)) {
					isPosition = true;
					_actualPage = i;
					var _diff = Camera.main.ViewportToWorldPoint(Vector3.one).x * 2;
					islandsParent.position = new Vector3(-_diff * _actualPage, islandsParent.position.y, islandsParent.position.z);
				}
			}
			if (!isPosition) {
				_actualPage = Mathf.Max(islandList.Count - 2, 0);

				var _diff = Camera.main.ViewportToWorldPoint(Vector3.one).x * 2;
				islandsParent.position = new Vector3(-_diff * _actualPage, islandsParent.position.y, islandsParent.position.z);
				//StartMove();
			}
		}
		_locationsGamePlay.ChangeLocation();

		ExEvent.GameEvents.SwipeComplete.Call();
	}

	// Стрелка в право
	public void RightArrow() {
		if (_isMove) return;
		_isPointer = false;

		if (_actualPage >= islandList.Count - 1 && !_changePageInSwipe) {
			_targetPosition.y = islandsParent.position.y;
			StartMove();
			return;
		}

		float dif = islandsParent.position.x + (_actualPage + (_changePageInSwipe ? 0 : 1)) * _diff;
		_targetPosition = islandsParent.position + new Vector3(-dif, 0, 0);

		if (dif == 0) {
			uiStatusTitle = false;
			uiStatusArrow = false;
			return;
		}
		uiStatusTitle = true;
		_arrowMove = true;

		PlaySwipeAudio();

		//_targetPosition = islandsParent.position - new Vector3(islandList[_actualPage].transform.localPosition.x, 0, 0);
		StartMove();
	}

	// Стрелка влево
	public void LeftArrow() {
		if (_isMove) return;
		_isPointer = false;

		if (_actualPage <= 0 && !_changePageInSwipe) {
			_targetPosition.y = islandsParent.position.y;
			StartMove();
			return;
		}

		float dif = islandsParent.position.x + (_actualPage - (_changePageInSwipe ? 0 : 1)) * _diff;
		_targetPosition = islandsParent.position + new Vector3(-dif, 0, 0);

		if (dif == 0) {
			uiStatusTitle = false;
			uiStatusArrow = false;
			return;
		}

		uiStatusTitle = true;
		_arrowMove = true;

		PlaySwipeAudio();
		//_targetPosition = islandsParent.position + new Vector3(islandList[_actualPage].transform.localPosition.x, 0, 0);
		StartMove();
	}

	void CancelMove() {
		if (moveCor != null)
			StopCoroutine(moveCor);
	}
	void StartMove() {
		moveCor = StartCoroutine(DiffMove());
	}

	private bool _isPointer;
	public float _lastDelta;
	public void PointerDown() {
		if (_arrowMove) return;
		_isPointer = true;
		CancelMove();
	}

	public void PointerUp() {
		if (!_isPointer) return;
		_isPointer = false;

		if (Mathf.Abs(_lastDelta) > 0.1f && !_changePageInSwipe) {
			if (Mathf.Sign(_lastDelta) <= 0) {
				RightArrow();
			} else {
				LeftArrow();
			}
			return;
		} else {

			float minDiff = 9999999;
			for (int i = 0; i < islandList.Count; i++) {
				float dif = islandsParent.position.x + i * _diff;
				if (Mathf.Abs(dif) < Mathf.Abs(minDiff)) {
					_actualPage = i;
					minDiff = dif;
				}
			}
			_targetPosition = islandsParent.position + new Vector3(-minDiff, 0, 0);

		}

		StartMove();
	}

	public void Swipe(float delta) {
		if (_arrowMove) return;

		uiStatusTitle = true;
		uiStatusArrow = true;
		downIsland = false;

		_lastDelta = delta;
		islandsParent.position += Vector3.right * delta;

		if (Mathf.Abs(delta) > 0.5f)
			PlaySwipeAudio();

		ChangePosition();
	}

	public void BackLocations() {
		IslandLocationsWorld ilw = islandList[PlayerManager.Instance.company.actualLocationNum];
		ilw.gameObject.SetActive(true);
	}

	public void PlayerButton() {

	}

	private Coroutine moveCor;

	private float _moveSpeed = 30;
	private bool _isMove;
	private bool _arrowMove;
	IEnumerator DiffMove() {
		_isMove = true;

		while (_isMove) {

			Vector3 diff = _targetPosition - islandsParent.position;
			if (diff.magnitude <= (diff.normalized * _moveSpeed * Time.deltaTime).magnitude) {
				islandsParent.position = _targetPosition;
				_isMove = false;
				uiStatusTitle = false;
				uiStatusArrow = false;
				_arrowMove = false;
				_changePageInSwipe = false;
			} else {
				islandsParent.position += diff.normalized * _moveSpeed * Time.deltaTime;
			}
			ChangePosition();

			yield return null;

		}
		ChangePosition();
		ExEvent.GameEvents.SwipeComplete.Call();

	}

	void ChangePosition() {
		float delta = 0;

		if (Mathf.Abs(islandList[_actualPage].transform.position.x) < _diff / 3)
			delta = 1 - Mathf.Abs(islandList[_actualPage].transform.position.x) / (_diff / 3);

		ExEvent.GameEvents.SwipeLocation.Call(delta);
	}

	private bool _changePageInSwipe;

	[ExEventHandler(typeof(GameEvents.SwipeLocation))]
	public void SwipeLocation(GameEvents.SwipeLocation cll) {

		float midDist = 9999;
		int pageMin = -1;

		for (int i = 0; i < islandList.Count; i++) {
			if (Mathf.Abs(islandList[i].transform.position.x) < midDist) {
				midDist = Mathf.Abs(islandList[i].transform.position.x);
				pageMin = i;
			}
		}

		if (pageMin != _actualPage) {
			_changePageInSwipe = true;
			_actualPage = pageMin;
			_locationsGamePlay.ChangeLocation();
		}

	}

	private AudioPoint swipeSourcePlay;
	public AudioClipData swipeAudio;
	public void PlaySwipeAudio() {

		if (swipeSourcePlay != null && swipeSourcePlay.isPlaing) return;

		swipeSourcePlay = AudioManager.PlayEffects(swipeAudio, AudioMixerTypes.effectUi, swipeSourcePlay != null ? swipeSourcePlay : null);
		swipeSourcePlay.isLocked = true;
	}


}
