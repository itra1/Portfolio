using System.Collections.Generic;
using UnityEngine;
using Game.Weapon;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(Game.User.UserWeapon))]
public class UserWeaponEditor : Editor
{
  private Game.Weapon.WeaponType weaponType;
  private int count;

  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();

    GUILayout.BeginHorizontal();

    weaponType = (Game.Weapon.WeaponType)EditorGUILayout.EnumFlagsField(weaponType);
    count = EditorGUILayout.IntField(count);

    if (GUILayout.Button("Add"))
    {
      ((Game.User.UserWeapon)target).AddWeapon(weaponType, count);
    }

    GUILayout.EndHorizontal();
  }

}


#endif

namespace Game.User {
  /// <summary>
  /// Оружие игрока
  /// </summary>
  [System.Serializable]
  public class UserWeapon: Singleton<UserWeapon> {
    public List<WeaponManager> weaponsManagers;                          // Массив всех существующих мнеджеров

    public static event System.Action<WeaponType, int> OnChangeBulletCount;

    public List<WeaponType> selectCompany = new List<WeaponType>();
    public List<WeaponType> selectSurvival = new List<WeaponType>();

    protected override void Awake() {
      base.Awake();
      WeaponManager.OnWeaponChange += OnWeaponChange;
    }
    protected override void OnDestroy() {
      base.OnDestroy();
      WeaponManager.OnWeaponChange -= OnWeaponChange;
    }

    public void SetDefaultWeapon() {

      selectCompany.Clear();
      selectSurvival.Clear();

      AddWeapon(WeaponType.automat, 1100);
      AddWeapon(WeaponType.tomatGun, 1100);

      //AddWeapon(WeaponType.axe, 1100);
      //AddWeapon(WeaponType.bazooka, 1100);
      //AddWeapon(WeaponType.bottle, 1100);
      //AddWeapon(WeaponType.brick, 1100);
      //AddWeapon(WeaponType.holyGrenade, 1100);
      //AddWeapon(WeaponType.hunter, 1100);
      //AddWeapon(WeaponType.chainLightning, 1100);
      //AddWeapon(WeaponType.molotov, 1100);
      //AddWeapon(WeaponType.obrez, 1100);
      //AddWeapon(WeaponType.radiator, 1100);
      //AddWeapon(WeaponType.tomatGun, 1100);
      //AddWeapon(WeaponType.tomato, 1100);
      //AddWeapon(WeaponType.bear, 1100);
      //AddWeapon(WeaponType.iscander, 1100);
      //AddWeapon(WeaponType.eagle, 1100);
      //AddWeapon(WeaponType.varganich, 1100);

      //  AddWeapon(WeaponType.obrez, 200);
      //  AddWeapon(WeaponType.tomato, 200);
      //  selectCompany.Add(WeaponType.tomato);
      //selectCompany.Add(WeaponType.obrez);
    }

    /// <summary>
    /// Загрузка данных
    /// </summary>
    /// <param name="saveWeapons"></param>
    public void Load() {
      weaponsManagers.ForEach(x => x.Load());

      if (!PlayerPrefs.HasKey("weapon"))
        return;

      SaveWeapons saveWeapons =
        Newtonsoft.Json.JsonConvert.DeserializeObject<SaveWeapons>(PlayerPrefs.GetString("weapon"));

      //weaponList = saveWeapons.list;

      selectCompany = saveWeapons.selectCompany;
      //selectCompany.RemoveAll(x => !weaponList.Exists(y => y.type == x));
      selectSurvival = saveWeapons.selectSurvival;
    }

    /// <summary>
    /// Сохранение данных
    /// </summary>
    /// <returns></returns>
    public void Save() {
      SaveWeapons sd = new SaveWeapons() {
        //list = weaponList,
        selectCompany = selectCompany,
        selectSurvival = selectSurvival
      };

      PlayerPrefs.SetString("weapon", Newtonsoft.Json.JsonConvert.SerializeObject(sd));

    }

    /// <summary>
    /// Добавить снаряды
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    public void AddBullet(WeaponType type, int count = 0) {
      if (type == WeaponType.none)
        return;

      WeaponManager wm = weaponsManagers.Find(x => x.weaponType == type);
      wm.BulletCount += count;

    }

    /// <summary>
    /// Получить выбранные оружия
    /// </summary>
    /// <param name="isSurvival"></param>
    /// <returns></returns>
    public List<WeaponType> GetSelected(bool isSurvival = false) {
      return isSurvival ? selectSurvival : selectCompany;
    }

    public List<WeaponManager> WeaponList {
      get { return weaponsManagers.FindAll(x => x.IsGet); }
    }

    /// <summary>
    /// Установка выбранного
    /// </summary>
    /// <param name="data"></param>
    /// <param name="isSurvival"></param>
    public void SetSelected(List<WeaponType> data, bool isSurvival = false) {

      if (ZbCatScene.CatSceneManager.Instance.isSpecLevel)
        return;

      if (isSurvival)
        selectSurvival = data;
      else
        selectCompany = data;
    }

    /// <summary>
    /// Добавляем оружие в массив
    /// </summary>
    /// <param name="newType"></param>
    /// <param name="count"></param>
    public void AddWeapon(WeaponType newType, int count = 0) {

      if (!AddOpenCondition(newType))
        return;

      if (ZbCatScene.CatSceneManager.Instance.isSpecLevel)
        return;

      AddOpenCondition(newType);

      GetWeapon(newType).BulletCount = count;

    }

    public bool AddOpenCondition(WeaponType type) {
      WeaponManager wm = weaponsManagers.Find(x => x.weaponType == type);

      if (wm == null)
        return false;

      if (wm.IsGet)
        return true;

      wm.IsGet = true;
      return true;
    }

    /// <summary>
    /// Изменение количество снарядов
    /// </summary>
    /// <param name="manager"></param>
    public void OnWeaponChange(WeaponManager manager) {

      //var elem = GetWeapon(manager.weaponType);

      //if (elem == null)
      //  return;

      //elem.bulletCount = manager.BulletCount;
      if (OnChangeBulletCount != null)
        OnChangeBulletCount(manager.weaponType, manager.BulletCount);
    }

    public bool ExistWeaponType(WeaponType weaponType) {
      return weaponsManagers.Find(x => x.weaponType == weaponType).IsGet;
    }

    public bool ExistsBullet() {
      return weaponsManagers.Find(x => x.BulletCount > 0 && x.category != WeaponCategory.asisstant);
    }

    public WeaponManager GetWeapon(WeaponType type) {
      return weaponsManagers.Find(x => x.weaponType == type);
    }

    #region Потеря оружия

    // Потеря оружия
    public void LostWeapons() {
      /// TODO ИСПРАВИТЬ!!!!!!!!!!

      //int tomato = 0;

      //for (int i = 0; i < weaponList.Count; i++)
      //{
      //  if (weaponList[0].type != WeaponType.tomato)
      //  {
      //    WeaponUserData lostWeapon = new WeaponUserData();
      //    lostWeapon.type = weaponList[i].type;
      //    lostWeapon.bulletCount = weaponList[i].bulletCount;
      //    UserManager.Instance.userProgress.lostWeaponList.Add(lostWeapon);
      //    lostWeapon.bulletCount = 0;
      //  } else
      //  {
      //    tomato = weaponList[i].bulletCount;
      //  }
      //}

      //weaponList = new List<WeaponUserData>();
      //weaponList[0].type = WeaponType.tomato;
      //weaponList[0].bulletCount = tomato;

      //UserManager.Instance.userProgress.returnLostPercent = 1;

    }

    public void ReturnListWeaponType(WeaponType wt) {
      AddBullet(wt, 0);
    }

    public void ReturnLostWeapons(float returnValue = 0.33f) {

      /// TODO ИСПРАВИТЬ!!!!!!!!!!

      //float koeff = returnValue / UserManager.Instance.userProgress.returnLostPercent;

      //UserManager.Instance.userProgress.returnLostPercent -= returnValue;

      //List<WeaponUserData> newListLost = new List<WeaponUserData>();

      //for (int i = 0; i < UserManager.Instance.userProgress.lostWeaponList.Count; i++)
      //{

      //  WeaponManager wm = weaponsManagers.Find(x => x.weaponType == weaponList[i].type);
      //  if (wm.category == WeaponCategory.asisstant)
      //    continue;

      //  int returnCount = (int)(UserManager.Instance.userProgress.lostWeaponList[i].bulletCount * koeff);

      //  WeaponUserData lostUser = new WeaponUserData();
      //  lostUser.type = weaponList[i].type;
      //  lostUser.bulletCount = weaponList[i].bulletCount - returnCount;

      //  AddBullet(UserManager.Instance.userProgress.lostWeaponList[i].type, returnCount);
      //  newListLost.Add(lostUser);

      //}

      //UserManager.Instance.userProgress.lostWeaponList = newListLost;
    }

    #endregion

#if UNITY_EDITOR
    [ContextMenu("Find All weapons")]
    public void FindAllWeapons() {
      weaponsManagers = EditorUtils.GetAllPrefabsOfType<WeaponManager>();
    }

#endif

  }


  ///// <summary>
  ///// Данные по оружию игроеа
  ///// </summary>
  [System.Serializable]
  public class WeaponUserData {
    public WeaponType type;
    public int bulletCount;
  }

  [System.Serializable]
  public class SaveWeapons {
    //public List<WeaponUserData> list;
    public List<WeaponType> selectCompany;
    public List<WeaponType> selectSurvival;
  }
}