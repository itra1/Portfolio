using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGroup: MonoBehaviour {

  public GroupType type;
  public string title;
  private bool isInstance = false;
  public int order;
  public bool wishBullets = true;

  public Sprite bulletIcon;

  public int _bulletCount;
  public int bulletCount {
    get {
      if (!isInstance)
        Load();
      return _bulletCount;
    }
    set {
      _bulletCount = value;
      if (!isInstance)
        Save();
      GroupChange();
    }
  }
  public enum GroupType {
    pistol,
    usi,
    obrez,
    automat,
    others
  }


  private void GroupChange() {
    ExEvent.BattleEvents.WeaponGroupChange.Call(this);
  }

  private void Start() {
    isInstance = true;
  }

  private string playerPrefsKey {
    get {
      return "weaponGroup" + type.ToString();
    }
  }

  public void Save() {

    SaveData saveData = new SaveData() {
      bulletCount = _bulletCount
    };
    PlayerPrefs.SetString(playerPrefsKey, JsonUtility.ToJson(saveData));
  }

  public void Load() {

    if (!PlayerPrefs.HasKey(playerPrefsKey)) {
      return;
    }

    SaveData saveData = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(playerPrefsKey));
    
    this._bulletCount = saveData.bulletCount;
  }

  public class SaveData {
    public bool isSelected;
    public int bulletCount;
    public bool isByed;
  }

}
