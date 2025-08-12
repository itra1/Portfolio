using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct BulletTypeParam {
	public BulletType type;
	public string prefabName;
}

/// <summary>
/// Генератор оружия
/// </summary>
public class WeaponSpawner : MonoBehaviour {

	public static WeaponSpawner instance;             // Ссылка на собственный экземпляр
	List<GameObject> weaponListPrefabs;               // Список префабов
	/// <summary>
	/// Список объектов первого плеера
	/// </summary>
	protected Dictionary<WeaponType,WeaponManager> weaponManagerListFirstPlayer = new Dictionary<WeaponType, WeaponManager>();
	/// <summary>
	/// Список объектов первого плеера
	/// </summary>
	protected Dictionary<WeaponType,WeaponManager> weaponManagerListSecondPlayer = new Dictionary<WeaponType, WeaponManager>();

	public List<BulletTypeParam> bulletList;

	void Awake() {
		instance = this;
	}

	void Start() {
		CreateArray();
	}

	/// <summary>
	/// Набиваем массив
	/// </summary>
	void CreateArray() {

		weaponListPrefabs = GameDesign.instance.weaponManagers;

		GameObject newGameObject = new GameObject();
		newGameObject.transform.parent = transform;
		newGameObject.name = "FirstPlayerWeapon";

		for(int i = 0; i < weaponListPrefabs.Count; i++) {
			GameObject weapon = Instantiate(weaponListPrefabs[i]);
			weapon.transform.parent = newGameObject.transform;
			weaponManagerListFirstPlayer.Add(weaponListPrefabs[i].GetComponent<WeaponManager>().weaponType, weapon.GetComponent<WeaponManager>());
		}

		GameObject newGameObject2 = new GameObject();
		newGameObject2.transform.parent = transform;
		newGameObject2.name = "SecondPlayerWeapon";

		for(int i = 0; i < weaponListPrefabs.Count; i++) {
			GameObject weapon = Instantiate(weaponListPrefabs[i]);
			weapon.transform.parent = newGameObject2.transform;
			weaponManagerListSecondPlayer.Add(weaponListPrefabs[i].GetComponent<WeaponManager>().weaponType, weapon.GetComponent<WeaponManager>());
		}
	}

	/// <summary>
	/// Установка оружия для игрока
	/// </summary>
	/// <param name="playerType"></param>
	/// <param name="newWeaponType"></param>
	/// <returns></returns>
	public WeaponManager GetPlayerWeapon(bool isFirst, WeaponType newWeaponType) {
		return (isFirst ? weaponManagerListFirstPlayer[newWeaponType] : weaponManagerListSecondPlayer[newWeaponType]);
	}

	/// <summary>
	/// Генерация снаряда
	/// </summary>
	/// <param name="weaponType"></param>
	/// <returns></returns>
	public GameObject GenerateBullet(BulletType weaponType) {
		return PoolerManager.GetPooledObject(bulletList.Find(x => x.type == weaponType).prefabName);
	}

}
