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
  [MotionName("Skin 1")]
  public class Skin1 : DressChange
  {

	 private Color _lightColor;
	 private Color _tailsColor;
	 protected override string VfxResourcePath => "VFX/Player/DressChange1";
	 public Skin1() : base()
	 {
		_Pack = Idle.GroupName();
		_Category = EnumMotionCategories.PLAYER;

		_Priority = 199;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Player-SM"; }
#endif
	 }
	 public Skin1(MotionController rController) : base(rController)
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
		_lightColor = ProjectSettings.Colors.Find(x => x.title == "PlayerDress1Light").color;
		_tailsColor = ProjectSettings.Colors.Find(x => x.title == "PlayerTails1LightColor").color;
	 }

	 public override bool TestActivate()
	 {
		if (mMotionController.CurrentDress == 1)
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

		mMotionController.CurrentDress = 1;
		EyeGO.gameObject.SetActive(true);
		Skin1GO.gameObject.SetActive(true);
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
		  renderersTails[i].material = ProjectSettings.PlayerTailsMaterials[1];
		}
		AnimateLightMaterial(Skin1GO, _lightColor);
		AnimateLightMaterial(TailsGO, _lightColor);
		return base.Activate(rPrevMotion);
	 }

  }
}