using System.Collections;
using System.Collections.Generic;
using FoW;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GraphicOrderHelper))]
[CanEditMultipleObjects]
public class GraphicOrderHelperEditor: Editor {

  //int id;
  private string[] layerName;
  private int[] layersId;

  private GraphicOrderHelper parentScript;

  void OnEnable() {
    parentScript = (GraphicOrderHelper)target;
    layersId = new int[SortingLayer.layers.Length];
    layerName = new string[SortingLayer.layers.Length];
    for (int i = 0; i < SortingLayer.layers.Length; i++) {
      layerName[i] = SortingLayer.layers[i].name;
      layersId[i] = SortingLayer.layers[i].id;
    }
  }

  public override void OnInspectorGUI() {

    foreach (GraphicOrderHelper targ in targets) {
      GraphicOrderHelper script = (GraphicOrderHelper)targ;

      for (int i = 0; i < layersId.Length; i++)
        if (layersId[i] == script.layerId) parentScript.layerId = i;

      parentScript.layerId = EditorGUILayout.Popup("Layer ID:", parentScript.layerId, layerName);

      //script.layerId = layersId[parentScript.layerId];
    }

    base.OnInspectorGUI();

    if (GUILayout.Button("DrawSolidLine")) {
      foreach (var targ in targets) {
        (targ as GraphicOrderHelper).DrawSolidLine();
      }
    }

    if (GUILayout.Button("DrawShtrihLine")) {
      foreach (var targ in targets) {
        (targ as GraphicOrderHelper).DrawShtrihLine();
      }
    }
  }
}

#endif

public class GraphicOrderHelper: MonoBehaviour {

  //[HideInInspector]
  //public int layerId;
  public int orderSprite;
  public int layerId;

  public float segmentCount = 10;

  public LineRenderer solidLine;
  public LineRenderer shtrihLine;
  public List<MapPointGraphic> mapPoints;
  public BezierCurve bezierCurve;
  readonly bool isReady = false;

  public FogOfWar FoWCamera;

  public void Initiate() {
    bool existsOpen = mapPoints.Exists(x => ((x.LevelInfo.Status & PointSatus.IsActive) != 0));
    bool existsClose = mapPoints.Exists(x => ((x.LevelInfo.Status & PointSatus.closed) != 0));
    bool existsApendix = mapPoints.Exists(x => x.LevelInfo.Mode == PointMode.appendix);
    bool openApendix = mapPoints.Exists(x => x.LevelInfo.isApendixComplete);

    shtrihLine.gameObject.SetActive(existsOpen && existsClose);
    solidLine.gameObject.SetActive(existsOpen && !existsClose && (!existsApendix || openApendix));

    if (existsOpen && !existsClose)
      fog = GetComponentInChildren<FogOfWarUnit>();

  }

  private FogOfWarUnit fog;

  IEnumerator Corat() {
    FogOfWarUnit fog = GetComponentInChildren<FogOfWarUnit>();

    float val = 0;
    if (FoWCamera != null)
      FoWCamera.Draw();
    while (val < 1) {
      fog.transform.position = bezierCurve.GetPointAt(val);
      val += (1f / segmentCount);
      if (FoWCamera != null)
        FoWCamera.Draw();
      yield return new WaitForSeconds(1);
    }
    fog.transform.position = bezierCurve.GetPointAt(val);
    if (FoWCamera != null)
      FoWCamera.Draw();

  }

  private float valLine = 0;
  private void Update() {

    if (valLine < 1 && fog != null) {
      fog.transform.position = bezierCurve.GetPointAt(valLine);
      if (FoWCamera != null)
        FoWCamera.Draw();
      valLine += (1f / segmentCount);
      if (valLine > 1) valLine = 1;
    }

  }

  public void DrawSolidLine() {

    List<Vector3> pointList = new List<Vector3>();

    float val = 0;

    while (val < 1) {
      pointList.Add(bezierCurve.GetPointAt(val));
      val += (1f / segmentCount);
    }
    pointList.Add(bezierCurve.GetPointAt(1));

    solidLine.positionCount = pointList.Count;
    solidLine.SetPositions(pointList.ToArray());
  }

  public void DrawShtrihLine() {

    List<Vector3> pointList = new List<Vector3>();

    float val = 0;

    while (val < 1) {
      pointList.Add(bezierCurve.GetPointAt(val));
      val += (1f / segmentCount);
    }
    pointList.Add(bezierCurve.GetPointAt(1));

    shtrihLine.positionCount = pointList.Count;
    shtrihLine.SetPositions(pointList.ToArray());
  }

}
