using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.User;

public class BattleResultDialog : UiDialog {

  [SerializeField]
  protected GameObject m_GetButton;
  [SerializeField]
  private GameObject m_dropPrefab;
  [SerializeField]
  private RectTransform m_dropParent;
  private List<GameObject> m_DropData = new List<GameObject>();
  private List<GameObject> m_ReadyData = new List<GameObject>();
  private Drop[] m_ActiveDrop = new Drop[0];

  [SerializeField]
  private Text m_titleLevel;

  private bool m_isClosing;

  public GameObject socialIcons;
  public GameObject socialOkButton;
  public GameObject socialVkButton;
  public GameObject socialFbButton;
  public GameObject socialGPlusButton;
  public GameObject socialTwitButton;
  public GameObject socialMailButton;

  public System.Action OnRestart;
  public System.Action OnExit;


  protected virtual void OnEnable() {
    m_isClosing = false;

    ActiveSocialButton();
    
    Configuration.Level lid 
      = GameDesign.Instance.allConfig.levels.Find(x => x.chapter == UserManager.Instance.ActiveBattleInfo.Group && x.level == UserManager.Instance.ActiveBattleInfo.Level);

    m_titleLevel.text = lid.title;


    if (UserManager.Instance.ActiveBattleInfo.Mode != PointMode.survival)
      InvokeCustom(1, ShowCatScene);
  }

  protected virtual void ShowCatScene() {  }

  public void StartAnimDrop() {
    m_DropData.Clear();
    while (m_dropParent.childCount > 0)
      DestroyImmediate(m_dropParent.GetChild(0).gameObject);
    
    float summaryLength = 0;
    int line = 0;

    for (int i = 0; i < m_ActiveDrop.Length; i++) {
      if (m_ActiveDrop[i].count == 0)
        continue;

      GameObject inst = Instantiate(m_dropPrefab, m_dropParent);
      inst.SetActive(false);
      m_DropData.Add(inst);
      m_ReadyData.Add(inst);
      inst.transform.localPosition = Vector3.zero;
      inst.transform.localScale = Vector2.zero;
      inst.GetComponent<ResultDropItem>().SetData(m_ActiveDrop[i]);
      inst.GetComponent<ResultDropItem>().OnShow = ShowNextDrop;

      if (summaryLength + inst.GetComponent<ResultDropItem>().block.sizeDelta.x > 350) {
        DropLine(line, summaryLength);
        summaryLength = 0;
        m_DropData.Clear();
        line++;
      }

      summaryLength += inst.GetComponent<ResultDropItem>().block.sizeDelta.x + 15;
    }

    if (m_DropData.Count > 0) {
      DropLine(line, summaryLength);
      summaryLength = 0;
      m_DropData.Clear();
    }

    ShowNextDrop();

  }

  public void SetDrop(Drop[] drop) {
    m_ActiveDrop = (Drop[])drop.Clone();
  }


  void ShowNextDrop() {
    if (m_ReadyData.Count == 0) {
      DropAllView();
      return;
    }
    m_ReadyData[0].SetActive(true);
    m_ReadyData.RemoveAt(0);
  }

  protected virtual void DropAllView() {  }

  void DropLine(int numLine, float summaryLength) {

    float startX = -summaryLength / 2;

    for (int i = 0; i < m_DropData.Count; i++) {
      RectTransform rectTr = m_DropData[i].GetComponent<RectTransform>();
      startX += rectTr.sizeDelta.x / 2;
      rectTr.anchoredPosition = new Vector2(startX, -30 * numLine - 15);
      startX += rectTr.sizeDelta.x / 2 + 15;
    }

  }

  private void ActiveSocialButton() {
#if OPTION_SOCIAL
		socialIcons.SetActive(true);
#else
    socialIcons.SetActive(false);
#endif
  }

  public void RestartButton() {

    if (m_isClosing)
      return;
    m_isClosing = true;

    UIController.ClickPlay();
    OnRestart?.Invoke();
  }

  public void ExitButton() {

    if (m_isClosing)
      return;
    m_isClosing = true;

    DarkTonic.MasterAudio.MasterAudio.StopPlaylist("PlaylistController");
    UIController.ClickPlay();
    OnExit?.Invoke();
  }


}
