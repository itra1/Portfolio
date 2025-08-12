using UnityEngine;
using com.ootii.Actors.AnimationControllers;
using com.ootii.Helpers;
using it.Game.Managers;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Skin 3")]
  public class Skin3 : DressChange
  {

	 private Color _lightColor;
	 private Color _tailsColor;
	 protected override string VfxResourcePath => "VFX/Player/DressChange3";
	 public Skin3() : base()
	 {
		_Pack = Idle.GroupName();
		_Category = EnumMotionCategories.PLAYER;

		_Priority = 199;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Player-SM"; }
#endif
	 }
	 public Skin3(MotionController rController) : base(rController)
	 {
		_Pack = Idle.GroupName();
		_Category = EnumMotionCategories.PLAYER;

		_Priority = 199;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Player-SM"; }
#endif
	 }
	 public override void Initialize()
	 {
		_lightColor = ProjectSettings.Colors.Find(x => x.title == "PlayerDress3Light").color;
		_tailsColor = ProjectSettings.Colors.Find(x => x.title == "PlayerTails3LightColor").color;
	 }

	 public override bool TestActivate()
	 {
		if (mMotionController.CurrentDress == 3)
		  return false;

		if (!base.TestActivate())
		  return false;

		if (mMotionController.CurrentDress == 0 && GameManager.Instance.EnergyManager.Percent > .3f)
		{
		  return true;
		}

		if (base._ActionAlias.Length > 0 && mMotionController._InputSource != null)
		{
		  if (mMotionController._InputSource.IsJustPressed(base._ActionAlias) && GameManager.Instance.EnergyManager.Percent > .3f)
		  {
			 return true;
		  }
		}

		return false;
	 }

	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		_timeActivate = Time.timeSinceLevelLoad;
		SpanVfx();

		Light[] tailsLight = PlayerBehaviour.Instance.TailsLights;
		for (int i = 0; i < tailsLight.Length; i++)
		{
		  tailsLight[i].gameObject.SetActive(false);
		}

		DesctiveAllBones();

		mMotionController.CurrentDress = 3;
		Skin3GO.gameObject.SetActive(true);
		AnimateLightMaterial(Skin3GO, _lightColor);
		//AnimateLightMaterial(TailsGO, LightColor);

		return base.Activate(rPrevMotion);
	 }


  }
}