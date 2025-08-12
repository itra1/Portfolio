using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager> {

  [SerializeField]
  private List<WeaponBehaviour> weaponListPrefab;

  private List<WeaponBehaviour> _readyList = new List<WeaponBehaviour>();

  private void Start() { }

	public WeaponBehaviour GetWeapon(PlayerType playerType, WeaponType weaponType, Hand hand) {

	  WeaponBehaviour inst = _readyList.Find(x => x.type == weaponType && x.playerType == playerType && !x.gameObject.activeInHierarchy && (x.hand == Hand.none || x.hand == hand));

	  if (inst == null) {
	    WeaponBehaviour pref = weaponListPrefab.Find(x => x.type == weaponType && x.playerType == playerType && (x.hand == Hand.none || x.hand == hand));

	    if (pref == null) {
	      Debug.LogWarning(String.Format("!!!Не найдено оружие типа {0} в {1} руку для персонажа типа {2}", weaponType.ToString(), hand.ToString(), playerType.ToString()));
	      return null;
      }
	    else {
	      Debug.LogWarning(String.Format("!!!Найдено оружие типа {0} в {1} руку для персонажа типа {2}", weaponType.ToString(), hand.ToString(), playerType.ToString()));
      }

	    inst = Instantiate(pref).GetComponent<WeaponBehaviour>();
	    inst.gameObject.SetActive(true);
	    _readyList.Add(inst);
	  }

	  return inst;
  }

  public static string GetName(PlayerType playerType, WeaponType weaponType, Hand hand) {
    string name = playerType.ToString() + "_" + weaponType.ToString();

    if (hand != Hand.none)
      name += "_" + hand.ToString();

    return name;
  }

}

public enum WeaponType {
	none,
	sword,
	pike,
	shield,
	bow
}