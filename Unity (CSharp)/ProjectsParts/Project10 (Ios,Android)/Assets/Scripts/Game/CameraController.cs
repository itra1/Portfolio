using UnityEngine;
using UnityStandardAssets.ImageEffects;

/// <summary>
/// Парметров камеры
/// </summary>
[System.Serializable]
public struct DisplayDiff {
  public float left;                  // Левая граница
  public float right;                 // Правая граница
  public float down;                  // Нижняя граница
  public float top;                   // Верхнаяя граница
  public float deltaX;
  public Transform transform;         // Трансформ камеры
  public float leftDif(float koef) { return transform.position.x + left * koef; }      // Смещение влево, от центра по коеффициенту
  public float rightDif(float koef) { return transform.position.x + right * koef; }   // Смещение вправо, от центра по коеффициенту
  public float topDif(float koef) { return transform.position.y + top * koef; }   // Смещение вправо, от центра по коеффициенту
  public float bottomDif(float koef) { return transform.position.y + down * koef; }   // Смещение вправо, от центра по коеффициенту
}

/// <summary>
/// Контроллер камеры
/// </summary>
public class CameraController: Singleton<CameraController> {

  public static CameraController instance;                             // Ссылка на самого себя
  private readonly float defaultHeight;                                  // Стандартное положение камеры
  private Animator animComp;                                    // Ссылка на компонент анимации
  public static DisplayDiff displayDiff = new DisplayDiff();    // Размер экрана
  /// <summary>
  /// Событие пересчета
  /// </summary>
  public delegate void RecalcPosition();
  public static event RecalcPosition recalcPosition;
  protected override void Awake() {
    base.Awake();
    RecalcDiff();
  }
  void Start() {
    RunnerController.OnChangeRunnerPhase += ChangeStatePahse;
    Invoke(RecalcDiff, 0.2f);
  }
  protected override void OnDestroy() {
    base.OnDestroy();
    RunnerController.OnChangeRunnerPhase -= ChangeStatePahse;
  }
  void OnEnable() {
    animComp = GetComponent<Animator>();
  }
  public void RecalcDiff() {
    displayDiff = CalcDisplayDiff(0);
    if (recalcPosition != null) recalcPosition();
  }
  /// <summary>
  /// Событие изменения фазы
  /// </summary>
  /// <param name="newPhase"> Новая устанавливаемая фаза</param>
  void ChangeStatePahse(RunnerPhase newPhase) {
    if (newPhase == RunnerPhase.boost)
      MoveEffects(true);

    if (newPhase == RunnerPhase.run || newPhase == RunnerPhase.dead)
      MoveEffects(false);
  }
  void MoveEffects(bool flag) {
    GetComponent<MotionBlur>().enabled = flag;
  }
  public DisplayDiff CalcDisplayDiff(float positionZ = 0) {

    DisplayDiff display = new DisplayDiff();

    float posX = Display.main.systemWidth;
    float posY = Display.main.systemHeight;
    float posZ = positionZ - transform.position.z;

    Vector3 ld = new Vector3(0, 0, posZ);
    Vector3 rt = new Vector3(posX, posY, posZ);

    Vector3 screen1 = Camera.main.ScreenToWorldPoint(ld);
    Vector3 screen2 = Camera.main.ScreenToWorldPoint(rt);

    display.left = screen1.x - transform.position.x;
    display.right = screen2.x - transform.position.x;
    display.down = screen1.y - transform.position.y;
    display.top = screen2.y - transform.position.y;
    display.deltaX = display.right - display.left;
    display.transform = transform;

    return display;
  }
  public static bool InDysplay(Vector3 pos) {
    DisplayDiff display = instance.CalcDisplayDiff(pos.z);

    return (pos.x >= display.leftDif(1)
         && pos.x <= display.rightDif(1)
         && pos.y <= display.topDif(1)
         && pos.y >= display.bottomDif(1));
  }
  public void GoBlur(bool flag) {
    try {

      animComp.SetBool("blur", flag);

    } catch { }
  }
  public void GoBloom() {
    animComp.SetTrigger("bloom");
  }

  public void Zoom(bool flag) {
    RecalcDiff();

    if (flag) {
      animComp.SetTrigger("zoom+");
    } else {
      animComp.SetTrigger("zoom-");
    }

    Invoke(RecalcDiff, 0.5f);
  }
}
