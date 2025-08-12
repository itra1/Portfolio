using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RewardDialogElement : MonoBehaviour, IPointerDownHandler {

  public Animator animator;
  public Image image;
  public Text text;
  public RewardQuestDialog rewarDialog;

  private void OnEnable() {
    animator.SetBool("show", true);
  }

  public void SetData(QuestionManager.Reward reward) {

    ResourceIncrementatorBehaviour res = Config.Instance.playerResource.Find(x => x.type == reward.type);

    image.sprite = res.iconBig;
    image.GetComponent<AspectRatioFitter>().aspectRatio = image.sprite.rect.width / image.sprite.rect.height;

    res.Increment(reward.count);

    text.text = reward.count.ToString();

  }

  public void HideComplete() {
    gameObject.SetActive(false);
    rewarDialog.ElementComplete();
  }

  public void OnPointerDown(PointerEventData eventData) {
    animator.SetBool("show", false);
  }
}
