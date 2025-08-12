using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZbCatScene;
using Game.User;

/// <summary>
/// Параметры карты
/// </summary>
[System.Serializable]
public struct MapPointParametr {
  public Vector3 position;
  public LevelInfo pointInfo;
}

/// <summary>
/// Менеджер карты
/// </summary>
public class MapManager: Singleton<MapManager> {

  public delegate void DragCameraDelegae();
  public static event DragCameraDelegae OnDragCamera;

  public CameraMapMove cameraMove;
  
  public GameObject cameraObject;             // Объект камеры на сцене

  public Camera cameraFog;                          // Контроллер кармеры

  public TextAsset pointsData;

  void Start() {
    //backMusic = AudioManager.PlayEffects(backClip, AudioMixerTypes.musicPlay);
    AudioManager.Instance.PlayBackGroundSound("BackGrounds/main_menu");

    if (UserManager.Instance.toCrussade) {
      ShowDialogToCrussade();
    }

    Time.timeScale = 1;
    ShowGamePlay();

        //instGamePlay.OnZoomEvent += ScrollButton;
        for (int i = 0; i < points.Count; i++) {
          if(points[i].LevelInfo.Mode == PointMode.appendix) {
            for (int j = 0; j < points[i].LevelInfo.PrevPoints.Count; j++) {
              if (points[points[i].LevelInfo.PrevPoints[j]].LevelInfo.isApendixComplete) {
                  points[i].LevelInfo.Status = PointSatus.open;
                  points[i].LevelInfo.isApendixComplete = true;
                  points[i].LevelInfo.Save();
              }
            }
          }
        }
        //LoadDataPoints();
        //PositingPoints();
        PointInitiate();

    OpenBlack(null);

    ZbCatScene.CatSceneManager.Instance.ShowCatScene(3, () => { });

    Invoke("ShowCatScene", 1);
  }

  private void ShowGamePlay() {
    UIController.ShowUi<Game.UI.MapGamePlay>().gameObject.SetActive(true);
  }

  void ShowCatScene() {

    if(!ZbCatScene.CatSceneManager.Instance.IsOn)
      return;

    ZbCatScene.CatSceneManager.Instance.ShowCatScene(3, () => { });

    if (UserManager.Instance.CheckLevelComplited(3, 5))
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(9, () => { });

    if (UserManager.Instance.CheckLevelComplited(1, 5) && UserManager.Instance.silverCoins.Value >= 2500)
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(12, () => { });

    if (UserManager.Instance.CheckLevelComplited(2, 5))
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(16, () => { });

    if (UserManager.Instance.CheckLevelComplited(4, 5))
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(19, () => { });

    if (UserManager.Instance.CheckLevelComplited(2, 1))
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(20, () => { });

    if (UserManager.Instance.CheckLevelComplited(2, 2))
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(21, () => { });

    if (UserManager.Instance.CheckLevelComplited(3, 5))
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(22, () => { });

    if (UserManager.Instance.CheckLevelComplited(6, 1))
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(23, () => { });

    if (UserManager.Instance.CheckLevelComplited(6, 2))
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(24, () => { });


    if (UserManager.Instance.CheckLevelComplited(9, 5)) {

      if (!Game.User.UserWeapon.Instance.ExistWeaponType(Game.Weapon.WeaponType.bear))
        ZbCatScene.CatSceneManager.Instance.ShowCatScene(29, () => { });
      else {
        ZbCatScene.CatSceneManager.Instance.ShowCatScene(31, () => { });
        Game.User.UserWeapon.Instance.LostWeapons();
      }
    }

    if (UserManager.Instance.CheckLevelComplited(21, 1)) {
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(32, () => { });
      Game.User.UserWeapon.Instance.ReturnListWeaponType(Game.Weapon.WeaponType.bear);
      Game.User.UserWeapon.Instance.ReturnLostWeapons(0.1f);
    }

    if (UserManager.Instance.CheckLevelComplited(21, 2)) {
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(33, () => { });
      Game.User.UserWeapon.Instance.ReturnLostWeapons(0.3f);
    }

    if (UserManager.Instance.CheckLevelComplited(21, 3)) {
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(34, () => { });
      Game.User.UserWeapon.Instance.ReturnLostWeapons(0.3f);
    }

    if (UserManager.Instance.CheckLevelComplited(21, 4)) {
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(35, () => { });
      Game.User.UserWeapon.Instance.ReturnListWeaponType(Game.Weapon.WeaponType.hunter);
      Game.User.UserWeapon.Instance.ReturnLostWeapons(0.3f);
    }

    if (UserManager.Instance.CheckLevelComplited(1, 2))
      ZbCatScene.CatSceneManager.Instance.ShowCatScene("36_1", () => { });

    if (CatSceneManager.Instance.isSpecLevel)
      ZbCatScene.CatSceneManager.Instance.ShowCatScene("36_4", () => { });

    if (!Game.User.UserWeapon.Instance.ExistsBullet() && !GameDesign.Instance.shopLibrary.shopLibrary.Find(x => x.price <= UserManager.Instance.silverCoins.Value))
      ZbCatScene.CatSceneManager.Instance.ShowCatScene(37, () => { });

  }

  protected override void OnDestroy() {
    base.OnDestroy();
  }
  
  #region Точки

  [SerializeField]
  public List<MapPointGraphic> points;
  [SerializeField]
  private List<GraphicOrderHelper> lines;


  /// <summary>
  /// Тап по точке на карте
  /// </summary>
  /// <param name="pointInfo"></param>
  public void TapMapPoint(Vector3 targetPosition, LevelInfo pointInfo) {
    cameraMove.ZoomPoint(targetPosition, ()=> {
      OpenBrif(pointInfo);
    });
  }

  public void OpenBrif(LevelInfo pointInfo) {
    Game.UI.Briefing instBrief = UIController.ShowUi<Game.UI.Briefing>();
    instBrief.gameObject.SetActive(true);
    instBrief.OnStart = TapMapPointConfirm;
    instBrief.FillInfo(pointInfo);
    instBrief.OnCancel = () => {
      cameraMove.CancelZoomPoint();
    };
  }

  public void TapMapPointConfirm(LevelInfo pointInfo) {
    UserManager.Instance.ActiveBattleInfo = pointInfo;
    
    CloseBlack(() => {
      SceneManager.LoadScene("Battle");
    });

  }

  public void PointInitiate() {

    var request = from p in points
                  join l in LevelsManager.Instance.LevelsList
                    on (p.Group * 100 + p.Level) equals (l.Group * 100 + l.Level)
                  select new { point = p, level = l };

    foreach (var p in request) {
      p.point.LevelInfo = p.level;
    }

    lines.ForEach(x => x.Initiate());

  }


  //void PositingPoints() {

  //List<LevelInfo> pointUser = new List<LevelInfo>();
  //try {
  //  pointUser = User.Instance.company.levelsData.ToList();
  //} catch { }

  //List<int> openGroup = new List<int>();
  //openGroup.Add(1);

  //for (int i = 0; i < points.Count; i++) {

  //  if (pointUser.Count == 0 && points[i].LevelInfo.Group == 1 && points[i].LevelInfo.Level == 1) {
  //    points[i].LevelInfo.Status = PointSatus.open;
  //    points[i].InitData();
  //    continue;
  //  }

  //  LevelInfo userInfoPoint = pointUser.Find(x => x.Group == points[i].LevelInfo.Group && x.Level == points[i].LevelInfo.Level);

  //  List<LevelInfo> conditionInfo = new List<LevelInfo>();
  //  if (points[i].LevelInfo.PrevPoints != null && points[i].LevelInfo.PrevPoints.Count > 0)
  //    conditionInfo = pointUser.FindAll(x => points[i].LevelInfo.PrevPoints.Exists(ex => ex == x.PointNum));

  //  if (userInfoPoint == null) {

  //    if (openGroup.Contains(points[i].LevelInfo.Group) && points[i].LevelInfo.Mode != PointMode.appendix)
  //      points[i].LevelInfo.Status = PointSatus.closed;

  //    foreach (LevelInfo oneCond in conditionInfo) {

  //      if (points[i].LevelInfo.Mode == PointMode.appendix) {
  //        if (points[i].LevelInfo.isApendixComplete)
  //          points[i].LevelInfo.Status = PointSatus.closed;
  //      } else {
  //        if (oneCond.Mode == PointMode.farm) {
  //          if (!openGroup.Contains(points[i].LevelInfo.Group)) openGroup.Add(points[i].LevelInfo.Group);
  //          points[i].LevelInfo.Status = PointSatus.open;
  //        }
  //      }
  //    }

  //  } else {

  //    if (!openGroup.Contains(points[i].LevelInfo.Group))
  //      openGroup.Add(points[i].LevelInfo.Group);

  //    points[i].LevelInfo = userInfoPoint;

  //  }
  //  points[i].InitData();

  //}

  //}

  #endregion

  public void SavePoints() {

    List<LevelInfo> savePoints = new List<LevelInfo>();
    points.ForEach(x => savePoints.Add(x.LevelInfo));

    string json = Newtonsoft.Json.JsonConvert.SerializeObject(savePoints);

    string dbPath = "Assets/Texts/mapPoint_database.txt";
    var sw = new StreamWriter(dbPath);
    sw.WriteLine(json);
    sw.Close();
    Debug.Log("Database " + dbPath + " recorder successfully");

  }

  public void LoadDataPoints() {
    string data = pointsData.text;

    List<LevelInfo> loadLevelInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LevelInfo>>(data);

    for (int i = 0; i < loadLevelInfo.Count; i++) {
      points[i].LevelInfo = loadLevelInfo[i];
    }
  }

  void ShowDialogToCrussade() {
    UserManager.Instance.toCrussade = false;
    UIController.Instance.MessageDialog("Переход в режим Крестового похода");
    UserManager.Instance.toCrussade = false;
  }

  void ShowDialogToCrussadeOk() {
    UserManager.Instance.toCrussade = false;
  }

  private void OpenBlack(Action OnComplited) {
    FillBlack inst = UIController.ShowUi<FillBlack>();
    inst.gameObject.SetActive(true);
    inst.PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.open, () => {
      if (OnComplited != null) OnComplited();
      inst.gameObject.SetActive(false);
    });

  }

  private void CloseBlack(Action OnComplited) {
    FillBlack inst = UIController.ShowUi<FillBlack>();
    inst.gameObject.SetActive(true);
    inst.PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.close, OnComplited);
  }

}
