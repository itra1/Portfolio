using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using Vectrosity;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CompanyRoad))]
public class CompanyRoadEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Positing lines")) {
			((CompanyRoad)target).PositingLines();
		}
		if (GUILayout.Button("Colored")) {
			((CompanyRoad)target).ColoredPercent(0.235f);
		}

	}
}

#endif


public class CompanyRoad : MonoBehaviour, IDragHandler {

  public BezierCurve bazier;

  public RectTransform roadObject;
  public RectTransform roadBack;

  public RoadPoint roadPoint;
  public List<RoadPoint> roadPointsList;

  public RectTransform roadSolid;

	public GameObject pathObject;
	public GameObject barrierRoad;

  public LineRenderer roadLineReady;
  public LineRenderer roadLineComplete;

  private float speedX;
	private int pixcelX;

	private float roadDistance = 2243;

  private bool isDrawRoad = false;

  private void OnEnable() {
		pixcelX = Screen.width;

    ProcessPoint();
    DrawRoad();
  }

	void ProcessPoint() {

    int levelCount = Config.Instance.config.levels.Count;

    DrawPoints(levelCount);
    DrawBackBack(levelCount);

    int cnt = (roadPointsList.Count < GameManager.level ? roadPointsList.Count : GameManager.level);

		for (int i = 0; i < roadPointsList.Count; i++) {
			if (i < cnt)
				roadPointsList[i].GetComponent<RoadPoint>().SetStatus(RoadPoint.Status.done);
			else if (i == cnt) {
				roadPointsList[i].GetComponent<RoadPoint>().SetStatus(RoadPoint.Status.active);
				CorrectPoint(i);
			} else
				roadPointsList[i].GetComponent<RoadPoint>().SetStatus(RoadPoint.Status.ready);
		}

	}

  public void DrawRoad() {
    if (isDrawRoad) return;
    DrawCurve();
    DrawColorBezier();
    DrawCompleteLine();
    PositingLines();
    isDrawRoad = true;
  }

  List<Vector3> ancorList = new List<Vector3>();

  private void DrawPoints(int levelCount) {

    ancorList.Clear();

    Vector2 rect = roadPoint.GetComponent<RectTransform>().anchoredPosition;
    roadPoint.gameObject.SetActive(false);

    for (int i = 0; i < levelCount; i++) {

      GameObject inst = Instantiate(roadPoint.gameObject);

      inst.transform.SetParent(roadPoint.transform.parent);
      inst.transform.localScale = Vector2.one;
      inst.gameObject.SetActive(true);
      inst.GetComponent<RectTransform>().anchoredPosition = new Vector2(rect.x + i * 400, rect.y);
      ancorList.Add(inst.transform.position);
      RoadPoint rp = inst.GetComponent<RoadPoint>();
      rp.SetData(i + 1, RoadPoint.Status.ready);
      roadPointsList.Add(rp);
    }
  }

  private void DrawBackBack(int levelCount) {

    float summaryLen = 1500 + levelCount * 430;

    roadSolid.sizeDelta = new Vector2(summaryLen, roadSolid.sizeDelta.y);
    
    Vector2 rectStart = roadBack.GetComponent<RectTransform>().anchoredPosition;
    float distance = rectStart.x;

    while (distance < summaryLen) {

      GameObject inst = Instantiate(roadBack.gameObject);
      inst.transform.SetParent(roadBack.transform.parent);
      inst.transform.localScale = Vector2.one;
      inst.gameObject.SetActive(true);
      inst.GetComponent<RectTransform>().anchoredPosition = new Vector2(distance, rectStart.y);
      distance += 716.9f;

    }
  }

  /// <summary>
  /// Отрисовка кривой Безье
  /// </summary>
  /// <param name="levelCount"></param>
  public void DrawCurve() {

    var points = bazier.GetAnchorPoints();

    for(int i = 0; i < points.Length; i++)
      bazier.RemovePoint(bazier[0]);

    bazier.AddPointAt(bazier.transform.position);

    int num = 0;
    ancorList.ForEach(elem => {
      num++;
      BezierPoint bp = bazier.AddPointAt(elem);
      bp.transform.SetParent(bazier.transform);
      bp.transform.position = elem;

      bp.handle1 = ((num & 1) != 0) ? new Vector3(-1.5f, 0.4f, 0) : new Vector3(-1.5f, -0.4f, 0);
      bp.handle2 = ((num & 1) != 0) ? new Vector3(1.5f, -0.4f, 0) : new Vector3(1.5f, 0.4f, 0);

    });

  }

  public void DrawColorBezier() {
    Vector3 startPosition = bazier.GetPointAt(0);
    Vector3 endPosition = bazier.GetPointAt(1);
    float distance = (endPosition - startPosition).magnitude;
    float deltaPerc = 10 / distance;

    Vector3 scaleParent = UiController.Instance.GetComponent<RectTransform>().localScale;

    List<Vector3> pointList = new List<Vector3>();

    float startPerc = 0;
    pointList.Add(new Vector3(startPosition.x / scaleParent.x - 740.5f, startPosition.y / scaleParent.y - 733.13f, 0));
    while (startPerc < 100) {
      startPerc += deltaPerc;
      Vector3 point = bazier.GetPointAt(startPerc/100);
      pointList.Add(new Vector3(point.x / scaleParent.x-740.5f, point.y / scaleParent.y-733.13f, 0));
    }
    pointList.Add(new Vector3(endPosition.x / scaleParent.x - 740.5f, endPosition.y / scaleParent.y - 733.13f, 0));

    roadLineComplete.positionCount = 0;
    roadLineReady.positionCount = pointList.Count;
    roadLineReady.SetPositions(pointList.ToArray());
  }

  public void DrawCompleteLine() {

    Vector3 scaleParent = UiController.Instance.GetComponent<RectTransform>().localScale;

    Vector3 startPosition = bazier.GetPointAt(0);
    Vector3 endPosition = bazier.GetPointAt(1);
    float distance = (endPosition - startPosition).magnitude;
    float deltaPerc = 10 / distance;

    //float targetPercent = (roadPoint.GetComponent<RectTransform>().anchoredPosition.x + GameManager.activeLevel * 400) / distance;
    float targetPercent = ((float)(GameManager.activeLevel+1)/ (float)roadPointsList.Count) * 100;
    float startPerc = 0;

    List<Vector3> pointList = new List<Vector3>();
    //pointList.Add(new Vector3(startPosition.x / scaleParent.x - 740.5f, startPosition.y / scaleParent.y - 733.13f, 0));

    while (startPerc < targetPercent) {
      Vector3 point = bazier.GetPointAt(startPerc / 100);
      pointList.Add(new Vector3(point.x / scaleParent.x - 740.5f, point.y / scaleParent.y - 733.13f, 0));
      startPerc += deltaPerc;
    }
    Vector3 targetPerc = bazier.GetPointAt(targetPercent / 100);
    endPosition = new Vector3(targetPerc.x / scaleParent.x - 740.5f, targetPerc.y / scaleParent.y - 733.13f, 0);

    pointList.Add(endPosition);
    roadLineComplete.positionCount = 0;
    roadLineComplete.positionCount = pointList.Count;
    roadLineComplete.SetPositions(pointList.ToArray());

  }

  void CorrectPoint(int point) {
		Vector2 pos = roadPointsList[point].GetComponent<RectTransform>().anchoredPosition;
		roadObject.anchoredPosition = new Vector2(-pos.x + (pixcelX / 2), roadObject.anchoredPosition.y);
		CorrectBorder();
		ColoredPercent(roadPointsList[point].GetComponent<RoadPoint>().positingPoint);
	}
	
	public void PositingLines() {
    Vector3 scaleParent = UiController.Instance.GetComponent<RectTransform>().localScale;
    float pos = 0;
		do {
			pos += Random.Range(0.0005f, 0.007f);
			GameObject inst = Instantiate(barrierRoad, barrierRoad.transform.parent);
			inst.transform.localScale = Vector2.one;

      Vector3 targetPerc = bazier.GetPointAt(pos);

      inst.GetComponent<RectTransform>().anchoredPosition = new Vector3(targetPerc.x / scaleParent.x - 640, targetPerc.y / scaleParent.y - 655, 0);
      inst.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, Random.Range(-25, 25));
			inst.SetActive(true);
		} while (pos < 1);

	}

	public void ColoredPercent(float percentColor) {
		//VectorLine line = pathObject.GetComponent<VectorObject2D>().vectorLine;

		//for (int i = 0; i < line.GetSegmentNumber() * percentColor; i++)
		//	line.SetColor(new Color(0.854f, 0.878f, 0.117f, 1), i);
		//line.Draw();

		//for (int i = 0; i < line.GetSegmentNumber() * percentColor; i++)
		//	line.SetColor(new Color(0.854f, 0.878f, 0.117f, 1), i);
		//line.Draw();
	}

	public void OnDrag(PointerEventData eventData) {
		roadObject.anchoredPosition = new Vector2(roadObject.anchoredPosition.x + (eventData.delta.x), roadObject.anchoredPosition.y);
		CorrectBorder();
	}

	void CorrectBorder() {
		if (roadObject.anchoredPosition.x >= 0)
			roadObject.anchoredPosition = new Vector2(0, roadObject.anchoredPosition.y);
		if (roadObject.anchoredPosition.x <= -roadDistance + pixcelX)
			roadObject.anchoredPosition = new Vector2(-roadDistance + pixcelX, roadObject.anchoredPosition.y);
	}

}
