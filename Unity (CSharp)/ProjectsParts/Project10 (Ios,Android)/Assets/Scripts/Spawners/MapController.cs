using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using Random = UnityEngine.Random;

public enum TimesBonuses { min15, hour1, hour5, hour12, hour12Animal };

[System.Serializable]
public struct LocationParametr {
  public int location;
  public int count;
  public int lastTimeGenerate;
}

[System.Serializable]
public struct MapBonus {
  public TimesBonuses bonusType;
  public int secondsWait;
  
  public LocationParametr[] allGenerate;
  public MapBonusGenerate[] array;
  public int GetCountInLocation(int locationNum) {
    int count = 0;
    foreach(MapBonusGenerate one in array)
      if(one.location == locationNum) count++;
    return count;
  }
  public int GetMaxCountInLocation(int locationNum) {
    int count = 0;
    foreach(LocationParametr one in allGenerate)
      if(one.location == locationNum) count = one.count;
    return count;
  }
  public int GetLastGenerateInLocation(int locationNum) {
    foreach(LocationParametr one in allGenerate)
      if(one.location == locationNum) return one.lastTimeGenerate;
    return 0;
  }
  public void SetLastGenerateInLocation(int locationNum, int timeValue) {
    for(int i = 0; i < allGenerate.Length; i++)
      if(allGenerate[i].location == locationNum) allGenerate[i].lastTimeGenerate = timeValue;
  }
  public bool Exists(string name) {
    for(int i = 0; i < array.Length; i++)
      if(array[i].prefName == name) return true;
    return false;
  }
}

[System.Serializable]
public struct MapBonusGenerate {
  public string prefName;
  public int coinsCount;
  public int location;
  public int genPosition;
}

[System.Serializable]
public struct ObjectPosition {
  public Vector3 position;
  public int location;
  public int order;
  public float localDiffY;
  public bool mirrowX;
}

[System.Serializable]
public struct GenerateObjects {
  public GameObject pref;
  public FloatSpan coinsCount;
  public TimesBonuses timeDiff;
  [Range(0,1)]
  public float probality;
  public ObjectPosition[] position;

  public int GetMinNumPosit(int loc) {
    int num = position.Length-1;
    for(int i = 0; i < position.Length; i++)
      if(position[i].location == loc && i < num) return i;
    return num;
  }

  public int GetMaxNumPosit(int loc) {
    int num = 0;
    for(int i = 0; i < position.Length; i++)
      if(position[i].location == loc && i > num) num = i;
    return num;
  }

  public bool ExistLocate(int locationNum) {
    foreach(ObjectPosition one in position)
      if(one.location == locationNum) return true;
    return false;
  }
}

[System.Serializable]
public struct Objects15Minut {
  public GameObject obj;
  public int objectId { get { return obj.GetInstanceID(); } }
  public int location;
}

/// <summary>
/// Основной контроллер карты
/// </summary>
public class MapController : Singleton<MapController> {
	
  
  public GameObject boardObject;                          // Объект доски

  public Transform grovers;
  public Transform groversLabels;
  public Transform groversLabelsGates;

  public string mainScene;
	
  public LineRenderer line;

  float moveSpeed;                                     // Скорость смещения
	
  public static event Action swipe;
  bool backInBorder;

  public AudioClip menyClip;

  private bool goToLevelReady = false;
  private int goToLevel = 0;

  private List<Transform> transformOrderLiser;

  public AudioClip[] coinsClip;
  public AudioClip tapClip;

  struct meshes {
    public string avaUrl;
    public MeshRenderer mat;
  }

  void Start() {

		Application.targetFrameRate = 30;
		MapGamePlay.OnTapDown += TapDown;
		MapGamePlay.OnTapUp += TapUp;
		MapGamePlay.OnTapDrag += Drag;
		Apikbs.OnLogin += GetFriendsPosition;
		//Установка фоновой музыки
		AudioManager.BackMusic(menyClip, AudioMixerTypes.mapMusic);
		
    barrierDist = CameraController.displayDiff.right;
    if(GameManager.activeLevelData.gameMode != GameMode.survival)
      SetPointPositions();
    SetPanametrPath();
    SetOpenGates();
    moveSpeed = (CameraController.displayDiff.right * 2) / Camera.main.pixelWidth;

    FillBlack.Instance.PlayAnim(FillBlack.AnimType.round, FillBlack.AnimVecor.open, Vector3.zero, () => {

    });



    ShowGamePlay();
		
    InitAudio();
    InitBonuses();
  }
	
	protected override void OnDestroy() {
		base.OnDestroy();
		Apikbs.OnLogin -= GetFriendsPosition;
		MapGamePlay.OnTapDown -= TapDown;
		MapGamePlay.OnTapUp -= TapUp;
		MapGamePlay.OnTapDrag -= Drag;
	}

	/// <summary>
	/// Обработка тапа по объекту
	/// </summary>
	/// <param name="objectId">Идентификатор предмета, по которому тапнули</param>
	/// <param name="needPosition">Позиция этого объекта</param>
	public bool TapClick(int objectId, Vector3 needPosition) {
    return CoinsOnject15Minut(objectId, needPosition);
  }

  void Update() {

    unixTime = (int)(System.DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
    UpdateBonus();
    UpdateBonus15Minut();
    
    UpdateAudio();
		
    if((!isTap || backInBorder) && smahSpeed != 0) {

      float diff = Mathf.Abs(smahSpeed);

      if(diff > 0) {
        diff -= 100 * Time.deltaTime;
        if(diff <= 0) diff = 0;
        smahSpeed = diff * Mathf.Sign(smahSpeed);
        AllMove(smahSpeed, false);
        if(smahSpeed <= 0) {
          backInBorder = checkBorderExcessSwipe();
        }
      }
    }

    if(backInBorder) {
      moveToBack();
    }
  }

  bool isTap;       // Выполняется тап

  /// <summary>
  /// Смахивание
  /// </summary>
  /// <param name="deltaMove">Дельта смаха</param>
  void Drag(Vector2 deltaMove) {
    if(deltaMove.x == 0) return;
    smahSpeed = deltaMove.x;
    AllMove(smahSpeed, true);
  }
  /// <summary>
  /// Отпускание тапа
  /// </summary>
  void TapUp() {
    backInBorder = checkBorderExcessSwipe();
    isTap = false;
  }
  /// <summary>
  /// Нажатие тапа
  /// </summary>
  void TapDown() {
    backInBorder = false;
    isTap = true;
  }

  #region Управление воротами
  [System.Serializable]
  public struct gates {
    public GameObject gate;
    public GameObject label;
    public TextMesh keyCount;
    public int needKeys;
  }

  public gates[] gate;

  //void ShowGamePlay() {
  //  gamePlay.SetActive(true);
  //  ShowCounts();
  //}

  public void ShowCounts() {
    gamePlay.GetComponent<MapGamePlay>().SetCountText();
  }

  List<int> countOpenGate;
  int openGate;

  public void SetOpenGates(bool isStart = true) {

    openGate = PlayerPrefs.GetInt("openGate");
    int keys = UserManager.Instance.keys;

#if UNITY_EDITOR
    //openGate = 3;
#endif
    int keysPref = 0;

    countOpenGate = new List<int>();

    for(int i = 0; i < gate.Length; i++) {
      if(i + 1 <= openGate) {
        if(gate[i].gate != null) {
          gate[i].gate.transform.Find("WoodGateFront").gameObject.SetActive(false);
          gate[i].gate.transform.Find("WoodGateDestroy").gameObject.SetActive(true);
          gate[i].gate.GetComponent<MapGateController>().isOpen = true;
        }
        gate[i].label.SetActive(false);
      } else {
        // gate[i].label.transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = gate[i].needKeys.ToString();
        gate[i].label.transform.Find("Title").GetComponent<TextMesh>().text = gate[i].needKeys.ToString();
        if(keys < gate[i].needKeys + keysPref) {
          gate[i].label.SetActive(true);

          if(openGate == i) {
            gate[i].label.transform.Find("Title").GetComponent<TextMesh>().text = (gate[i].needKeys - keys).ToString();
          } else
            gate[i].label.transform.Find("Title").GetComponent<TextMesh>().text = gate[i].needKeys.ToString();

        } else {

          keysPref += gate[i].needKeys;

          if(isStart) {
            gate[i].gate.transform.Find("WoodGateDestroy").gameObject.SetActive(true);

            gate[i].gate.transform.Find("WoodGateFront").gameObject.SetActive(false);
            gate[i].gate.transform.Find("WoodGateDestroy").gameObject.SetActive(true);
            gate[i].gate.GetComponent<MapGateController>().isOpen = true;
            //countOpenGate.Add(i);
            gate[i].label.SetActive(false);
          } else {
            countOpenGate.Add(i);
          }

        }
      }
    }
  }

  [SerializeField]
  AudioClip gateBomClip;

  public void StartAnimOpenGate() {


    if(countOpenGate != null && countOpenGate.Count > 0) {
      foreach(int one in countOpenGate) {
        AudioManager.PlayEffect(gateBomClip, AudioMixerTypes.mapEffect);
        gate[one].gate.transform.Find("GatesBoom").gameObject.SetActive(true);
        gate[one].gate.transform.Find("WoodGateFront").gameObject.SetActive(false);
        gate[one].gate.transform.Find("WoodGateDestroy").gameObject.SetActive(true);
        gate[one].gate.GetComponent<MapGateController>().isOpen = true;
        gate[one].label.SetActive(false);
      }
    }

    countOpenGate = null;
  }

  #endregion

  #region Расстановка фигурок

  public GameObject path;
  public GameObject point;
  public GameObject pointLabel;
  public GameObject[] graves;
  public GameObject gravesLabel;
  void SetPanametrPath() {
    SetPlayerPosition();
    if(FBController.CheckFbLogin) {
      Apikbs.Instance.GetLeaderBoard(SetFriendsPosition);
    }
  }

  public void GetFriendsPosition() {
    Apikbs.Instance.GetLeaderBoard(SetFriendsPosition);
  }
  void SetPlayerPosition() {

    if(GameManager.activeLevelData.gameMode != GameMode.survival) return;

    if(gamePlay != null && gamePlay.gameObject.activeInHierarchy)
      gamePlay.GetComponent<MapGamePlay>().SetCountText();
    float position = UserManager.Instance.survivleMaxRunDistance;
    float lastPosition = PlayerPrefs.GetInt("lastDistInMap");
    PlayerPrefs.SetInt("lastDistInMap", (int)position);

    float percent = GetPositionInPath(position,true);
    float lastPercent = GetPositionInPath(lastPosition);

    if(lastPercent > 0) {
      writeLine(lastPercent);
    }

    if(position >= 5800) ChangeColorLine();

    StartCoroutine(movePlayerPoint(percent, lastPercent));
  }
  void writeLine(float lastPerc) {

    if(lastPerc > 0.999f) lastPerc = 0.999f;

    List<Vector3> points = new List<Vector3>();
    float thisPercent = -0.01f;
    Vector3 coord;

    while(thisPercent < lastPerc) {
      thisPercent += 0.01f;

      if(thisPercent > lastPerc) thisPercent = lastPerc;
      if(thisPercent > 0.999f) thisPercent = 0.999f;

      coord = path.GetComponent<BezierCurve>().GetPointAt(thisPercent);
      points.Add(coord);
    }

    Vector3[] allPoints = points.ToArray();
    //line.SetVertexCount(points.Count);
		line.positionCount = points.Count;

		line.SetPositions(allPoints);

    line.gameObject.SetActive(GameManager.activeLevelData.gameMode == GameMode.survival);

  }

  void ChangeColorLine() {
		line.startColor = new Color(1, 1, 1, 1);
		line.endColor = new Color(1, 1, 1, 0);
		//line.SetColors(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0));
  }
  void SetFriendsPosition(List<LeaderboardItem> friensAll) {

    Transform[]chilrder =  grovers.GetComponentsInChildren<Transform>();

    if(chilrder.Length > 0) {
      for(int i = 0; i < chilrder.Length; i++) {
        if(chilrder[i] != null && chilrder[i].parent == (grovers))
          Destroy(chilrder[i].gameObject);
      }
    }

    Transform[]chilrder1 =  groversLabels.GetComponentsInChildren<Transform>();

    if(chilrder1.Length > 0) {
      for(int i = 0; i < chilrder1.Length; i++) {
        if(chilrder1[i] != null && chilrder1[i].parent == (groversLabels))
          Destroy(chilrder1[i].gameObject);
      }
    }

    float percent = 0;
    Vector3 coord;

    foreach(LeaderboardItem one in friensAll) {

      if(one.fb == FBController.GetUserId) continue;

      percent = GetPositionInPath(float.Parse(one.bestDistantion));
      coord = path.GetComponent<BezierCurve>().GetPointAt(percent);
      GameObject gravesInst =Instantiate(graves[Random.Range(0, graves.Length)], coord, Quaternion.identity) as GameObject;

      gravesInst.transform.parent = grovers;

      GameObject label = Instantiate(gravesLabel, new Vector3(coord.x, 1.435f, 0), Quaternion.identity) as GameObject;

      label.transform.parent = groversLabels;
      //label.transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = one.bestDistantion + " M";
      label.transform.Find("Title").GetComponent<TextMesh>().text = one.bestDistantion + " M";
      label.GetComponent<MapPeopleLabel>().avatarUrl = one.picture;
      label.GetComponent<MapPeopleLabel>().awaWait = false;
      label.GetComponent<MapPeopleLabel>().GetAvatar(one.fb);
    }

    ChangeLabelPosition();

    SetPlayerPosition();
    SetOpenGates();
  }
  IEnumerator DownloadAllSprite(List<meshes> mesh) {

    for(int i = 0; i < mesh.Count; i++) {
      WWW www = new WWW(mesh[i].avaUrl);
      yield return www;
      mesh[i].mat.material.mainTexture = www.texture;
      yield return new WaitForSeconds(0.2f);
    }
  }
  float GetPositionInPath(float position, bool player = false) {

    int dors = 20;

    if(player) dors = PlayerPrefs.GetInt("openGate");

    float sumMapDist = 0;

    for(int i = 0; i < GameManager.Instance.mapRun.Count; i++) {
      if(GameManager.Instance.mapRun[i].distance + GameManager.Instance.mapRun[i].distanceStart >= position) {

        if(i <= dors) {
          if(i == 0) {
            sumMapDist += GameManager.Instance.mapRun[i].mapDistance / GameManager.Instance.mapRun[i].distance * position;
          } else {
            sumMapDist += ((GameManager.Instance.mapRun[i].mapDistance - GameManager.Instance.mapRun[i - 1].mapDistance)
                        / GameManager.Instance.mapRun[i].distance)
                        * (position - (GameManager.Instance.mapRun[i - 1].distanceStart + GameManager.Instance.mapRun[i - 1].distance));
          }
        }
        return sumMapDist / 100;
      }
      sumMapDist = GameManager.Instance.mapRun[i].mapDistance;
    }
    return 0;
  }
  IEnumerator movePlayerPoint(float percent, float lastPerc = 0) {

    float thisPercent = lastPerc;

    Vector3 coord = path.GetComponent<BezierCurve>().GetPointAt(thisPercent);
    GameObject player = Instantiate(point, coord, Quaternion.identity) as GameObject;

    player.transform.parent = grovers;

    coord = path.GetComponent<BezierCurve>().GetPointAt(thisPercent);
    player.transform.position = coord;
    SetCameraPoxitionX(coord.x);
    GameObject pointLabelObj = Instantiate(pointLabel, coord, Quaternion.identity) as GameObject;
    pointLabelObj.transform.parent = groversLabels;
    pointLabelObj.transform.position = new Vector3(player.transform.position.x, 1.435f, 0);
    AllMove(0, false);

    if(thisPercent < percent) {
      GetComponent<AudioSource>().Play();
    }

    while(thisPercent < percent) {
      yield return new WaitForSeconds(0.001f);
      thisPercent += 0.003f;
      if(thisPercent > percent) thisPercent = percent;

      coord = path.GetComponent<BezierCurve>().GetPointAt(thisPercent);
      player.transform.position = coord;
      pointLabelObj.transform.position = new Vector3(player.transform.position.x, 1.435f, 0);
      ChangeLabelPosition();
      SetCameraPoxitionX(coord.x);
      // Событие смахивания
      if(swipe != null) swipe();
    }

    GetComponent<AudioSource>().Stop();

    if(IsInvoking("ChangeLabelPosition")) CancelInvoke("ChangeLabelPosition");
    Invoke("ChangeLabelPosition", 0.2f);
    //ChangeLabelPosition();
    yield return 0;
  }
  void SetCameraPoxitionX(float posX) {

    if(posX < mapMaxBorder.min + 2 + barrierDist)
      posX = mapMaxBorder.min + 2 + barrierDist;
    else if(posX > mapMaxBorder.max - 2 - barrierDist)
      posX = mapMaxBorder.max - 2 - barrierDist;

		Transform cameraTrans = CameraController.Instance.transform;

		cameraTrans.position = new Vector3(posX, cameraTrans.position.y, cameraTrans.position.z);
    boardObject.transform.position = new Vector3(cameraTrans.position.x, boardObject.transform.position.y, boardObject.transform.position.z);
  }

  #endregion

  #region Смахивание

  public FloatSpan mapMaxBorder;                              // Границы движения карты
  // Смещение смахом
  float smahSpeed;                                            // длинна смаха за кадр
  float barrierDist;                                          // Расстояние видимого края
                                                              // Смещение всех элементов без ограничения
  void AllMove(float alldiff, bool touch) {

    float newSpeed = moveSpeed;
		Transform cameraTrans = CameraController.Instance.transform;

		if (cameraTrans.position.x - barrierDist <= mapMaxBorder.min + 2 && alldiff > 0) {
      if(!touch) {
        alldiff = 0;
        smahSpeed = 0;
      }
      newSpeed = moveSpeed - (moveSpeed / 2 * (2.7f - (Mathf.Abs(mapMaxBorder.min - (cameraTrans.position.x - barrierDist)))));
    } else if(cameraTrans.position.x + barrierDist >= (mapMaxBorder.max - 2) && alldiff < 0) {
      if(!touch) {
        alldiff = 0;
        smahSpeed = 0;
      }
      newSpeed = moveSpeed - (moveSpeed / 2 * (2.7f - (Mathf.Abs((mapMaxBorder.max) - (cameraTrans.position.x + barrierDist)))));
    } else {
      newSpeed = moveSpeed;
    }

    if(newSpeed <= moveSpeed / 50) newSpeed = moveSpeed / 50;

    float newPositionX = cameraTrans.position.x - alldiff * newSpeed;

		cameraTrans.position = new Vector3(Mathf.Clamp(newPositionX, mapMaxBorder.min, mapMaxBorder.max), cameraTrans.position.y, cameraTrans.position.z);
    boardObject.transform.position = new Vector3(cameraTrans.position.x, boardObject.transform.position.y, boardObject.transform.position.z);

    // Событие смахивания
    if(swipe != null) swipe();
  }

  // Проверка на выход за границы
  bool checkBorderExcessSwipe() {
    if(CameraController.Instance.transform.position.x - barrierDist <= mapMaxBorder.min + 2 || CameraController.Instance.transform.position.x + barrierDist >= mapMaxBorder.max - 2)
      return true;
    else
      return false;

  }

  void moveToBack() {
    smahSpeed = 0;
    if(CameraController.Instance.transform.position.x - barrierDist <= mapMaxBorder.min + 2)
      AllMove(-20, true);
    else if(CameraController.Instance.transform.position.x + barrierDist >= (mapMaxBorder.max - 2))
      AllMove(20, true);
    else {
      backInBorder = false;
    }
  }

	#endregion
	MapGamePlay gamePlay;
	public void ShowGamePlay() {
		gamePlay = UiController.ShowUi<MapGamePlay>();
		gamePlay.gameObject.SetActive(true);
		gamePlay.SetCountText();

		gamePlay.OnHome = () => {
			//gamePlay.ClosePanel();
			//gamePlay.OnClose = () => {
				GameManager.LoadScene("Menu");
			//};
		};
		
		gamePlay.OnPlay = () => {
			//gamePlay.ClosePanel();
			//gamePlay.OnClose = () => {
				GameManager.startFromMap = true;
				GameManager.LoadScene("ClassicRun");
			//};
		};
		
		gamePlay.OnChangeKey = () => {
			SetOpenGates(false);
		};
		
	}
	
	#region Назад в игру
	
	public void ShowBlackBg() {
    if(!goToLevelReady) return;
    //Application.LoadLevel(goToLevel);
    SceneManager.LoadScene(goToLevel);
  }
  #endregion

  void ChangeLabelPosition() {

    transformOrderLiser = new List<Transform>();

    Transform[] alltrans = groversLabels.gameObject.GetComponentsInChildren<Transform>();

    for(int i = 0; i < alltrans.Length; i++) {
      if(alltrans[i].parent == groversLabels)
        transformOrderLiser.Add(alltrans[i]);
    }
		
    Transform[] alltransGates = groversLabelsGates.gameObject.GetComponentsInChildren<Transform>();

    for(int i = 0; i < alltransGates.Length; i++) {
      if(alltransGates[i].parent == groversLabelsGates)
        transformOrderLiser.Add(alltransGates[i]);
    }

    transformOrderLiser.Sort((a, b) => a.position.x.CompareTo(b.position.x));

    float lastXPosition = -100000000;
    float lastIndex = 0;

    float inc = 1.2f;

    for(int i = 0; i < transformOrderLiser.Count; i++) {

      if(!transformOrderLiser[i].gameObject.activeInHierarchy) continue;

      if(lastXPosition + inc > transformOrderLiser[i].position.x) {
        lastIndex++;
      } else {
        lastIndex = 0;
      }
      transformOrderLiser[i].position = new Vector3(transformOrderLiser[i].position.x, 1.435f + (lastIndex * -0.385f), transformOrderLiser[i].position.z);
      lastXPosition = transformOrderLiser[i].position.x;
    }

  }

  #region Звуки

  public AudioClip[] ambiendClips;
  public FloatSpan tineWaitAmbientAudio;
  float timeAudioAmbientPlay;

  void InitAudio() {
    timeAudioAmbientPlay = Time.time + Random.Range(tineWaitAmbientAudio.min, tineWaitAmbientAudio.max);
  }

  void UpdateAudio() {
    if(timeAudioAmbientPlay < Time.time) {
      AudioManager.PlayEffect(ambiendClips[Random.Range(0, ambiendClips.Length)], AudioMixerTypes.mapEffect);
      timeAudioAmbientPlay = Time.time + Random.Range(tineWaitAmbientAudio.min, tineWaitAmbientAudio.max);
    }
  }

  #endregion

  #region Позиционирование объектов

  public MapBonus[] allBonusGenerate;
  public GenerateObjects[] objectsParametrs;
  public Objects15Minut[] objects15minutes;

  void InitBonuses() {
    ReturnSerializableBonus();
    PositingObjects();
  }

  void SaveBonuses() {
    SeriasableBonus();
  }

  void PositingObjects() {
    for(int i = 0; i < allBonusGenerate.Length; i++) {
      if(allBonusGenerate[i].array.Length > 0) {

        foreach(var one in allBonusGenerate[i].array) {
          foreach(var pre in objectsParametrs) {
            if(pre.pref.name == one.prefName) {
              GameObject clone = Instantiate(pre.pref);
              clone.transform.position = pre.position[one.genPosition].position;
              clone.GetComponent<MapActiveObject>().PositingObject(pre.position[one.genPosition]);
              clone.GetComponent<MapActiveObject>().SetBonus(allBonusGenerate[i].bonusType, one);
            }
          }
        }
      }
    }
  }

  int arrayNumBonus;
  int unixTime;
  int useCountGate;
  void UpdateBonus() {


    for(useCountGate = 0; useCountGate <= openGate; useCountGate++) {

      for(arrayNumBonus = 0; arrayNumBonus < allBonusGenerate.Length; arrayNumBonus++) {


        if((allBonusGenerate[arrayNumBonus].secondsWait - (unixTime - allBonusGenerate[arrayNumBonus].GetLastGenerateInLocation(useCountGate))) <= 0
            && allBonusGenerate[arrayNumBonus].GetCountInLocation(useCountGate) < allBonusGenerate[arrayNumBonus].GetMaxCountInLocation(useCountGate)
            && allBonusGenerate[arrayNumBonus].bonusType != TimesBonuses.min15) {

          List<int> arr = new List<int>();

          for(int i = 0; i < objectsParametrs.Length; i++) {
            if(objectsParametrs[i].timeDiff == allBonusGenerate[arrayNumBonus].bonusType && objectsParametrs[i].ExistLocate(useCountGate))
              arr.Add(i);
          }

          int needNum = Random.Range(0,arr.Count);
          int needPosition = Random.Range(objectsParametrs[arr[needNum]].GetMinNumPosit(useCountGate), objectsParametrs[arr[needNum]].GetMaxNumPosit(useCountGate)+1);

          int repeat = 0;
          while(CheckPosition(arr[needNum], needPosition) && repeat < 20) {
            repeat++;

            if(repeat % 3 == 0)
              needNum = Random.Range(0, arr.Count);

            needPosition = Random.Range(objectsParametrs[arr[needNum]].GetMinNumPosit(useCountGate), objectsParametrs[arr[needNum]].GetMaxNumPosit(useCountGate) + 1);
          }


          GameObject clone = Instantiate(objectsParametrs[arr[needNum]].pref);
          clone.transform.position = objectsParametrs[arr[needNum]].position[needPosition].position;
          clone.GetComponent<MapActiveObject>().PositingObject(objectsParametrs[arr[needNum]].position[needPosition]);

          MapBonusGenerate[] newArr = new MapBonusGenerate[allBonusGenerate[arrayNumBonus].array.Length+1];
          for(int i = 0; i < allBonusGenerate[arrayNumBonus].array.Length; i++) newArr[i] = allBonusGenerate[arrayNumBonus].array[i];

          newArr[newArr.Length - 1].genPosition = needPosition;
          newArr[newArr.Length - 1].prefName = objectsParametrs[arr[needNum]].pref.name;
          newArr[newArr.Length - 1].coinsCount = Random.Range((int)objectsParametrs[arr[needNum]].coinsCount.min, (int)objectsParametrs[arr[needNum]].coinsCount.max + 1);
          newArr[newArr.Length - 1].location = useCountGate;

          allBonusGenerate[arrayNumBonus].array = newArr;
          allBonusGenerate[arrayNumBonus].SetLastGenerateInLocation(useCountGate, unixTime);

          clone.GetComponent<MapActiveObject>().SetBonus(allBonusGenerate[arrayNumBonus].bonusType, newArr[newArr.Length - 1]);

          SeriasableBonus();
        }
      }
    }
  }

  bool CheckPosition(int number, int pos) {

    bool result = false;

    Vector3 thisPosition = objectsParametrs[number].position[pos].position;

    foreach(var one in allBonusGenerate) {
      foreach(var one1 in one.array) {
        if(one1.genPosition == pos) {
          foreach(var one2 in objectsParametrs) {
            if(one2.pref.name == one1.prefName && one2.position[one1.genPosition].position == thisPosition) result = true;
          }
        }
      }
    }

    return result;
  }

  public void TapObject(TimesBonuses timeBonus, MapBonusGenerate tapBonus) {

    int deleteArrayElemJ = -1;
    int deleteArrayElemI = -1;

    for(int i = 0; i < allBonusGenerate.Length; i++) {
      if(allBonusGenerate[i].bonusType == timeBonus) {
        for(int j = 0; j < allBonusGenerate[i].array.Length; j++) {
          if(allBonusGenerate[i].array[j].location == tapBonus.location && allBonusGenerate[i].array[j].prefName == tapBonus.prefName && allBonusGenerate[i].array[j].genPosition == tapBonus.genPosition) {
            if(tapBonus.coinsCount == 0) {
              deleteArrayElemJ = j;
              deleteArrayElemI = i;
            } else {
              allBonusGenerate[i].array[j].coinsCount = tapBonus.coinsCount;
            }
          }
        }
      }
    }

    if(deleteArrayElemJ >= 0 && deleteArrayElemI >= 0) {
      MapBonusGenerate[] tmp = new MapBonusGenerate[allBonusGenerate[deleteArrayElemI].array.Length-1];
      for(int j = 0, k = 0; j < allBonusGenerate[deleteArrayElemI].array.Length; j++) {
        if(j != deleteArrayElemJ) {
          tmp[k].genPosition = allBonusGenerate[deleteArrayElemI].array[j].genPosition;
          tmp[k].coinsCount = allBonusGenerate[deleteArrayElemI].array[j].coinsCount;
          tmp[k].location = allBonusGenerate[deleteArrayElemI].array[j].location;
          tmp[k].prefName = allBonusGenerate[deleteArrayElemI].array[j].prefName;
          k++;
        }
      }
      allBonusGenerate[deleteArrayElemI].array = tmp;

      deleteArrayElemI = -1;
      deleteArrayElemJ = -1;
    }

    SeriasableBonus();
  }

  public GameObject coinsPref;

  public void GenerateCoins(Vector3 newPosition, int nomination) {
    GameObject clone = Instantiate(coinsPref, newPosition, Quaternion.identity) as GameObject;
    Transform tmpPos = gamePlay.GetComponent<MapGamePlay>().GetCoinsCountPositionInWorld();

		clone.GetComponent<Coin>().SetNomination(nomination);
    clone.GetComponent<Coin>().GenMap(tmpPos);
  }

  public void AddCoins(int nom) {
    int couns = UserManager.coins;
		UserManager.coins = couns + nom;
    AudioManager.PlayEffect(coinsClip[Random.Range(0, coinsClip.Length)], AudioMixerTypes.mapEffect);
    ShowCounts();
  }

  void UpdateBonus15Minut() {
    for(useCountGate = 0; useCountGate <= openGate; useCountGate++) {
      for(arrayNumBonus = 0; arrayNumBonus < allBonusGenerate.Length; arrayNumBonus++) {

        if((allBonusGenerate[arrayNumBonus].secondsWait - (unixTime - allBonusGenerate[arrayNumBonus].GetLastGenerateInLocation(useCountGate))) <= 0
            && allBonusGenerate[arrayNumBonus].GetCountInLocation(useCountGate) < allBonusGenerate[arrayNumBonus].GetMaxCountInLocation(useCountGate)
            && allBonusGenerate[arrayNumBonus].bonusType == TimesBonuses.min15) {

          List<int> arr = new List<int>();

          for(int i = 0; i < objects15minutes.Length; i++) {
            if(objects15minutes[i].location == useCountGate && !allBonusGenerate[arrayNumBonus].Exists(objects15minutes[i].objectId.ToString()))
              arr.Add(i);
          }

          int needNum = Random.Range(0,arr.Count);

          MapBonusGenerate[] newArr = new MapBonusGenerate[allBonusGenerate[arrayNumBonus].array.Length+1];
          for(int i = 0; i < allBonusGenerate[arrayNumBonus].array.Length; i++) newArr[i] = allBonusGenerate[arrayNumBonus].array[i];

          newArr[newArr.Length - 1].prefName = objects15minutes[arr[needNum]].objectId.ToString();
          newArr[newArr.Length - 1].coinsCount = Random.Range(10, 20);
          newArr[newArr.Length - 1].location = useCountGate;

          allBonusGenerate[arrayNumBonus].array = newArr;
          allBonusGenerate[arrayNumBonus].SetLastGenerateInLocation(useCountGate, unixTime);

          SeriasableBonus();
        }

      }
    }
  }

  bool CoinsOnject15Minut(int predId, Vector3 newPosition) {

    int deleteArrayElemJ = -1;
    int deleteArrayElemI = -1;
    bool serialize = false;

    for(int i = 0; i < allBonusGenerate.Length; i++) {
      if(allBonusGenerate[i].bonusType == TimesBonuses.min15) {
        for(int j = 0; j < allBonusGenerate[i].array.Length; j++) {
          if(predId.ToString() == allBonusGenerate[i].array[j].prefName && allBonusGenerate[i].array[j].coinsCount > 0) {
            GenerateCoins(newPosition, 1);
            allBonusGenerate[i].array[j].coinsCount--;
            serialize = true;
            if(allBonusGenerate[i].array[j].coinsCount <= 0) {
              deleteArrayElemJ = j;
              deleteArrayElemI = i;
            }

          }
        }
      }
    }

    if(deleteArrayElemJ >= 0 && deleteArrayElemI >= 0) {
      MapBonusGenerate[] tmp = new MapBonusGenerate[allBonusGenerate[deleteArrayElemI].array.Length-1];
      for(int j = 0, k = 0; j < allBonusGenerate[deleteArrayElemI].array.Length; j++) {
        if(j != deleteArrayElemJ) {
          tmp[k].genPosition = allBonusGenerate[deleteArrayElemI].array[j].genPosition;
          tmp[k].coinsCount = allBonusGenerate[deleteArrayElemI].array[j].coinsCount;
          tmp[k].location = allBonusGenerate[deleteArrayElemI].array[j].location;
          tmp[k].prefName = allBonusGenerate[deleteArrayElemI].array[j].prefName;
          k++;
        }
      }
      allBonusGenerate[deleteArrayElemI].array = tmp;

      deleteArrayElemI = -1;
      deleteArrayElemJ = -1;
    }
    if(serialize) {
      SeriasableBonus();
      return true;
    }

    return false;

  }

  /// <summary>
  /// Сереализация и сохранение данных массива
  /// </summary>
  public void SeriasableBonus() {

    var seriasableObject = new List<object>();

    foreach(MapBonus one in allBonusGenerate) {

      var level1 = new Dictionary<string,object>();
      level1["BonusType"] = one.bonusType.ToString();

      List<object> listArr = new List<object>();
      foreach(LocationParametr oneBonus in one.allGenerate) {
        var level2 = new Dictionary<string,string>();
        level2["location"] = oneBonus.location.ToString();
        level2["lastTimeGenerate"] = oneBonus.lastTimeGenerate.ToString();
        listArr.Add(level2);
      }
      level1["allGenerate"] = listArr;

      List<object> listArr2 = new List<object>();
      foreach(MapBonusGenerate oneBonus in one.array) {

        var level2 = new Dictionary<string,string>();
        level2["name"] = oneBonus.prefName;
        level2["positing"] = oneBonus.genPosition.ToString();
        level2["location"] = oneBonus.location.ToString();
        level2["coins"] = oneBonus.coinsCount.ToString();
        listArr2.Add(level2);
      }

      level1["array"] = listArr2;
      seriasableObject.Add(level1);
    }

    string seriasable = Json.Serialize(seriasableObject);
    PlayerPrefs.SetString("mapBonus", seriasable);
  }

  /// <summary>
  /// Восстановление сохраненных данных
  /// </summary>
  public void ReturnSerializableBonus() {

    string seriasableBonus = PlayerPrefs.GetString("mapBonus");
    if(seriasableBonus == "") {

      for(int i = 0; i < allBonusGenerate.Length; i++) {
        for(int j = 0; j < allBonusGenerate[i].allGenerate.Length; j++) {
          allBonusGenerate[i].allGenerate[j].lastTimeGenerate = 100;
        }
      }

      return;
    }

    var deseriasable = Json.Deserialize(seriasableBonus);

    foreach(var one in (List<object>)deseriasable) {

      string bonusType = ( (Dictionary<string, object>)one )["BonusType"].ToString();

      for(int i = 0; i < allBonusGenerate.Length; i++) {
        if(allBonusGenerate[i].bonusType.ToString() == bonusType) {
          var level1 = (Dictionary<string, object>)one;

          List<object>level2 = (List<object>)level1["allGenerate"];
          int numAnn = 0;
          foreach(object elementAllay in level2) {
            Dictionary<string,object> level3 = (Dictionary<string,object>)elementAllay;
            allBonusGenerate[i].allGenerate[numAnn].location = int.Parse(level3["location"].ToString());
            allBonusGenerate[i].allGenerate[numAnn].lastTimeGenerate = int.Parse(level3["lastTimeGenerate"].ToString());
            numAnn++;
          }

          List<object>level22 = (List<object>)level1["array"];
          allBonusGenerate[i].array = new MapBonusGenerate[level22.Count];
          numAnn = 0;
          foreach(object elementAllay in level22) {
            Dictionary<string,object> level3 = (Dictionary<string,object>)elementAllay;
            allBonusGenerate[i].array[numAnn].genPosition = int.Parse(level3["positing"].ToString());
            allBonusGenerate[i].array[numAnn].prefName = level3["name"].ToString();
            allBonusGenerate[i].array[numAnn].location = int.Parse(level3["location"].ToString());
            allBonusGenerate[i].array[numAnn].coinsCount = int.Parse(level3["coins"].ToString());
            numAnn++;
          }
        }
      }
    }
  }
  #endregion

  #region Editor

  List<GameObject> testActiveObjects;
  public void GenerateTestActiveObjects() {

    if(testActiveObjects != null && testActiveObjects.Count > 0)
      ClearAllayActiveObjects();
    else
      testActiveObjects = new List<GameObject>();

    foreach(GenerateObjects one in objectsParametrs) {
      foreach(ObjectPosition onePos in one.position) {
        GameObject clone = Instantiate(one.pref);
        clone.transform.position = onePos.position;
        clone.GetComponent<MapActiveObject>().PositingObject(onePos);
        testActiveObjects.Add(clone);
      }
    }
  }
  public void ClearAllayActiveObjects() {
    foreach(GameObject one in testActiveObjects)
      DestroyImmediate(one);
    testActiveObjects.Clear();
  }
  public void CloarBonuses() {
    PlayerPrefs.SetString("mapBonus", "");
  }

  #endregion

  #region Расстановка меток

  /// <summary>
  /// Префаб точек
  /// </summary>
  public GameObject pointPrefab;

  /// <summary>
  /// Расстановка точек
  /// </summary>
  public void SetPointPositions() {

    float positionBefore = 0;
    float thisPercent;

    for(int i = 0; i < GameManager.levelCount; i++) {

      GameObject pointInst = GameObject.Instantiate(pointPrefab);
      pointInst.transform.parent = transform;
      thisPercent = 100f / (GameManager.levelCount - 1) * 0.01f * i;
      thisPercent = GetPercent(i);

      pointInst.transform.position = path.GetComponent<BezierCurve>().GetPointAt(thisPercent);
      pointInst.GetComponent<MapPathPointer>().textMesh.text = i.ToString();

      if(i < GameManager.level)
        pointInst.GetComponent<MapPathPointer>().SetData(i, MapPathPointer.Status.complited);
      else if(i == GameManager.level) {
        SetCameraPoxitionX(pointInst.transform.position.x);
        pointInst.GetComponent<MapPathPointer>().SetData(i, MapPathPointer.Status.active);
      } else
        pointInst.GetComponent<MapPathPointer>().SetData(i, MapPathPointer.Status.ready);

      pointInst.GetComponent<MapPathPointer>().SetPath(path.GetComponent<BezierCurve>(), positionBefore, thisPercent);
			
      pointInst.GetComponent<MapPathPointer>().OnClick += MapLevelPointClick;

      positionBefore = thisPercent;
    }

  }

  float GetPercent(int num) {

    float perc = 0;
    float percMin = 0;

    float keysCount = 0;
    int pointNum = num;

    for(int i = 0; i < GameManager.Instance.mapRun.Count; i++) {
      keysCount = GameManager.Instance.mapRun[i].keys;
      if(keysCount <= pointNum) {
        pointNum -= GameManager.Instance.mapRun[i].keys;
        percMin = GameManager.Instance.mapRun[i].mapDistance;
      } else {

        if(i == 0) {
          keysCount--;
        } else {
          pointNum++;
        }

        return ((GameManager.Instance.mapRun[i].mapDistance - percMin) * 0.01f) / keysCount * pointNum + (percMin * 0.01f);
      }
    }

    return perc;
  }



	/// <summary>
	/// Событие тапа по точке на карте
	/// </summary>
	/// <param name="levelClick"></param>
	void MapLevelPointClick(int levelClick) {

    if(GameManager.Instance.isDebug && !GameManager.Instance.DebagValue("pointBrifing")) {
			GameManager.startFromMap = true;
			GameManager.Instance.ChangeLevel(levelClick);
			GameManager.LoadScene("ClassicRun");
			return;
    }

		LevelInfo levelInfo = UiController.ShowUi<LevelInfo>();
		levelInfo.SetLevel(levelClick);

		levelInfo.OnForward = () => {

			float energyValue = Company.Live.LiveCompany.Instance.value;

			if (energyValue != -1 && energyValue < Company.Live.LiveCompany.Instance.oneRunPrice) {
				EnergySaleShow(null);
			} else {
				levelInfo.Close();
				ConfirmPointLevel(levelClick);
			}
			
		};
		
  }

	void EnergySaleShow(Action OnClose) {
		EnergyDialog salePanel = UiController.ShowUi<EnergyDialog>();
		salePanel.gameObject.SetActive(true);
		salePanel.OnClose = OnClose;
	}


	/// <summary>
	/// Подтверждение клика по карте
	/// </summary>
	/// <param name="levelPoint"></param>
	void ConfirmPointLevel(int levelPoint) {
		GameManager.startFromMap = true;
		GameManager.Instance.ChangeLevel(levelPoint);
		GameManager.LoadScene("ClassicRun");
	}

	#endregion

	#region Иконка дополнительного квеста

	public GameObject oneQuestObject;           // Префаб на карте

  /// <summary>
  /// Инициализация одного квеста
  /// </summary>
  void InitOneQuest() {
    oneQuestObject.SetActive(true);
  }

  #endregion

}
