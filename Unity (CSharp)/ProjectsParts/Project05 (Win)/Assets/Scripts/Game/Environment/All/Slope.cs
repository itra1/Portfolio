using UnityEngine;
using System.Collections;
using it.Game.Player;
using it.Game.Player.MotionControllers.Motions;
using com.ootii.Actors.AnimationControllers;
using RunningJump = it.Game.Player.MotionControllers.Motions.RunningJump;

namespace it.Game.Environment
{
  public class Slope : Environment
  {
	 [SerializeField]
	 private FluffyUnderware.Curvy.CurvySpline m_Spline;

	 public void PlayerEnter()
	 {
      if (State == 1)
        return;
      State = 1;
      EnableComponent();

    }

	 public void PlayerOut()
	 {

      if (State == 0)
        return;
      State = 0;
      DisableSlop();
    }
    private void EnableComponent()
    {

      Debug.Log("ActivateSlope");

      //m_SliceVfx.transform.SetParent(CameraBehaviour.Instance.transform);
      //m_SliceVfx.transform.localPosition = Vector3.zero;

      PlayerBehaviour.Instance.MotionController.GetMotion<JumpPastel>(true).IsEnabled = false;
      PlayerBehaviour.Instance.MotionController.GetMotion<RunningJump_v3>(true).IsEnabled = false;

      PlayerBehaviour.Instance.MotionController.GetMotion<SlopSlideJump>(true).IsEnabled = true;
      SlopSlide_v2 ss = PlayerBehaviour.Instance.MotionController.GetMotion<SlopSlide_v2>(true);
      ss.IsEnabled = true;

      ss.SetSpline(m_Spline);
      //ss.SetSliceVfx(m_SliceVfx, PlayerBehaviour.Instance.FullBodyBipedIK.references.leftFoot, PlayerBehaviour.Instance.FullBodyBipedIK.references.rightFoot);

    }

    private void DisableSlop()
    {
      if (PlayerBehaviour.Instance == null)
        return;
      Debug.Log("DeactiveSlope");
      SlopSlide_v2 ss = PlayerBehaviour.Instance.MotionController.GetMotion<SlopSlide_v2>(true);

      ss.Slowdown(() => {

        PlayerBehaviour.Instance.MotionController.GetMotion<JumpPastel>(true).IsEnabled = true;
        PlayerBehaviour.Instance.MotionController.GetMotion<RunningJump_v3>(true).IsEnabled = true;
        //m_SliceVfx.transform.SetParent(transform);
        ss.Deactivate();
        ss.IsEnabled = false;
        SlopSlideJump ssj = PlayerBehaviour.Instance.MotionController.GetMotion<SlopSlideJump>(true);
        ssj.Deactivate();
        ssj.IsEnabled = false;
      });
    }

  }
}