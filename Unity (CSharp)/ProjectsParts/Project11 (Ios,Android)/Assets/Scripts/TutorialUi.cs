using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUi : UiPanel {

  private int pageNum = 0;

  public BezierCurve bezier;

  public List<GameObject> pagesList;
  public List<GameObject> radialButtonList;
  public GameObject rightArrow;
  public GameObject leftArrow;

  public Animation animComp;

  protected override void OnEnable() {
    base.OnEnable();
    SetPage(0);
    StartCoroutine(PointerAnimation());
  }

  public void CloseButton() {
    Hide(()=> {

      gameObject.SetActive(false);
    });
  }

  public override void ManagerClose() {
    CloseButton();
  }

  public void SetPage(int num) {
    pageNum = num;
    ConfirmChangePage();
  }

  public override void Show(System.Action OnShow = null) {
    base.Show(OnShow);
    AudioManager.Instance.library.PlayWindowOpenAudio();
    animComp.Play("show");
  }

  public override void Hide(System.Action OnHide = null) {
    base.Hide(OnHide);
    AudioManager.Instance.library.PlayWindowCloseAudio();
    animComp.Play("hide");
  }

  public void RightArrowButton() {
    pageNum++;
    if (pagesList.Count <= pageNum)
      pageNum = 0;
    ConfirmChangePage();
  }

  public void LeftArrowButton() {
    pageNum--;
    if (pageNum < 0)
      pageNum = pagesList.Count -1;
    ConfirmChangePage();
  }

  private void ConfirmChangePage() {

    for (int i = 0; i < pagesList.Count; i++)
      pagesList[i].SetActive(i == pageNum);

    for (int i = 0; i < radialButtonList.Count; i++)
      radialButtonList[i].SetActive(i == pageNum);

  }

  public RectTransform pointerTransform;
  public Image pointerImage;

  IEnumerator PointerAnimation() {

    while (true) {

      pointerTransform.position = bezier.GetPointAt(0);
      float colorA = 0;
      pointerImage.color = new Color(1, 1, 1, colorA);
      while (colorA < 1) {
        colorA += 1 * Time.deltaTime;
        if (colorA > 1)
          colorA = 1;
        pointerImage.color = new Color(1, 1, 1, colorA);
        yield return null;
      }
      colorA = 1;
      pointerImage.color = new Color(1, 1, 1, colorA);

      float posBez = 0;

      while(posBez < 1) {
        posBez += .5f * Time.deltaTime;
        pointerTransform.position = bezier.GetPointAt(posBez);
        yield return null;
      }
      pointerTransform.position = bezier.GetPointAt(1);

      colorA = 1;
      pointerImage.color = new Color(1, 1, 1, colorA);
      while (colorA > 0) {
        colorA -= 1 * Time.deltaTime;
        if (colorA < 0)
          colorA = 0;
        pointerImage.color = new Color(1, 1, 1, colorA);
        yield return null;
      }

      yield return new WaitForSeconds(0);
    }

  }


}
