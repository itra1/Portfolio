using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Spine.Unity;
using Game.User;

public class LoseDialog : BattleResultDialog {

  [SerializeField]
  private AnimatorHelper m_lostHelper;

  public SkeletonGraphic m_skeletonGraphicBear;
  private readonly string m_bearAnimationFall = "fall";
  private readonly string m_bearAnimationIdle = "idle";

  protected override void OnEnable() {
    base.OnEnable();

    m_GetButton.SetActive(UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival);

    m_lostHelper.OnBearDown = () => {
      m_skeletonGraphicBear.startingLoop = false;
      m_skeletonGraphicBear.timeScale = 0;
      m_skeletonGraphicBear.startingAnimation = m_bearAnimationFall;
      m_skeletonGraphicBear.Initialize(true);
      m_skeletonGraphicBear.Rebuild(CanvasUpdate.PreRender);

      Helpers.Invoke(this, InitBearLost, 2);
    };

    if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival) {
      StartAnimDrop();
    }

    m_GetButton.SetActive(UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival);
  }

  void StartIdleLost() {
    m_skeletonGraphicBear.startingLoop = true;
    m_skeletonGraphicBear.startingAnimation = m_bearAnimationIdle;
    m_skeletonGraphicBear.Initialize(true);
    m_skeletonGraphicBear.Rebuild(CanvasUpdate.PreRender);
  }

  protected override void ShowCatScene() {
    base.ShowCatScene();

    if (UserManager.Instance.ActiveBattleInfo.Group == 1
        && UserManager.Instance.ActiveBattleInfo.Level == 3
        && ZbCatScene.CatSceneManager.Instance.ShowCatScene(5, () => {

        }))
      return;

    if (UserManager.Instance.ActiveBattleInfo.Group == 1
        && UserManager.Instance.ActiveBattleInfo.Level == 4
        && ZbCatScene.CatSceneManager.Instance.ShowCatScene(7, () => {

        }))
      return;

  }


  void InitBearLost() {
    m_skeletonGraphicBear.timeScale = 1;
  }

}
