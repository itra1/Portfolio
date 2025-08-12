using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Location", menuName = "Create/Location")]
public class Location: ScriptableObject {
#if UNITY_EDITOR
  [SerializeField]
  private string titleEditor;
#endif
  [UUID]
  public string uuid;

  [SerializeField]
  private int m_Index;
  public int Index {
    get { return m_Index; }
  }

  [SerializeField]
  private int _block;
  public int Block {
    get { return _block; }
  }

  public int Level {
    get { return _block * 100 + m_Index; }
  }

  private Sprite _icon;
  public Sprite IconLogo {
    get { return _icon; }
  }

  private Sprite _backGround;
  public Sprite BackGround {
    get { return _backGround; }
  }
  [SerializeField]
  private string _title;
  public string Title {
    get { return _title; }
  }
  [SerializeField]
  private string _description;
  public string Description {
    get { return _description; }
  }
  public int stageCount;
  public bool bossExists;

  public int goldWin;

  public EnemyProgressChange changeHealth;
  [System.Serializable]
  public struct ValueSpan {
    public float min;
    public float max;
  }
  public ValueSpan health;

  /// <summary>
  /// Механизм увеличения жизней врага в бою
  /// </summary>
  public enum EnemyProgressChange {
    random,     // Случайный в диапазоне
    linear      // Линейное увеличение
  }

  public float GetEnemyHealth(int stage) {

    switch (changeHealth) {
      case EnemyProgressChange.linear:
        if(stage <= 0)
          return health.min;
        else
        return ((health.max - health.min) / stageCount) * stage + health.min;
      default:
        return Random.Range(health.min, health.max);
    }

  }

  public EnemyProgressChange changeDamage;
  public ValueSpan damage;

  public float GetEnemyDamage(int stage) {

    switch (changeDamage) {
      case EnemyProgressChange.linear:
        if (stage <= 0)
          return damage.min;
        else
          return ((damage.max - damage.min) / stageCount) * stage + damage.min;
      default:
        return Random.Range(damage.min, damage.max);
    }
  }

  public EnemyProgressChange changeReload;
  public ValueSpan reload;

  public float GetEnemyReload(int stage) {

    switch (changeReload) {
      case EnemyProgressChange.linear:
        if (stage <= 0)
          return damage.min;
        else
          return ((reload.max - reload.min) / stageCount) * stage + reload.min;
      default:
        return Random.Range(reload.min, reload.max);
    }
  }

  public List<DropStruct> dropList;

  public string backgroundClipName;

  [System.Serializable]
  public struct DropStruct {
    public DropType type;
    public SpanFloat values;
    public float probability;
    [HideInInspector]
    public GameObject prefab;

    public float GetValue() {
      return Random.Range(values.min, values.max);
    }

  }

  public void Initiate() {
    InitGraphic();
  }

  private void InitGraphic() {
    if (_icon == null)
      _icon = Resources.Load<Sprite>(String.Format("Graphic/Locations/{0}_i", Level));
    if (_backGround == null)
      _backGround = Resources.Load<Sprite>(String.Format("Graphic/Locations/{0}", Level));
  }

  public void Complete() {
    UserManager.Instance.CompleteLastLevel = Level;
    UserManager.Instance.MaxCompleteLevel = Level;
    Debug.Log("Save CompleteLastLevel: " + UserManager.Instance.CompleteLastLevel);
    Debug.Log("Save MaxCompleteLevel: " + UserManager.Instance.MaxCompleteLevel);
  }

  /// <summary>
  /// Возвращает текущую локацию
  /// </summary>
  /// <param name="location">Локация</param>
  /// <returns>успешно полученя</returns>
  public bool GetNextLocation(out Location location) {
    return LocationManager.Instance.GetNextLocation(this, out location);
  }
  public Location GetNextLocation() {
    return LocationManager.Instance.GetNextLocation(this);
  }

  public bool ReadyPlay {
    get { return IsComplete || IsReady; }
  }

  public bool IsComplete {
    get { return UserManager.Instance.MaxCompleteLevel >= Level; }
  }
  public bool IsReady {
    get { return UserManager.Instance.MaxCompleteLevel + 1 == Level; }
  }
  public bool IsWait {
    get { return UserManager.Instance.MaxCompleteLevel + 1 < Level; }
  }

  public StateType State {
    get {
      if (IsComplete)
        return StateType.complete;
      else if (IsReady)
        return bossExists ? StateType.boss : StateType.active;
      else
        return StateType.wait;
    }
  }

  public enum StateType {
    wait,
    complete,
    active,
    boss
  }

}

[System.Flags]
public enum DropType {
  none = 0,
  coins = 1,
  medical = 2,
  energy = 4,
  bulletPistol = 8,
  bulletUzi = 16,
  bulletObrez = 32,
  bulletAutomat = 64,
  hardCashe = 128
}
