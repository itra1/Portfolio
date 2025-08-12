using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Billing {

  /// <summary>
  /// Спициальная покупка
  /// </summary>
  public class SpecialBillingProduct: BillingProductAbstract {

    [SerializeField]
    private int goldCount;
    [SerializeField]
    public int clothCount;
    [SerializeField]
    private int gadgetCount;

    public override void Bye(bool isRestore = false) {
      base.Bye(isRestore);

      CalcProducts();

    }

    [HideInInspector]
    public int goldAdded;

    [HideInInspector]
    public List<ShopElementType> clothestAdded = new List<ShopElementType>();
    [HideInInspector]
    public Dictionary<ShopElementType, int> gadgetAdded = new Dictionary<ShopElementType, int>();

    private void CalcProducts() {

      goldAdded = goldCount;

      CalcClothes();
      CalcGadget();
    }

    private void CalcClothes() {

      ShopElementType[] productClothes = Config.GetProdyctArrayByType(shopTypes.clothes);

      List<ShopElementType> clothesList = new List<ShopElementType>();

      foreach (ShopElementType one in productClothes) {
        if (PlayerPrefs.GetInt(one.ToString(), 0) == 0)
          clothesList.Add(one);
      }

      int addCloth = clothCount;

      // Если доступных шпотом не хватает, остаток выражаем в золоте
      if (clothesList.Count < addCloth) {
        goldAdded += (addCloth - clothesList.Count) * 10000;
        addCloth = clothesList.Count;
      }

      if (addCloth > 0) {
        for (int i = 0; i < addCloth; i++) {
          ShopElementType need = clothesList[UnityEngine.Random.Range(0, clothesList.Count)];
          clothestAdded.Add(need);
        }
        clothesList.Clear();
      }

      // Сохраняем полученный список
      foreach (ShopElementType one in clothestAdded) {
        PlayerPrefs.SetInt(one.ToString(), 1);
      }

    }

    private void CalcGadget() {

      gadgetAdded.Clear();

      int addGadget = gadgetCount;

      ShopElementType[] productPowers = Config.GetProdyctArrayByType(shopTypes.powers);

      for (int i = 0; i < addGadget; i++) {
        ShopElementType need = productPowers[UnityEngine.Random.Range(0, productPowers.Length)];
        if (gadgetAdded.Count < 3) {
          while (gadgetAdded.ContainsKey(need))
            need = productPowers[UnityEngine.Random.Range(0, productPowers.Length)];
        } else {
          while (!gadgetAdded.ContainsKey(need))
            need = productPowers[UnityEngine.Random.Range(0, productPowers.Length)];
        }

        if (gadgetAdded.ContainsKey(need))
          gadgetAdded[need] = gadgetAdded[need] + 1;
        else
          gadgetAdded.Add(need, 1);

      }

      foreach (ShopElementType one in gadgetAdded.Keys) {
        int ctn = PlayerPrefs.GetInt(one.ToString(), 0);
        PlayerPrefs.SetInt(one.ToString(), ctn + 1);
      }

    }

  }

}