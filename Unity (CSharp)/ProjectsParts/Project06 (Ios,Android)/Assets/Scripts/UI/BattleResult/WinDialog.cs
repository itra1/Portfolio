using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.User;

public class WinDialog : BattleResultDialog {

  [SerializeField]
  private Animator m_Animator;
  [SerializeField]
  private AnimatorHelper m_AnimHelper;

  public List<ParticleSystem> particleList;
  public ParticleSystem starParticles;

  protected override void OnEnable() {
    base.OnEnable();

    m_AnimHelper.OnAnim1 = () => {
      //winAnimator.SetTrigger("startBear");
      if (UserManager.Instance.ActiveBattleInfo.Mode != PointMode.survival)
        StartAnimDrop();
    };

    StartCoroutine(ShowSalutWin());

    m_GetButton.SetActive(true);

  }

  protected override void DropAllView() {

    m_Animator.SetTrigger("startBear");

  }
  IEnumerator ShowSalutWin() {

    List<ParticleSystem> showParticle = new List<ParticleSystem>();
    starParticles.gameObject.SetActive(true);
    starParticles.Play();
    while (showParticle.Count < particleList.Count) {
      int num = UnityEngine.Random.Range(0, particleList.Count);
      if (!showParticle.Contains(particleList[num]))
        showParticle.Add(particleList[num]);
    }
    float timeWait = 1.5f;
    int numShow = 0;
    while (true) {
      yield return new WaitForSeconds(timeWait);
      showParticle[numShow].GetComponent<RectTransform>().anchoredPosition = new Vector2(UnityEngine.Random.Range(-220, 220), UnityEngine.Random.Range(180, 280));
      showParticle[numShow].gameObject.SetActive(true);
      showParticle[numShow].Play();
      numShow++;
      if (numShow == showParticle.Count)
        numShow = 0;
      timeWait = UnityEngine.Random.Range(1, 2);
    }

  }

  protected override void ShowCatScene() {
    base.ShowCatScene();

    if (UserManager.Instance.ActiveBattleInfo.Group == 1
        && UserManager.Instance.ActiveBattleInfo.Level == 1
        && ZbCatScene.CatSceneManager.Instance.ShowCatScene(2, () => {

        }))
      return;
    
    if (UserManager.Instance.ActiveBattleInfo.Group == 1
        && UserManager.Instance.ActiveBattleInfo.Level == 4
        && ZbCatScene.CatSceneManager.Instance.ShowCatScene(8, () => {

        }))
      return;
  }

}
