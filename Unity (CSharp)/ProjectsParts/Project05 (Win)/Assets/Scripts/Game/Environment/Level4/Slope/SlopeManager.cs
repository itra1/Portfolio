using it.Game.Handles;
using it.Game.Player;
using UnityEngine;
using UnityEngine.VFX;
using it.Game.Player.MotionControllers.Motions;
using RunningJump = it.Game.Player.MotionControllers.Motions.RunningJump;

namespace it.Game.Environment{

  public class SlopeManager : Environment
  {

    [SerializeField]
    private TriggerPlayerHandler m_Collider;

    [SerializeField]
    private FluffyUnderware.Curvy.CurvySpline m_Spline;

    [SerializeField]
    private VisualEffect m_SliceVfx;

    protected override void Start () {
      base.Start();
      m_Collider.onTriggerEnter.AddListener(PlayerEnterSlopeCollider);
      m_Collider.onTriggerExit.AddListener(PlayerExitSlopeCollider);
      m_SliceVfx.SendEvent ("OnStop");
      m_SliceVfx.gameObject.SetActive(false);
    }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
        if (State <= 1)
          DisableSlop();
      }
	 }

	 public void PlayerEnterSlopeCollider () {
      ActivateSlope(PlayerBehaviour.Instance);
    }

    public void PlayerExitSlopeCollider ()
    {
      DeactiveSlope(PlayerBehaviour.Instance);
    }

    private void ActivateSlope (PlayerBehaviour player) {

      if (State > 0)
        return;
      State = 1;
      Save();
      EnableComponent();
    }

    private void DeactiveSlope(PlayerBehaviour player)
    {

      if (State < 1)
        return;
      State = 2;

      DisableSlop();
    }

    private void EnableComponent()
	 {

      Debug.Log("ActivateSlope");

      m_SliceVfx.transform.SetParent(CameraBehaviour.Instance.transform);
      m_SliceVfx.transform.localPosition = Vector3.zero;

      PlayerBehaviour.Instance.MotionController.GetMotion<JumpPastel>(true).IsEnabled = false;
      PlayerBehaviour.Instance.MotionController.GetMotion<RunningJump_v3>(true).IsEnabled = false;

      PlayerBehaviour.Instance.MotionController.GetMotion<SlopSlideJump>(true).IsEnabled = true;
      SlopSlide_v2 ss = PlayerBehaviour.Instance.MotionController.GetMotion<SlopSlide_v2>(true);
      ss.IsEnabled = true;

      ss.SetSpline(m_Spline);
      ss.SetSliceVfx(m_SliceVfx);

    }

    private void DisableSlop()
	 {
      if (PlayerBehaviour.Instance == null)
        return;

      SlopSlide_v2 ss = PlayerBehaviour.Instance.MotionController.GetMotion<SlopSlide_v2>(true);

      ss.Slowdown(() => {

        PlayerBehaviour.Instance.MotionController.GetMotion<JumpPastel>(true).IsEnabled = true;
        PlayerBehaviour.Instance.MotionController.GetMotion<RunningJump_v3>(true).IsEnabled = true;
        m_SliceVfx.transform.SetParent(transform);
        ss.Deactivate();
        ss.IsEnabled = false;
        SlopSlideJump ssj = PlayerBehaviour.Instance.MotionController.GetMotion<SlopSlideJump>(true);
        ssj.Deactivate();
        ssj.IsEnabled = false;
      });
    }

  }
}