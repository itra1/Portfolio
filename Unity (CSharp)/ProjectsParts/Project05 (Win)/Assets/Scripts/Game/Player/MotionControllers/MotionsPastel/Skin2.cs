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
  [MotionName("Skin 2")]
  public class Skin2 : DressChange
  {

	 private Color _lightColor;
	 private Color _tailsColor;

	 protected override string VfxResourcePath => "VFX/Player/DressChange2";

	 public Skin2() : base()
	 {
		_Pack = Idle.GroupName();
		_Category = EnumMotionCategories.PLAYER;

		_Priority = 199;

	 }
	 public Skin2(MotionController rController) : base(rController)
	 {
		_Pack = Idle.GroupName();
		_Category = EnumMotionCategories.PLAYER;

		_Priority = 199;

	 }

	 public override void Initialize()
	 {
		_lightColor = ProjectSettings.Colors.Find(x => x.title == "PlayerDress2Light").color;
		_tailsColor = ProjectSettings.Colors.Find(x => x.title == "PlayerTails2LightColor").color;
	 }

	 public override bool TestActivate()
	 {
		if (mMotionController.CurrentDress == 2)
		  return false;

		if (!base.TestActivate())
		  return false;

		if (base._ActionAlias.Length > 0 && mMotionController._InputSource != null)
		{
		  if (mMotionController._InputSource.IsJustPressed(base._ActionAlias))
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

		DesctiveAllBones();

		mMotionController.CurrentDress = 2;
		EyeGO.gameObject.SetActive(true);
		Skin2GO.gameObject.SetActive(true);
		TailsGO.gameObject.SetActive(true);
		SetTailsMaterial();

		Light[] tailsLight = PlayerBehaviour.Instance.TailsLights;
		for (int i = 0; i < tailsLight.Length; i++)
		{
		  tailsLight[i].gameObject.SetActive(true);
		  tailsLight[i].color = _tailsColor;
		}

		Renderer[] renderersTails = TailsGO.GetComponentsInChildren<Renderer>();

		for (int i = 0; i < renderersTails.Length; i++)
		{
		  renderersTails[i].material = ProjectSettings.PlayerTailsMaterials[2];
		}
		AnimateLightMaterial(Skin2GO, _lightColor);
		AnimateLightMaterial(TailsGO, _lightColor);

		return base.Activate(rPrevMotion);
	 }

	 public override void Deactivate()
	 {
		base.Deactivate();
	 }

	 public override bool TestInterruption(MotionControllerMotion rMotion)
	 {
		return false;
	 }
  }
}