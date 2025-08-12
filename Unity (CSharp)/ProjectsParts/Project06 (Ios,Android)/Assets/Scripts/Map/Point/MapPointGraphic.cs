using UnityEngine;
using System.Collections;
using FoW;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapPointGraphic))]
[CanEditMultipleObjects]
public class MapPointGraphicEditor : Editor
{
  public override void OnInspectorGUI() {

    base.OnInspectorGUI();

    if (GUILayout.Button("CreatePointer"))
    {
      foreach (var target in targets)
      {
        ((MapPointGraphic)target).CreatePointer();
      }
    }

  }


} 

#endif

[System.Serializable]
public class MapPointGraphic: MonoBehaviour {
  
  [SerializeField]
  private int group;
  public int Group {
    get {
      return group;
    }
  }
  [SerializeField]
  private int level;
  public int Level {
    get {
      return level;
    }
  }

  [SerializeField]
  private MapPoint mapPoint;

  [SerializeField]
  private GameObject graphic;

  public TextMesh noAccessText;

  private bool _isActive;
  private bool IsActive {
    set {
      _isActive = value;
    }
  }

  private LevelInfo _levelInfo;
  public LevelInfo LevelInfo {
    get {
      if (_levelInfo == null)
        _levelInfo = LevelsManager.Instance.LevelsList.Find(x => x.Group == group && x.Level == level);
      return _levelInfo;
    }
    set {
      _levelInfo = value;
      Initiate();
    }
  }

  bool isClick;

  void Start() {
    MapManager.OnDragCamera += OnDragCamera;
  }

  void OnDestroy() {
    MapManager.OnDragCamera -= OnDragCamera;
  }

  public void OnDragCamera() {
    if (!_isActive) return;
    isClick = false;
  }

  private bool _isActiveClick;

  public bool IsVisible {
    set {
      _isActiveClick = value;
      graphic.SetActive(_isActiveClick);
    }
    get {
      return _isActiveClick;
    }
  }


  public void Initiate() {
    InitData();
    //graphic.SetActive(false);
    mapPoint.transform.Find("TextMeshPro").gameObject.SetActive(false);
    mapPoint.enemyImage.transform.parent.GetComponent<Canvas>().sortingLayerName = "MapPoint";
  }
  
  /// <summary>
  /// Инифиализация данных по точке
  /// </summary>
  /// <param name="newPointInfo"></param>
  public void InitData() {

    IsVisible = LevelInfo.Status != PointSatus.blocked;

    if (!IsVisible) return;

    if (LevelInfo.Mode != PointMode.farm) {
      mapPoint.enemyImage.gameObject.SetActive(true);
    }
    mapPoint.ShowTitle(false);

    mapPoint.SetSections(LevelInfo.FarmPointActive, LevelInfo.FarmPoint);

    Configuration.Level lid = GameDesign.Instance.allConfig.levels.Find(x => x.chapter == LevelInfo.Group && x.level == LevelInfo.Level);

    if (lid != null) {
      mapPoint.ShowTitle(false, lid.title);
      //mapPoint.title.text = lid.title;
      transform.localScale = new Vector3(lid.mapLevelSize, lid.mapLevelSize, lid.mapLevelSize);
    }
    mapPoint.ShowTitle(false);

    LevelInfo.Change();

    EnemyType enemyType = EnemyType.DremuchiyRisovod;
    try {
      enemyType = (EnemyType)GameDesign.Instance.allConfig.chapters.Find(x => x.level == LevelInfo.Level && x.chapter == LevelInfo.Group && x.mobType == "keyEnemy").idMob;
    } catch { }

    if (enemyType == EnemyType.None)
      enemyType = EnemyType.DremuchiyRisovod;

    EnemyInfo ei = GameDesign.Instance.enemyInfo.Find(x => x.type == enemyType);
    mapPoint.enemyImage.sprite = ei.icon;

    GetComponent<FogOfWarUnit>().enabled = ((LevelInfo.Status & (PointSatus.IsActive)) != 0);
  }


  public void OnPointerDown() {
    if (!_isActiveClick) return;
    isClick = true;
    PlayClickAudio();
  }

  public AudioBlock clickAudioBlock;

  protected virtual void PlayClickAudio() {
    if (clickAudioBlock != null) clickAudioBlock.PlayRandom(this);
  }

  public void OnPointerUp() {
    if (!_isActiveClick) return;
    if ((LevelInfo.Status & ( PointSatus.closed | PointSatus.blocked) ) != 0) return;

    if ((LevelInfo.Status & (PointSatus.closed | PointSatus.complited))!= 0) {
      if (NoAccessShowCor != null) StopCoroutine(NoAccessShowCor);
      NoAccessShowCor = StartCoroutine(NoAccessShow());
      return;
    }

    MapManager.Instance.TapMapPoint(transform.position, LevelInfo);
  }
  
  public void OnPointerEnter() {
    isForcus = true;
    if (isNoAccessText) return;
    mapPoint.ShowTitle(true);
  }

  public void OnPointerExit() {
    isForcus = false;
    mapPoint.ShowTitle(false);
  }

  private bool isForcus;
  private bool isNoAccessText;
  private Coroutine NoAccessShowCor;
  IEnumerator NoAccessShow() {
    UIController.RejectPlay();
    isNoAccessText = true;
    mapPoint.ShowTitle(false);
    //mapPoint.title.transform.parent.gameObject.SetActive(false);
    //noAccessText.gameObject.SetActive(true);
    yield return new WaitForSeconds(1);
    isNoAccessText = false;
    //noAccessText.gameObject.SetActive(false);
    mapPoint.ShowTitle(isForcus);
    //mapPoint.title.transform.parent.gameObject.SetActive(isForcus);
  }
  [ContextMenu("Complete")]
  public void CompletePoint() {
    LevelInfo li = LevelsManager.Instance.LevelsList.Find(x => x.Group == Group && x.Level == Level);
    li.SetComplete(10);
    MapManager.Instance.PointInitiate();
  }

#if UNITY_EDITOR

  public void CreatePointer() {
    DestroyImmediate(GetComponent<CircleCollider2D>());

    GameObject pref = transform.parent.Find("PointerEventer").gameObject;
    GameObject inst = Instantiate(pref, transform);
    inst.SetActive(true);
    inst.transform.localPosition = Vector3.zero;
    inst.transform.localScale = pref.transform.localScale;
    EventTrigger et = inst.GetComponent<EventTrigger>();
    
  }

#endif

}
