using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationManager: Singleton<LocationManager> {

  [SerializeField]
  private List<Location> _location;

  private void Start() {
    LoadLocations();
  }

  private void LoadLocations() {
    _location = Resources.LoadAll<Location>("Locations").ToList();
  }

  public Location FindLocation(int block, int index) {
    return _location.Find(x => x.Block == block && x.Index == index);
  }
  public Location FindLocation(int level) {
    return _location.Find(x => x.Level == level);
  }

  /// <summary>
  /// Возвращает следующую локацию
  /// </summary>
  /// <param name="source">Текущая локация</param>
  /// <param name="next">Следующая локация</param>
  /// <returns>Успешно найден</returns>
  public bool GetNextLocation(Location source, out Location next) {
    next = _location.Find(x => x.Level == source.Level + 1);
    return next != null;
  }
  /// <summary>
  /// Возвращает следующую локацию
  /// </summary>
  /// <param name="source">Текущая локация</param>
  /// <returns>Успешно найден</returns>
  public Location GetNextLocation(Location source) {
    return _location.Find(x => x.Level == source.Level + 1);
  }

  public Location GetLocationByIndex(int index)
  {
    return _location[index];
  }
  public List<Location> GetLocationsByBlock(int block)
  {
    return _location.FindAll(x => x.Block == block).OrderBy(x=>x.Level).ToList();
  }

  public Location GetLastLocation()
  {
    return _location[_location.Count-1];
  }

  public int GetMaxBlock()
  {
    return _location.OrderByDescending(x => x.Block).ToArray()[0].Block;
  }

}
