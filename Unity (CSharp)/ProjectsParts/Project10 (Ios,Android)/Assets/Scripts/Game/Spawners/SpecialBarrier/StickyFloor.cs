using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Контроллер липкого пола
/// </summary>
public class StickyFloor: SpawnAbstract {

  public GameObject midleSegment;
  public GameObject rightSegment;
  public GameObject leftSegment;
  public IntSpan sizeElement;

  public float midleSize = 2.5f;

  [SerializeField]
  GameObject slickyLabel;

  List<GameObject> listMidleElement;
  List<int> topDecorList;

  [System.Serializable]
  public struct StickDecorPosition {
    public string name;
    [Range(0, 1)]
    public float prabability;
    public FloatSpan posX;
  }

  public StickDecorPosition[] decorTopParametrs;
  public StickDecorPosition[] decorBottomParametrs;

  static bool showLabel;

  void Awake() {
    showLabel = PlayerPrefs.GetInt("stick", 0) < 5;
  }

  void OnEnable() {

    SetDown();
    SetSize(Random.Range(sizeElement.min, sizeElement.max + 1));
  }

  void Update() {
    if (transform.position.x < CameraController.displayDiff.leftDif(2))
      gameObject.SetActive(false);
  }

  public void SetSize(int countElement) {
    if (listMidleElement != null) {
      listMidleElement.RemoveAt(0);
      listMidleElement.ForEach(x => Destroy(x));
      listMidleElement.Clear();
    } else
      listMidleElement = new List<GameObject>();
    listMidleElement.Add(midleSegment);

    topDecorList = new List<int>();

    for (int i = 0; i < countElement; i++) {
      if (i > 0) {
        GameObject clone = Instantiate(midleSegment) as GameObject;
        clone.transform.parent = transform;
        clone.transform.localPosition = midleSegment.transform.localPosition;
        listMidleElement.Add(clone);
      }

      listMidleElement[i].transform.localPosition = new Vector3(-(countElement * midleSize / 2) + (midleSize * i + (midleSize / 2)),
                                                                  listMidleElement[i].transform.localPosition.y,
                                                                  listMidleElement[i].transform.localPosition.z);
      PositionDecoration(listMidleElement[i].transform);
    }

    rightSegment.transform.localPosition = new Vector3(countElement * midleSize / 2, rightSegment.transform.localPosition.y, rightSegment.transform.localPosition.z);
    leftSegment.transform.localPosition = new Vector3(countElement * midleSize / -2, rightSegment.transform.localPosition.y, rightSegment.transform.localPosition.z);

    CheckBreackTarget(leftSegment.transform.position);
    CheckBreackTarget(rightSegment.transform.position);

    BoxCollider2D box = GetComponent<BoxCollider2D>();
    box.size = new Vector3(countElement * midleSize + 0.5f, box.size.y);
  }

  /// <summary>
  /// Расположение декораций
  /// </summary>
  void PositionDecoration(Transform parent) {

    Transform finden;

    foreach (StickDecorPosition oneDecor in decorBottomParametrs) {
      finden = parent.Find(oneDecor.name);
      if (oneDecor.prabability >= Random.value) {
        finden.gameObject.SetActive(true);
        if (finden != null) finden.localPosition = new Vector3(Random.Range(oneDecor.posX.min, oneDecor.posX.max), finden.localPosition.y, finden.localPosition.z);
      } else
        finden.gameObject.SetActive(false);
    }

    float minX = 0;
    float maxX = 0;
    float useX = 0;

    for (int i = 0; i < decorTopParametrs.Length; i++) {
      finden = parent.Find(decorTopParametrs[i].name);
      if (!topDecorList.Exists(x => x == i) && decorTopParametrs[i].prabability >= Random.value) {

        finden.gameObject.SetActive(true);

        if (i > 0) {
          float tmp = Random.value;
          if (decorTopParametrs[i].posX.min < minX - 0.3f && tmp <= 0.5f)
            useX = Random.Range(Mathf.Min(minX - 0.3f, decorTopParametrs[i].posX.min), decorTopParametrs[i].posX.max);

          if (decorTopParametrs[i].posX.min >= minX - 0.3f || tmp > 0.5f)
            useX = Random.Range(Mathf.Max(maxX + 0.3f, decorTopParametrs[i].posX.min), decorTopParametrs[i].posX.max);
        } else
          useX = Random.Range(decorTopParametrs[i].posX.min, decorTopParametrs[i].posX.max);

        if (finden != null) finden.localPosition = new Vector3(useX, finden.localPosition.y, finden.localPosition.z);

        minX = Mathf.Min(minX, useX);
        maxX = Mathf.Max(maxX, useX);
        topDecorList.Add(i);
      } else
        finden.gameObject.SetActive(false);
    }
  }

  public LayerMask groundMask;

  public void SetDown() {
    transform.localScale = new Vector3(1, 1, 1);
    RaycastHit[] grnd = Physics.RaycastAll(transform.position, Vector3.down * 3, groundMask);

    bool breack = true;
    if (grnd.Length > 0) {
      foreach (RaycastHit one in grnd) {
        if (LayerMask.LayerToName(one.transform.gameObject.layer) == "Ground") {
          transform.position = new Vector3(transform.position.x, one.transform.position.y, 0);
          breack = false;
        }
      }
    }

    if (breack) gameObject.SetActive(false);

  }

  void OnTriggerEnter2D(Collider2D col) {
    if (LayerMask.LayerToName(col.gameObject.layer) == "Player") {
      Player.Jack.PlayerMove.isSticky = true;
      Player.Jack.PlayerMove.OnPlayerJump += OnPlayerJump;
      FirstFixed();

      GameObject sfx = Pooler.GetPooledObject("StickyPoof");
      sfx.transform.position = new Vector3(col.transform.position.x, transform.position.y, transform.position.z);
      sfx.SetActive(true);

    }
  }

  GameObject labelObj;

  int clickCount;
  float lastClick;

  void FirstFixed() {
    clickCount = 0;
    lastClick = Time.time;

    if (showLabel) {
      int showLabelCount = PlayerPrefs.GetInt("stick", 0);
      PlayerPrefs.SetInt("stick", ++showLabelCount);

      labelObj = Instantiate(slickyLabel, new Vector3(Player.Jack.PlayerController.Instance.transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
      if (showLabelCount <= 1)
        Time.timeScale = 0.1f;
    }
  }

  void OnPlayerJump() {
    if (lastClick + 0.4f < Time.time) {
      clickCount = 0;
    }
    lastClick = Time.time;

    clickCount++;

    if (clickCount == 4) {
      Player.Jack.PlayerMove.isSticky = false;

      if (labelObj != null)
        HideLabel();
    }
  }

  void OnTriggerExit2D(Collider2D col) {
    if (LayerMask.LayerToName(col.gameObject.layer) == "Player") {
      Player.Jack.PlayerMove.OnPlayerJump -= OnPlayerJump;

      if (showLabel && labelObj != null) {
        showLabel = PlayerPrefs.GetInt("stick", 0) < 5;
        if (labelObj != null)
          HideLabel();
        Time.timeScale = 1f;
      }

      Player.Jack.PlayerMove.isSticky = false;
    }
  }

  void HideLabel() {
    if (labelObj == null)
      return;

    labelObj.GetComponent<Animator>().SetTrigger("close");
    Destroy(labelObj, 0.5f);
  }

  void CheckBreackTarget(Vector3 needPosition) {
    RaycastHit[] grnd = Physics.RaycastAll(needPosition + Vector3.up, Vector3.down * 3, groundMask);

    bool breack = true;

    if (grnd.Length > 0) {
      foreach (RaycastHit one in grnd) {
        if (LayerMask.LayerToName(one.transform.gameObject.layer) == "Ground") {
          breack = false;
        }
      }
    }

    if (breack)
      gameObject.SetActive(false);
  }

}
