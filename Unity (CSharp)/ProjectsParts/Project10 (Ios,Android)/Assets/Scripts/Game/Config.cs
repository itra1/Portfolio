using UnityEngine;
using System.Collections.Generic;


// Элемента магазина
public enum ShopElementType {
  roulette, weaponBox, coinsPerk, magnetPerk, healthPerk, blackMark, bulletMagic, piratMagic, savesPerk, ressurection, doubleWeapon,
  headPalm, headFeather, headShlem, headTrigle, headRom, headMaskAztec, headScience, headFish, headSprut, headBandane,
  spineFlag, spineShroud, spineWeapon, spineCristal, spineBarrel, spinePalm, spinec3p0, spineNet, spineBackpack, none,
  coins, accessoryBelt, accessoryMap, accessoryTorch, accessoryBottle, accessoryStraps, accessoryShackle, accessorySword,
  accessoryCrutch, accessoryHook, headBag, spineLamp, accessoryInsurance, accessoryLeaves, penBat, penSpider, penDino,
  GoldJack1, GoldJack2, GoldJack3, GoldJack4, GoldJack5, runBoost, skateBoost, barrelBoost, millWhellBoost, shipBoost, Special,
  livePerk, liveAddPerk
}

public struct ClothesBonus {
  public bool head;
  public bool spine;
  public bool accessory;
  public bool full {
    get {
      return (head && spine && accessory);
    }
  }
}
public enum ClothesSets {
  roulette, defendBarrier, moreBox, noEnemy, heart, magnet, money, noAirAttack, noBreack, noHendingBarrier
}
public enum ClothesPosition { none, head, spine, accessory }
[System.Serializable]
public struct ShopElementToType {
  [SerializeField]
  string name;                        // Для удобства в инспекторе
  public shopTypes type;
  public ClothesPosition position;
  public ShopElementType[] elem;
}
[System.Serializable]
public struct ClothesTypezes {
  public ClothesPosition position;
  public ShopElementType elem;
}
[System.Serializable]
public struct ClothesSet {
  public string name;
  public string title;
  public string description;
  public ClothesSets type;
  public ClothesTypezes[] element;
}
[System.Serializable]
public struct ShopPrices {
  public string name;
  public ShopElementType type;
  public string title;
  public string description;
  public Sprite sprite;
  public ShopElementPrice[] levels;
  public bool unLimLevels;
}
/// <summary>
/// Настройки
/// </summary>
public class Config: Singleton<Config> {

  protected override void Awake() {
    ParsingJson();
  }

  void Start() { }

  #region Магазин

  public ShopElementToType[] shopElementToType;                       // Соотношение элемента магазина к типу магаищина
  public ClothesSet[] clothesSet;

  // Получаем количество доступных элементов
  public static int GetFolowsCount(Shop.Products.ProductType productType) {
    int allCoins = UserManager.coins;
    int allCount = 0;

    var productList = Shop.ShopManager.Instance.GetProductList(productType);

    productList.ForEach(elem => {
      allCount += elem.ByePossible() ? 1 : 0;
    });
    return allCount;

  }

  public static ClothesPosition GetlothesPosition(ShopElementType element) {

    foreach (ShopElementToType elementsToType in Instance.shopElementToType) {

      foreach (ShopElementType elementInType in elementsToType.elem) {
        if (elementInType == element)
          return elementsToType.position;
      }
    }
    return ClothesPosition.none;
  }

  /// <summary>
  /// Возвращаем массив продуктов по типу
  /// </summary>
  /// <param name="needType"></param>
  /// <returns></returns>
  public static ShopElementType[] GetProdyctArrayByType(shopTypes needType) {
    ShopElementType[] returnArray = new ShopElementType[0];
    foreach (ShopElementToType elementsToType in Instance.shopElementToType) {
      if (elementsToType.type == needType)
        returnArray = IncArrayShopElementType(returnArray, elementsToType.elem);
    }
    return returnArray;
  }

  static ShopElementType[] IncArrayShopElementType(ShopElementType[] source, ShopElementType[] inc) {
    ShopElementType[] tmp = new ShopElementType[source.Length + inc.Length];
    for (int i = 0; i < source.Length; i++) {
      tmp[i] = source[i];
    }
    for (int i = source.Length; i < source.Length + inc.Length; i++) {
      tmp[i] = inc[i - source.Length];
    }

    return tmp;
  }

  public static ClothesSet GetGroup(out int countMax) {

    ShopElementType spine = GetActiveCloth(ClothesPosition.spine);
    ShopElementType head = GetActiveCloth(ClothesPosition.head);
    ShopElementType accessory = GetActiveCloth(ClothesPosition.accessory);

    countMax = 0;

    ClothesSet thisSet = new ClothesSet();

    foreach (ClothesSet clot in Instance.clothesSet) {
      int count = 0;
      foreach (ClothesTypezes elem in clot.element) {

        if (elem.position == ClothesPosition.head && (elem.elem == head || head == ShopElementType.headSprut))
          count++;
        if (elem.position == ClothesPosition.spine && elem.elem == spine)
          count++;
        if (elem.position == ClothesPosition.accessory && elem.elem == accessory)
          count++;
      }

      countMax = count > countMax ? count : countMax;
      if (count == 3)
        thisSet = clot;
    }

    return thisSet;

  }

  public static ClothesBonus GetActiveCloth(ClothesSets type) {

    ClothesBonus result = new ClothesBonus();

    if (Instance == null) {
      return result;
    } else {
      return Instance.getActiveCloth(type, result);
    }
  }

  public ClothesBonus getActiveCloth(ClothesSets type, ClothesBonus result) {
    foreach (ClothesSet one in clothesSet) {
      if (one.type == type) {

        foreach (ClothesTypezes clot in one.element) {
          if (clot.position == ClothesPosition.head)
            result.head = PlayerPrefs.GetInt(clot.elem.ToString()) == 2;
          if (clot.position == ClothesPosition.spine)
            result.spine = PlayerPrefs.GetInt(clot.elem.ToString()) == 2;
          if (clot.position == ClothesPosition.accessory)
            result.accessory = PlayerPrefs.GetInt(clot.elem.ToString()) == 2;
        }

        if (result.spine && result.accessory && !result.head)
          result.head = PlayerPrefs.GetInt("headSprut") == 2;
      }
    }

    return result;
  }

  #endregion

  #region Кости джека

  public static ShopElementType GetActiveCloth(ClothesPosition clothPosition) {
    if (Instance == null)
      return ShopElementType.none;

    foreach (ShopElementToType shop in Instance.shopElementToType) {
      if (clothPosition == shop.position) {
        foreach (ShopElementType elem in shop.elem) {
          int level = PlayerPrefs.GetInt(elem.ToString());
          if (level == 2)
            return elem;
        }
      }
    }
    return ShopElementType.none;
  }

  public static void SetBoundsCloth(Shop.Products.Product activeCloth) {
    ClothesPosition changeCloth = ClothesPosition.none;

    List<Shop.Products.Product> productList = Shop.ShopManager.Instance.GetProductList(activeCloth.type);

    productList.ForEach(xl => {
      if (xl.GetLevel() >= 2)
        xl.SetLevel(1);
    });
    activeCloth.SetLevel(2);

  }

  public static void UnSetBoundsCloth(Shop.Products.Product activeCloth) {
    activeCloth.SetLevel(1);
  }

  #endregion

  #region Парсинг Json

  public TextAsset gameDesign;
  public Configuration.GameDisign config = new Configuration.GameDisign();

  /// <summary>
  /// Парсинг данных при старте
  /// </summary>
  public void ParsingJson() {
    config = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration.GameDisign>(gameDesign.text);
  }

  #endregion

}