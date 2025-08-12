using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;

public class CupidonBallons : MonoBehaviour {

  public SkeletonAnimation skeletonAnimation;         // Ссылка на скелетную анимацию
  public System.Action<int> OnBalloneChange;

  [System.Serializable]
  public struct BallonData {
    public CupidonBallonsHelper helper;
    public Transform ik;
    [SpineSlot(dataField: "skeletonAnimation")]
    public string rope;
    [SpineSlot(dataField: "skeletonAnimation")]
    public string ballon;
    public bool isActive;
  }

  public BallonData[] ballonData;

  private void OnEnable() {
    Init();
    Position();
    OnChange();
  }

  void Init() {

    for(int i = 0; i < ballonData.Length; i++) {
      ballonData[i].isActive = true;
      ballonData[i].helper.OnDestroy = OnBalloneDestroy;
      try {
        skeletonAnimation.skeleton.FindSlot(ballonData[i].rope).a = 1;
        skeletonAnimation.skeleton.FindSlot(ballonData[i].ballon).a = 1;
      } catch { }
    }
  }

	public void DestroyAll() {

		for (int i = 0; i < ballonData.Length; i++) {
			if (ballonData[i].isActive) {
				try {
					if (gameObject != null)
						skeletonAnimation.skeleton.FindSlot(ballonData[i].rope).a = 0;
					if (gameObject != null)
						skeletonAnimation.skeleton.FindSlot(ballonData[i].ballon).a = 0;
				}
				catch { }
			}
		}

	}

  void OnBalloneDestroy(int number) {

    for(int i = 0; i < ballonData.Length; i++) {
      if(i == number) {
        ballonData[i].isActive = false;
        try {
          if(gameObject != null) skeletonAnimation.skeleton.FindSlot(ballonData[i].rope).a = 0;
        if(gameObject != null) skeletonAnimation.skeleton.FindSlot(ballonData[i].ballon).a = 0;
        } catch { }
      }
    }

    Position();
    OnChange();
  }

  void OnChange() {

    int activeCount = 0;

    for(int i = 0; i < ballonData.Length; i++)
      if(ballonData[i].isActive) activeCount++;

    if(OnBalloneChange != null) OnBalloneChange(activeCount);
  }

  void Position() {

    int activeCount = 0;

    for(int i = 0; i < ballonData.Length; i++)
      if(ballonData[i].isActive) activeCount++;

    float leftPos = (activeCount - 1)*0.45f/2 * -1;

    int num = 0;
    for(int i = 0; i < ballonData.Length; i++)
      if(ballonData[i].isActive) {
        ballonData[i].ik.localPosition = new Vector3(leftPos + (num * 0.45f), -0.5f, 0);
        num++;
      }

  }

}
