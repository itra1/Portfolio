using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestDialog : UiDialog {

  public RewardChest rewardPrefab;
  public AnimationHelper chestAnimationHelper;

  private bool isOpen;
  public Animator chestAnimator;
  public RewardChest rewardChest;

  private System.Action _onShowComplete;

  private List<ChestProduct.Item> rewardList = new List<ChestProduct.Item>();

  private ChestProduct product;

  protected override void OnEnable() {
    base.OnEnable();

    isOpen = false;

    chestAnimationHelper.Event1 = () => {
      NextReward();
    };

    rewardPrefab.OnHide = () => {

      NextReward();
    };

  }

  public void SetData(ChestProduct product, System.Action onShowComplete = null)
  {
    _onShowComplete = onShowComplete;
    this.product = product;
    rewardList = new List<ChestProduct.Item>(product.items);
  }

  public void OpenChest() {

    if (!isOpen) {
      DarkTonic.MasterAudio.MasterAudio.PlaySound("Uther", 1, 1, 0, "OpenBox");
      chestAnimator.SetTrigger("open");
      isOpen = true;
    } else {
      if (rewardPrefab.gameObject.activeInHierarchy)
        rewardPrefab.Click();

    }
  }

  public void NextReward() {

    if(rewardList.Count <= 0) {
      Hide(()=> {
        ExEvent.GameEvents.ChestShowComplete.Call();
        _onShowComplete?.Invoke();

        IAfterGetProduct afp = product.GetComponent<IAfterGetProduct>();
        if (afp != null)
        {
          afp.AfterGetProduct();
        }

      });
      return;
    }


    ChestProduct.Item item = rewardList[0];
    rewardList.RemoveAt(0);

    rewardPrefab.gameObject.SetActive(true);
    rewardPrefab.SetData(item);
  }
  
}
