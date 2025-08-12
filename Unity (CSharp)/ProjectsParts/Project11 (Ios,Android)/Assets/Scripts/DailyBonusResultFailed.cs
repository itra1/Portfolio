using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyBonusResultFailed: UiPanel {

  public Action OnBack;
  public Action OnOk;

  public Animation animComp;
  public GameObject lockedVideo;

  public string iosVideoId;
  public string androidVideoId;
  public Transform octopus;

  protected override void OnEnable() {
    base.OnEnable();
    lockedVideo.gameObject.SetActive(false);

    //OctopusController.Instance.PlayHide(() => {
    //  OctopusController.Instance.graphic.transform.position = octopus.position;
    //  OctopusController.Instance.ToFrontOrder();
    //  OctopusController.Instance.PlayShow(() => {
    //    OctopusController.Instance.Sad();
    //  });
    //});

  }

  private string getVideoId {
    get {
#if UNITY_IOS
      return iosVideoId;
#elif UNITY_ANDROID
      return androidVideoId;
#else
      return null;
#endif
    }
  }

  public void BackButton() {
    if (OnBack != null) OnBack();
  }

  public void VideoButton() {

    lockedVideo.gameObject.SetActive(true);

    GoogleAdsMobile.Instance.ShowRewardVideo(
      (type, amount) => {

        lockedVideo.gameObject.SetActive(false);

        //TODO Обрабокта просмотра видео
        if (OnOk != null) OnOk();

      },
      (err) => {
        Debug.Log(err);
        lockedVideo.gameObject.SetActive(false);
      }
    );


    //UnityAdsVideo.Instance.ShowVideo(new UnityAdsVideo.UnityAdsVideoData(),  getVideoId, (result, tocken) => {

    //    lockedVideo.gameObject.SetActive(false);

    //    if (result == UnityEngine.Advertisements.ShowResult.Finished) {
    //      //TODO Обрабокта просмотра видео
    //    } else {
    //      // Обработка отказа или ошибка выполнения видео
    //    }

    //  });
  }

  public override void HideComplited() {
    base.HideComplited();
  }

  public override void ShowComplited() {
    base.ShowComplited();
  }

  public void Close() {
    animComp.Play("hide");
  }

  public override void ManagerClose() {
    Close();
  }
}
