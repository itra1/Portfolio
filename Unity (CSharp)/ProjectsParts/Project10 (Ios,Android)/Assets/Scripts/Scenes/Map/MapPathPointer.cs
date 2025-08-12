using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Настройка точки на карте
/// </summary>
public class MapPathPointer : MonoBehaviour {

  public enum Status { complited, active, ready }
  int level;

  Status status;

  public delegate void OnClickDelegate(int level);
  public OnClickDelegate OnClick;
  public LineRenderer lineRenderer;

  public Color colorComplited;
  public Color colorActive;
  public Color colorReady;
  public Color colorLineComplited;
  public Color colorLineActive;

  public GameObject tringle;
  public SpriteRenderer render;
  public TextMesh textMesh;

  void Start() {
		MapGamePlay.OnTapDrag += Drag;
  }

  void OnDestroy() {
		MapGamePlay.OnTapDrag -= Drag;
  }

  bool isClick;

  void Drag(Vector2 touchDelta) {
    if(isClick && touchDelta.x != 0)
      isClick = false;
  }
  

  /// <summary>
  /// Установка значения
  /// </summary>
  /// <param name="newStatus"></param>
  public void SetData(int newLevel, Status newStatus) {

    status = newStatus;

    switch(status) {
      case Status.active:
        render.color = colorActive;
        tringle.SetActive(true);
        break;
      case Status.complited:
        render.color = colorComplited;
        tringle.SetActive(false);
        break;
      case Status.ready:
        render.color = colorReady;
        tringle.SetActive(false);
        break;
    }
    level = newLevel;
    textMesh.text = (level+1).ToString();
  }

  public void SetPath(BezierCurve bezier, float percBefore, float percThis) {
    List<Vector3> points = new List<Vector3>();

    points.Add(bezier.GetPointAt(percBefore));
    
    while(percBefore < percThis) {
      percBefore += 0.01f;
      if(percBefore > percThis) percBefore = percThis;
      points.Add(bezier.GetPointAt(percBefore));
    }
    Vector3[] allPoints = points.ToArray();
    lineRenderer.positionCount = points.Count;
    lineRenderer.SetPositions(allPoints);
    
    switch(status) {
      case Status.active:
				lineRenderer.startColor = colorLineActive;
				lineRenderer.endColor = colorLineActive;
				//lineRenderer.SetColors(colorLineActive, colorLineActive);
        break;
      case Status.complited:
        render.color = colorComplited;
				lineRenderer.startColor = colorLineComplited;
				lineRenderer.endColor = colorLineComplited;
				//lineRenderer.SetColors(colorLineComplited, colorLineComplited);
				break;
      case Status.ready:
        //render.color = colorReady;
        lineRenderer.gameObject.SetActive(false);
        break;
    }

  }
  
  /// <summary>
  /// Обработка тапа
  /// </summary>
  public void OnMouseUp() {
    if(isClick && OnClick != null) OnClick(level);
  }

  public void OnMouseDown() {
    isClick = true;
  }

  #region Editor
  public LayerMask groundMask;
  public void PositingGround() {
    Ray ray = new Ray(transform.position+new Vector3(0,0,-2),transform.forward);
    RaycastHit hit;
    Physics.Raycast(ray, out hit, 7, groundMask);
    transform.position = hit.point;
  }

  #endregion

}
