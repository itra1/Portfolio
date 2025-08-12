using System.Collections.Generic;
using UnityEngine;

public class WeaponManager: Singleton<WeaponManager> {

  [SerializeField]
  private List<WeaponBehaviour> weaponList;
  public List<WeaponGroup> groupList;

  public Sprite grayWeapon;
  public Sprite coinIcon;


  private void Start() {

    weaponList.ForEach(wep => {

      weaponData.Add(new WeaponData(wep));

    });
  }

  [HideInInspector]
  public List<WeaponBehaviour> instanceList = new List<WeaponBehaviour>();

  public void CreateInstances() {
    instanceList.Clear();
    weaponList.ForEach(wep => {
      if (!wep.weaponData.IsByed)
        return;
      WeaponBehaviour wp = Instantiate(wep.gameObject).GetComponent<WeaponBehaviour>();
      instanceList.Add(wp);
    });
  }


  public void SetActiveWeapon(string weaponUuid) {
    //weaponList.ForEach(x=>x.Set)
  }


  public void ByeGun(string uuid) {
    weaponList.Find(x => x.uuid == uuid).weaponData.IsByed = true;
  }

  public void AddBullets(string weaponUuid, int bullet) {

    if (instanceList.Count <= 0 || instanceList.Exists(x => x != null && x.uuid == weaponUuid)) {
    }
    //weaponPrefabs.Find(x => x.type == weaponType). = true;
  }

  public List<WeaponBehaviour> GetWeapons() {
    return weaponList;
  }
  public WeaponBehaviour GetWeaponByUuid(string uuid) {
    return weaponList.Find(x=>x.uuid.Equals(uuid));
  }
  public List<WeaponBehaviour> GetByedWeaponsByGroup(WeaponGroup.GroupType group) {
    return weaponList.FindAll(x => x.groupType == group && x.weaponData.IsByed);
  }
  public List<WeaponBehaviour> GetWeaponsEquipped() {
    return weaponList.FindAll(x => x.weaponData.IsEquipped);
  }
  public List<WeaponBehaviour> GetWeaponsForReward() {
    return weaponList.FindAll(x => !x.weaponData.IsByed && !x.IsTutorOnly);
  }

  public bool IsFullEquipped
  {
    get { return GetWeaponsEquipped().Count >= 4; }
  }

  [HideInInspector]
  public List<WeaponData> weaponData = new List<WeaponData>();

  public class WeaponData {
    public string uuid;
    public WeaponBehaviour weaponBehaviour;

    public WeaponData(WeaponBehaviour wepBehaviour) {
      weaponBehaviour = wepBehaviour;
      uuid = weaponBehaviour.uuid;
      weaponBehaviour.weaponData = this;
      Load();
    }

    public void Save() {

      SaveData saveData = new SaveData();
      saveData.isSelected = weaponBehaviour.IsSelected;
      saveData.isByed = _isByed;
      saveData.isEquipped = _isEquipped;
      saveData.index = index;

      PlayerPrefs.SetString(uuid, JsonUtility.ToJson(saveData));
    }

    public void Load() {

      if (!PlayerPrefs.HasKey(uuid)) {
        _isByed = false;
        return;
      }

      SaveData saveData = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(uuid));

      _isByed = saveData.isByed;
      weaponBehaviour.IsSelected = saveData.isSelected;
      _isEquipped = saveData.isEquipped;
      index = saveData.index;
    }

    public bool _isEquipped = false;
    public int index = 0;

    /// <summary>
    /// Экипировано
    /// </summary>
    public bool IsEquipped {
      get {
        return _isEquipped;
      }
      set {

        if (_isEquipped == value)
          return;

        _isEquipped = value;
        Save();
        weaponBehaviour.OnChengeWeapon();
      }
    }

    private bool _isByed = false;
    /// <summary>
    /// Куплено
    /// </summary>
    public bool IsByed {
      get {
        return _isByed || weaponBehaviour.isStartReady;
      }
      set {

        if (_isByed == value)
          return;

        WeaponManager.Instance.groupList.Find(x => x.type == weaponBehaviour.groupType).bulletCount += 50;

        _isByed = value;
        Save();
        weaponBehaviour.OnChengeWeapon();
      }
    }

    public class SaveData {
      public bool isSelected;
      public bool isByed;
      public bool isEquipped;
      public int index;
    }

  }

}
