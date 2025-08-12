using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalElementReward : MonoBehaviour {

  public Image icon;
  public Text text;

  public void SetData(Sprite sprite, float count) {
    this.icon.sprite = sprite;
    this.icon.GetComponent<AspectRatioFitter>().aspectRatio = sprite.rect.width / sprite.rect.height;
    this.text.text = Utils.RoundValueString(count);
  }

}
