using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalElement : MonoBehaviour {

  public System.Action OnClickGet;

  public JournalElementReward rewardObject;

  public I2.Loc.Localize title;
  public I2.Loc.Localize description;
  public Text countText;

  public RectTransform progressRect;

  public GameObject progressBlock;
  public GameObject buttonCompleteBlock;

  private List<GameObject> rewardList = new List<GameObject>();

  float pixelW = 0;
  float pixelH = 0;

  public void SetData(QuestionManager.Question question) {
    title.Term = question.title;
    description.Term = question.description;
    pixelW = progressRect.rect.width;
    pixelH = progressRect.rect.height;
    progressRect.anchorMin = new Vector2(0, 0.5f);
    progressRect.anchorMax = new Vector2(0, 0.5f);
    progressRect.sizeDelta = new Vector2(pixelW * (question.value/ question.count), pixelH);
    countText.text = string.Format("{0}/{1}", question.value, question.count);
    progressBlock.SetActive(!question.isComplete);
    buttonCompleteBlock.SetActive(question.isComplete);

    rewardList.ForEach(x => Destroy(x));
    rewardList.Clear();

    question.rewardList.ForEach(rew => {

      GameObject rewRecord = Instantiate(rewardObject.gameObject, rewardObject.transform.parent);

      var rewData = Config.Instance.playerResource.Find(x => x.type == rew.type);
      rewRecord.GetComponent<JournalElementReward>().SetData(rewData.iconBig, rew.count);
      rewRecord.SetActive(true);
      rewardList.Add(rewRecord);
    });

  }

  public void ButtonGet() {
    if (OnClickGet != null)
      OnClickGet();
  }

}
