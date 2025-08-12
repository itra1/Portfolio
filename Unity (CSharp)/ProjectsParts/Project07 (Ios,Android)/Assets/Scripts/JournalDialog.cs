using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalDialog : UiDialog {
	public ScrollRect scrollRect;

  public JournalElement journalTemplate;

  private List<JournalElement> labelsList = new List<JournalElement>();

	public void CloseButton() {
		Hide();

	}

	public override void ShowComplete() {
		base.ShowComplete();
		scrollRect.verticalNormalizedPosition = 1;

	}

  protected override void OnEnable() {
    base.OnEnable();

    InitList();

  }

  private void InitList() {
    
    labelsList.ForEach(x => x.gameObject.SetActive(false));

    float posit = -150;
    QuestionManager.Instance.readyQuestion.ForEach(quest => {

      JournalElement jor = GetElement();

      jor.GetComponent<RectTransform>().anchoredPosition = new Vector2(journalTemplate.GetComponent<RectTransform>().anchoredPosition.x, posit);
      jor.GetComponent<RectTransform>().sizeDelta = journalTemplate.GetComponent<RectTransform>().sizeDelta;
      jor.gameObject.SetActive(true);
      jor.SetData(quest);
      posit -= 300;

      jor.OnClickGet = () => {
        if (quest.value >= quest.count) {
          quest.isGet = true;
          QuestionManager.Instance.AddGetReward(quest);
          QuestionManager.Instance.Save(quest);
          QuestionManager.Instance.InitList();
          InitList();
        }
      };

    });
  }

  public JournalElement GetElement() {

    JournalElement label = labelsList.Find(x => !x.gameObject.activeInHierarchy);

    if(label == null) {

      GameObject inst = Instantiate(journalTemplate.gameObject, journalTemplate.transform.parent);
      label = inst.GetComponent<JournalElement>();
      labelsList.Add(label);
    }
    return label;
  }

}
