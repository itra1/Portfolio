using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor: Editor {

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    if (GUILayout.Button("Create map")) {
      ((MapManager)target).CreateMap();
    }

  }

}


#endif

public class MapManager: Singleton<MapManager> {

  [SerializeField]
  private Transform mapParent;
  [SerializeField]
  private Transform mapBlocksParent;
  [SerializeField]
  public Transform bigDecorParent;
  [SerializeField]
  public Transform mapPointParent;
  [SerializeField]
  private GameObject mapPointPrefab;
  [SerializeField]
  private List<MapBlock> mapBlockPrefabsList;
  [SerializeField]
  private MapBlock mapTransactionPrefabsList;

  public Transform cloudBlock;
  private MapPoint mp;

  private MapScroll _mapScroll;

  public List<MapGraphicLibrary> mapGraphicLibrary;

  public MapScroll MapScroll {
    get {
      if (_mapScroll == null)
        _mapScroll = GetComponent<MapScroll>();
      return _mapScroll;
    }
  }

  public List<MapPoint> mapPoints = new List<MapPoint>();

  [HideInInspector]
  public MapUI menu;
  private void Start() {

    menu = UIManager.Instance.GetPanel<MapUI>();

    menu.OnMenu = () => {
      GameManager.Instance.ToBack();
    };

    Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 5));
    Vector3 pos0 = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 5));
    mapParent.localScale = new Vector3(pos.x / 5.3f, pos.x / 5.3f, pos.x / 5.3f);

    MapPoint mpMax = mapPoints.Find(x => x.number == PlayerManager.Instance.company.lastLevelComplete + 14);

    if (mpMax != null) {
      MapScroll.maxPosition = mpMax.transform.position.y;
      cloudBlock.position = new Vector3(0, mpMax.transform.position.y + ((pos.y - pos0.y) / 2) * mapParent.localScale.y, 0);
    } else {
      MapScroll.maxPosition = mapPoints[mapPoints.Count - 1].transform.position.y;
      cloudBlock.gameObject.SetActive(false);
    }

    mp = mapPoints.Find(x => x.number == PlayerManager.Instance.company.lastLevelComplete + 1);
    if (mp == null)
      mp = mapPoints[mapPoints.Count - 1];

    MapScroll.SetPosition(mp.transform.position.y);

    MapScroll.heightPixel = (pos.y - pos0.y) / Screen.height;
    MapScroll.visibleHeight = (pos.y - pos0.y);
    MapScroll.SetPosition(0);
  }

  public void CreateMap() {

    while(mapBlocksParent.childCount > 0) {
      DestroyImmediate(mapBlocksParent.GetChild(0).gameObject);
    }
    while (bigDecorParent.childCount > 0) {
      DestroyImmediate(bigDecorParent.GetChild(0).gameObject);
    }
    while (mapPointParent.childCount > 0) {
      DestroyImmediate(mapPointParent.GetChild(0).gameObject);
    }

    

    CreateMapPoints();
  }

  private void CreateMapPoints() {

    int actualRegion = 0;
    int targetRegion = 0;
    float lastPercent = 0;
    int numberBlock = -1;
    float lastMapBlockYPosition = 0;
    sizeLastBlockY = 0;
    mapParent.localScale = Vector3.one;
    mapPoints.Clear();

    float distanceBetweenPoint = 3;
    float actualDistance = 0;

    int allCountInRegion = PlayerManager.Instance.company.companies.Max(x => x.levels.Count) / 4;
    int countInregion = 0;

    MapBlock createBlock = null;
    Vector3 positionPoint = Vector3.zero;
    Vector3 peforeDistance = Vector3.zero;

    for (int i = 0; i < PlayerManager.Instance.company.companies.Max(x => x.levels.Count); i++) {
      countInregion++;

      while (actualDistance < distanceBetweenPoint) {
        lastPercent += 0.05f;
        if (createBlock == null || lastPercent >= 1) {
          numberBlock++;

          if (countInregion >= allCountInRegion) {
            actualRegion++;
            countInregion = 0;
          }

          createBlock = CreateRoadBlocks(actualRegion != targetRegion ? actualRegion - 1 : actualRegion, numberBlock, ref lastMapBlockYPosition, actualRegion != targetRegion);

          if (actualRegion != targetRegion) {
            numberBlock = -1;
            targetRegion = actualRegion;
          }

        }
        if (lastPercent >= 1f)
          lastPercent -= 1f;

        lastPercent = (float)System.Math.Round(lastPercent, 2);

        positionPoint = createBlock.bezier.GetPointAt(1 - lastPercent);
        actualDistance += (positionPoint - peforeDistance).magnitude;
        peforeDistance = positionPoint;
      }
      //Debug.Log(lastPercent);

      GameObject instPoint = Instantiate(mapPointPrefab, mapPointParent);
      instPoint.transform.position = positionPoint;
      instPoint.GetComponent<MapPoint>().SetType(i);
      actualDistance = 0;

      mapPoints.Add(instPoint.GetComponent<MapPoint>());
    }
  }

  private float sizeLastBlockY = 0;

  private MapBlock CreateRoadBlocks(int region, int numberBlock, ref float lastMapBlockYPosition, bool isTransaction) {

    GameObject inst = isTransaction ?
      Instantiate(mapTransactionPrefabsList.gameObject, mapBlocksParent)
      : Instantiate(mapBlockPrefabsList[numberBlock % mapBlockPrefabsList.Count].gameObject, mapBlocksParent);

    MapBlock mapBlock = inst.GetComponent<MapBlock>();
    lastMapBlockYPosition += sizeLastBlockY/2;

    sizeLastBlockY = mapBlock.SetGraphic(mapGraphicLibrary[region], numberBlock % mapBlockPrefabsList.Count, isTransaction);

    inst.GetComponent<SortingGroup>().sortingOrder = -numberBlock;
    inst.transform.position = new Vector3(0, lastMapBlockYPosition + sizeLastBlockY / 2, 0);

    mapBlock.back.size = new Vector2(mapBlock.back.size.x, mapBlock.road.size.y);
    lastMapBlockYPosition += sizeLastBlockY/2;
    mapBlock.Inite(mapGraphicLibrary[region]);

    return mapBlock;
  }
  
  public void SetMapPoint(MapPoint mapPoint) {

    if (mapPoint.number >= 5 && !PlayerManager.Instance.noAds) {

      GoogleAdsMobile.Instance.ShowInterstitialVideo(
        complete: (result) => {
          Play(mapPoint);
        }
      );

    } else {
      Play(mapPoint);
    }

  }

  private void Play(MapPoint mapPoint) {

    PlayerManager.Instance.company.actualLevelNum = mapPoint.number;
    GameManager.gamePhase = GamePhase.game;
    GameManager.Instance.PlayGame(() => { });
  }


}
