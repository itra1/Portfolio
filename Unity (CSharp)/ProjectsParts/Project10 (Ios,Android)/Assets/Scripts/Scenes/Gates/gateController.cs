using UnityEngine;

/// <summary>
/// Ворота на сцене
/// </summary>
public class gateController : MonoBehaviour {

  public delegate void EndGate(int gateNum);
  public static event EndGate OnEndGate;

  public int gateNum = 0;
  public int keyCount = 0;
  private int allKeys;

  public GameObject label;

  public Color openColor;
  public Color closeColor;
  public SpriteRenderer zamok;

  public TextMesh keysCountText;
  
  public GameObject[] openEffects;
  public GameObject[] hideObjects;

  bool isOpen;

  void OnEnable() {
    GateHelper[] checkers = GetComponentsInChildren<GateHelper>();
    foreach(GateHelper one in checkers) one.OnCheck += OpenGate;
    isOpen = false;
  }
  /// <summary>
  /// Запускаем процесс ожидания уничтожения
  /// </summary>
  void ReadyDestroy() {
    Destroy(gameObject, 60);
  }

  void Update() {
    if(!isOpen && transform.position.x < CameraController.displayDiff.rightDif(0.8f) && !ParentCamera.CameraStop) {
      ParentCamera.CameraStop = true;
      RunnerController.stopCalcDistantion = true;
    }
  }

  public void SetLabel() {

    allKeys = UserManager.Instance.keys + RunnerController.Instance.keysInRaceCalc;
    keysCountText.text = allKeys + "/" + keyCount;
    keysCountText.GetComponent<MeshRenderer>().sortingOrder = zamok.sortingOrder;
    keysCountText.GetComponent<MeshRenderer>().sortingLayerID = zamok.sortingLayerID;

    if(keyCount > allKeys)
      zamok.color = closeColor;
    else
      zamok.color = openColor;
  }

  void OnTriggerEnter2D(Collider2D obj) {

    if(isOpen && obj.tag == "Player") {
      if(OnEndGate != null) OnEndGate(gateNum);
      return;
    }

    if(obj.tag == "Player") {

      if(keyCount <= allKeys) {
        OpenGate();
        return;
      }

      RunnerController.Instance.GateDamage(transform.position + Vector3.up);
      isOpen = true;
      //Questions.QuestionManager.addNewMyRecords(obj.transform.position);
      Questions.QuestionManager.ConfirmQuestion(Quest.newMyRecords, 1, obj.transform.position);
      return;
    }
  }

  public void OpenGate() {

    if(isOpen || keyCount > allKeys) return;

    RunnerController.Instance.OpenGate(gateNum, keyCount);
    if(OnEndGate != null) OnEndGate(gateNum);
    OpenAnim();
    ParentCamera.CameraStop = false;
    RunnerController.stopCalcDistantion = false;
  }

  /// <summary>
  /// Анимация открытия ворот
  /// </summary>
  void OpenAnim() {

    foreach(GameObject one in hideObjects)
      one.SetActive(false);
    foreach(GameObject one in openEffects)
      one.SetActive(true);
    isOpen = true;
  }

  /// <summary>
  /// Опкрытие без анимации
  /// </summary>
  public void OpenDef() {

    foreach(GameObject one in hideObjects)
      one.SetActive(false);
    foreach(GameObject one in openEffects)
      one.SetActive(true);

    isOpen = true;
    label.SetActive(false);
    ReadyDestroy();
  }

}
