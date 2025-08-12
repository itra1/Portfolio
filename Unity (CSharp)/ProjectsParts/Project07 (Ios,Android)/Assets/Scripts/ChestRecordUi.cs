using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestRecordUi : MonoBehaviour {

  public Image icon;
  public Text title;

  public void SetData(ChestProduct.Item item) {

    //this.icon.sprite = image;
    //this.icon.GetComponent<AspectRatioFitter>().aspectRatio = image.rect.width/image.rect.height;

    if(item.reward == ChestProduct.Item.Reward.percent) {
      this.title.text = string.Format("{0}%", item.percent*100);
    } else if(item.reward == ChestProduct.Item.Reward.rand) {
      this.title.text = string.Format("{0}-{1}", item.span.min, item.span.max);
    }

    var reward = Config.Instance.playerResource.Find(x => x.type == item.rewourceType);
    this.icon.sprite = reward.iconBig;
    this.icon.GetComponent<AspectRatioFitter>().aspectRatio = this.icon.sprite.rect.width / this.icon.sprite.rect.height;

  }

}
