using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.User;

public class CameraMapMove: Singleton<CameraMapMove> {

  [SerializeField]
  private Camera camera;
  [SerializeField]
  private Camera cameraFog;

  private System.Action callbackMove;
  private System.Action callbackOpenBrifing;
  [SerializeField]
  private AnimationCurve curveAnimate;

  [SerializeField]
  private AnimationCurve curveMove;
  [SerializeField]
  private AnimationCurve curveZoom;

  [SerializeField]
  private SpanFloat verticalBorder;
  [SerializeField]
  private SpanFloat horizontalBorder;

  private bool zoomComplited = false;

  private float zoomSpeed = .3f;          // Скорость скролирования карты
  [SerializeField]
  private SpanFloat zoomBorders;         // Границы скролирования карты
  private float pixelSize { get { return (camera.orthographicSize * 2) / Screen.height; } }               // Дельта скролирование

  [SerializeField]
  private Phases phase;

  private enum Phases {
    free,
    move,
    zoomIn,
    zoomOut,
    stay
  }

  private void Start() {

    ScrollPosition();

    SetScrollValue((UserManager.Instance.MapZoom != 0 ? UserManager.Instance.MapZoom : (zoomBorders.max - zoomBorders.min) / 2 + zoomBorders.min));

    if (camera.orthographicSize < zoomBorders.min)
      camera.orthographicSize = zoomBorders.min;
    cameraFog.orthographicSize = camera.orthographicSize;

  }
  
  private void Update() {

    if (phase == Phases.move)
      MoveTarget();

    if (phase == Phases.zoomIn)
      ZoomTarget();

    if (phase == Phases.zoomOut)
      ZoomBack();

  }

  private void Save() {

  }

  private void Load() {

  }

  private Vector3 targetPosition;

  public void ZoomPoint(Vector3 targetPosition, System.Action callBack) {
    if (phase != Phases.free)
      return;
    callbackOpenBrifing = callBack;
    this.targetPosition = targetPosition;
    phase = Phases.move;
  }
  public void CancelZoomPoint() {
    phase = Phases.zoomOut;
  }

  private void MoveTarget() {

    Vector3 newPosition = camera.transform.position + (targetPosition - camera.transform.position).normalized * Time.deltaTime * 5;

    if((newPosition - camera.transform.position).magnitude < (targetPosition - camera.transform.position).magnitude) {
      camera.transform.position = newPosition;
    } else {
      camera.transform.position = targetPosition;
      phase = Phases.zoomIn;
    }

  }

  private void ZoomBack() {

    if(camera.orthographicSize >= zoomBorders.min) {
      SetScrollValue(zoomBorders.min,false);
      phase = Phases.free;
      return;
    }

    SetScrollValue(camera.orthographicSize + 2f * Time.deltaTime);

  }

  private void ZoomTarget() {

    if (camera.orthographicSize <= zoomBorders.min - 1.5f) {
      SetScrollValue(zoomBorders.min - 1.5f, false);
      phase = Phases.stay;
      if (callbackOpenBrifing != null)
        callbackOpenBrifing();
      return;
    }

    SetScrollValue(camera.orthographicSize - 2f * Time.deltaTime);

  }
  
  void SetPosition(Vector3 newPosition) {
    camera.transform.position = newPosition;
    CheckBorder();

  }

  private void ScrollPosition() {

    if (UserManager.Instance.ActiveBattleInfo == null) {
      UserManager.Instance.ActiveBattleInfo = new LevelInfo();
      UserManager.Instance.ActiveBattleInfo.Group = 1;
      UserManager.Instance.ActiveBattleInfo.Level = 1;
    }

    MapPointGraphic mp =
      MapManager.Instance.points.Find(
        x =>
          x.LevelInfo.Group == UserManager.Instance.ActiveBattleInfo.Group &&
          x.LevelInfo.Level == UserManager.Instance.ActiveBattleInfo.Level);
    try {
      camera.transform.position = mp.transform.position;
    } catch { }
    if (!UserManager.Instance.lastComplited) return;

    SetScrollValue(zoomBorders.min);
    //moveCoroutine = StartCoroutine(CameraMove());
  }

  private Vector3 startPositionScreen;
  private Vector3 startPositionWorld;
  [ExEvent.ExEventHandler(typeof(ExEvent.ScreenEvents.PointerDown))]
  private void OnPointerDown(ExEvent.ScreenEvents.PointerDown eventData) {
    startPositionScreen = eventData.position;
    startPositionWorld = camera.transform.position;
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.ScreenEvents.PointerUp))]
  private void OnPointerUp(ExEvent.ScreenEvents.PointerUp eventData) { }

  /// <summary>
  /// Скролинг мышкой
  /// </summary>
  /// <param name="scrollValue"></param>
  [ExEvent.ExEventHandler(typeof(ExEvent.ScreenEvents.Scroll))]
  void Scroll(ExEvent.ScreenEvents.Scroll eventData) {
    if (phase != Phases.free) return;
    ScrollCamera(-eventData.delta);
  }
  /// <summary>
  /// Собятие скрола экрана
  /// </summary>
  /// <param name="dragValue">Дельта пикселей</param>
  [ExEvent.ExEventHandler(typeof(ExEvent.ScreenEvents.PointerDrag))]
  void Drag(ExEvent.ScreenEvents.PointerDrag eventData) {
    if (phase != Phases.free) return;
    SetPosition(startPositionWorld + ((eventData.position - startPositionScreen) * -pixelSize));
  }

  bool CheckBorder() {

    bool isBorder = false;

    if (camera.transform.position.x + (pixelSize * (camera.pixelWidth / 2)) > horizontalBorder.max) {
      camera.transform.position = new Vector3(horizontalBorder.max - (pixelSize * (camera.pixelWidth / 2)), camera.transform.position.y, camera.transform.position.z);
      isBorder = true;
    }

    if (camera.transform.position.x - (pixelSize * (camera.pixelWidth / 2)) < horizontalBorder.min) {
      camera.transform.position = new Vector3(horizontalBorder.min + (pixelSize * (camera.pixelWidth / 2)), camera.transform.position.y, camera.transform.position.z);
      isBorder = true;
    }

    if (camera.transform.position.y + (pixelSize * (camera.pixelHeight / 2)) > verticalBorder.max) {
      camera.transform.position = new Vector3(camera.transform.position.x, verticalBorder.max - (pixelSize * (camera.pixelHeight / 2)), camera.transform.position.z);
      isBorder = true;
    }
    if (camera.transform.position.y - (pixelSize * (camera.pixelHeight / 2)) < verticalBorder.min) {
      camera.transform.position = new Vector3(camera.transform.position.x, verticalBorder.min + (pixelSize * (camera.pixelHeight / 2)), camera.transform.position.z);
      isBorder = true;
    }

    return isBorder;
  }


  /// <summary>
  /// Смещение зумма карты
  /// </summary>
  /// <param name="scrollValue"></param>
  void ScrollCamera(float scrollValue) {
    SetScrollValue(Mathf.Clamp(camera.orthographicSize + zoomSpeed * scrollValue, zoomBorders.min, zoomBorders.max));
  }

  void SetScrollValue(float scrollValue, bool checkZoom = true) {
    camera.orthographicSize = scrollValue;
    cameraFog.orthographicSize = camera.orthographicSize;

    if (checkZoom)
      CheckBorder();
    UserManager.Instance.MapZoom = camera.orthographicSize;
  }
  
  /// <summary>
  /// Интерфейсные кнопки
  /// </summary>
  /// <param name="scrollValue"></param>
  void ScrollButton(float scrollValue) {
    if (phase != Phases.free)
      return;
    ScrollCamera(-scrollValue);
  }

}
