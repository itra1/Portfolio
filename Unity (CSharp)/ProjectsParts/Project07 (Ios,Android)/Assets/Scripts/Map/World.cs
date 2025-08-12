using System.Collections.Generic;
using UnityEngine;

namespace Game.Map {

  public class World: MonoBehaviour {
    [SerializeField]
    private int _block;

    public int Block {
      get { return _block; }

    }
    [SerializeField]
    private List<MapPoint> _mapPoints;

    private List<Location> _locations;

    private void OnEnable() {
      Initiate();
    }

    private void Initiate() {
      _locations = LocationManager.Instance.GetLocationsByBlock(_block);

      for (int i = 0; i < _mapPoints.Count; i++) {
        _mapPoints[i].SetState(_locations[i].State);
        _mapPoints[i].onClick = ClickPoint;
      }
    }

    private void ClickPoint(int pointIndexs)
    {
      Location loc = LocationManager.Instance.FindLocation(_block, pointIndexs);
      if (!loc.ReadyPlay)
        return;

      MapController.Instance.ClickMapPoint(loc);
    }

  }
}