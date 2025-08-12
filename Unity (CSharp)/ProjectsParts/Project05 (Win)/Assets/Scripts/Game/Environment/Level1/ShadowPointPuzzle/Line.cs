using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.Challenges.ShadowPointPuzzle
{
  public class Line: MonoBehaviourBase {
    [SerializeField]
    private LineRenderer _lineRendererPrefab;
    private List<LineRenderer> _lineList = new List<LineRenderer>();
    private LineRenderer _activeLine;

    public void Initiate() {
      Clear();
    }

    public void Clear() {
      _lineList.ForEach(x => x.gameObject.SetActive(false));
      _activeLine = null;
    }

    public LineRenderer GetInstance() {
      LineRenderer inst = Game.Utils.InstantiateUtils.GetDisableInstanceFromList<LineRenderer>(this, _lineRendererPrefab, _lineList);
      inst.gameObject.SetActive(true);
      return inst;
    }

    public void SetPoints(List<Vector3> points) {
      foreach (var point in points) {
        AppPoint(point);
      }
    }

    public void AppPoint(Vector3 point) {
      if (_activeLine != null) {
        _activeLine.SetPosition(1, point);
      }

      _activeLine = GetInstance();
      _activeLine.positionCount = 2;
      _activeLine.SetPosition(0, point);
      _activeLine.SetPosition(1, point);

    }

    public void RemoveLast()
    {
      if (_activeLine != null) {
        _activeLine.gameObject.SetActive(false);
        _activeLine = null;
      }
    }

    public void UpdateLastPoint(Vector3 point) {

      if (_activeLine == null)
        return;

      _activeLine.SetPosition(1, point);
    }


  }
}