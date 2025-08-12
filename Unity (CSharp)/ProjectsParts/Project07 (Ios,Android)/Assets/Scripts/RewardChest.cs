using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardChest: MonoBehaviour {

  public System.Action OnHide;
  public Animator animComponent;
  public I2.Loc.Localize titleText;
  public Text coinsText;
  public Image icon;

  ChestProduct.Item _data;

  private bool readyClick = false;

  public void SetData(ChestProduct.Item chestData) {
    _data = chestData;

    ResourceIncrementatorBehaviour reward = Config.Instance.playerResource.Find(x => x.type == chestData.rewourceType);

    bool dataExists = true;

    switch (chestData.reward) {
      case ChestProduct.Item.Reward.percent:
        chestData.value = Random.value;

        if (chestData.value > chestData.percent)
          dataExists = false;

        if (dataExists && chestData.rewourceType == Config.ResourceType.weapon) {
          List<WeaponBehaviour> wepList = WeaponManager.Instance.GetWeaponsForReward();

          WeaponBehaviour wep = null;

          if (wepList.Count <= 0)
            dataExists = false;
          else {
            wep = wepList[Random.Range(0, wepList.Count)];
          }

          if (wep != null) {
            reward = new WeaponResourceIncrementator();
            reward.type = Config.ResourceType.weapon;
            reward.title = wep.title;
            reward.iconBig = wep.WeaponIcon;
            
            titleText.Term = wep.title;
            coinsText.text = 1.ToString();

            QuestionManager.Instance.AddValueQuest(QuestionManager.Type.getWeapon, 1);

            wep.weaponData.IsByed = true;
          } else {
            dataExists = false;
          }
        }


        break;
      case ChestProduct.Item.Reward.rand:
        if (chestData.span.max <= 0) {
          dataExists = false;
          break;
        }
        chestData.value = Random.Range(chestData.span.min, chestData.span.max);

        reward = Config.Instance.playerResource.Find(x => x.type == chestData.rewourceType);
        reward.Increment(chestData.value);
        coinsText.text = Utils.RoundValueString(chestData.value);

        break;
    }

    if (reward != null)
      titleText.Term = reward.title;

    if (!dataExists) {
      HideComplete();
      return;
    }

    icon.sprite = reward.iconBig;
    icon.GetComponent<AspectRatioFitter>().aspectRatio = reward.iconBig.rect.width / reward.iconBig.rect.height;
  }

  public void Click() {
    if (!readyClick) return;
    animComponent.SetTrigger("hide");
    readyClick = false;

  }
  
  public void HideComplete() {
    gameObject.SetActive(false);
    if (OnHide != null) OnHide();
  }
  public void ShowComplete() {
    readyClick = true;
  }

}
