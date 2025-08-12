using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Продукт магазина
/// </summary>
[System.Serializable]
public struct ShopProduct {
	public string name;
	public Game.Weapon.WeaponType type;
	public Sprite sprite;
	public int bullet;
	public int price;
}

/// <summary>
/// Работа с гемдизайном игры
/// </summary>
public class GameDesign : Singleton<GameDesign> {

  [HideInInspector]
  public Configuration.Config allConfig;             // Конфиг уровней

  protected override void Awake() {
		base.Awake();
		XMLParcer.ParsingComplited += GetConfigJson;
	}

  private void Start(){}
  
	/// <summary>
	/// Получение данных
	/// </summary>
	public void GetConfigJson(Configuration.Config data) {
		allConfig = data;
		FillProducts();
    //GetConfig();
    //ParceEnemyWawe();
    ExEvent.GameEvents.GameDesignLoad.CallAsync();
	}

	#region Магазин
	public ShopLibrary shopLibrary;
	public List<ShopProduct> shopProducts;

	void FillProducts() {
		shopProducts.Clear();

		foreach (Configuration.Shop shopProduct in allConfig.shop) {
			ShopProduct product = new ShopProduct();
			Game.Weapon.WeaponManager weapon = Game.User.UserWeapon.Instance.weaponsManagers.Find(x => (int)x.weaponType == shopProduct.id);
			product.name = weapon.name;
			product.type = weapon.weaponType;
			product.price = shopProduct.price;
			product.bullet = shopProduct.count;
			shopProducts.Add(product);
		}
	}

	#endregion

	public List<int> statsPrice;

	public List<EnemyInfo> enemyInfo;

	public void CreateProductsPrefab() {
		foreach (var shopProductsItem in shopProducts) {
			GameObject inst = new GameObject();
			WeaponShopProduct sp = inst.AddComponent<WeaponShopProduct>();
			inst.name = shopProductsItem.name+ shopProductsItem.bullet;
			sp.title = shopProductsItem.name;
			sp.weaponType = shopProductsItem.type;
			sp.icon = shopProductsItem.sprite;
			sp.bulletCount = shopProductsItem.bullet;
			sp.price = shopProductsItem.price;
		}
	}
  
}
