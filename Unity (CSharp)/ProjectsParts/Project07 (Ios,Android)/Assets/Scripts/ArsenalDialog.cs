using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArsenalDialog: UiDialog {
  public ScrollRect scrollRect;
  public ArsenalItem arsenalElementPrefab;
  public ArsenalGroup arsenalGroupPrefab;
  public RectTransform scrollContent;

  private List<ArsenalItem> elementsList = new List<ArsenalItem>();
  private List<ArsenalGroup> groupList = new List<ArsenalGroup>();

  private List<WeaponBehaviour> equippedList = new List<WeaponBehaviour>();

  protected override void OnEnable() {
    arsenalElementPrefab.gameObject.SetActive(false);
    base.OnEnable();
    SpawnElements();
  }

  public void CloseButton() {
    UiController.Instance.ClickSound();
    Hide();
  }

  public override void ShowComplete() {
    base.ShowComplete();
    scrollRect.verticalNormalizedPosition = 1;

  }

  private void SpawnElements() {

    elementsList.ForEach(x => Destroy(x.gameObject));
    elementsList.Clear();
    groupList.ForEach(x => Destroy(x.gameObject));
    groupList.Clear();
    equippedList.Clear();

    arsenalElementPrefab.gameObject.SetActive(false);
    arsenalGroupPrefab.gameObject.SetActive(false);

    float positionY = -20;

    WeaponManager.Instance.groupList.OrderBy(x => x.order).ToList().ForEach(group => {

      List<WeaponBehaviour> wepList = WeaponManager.Instance.GetByedWeaponsByGroup(group.type);

      if (wepList.Count <= 0) return;

      CreateGroupLabel(group, ref positionY);

      wepList.ForEach(element => CreateElement(element, ref positionY));

    });

    equippedList = WeaponManager.Instance.GetWeaponsEquipped().OrderBy(x => x.SortIndex).ToList();

    scrollContent.sizeDelta = new Vector2(scrollContent.sizeDelta.x, -positionY + 40);

    CreateEquippedList();
  }

  private void CreateElement(WeaponBehaviour weapon, ref float positionY) {
    GameObject inst = Instantiate(arsenalElementPrefab.gameObject);
    inst.transform.SetParent(scrollContent);
    inst.transform.localScale = arsenalElementPrefab.transform.localScale;
    inst.GetComponent<RectTransform>().anchoredPosition = new Vector2(arsenalElementPrefab.GetComponent<RectTransform>().anchoredPosition.x, positionY);
    inst.GetComponent<RectTransform>().sizeDelta = arsenalElementPrefab.GetComponent<RectTransform>().sizeDelta;
    inst.gameObject.SetActive(true);
    ArsenalItem jor = inst.GetComponent<ArsenalItem>();
    jor.SetData(weapon);

    //if (weapon.weaponData.isEquipped)
    //  equippedList.Add(weapon);

    elementsList.Add(jor);
    positionY -= 340;
  }

  private void CreateEquippedList() {

    for(int i = 0; i < arsenalWeapon.Count; i++) {

      if (equippedList.Count > i)
        arsenalWeapon[i].SetWeapon(equippedList[i]);
      else
        arsenalWeapon[i].SetWeapon(null);
    }

  }

  private void CreateGroupLabel(WeaponGroup group, ref float positionY) {

    BulletsProduct prod = (BulletsProduct)Shop.Instance.productList.Find(x => x.GetType().ToString() == typeof(BulletsProduct).ToString() && (x as BulletsProduct).groupType == group.type);

    positionY -= 30;
    GameObject inst = Instantiate(arsenalGroupPrefab.gameObject);
    inst.transform.SetParent(scrollContent);
    inst.transform.localScale = arsenalGroupPrefab.transform.localScale;
    inst.GetComponent<RectTransform>().anchoredPosition = new Vector2(arsenalGroupPrefab.GetComponent<RectTransform>().anchoredPosition.x, positionY);
    inst.GetComponent<RectTransform>().sizeDelta = arsenalGroupPrefab.GetComponent<RectTransform>().sizeDelta;
    inst.gameObject.SetActive(true);

    if (group.wishBullets)
      positionY -= 183;
    else
      positionY -= 80;

    ArsenalGroup gr = inst.GetComponent<ArsenalGroup>();
    gr.SetData(group, prod);
    groupList.Add(gr);
  }

  public void ClickItemWeapon(ArsenalItem item) {
    Debug.Log(item.weapon.groupType);

    if (equippedList.Count >= 5 || equippedList.Contains(item.weapon)) return;

    item.weapon.weaponData.index = equippedList.Count + 1;
    item.weapon.weaponData.IsEquipped = true;
    equippedList.Add(item.weapon);
    elementsList.ForEach(wep => wep.UpdateStatus());
    CreateEquippedList();
  }

  public List<ArsenalWeapon> arsenalWeapon;

  public void ClickWeaponList(WeaponBehaviour weapon) {

    weapon.weaponData.IsEquipped = false;
    equippedList.Remove(weapon);
    elementsList.ForEach(wep => wep.UpdateStatus());
    CreateEquippedList();
  }

}
