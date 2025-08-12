using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardQuestDialog : UiDialog {

  public RewardDialogElement element;

  private List<QuestionManager.Reward> rewardList;

  private bool isVisibleReward = false;

  public void SetData(QuestionManager.Question quest) {

    rewardList = new List<QuestionManager.Reward>(quest.rewardList);
    
  }

  public override void ShowComplete() {
    base.ShowComplete();

    ShowElement();

  }

  private void ShowElement() {

    if (isVisibleReward) return;
    if (rewardList.Count <= 0) {

      Hide(() => {

        gameObject.SetActive(false);
        QuestionManager.Instance.RewardComplete();

      });


      return;
    }

    QuestionManager.Reward rew = rewardList[0];
    rewardList.RemoveAt(0);
    element.SetData(rew);
    element.gameObject.SetActive(true);
  }

  public void ElementComplete() {

    isVisibleReward = false;

    ShowElement();

  }
}
