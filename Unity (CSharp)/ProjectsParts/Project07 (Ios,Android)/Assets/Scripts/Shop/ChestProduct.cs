using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestProduct : ProductBase {

  public BoxType type;
  public Color color;

  public List<Item> items;

  public System.Action onShowComplete;

  public override bool Buy() {

    if (!CheckCash()) return false;
    ChangeCash();

    if(items.Count > 1 || (items.Count == 1 && items[0].rewourceType != Config.ResourceType.pumpGun))
      AppMetrica.Instance.ReportEvent("buy_chest");
    QuestionManager.Instance.AddValueQuest(QuestionManager.Type.byeChest, 1);
    OpenChest();
    return true;
  }

  private void OpenChest() {
    ChestDialog panel = UiController.GetUi<ChestDialog>();
    panel.SetData(this, onShowComplete);
    panel.Show();
  }

  public enum BoxType {
    black,
    green,
    blue,
    yellow,
    magenta,
    red,
    white
  }
  
  [System.Serializable]
  public class Item {
    public Reward reward;
    public Config.ResourceType rewourceType;
    public float percent;
    public Span span;

    [HideInInspector]
    public float value;

    public enum Reward {
      rand,
      percent
    }
    [System.Serializable]
    public struct Span {
      public float min;
      public float max;
    }

  }
  
}
