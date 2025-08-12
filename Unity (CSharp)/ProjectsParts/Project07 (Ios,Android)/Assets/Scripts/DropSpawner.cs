using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpawner : Singleton<DropSpawner> {

  public GameObject medicineChestPrefab;
  public GameObject softCashPrefab;
  public GameObject hardCashPrefab;
  public GameObject patronPrefab;
  public GameObject hitEnemy;
  public GameObject atakaLabelPrefab;

  public GameObject bulletPistolPrefab;
  public GameObject bulletUziPrefab;
  public GameObject bulletObrezPrefab;
  public GameObject bulletAutomatPrefab;

  private Dictionary<string, List<GameObject>> instanceList = new Dictionary<string, List<GameObject>>();

  private List<Location.DropStruct> probabilityObject = new List<Location.DropStruct>();
  private List<float> probabilityValue = new List<float>();

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattleStart))]
  private void BattleStart(ExEvent.BattleEvents.BattleStart eventData) {

    List<Location.DropStruct> drop = UserManager.Instance.ActiveLocation.dropList;
    CalcProbability();
  }

  void CalcProbability() {

    probabilityObject.Clear();
    probabilityValue.Clear();

    float sum = 0;

    probabilityValue.Add(0);

    UserManager.Instance.ActiveLocation.dropList.ForEach(drop => {

      switch (drop.type) {
        case DropType.coins:
          drop.prefab = softCashPrefab;
          break;
        case DropType.energy:
          return;
        case DropType.hardCashe:
          drop.prefab = hardCashPrefab;
          return;
        case DropType.medical:
          drop.prefab = medicineChestPrefab;
          break;
        case DropType.bulletAutomat:
          drop.prefab = bulletAutomatPrefab;
          break;
        case DropType.bulletObrez:
          drop.prefab = bulletObrezPrefab;
          break;
        case DropType.bulletPistol:
          drop.prefab = bulletPistolPrefab;
          break;
        case DropType.bulletUzi:
          drop.prefab = bulletUziPrefab;
          break;
      }

      sum += drop.probability;
      probabilityValue.Add(sum);
      probabilityObject.Add(drop);

    });

    probabilityValue = probabilityValue.ConvertAll(x => x / sum);

  }

  private bool isReadyMed = true;

  public void GenerateBonus(Enemy enemy) {

    int Index = BinarySearch.RandomNumberGenerator(probabilityValue);

    if (isReadyMed && Tutorial.TutorialManager.Instance.IsActive && UserManager.Instance.damageCount >= 2) {
      isReadyMed = false;
      Index = probabilityObject.FindIndex(x => x.prefab == medicineChestPrefab);
    }

    Location.DropStruct ds = probabilityObject[Index];

    if (ds.type == DropType.none) return;

    GameObject pref = Instantiate(probabilityObject[Index].prefab, new Vector3(enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z), Quaternion.identity);
    pref.transform.localScale = enemy.transform.localScale;
    pref.GetComponent<BonusBehaviour>().count = ds.GetValue();

  }

  public void GreateHinCount(float count, int sorting, Vector3 position, Vector3 scale) {

    GameObject hitObj = CreateObjectInstance(hitEnemy.gameObject);
    hitObj.transform.position = position;
    hitObj.transform.localScale = scale;
    hitObj.GetComponent<HitCountEnemy>().SetData(count, sorting);
    hitObj.SetActive(true);
  }

  public void GreateAttackLabel(int sorting, Vector3 position, Vector3 scale) {

    GameObject hitObj = CreateObjectInstance(atakaLabelPrefab.gameObject);
    hitObj.transform.position = position;
    hitObj.transform.localScale = scale;
    hitObj.GetComponent<AttackLabel>().SetData(sorting);
  }

  private GameObject CreateObjectInstance(GameObject prefab) {

    GameObject go = null;

    if (instanceList.ContainsKey(prefab.name))
      go = instanceList[prefab.name].Find(x => !x.activeInHierarchy);

    if (go == null) {
      go = Instantiate(prefab);

      if (instanceList.ContainsKey(prefab.name))
        instanceList[prefab.name].Add(go);
      else {
        List<GameObject> objList = new List<GameObject>();
        objList.Add(go);
        instanceList.Add(prefab.name, objList);
      }
    }
    return go;

  }


}
