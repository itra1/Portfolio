using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUi : UiPanel
{

  public Transform hardButton;
  public Transform coinsButton;
  public Transform energyButton;

  public System.Action onSetting;
	public System.Action onShop;
	public System.Action onArsenal;
	public System.Action onJournal;
	public System.Action onBattle;
	public System.Action onMap;

  protected override void OnDisable()
  {
    base.OnDisable();
    UserManager.Instance.ConfirmAllActions();
  }


  public void SettingButton() {
		if (onSetting != null) onSetting();
	}

	public void ShopButtun() {
		if (onShop != null) onShop();
	}

	public void ArsenalButtun() {
		if (onArsenal != null) onArsenal();
	}

	public void JournalButtun() {
		if (onJournal != null) onJournal();
	}

	public void BattleButtun() {
		if (onBattle != null) onBattle();
	}

	public void MapButtun() {
		if (onMap != null) onMap();
	}

  public enum BonusType
  {
    coins,
    energy
  }

  [SerializeField]
  private MoveItem prefabMveItem;

  private List<MoveItem> moveItemsList = new List<MoveItem>();

  public void AddNewItem(Image image, BonusType type)
  {
    Transform targetTransform = transform;

    switch (type)
    {
      case BonusType.coins:
        targetTransform = coinsButton;
        break;
      case BonusType.energy:
        targetTransform = energyButton;
        break;
    }

    MoveItem mi = GetMoveItemInstance();
    mi.gameObject.SetActive(true);
    mi.SetMove(image, targetTransform);

  }

  private MoveItem GetMoveItemInstance()
  {
    MoveItem mi = moveItemsList.Find(x => x.gameObject.activeInHierarchy);

    if (mi == null)
    {
      GameObject inst = Instantiate(prefabMveItem.gameObject, prefabMveItem.transform.parent);
      mi = inst.GetComponent<MoveItem>();
      moveItemsList.Add(mi);
    }

    return mi;
  }

}
