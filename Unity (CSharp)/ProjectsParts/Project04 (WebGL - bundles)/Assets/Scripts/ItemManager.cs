using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager> {

	public GameObject horsePrefab;
	public GameObject horseWhitePrefab;

	public List<ItemLibrary> itemLibrary;

	private void Start() { }

	public HorseBehaviour GetHorse(bool isWhite = false) {
		if(isWhite)
			return Instantiate(horseWhitePrefab).GetComponent<HorseBehaviour>();
		else
			return Instantiate(horsePrefab).GetComponent<HorseBehaviour>();
	}

}

[System.Serializable]
public struct ItemLibrary {
	public ItemType type;
	public string name;
	public int typeBegin;
	public int typeEnd;
	public int group;
}

public enum ItemType {
	none = 0,
	shield = 1,
	weapon = 2,
	throwingWeapon = 3,
	smallArms = 4,
	resources = 5,
	helmet = 6,
	shoulder = 7,
	amulets = 8,
	pants = 9,
	armor = 10,
	horses = 11,
	medicines = 12,
	potions = 13,
	scrolls = 14,
	ammunition = 15,
	tools = 18,
	forage = 19,
	artifacts = 20,
	gifts = 30,
	recipes = 31,
	rings = 32,
	belts = 33,
	runes = 34,
	images = 35,
	another = 37
}