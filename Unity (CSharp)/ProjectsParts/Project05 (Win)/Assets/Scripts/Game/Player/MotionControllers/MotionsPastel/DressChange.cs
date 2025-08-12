using UnityEngine;
using System;
using com.ootii.Actors.AnimationControllers;
using com.ootii.Helpers;
using it.Game.Managers;
using UnityEngine.VFX;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Player.MotionControllers.Motions
{

  [MotionName("Dress Change")]
  [MotionDescription("Изменение внешнего вида")]
  public abstract class DressChange : MotionControllerMotion
  {
	 protected Transform EyeGO { get; set; }
	 protected Transform Skin0GO { get; set; }
	 protected Transform Skin1GO { get; set; }
	 protected Transform Skin2GO { get; set; }
	 protected Transform Skin3GO { get; set; }
	 protected Transform TailsGO { get; set; }

	 private string _ActionAlias1;
	 public string ActionAlias1 { get => _ActionAlias1; set => _ActionAlias1 = value; }

	 private string _ActionAlias2;
	 public string ActionAlias2 { get => _ActionAlias2; set => _ActionAlias2 = value; }

	 private string _ActionAlias3;
	 public string ActionAlias3 { get => _ActionAlias3; set => _ActionAlias3 = value; }

	 protected PlayerBehaviour Behaviour { get; set; }
	 private int _targetDress;

	 private Material _tailsMaterial;
	 public Material TailsMaterial { get => _tailsMaterial; set => _tailsMaterial = value; }

	 protected float _timeActivate;
	 protected abstract string VfxResourcePath { get; }

	 protected GameObject VFXPrefab;

	// protected VisualEffect _instanceVfx;

	 public DressChange() : base()
	 {
		_Pack = Idle.GroupName();
		_Category = EnumMotionCategories.PLAYER;

		_Priority = 200;
		_ActionAlias = "PlayerForm2";
		_OverrideLayers = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Player-SM"; }
#endif
	 }

	 public DressChange(MotionController rController) : base(rController)
	 {
		_Pack = Idle.GroupName();
		_Category = EnumMotionCategories.PLAYER;

		_Priority = 200;
		_ActionAlias = "PlayerForm2";

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Player-SM"; }
#endif
	 }

	 public override void Awake()
	 {
		base.Awake();

		VFXPrefab = Resources.Load<GameObject>(VfxResourcePath);
		if (VFXPrefab == null)
		  Debug.LogError("[DressChange] No exists Dress Effect " + VfxResourcePath);

		Behaviour = mMotionController.GetComponent<PlayerBehaviour>();

		EyeGO = Behaviour.transform.Find("model_grp/eyes_grp");
		Skin0GO = Behaviour.transform.Find("model_grp/skin1_grp");
		Skin1GO = Behaviour.transform.Find("model_grp/skin2_grp");
		Skin2GO = Behaviour.transform.Find("model_grp/skin3_grp");
		Skin3GO = Behaviour.transform.Find("model_grp/skin4_grp");
		TailsGO = Behaviour.transform.Find("model_grp/tails_grp");
	 }

	 public void DesctiveAllBones()
	 {
		EyeGO.gameObject.SetActive(false);
		Skin0GO.gameObject.SetActive(false);
		Skin1GO.gameObject.SetActive(false);
		Skin2GO.gameObject.SetActive(false);
		Skin3GO.gameObject.SetActive(false);
		TailsGO.gameObject.SetActive(false);
	 }

	 public void SetDress(int dress)
	 {
		_targetDress = dress;
	 }

	 public override bool TestUpdate()
	 {
		return (_timeActivate + 2 > Time.timeSinceLevelLoad);
	 }

	 public override bool TestActivate()
	 {
		if (!mActorController.IsGrounded)
		  return false;

		return true;
	 }

	 private void OnChange()
	 {

		var dress = mMotionController.GetComponent<PlayerDress>();

		it.Game.Managers.PostProcessManager.Instance.PlayerForm2Process.weight = 0;
		mMotionController.CurrentDress = _targetDress;
		dress.SetDress(_targetDress);

	 }

	 protected void SetTailsMaterial()
	 {
		if (_tailsMaterial == null)
		  return;

		Renderer[] renderersTails = TailsGO.GetComponentsInChildren<Renderer>();

		for (int i = 0; i < renderersTails.Length; i++)
		{
		  renderersTails[i].material = _tailsMaterial;
		}

	 }

	 protected void SpanVfx()
	 {
		GameObject inst = MonoBehaviour.Instantiate(VFXPrefab);
		inst.transform.position = mActorController._Transform.position;
		inst.GetComponent<VisualEffect>().SendEvent("OnPlay");
		MonoBehaviour.Destroy(inst, 15);
	 }

	 protected void AnimateLightMaterial(Transform parent, Color targetColor)
	 {
		Renderer[] _renderers = parent.GetComponentsInChildren<Renderer>();

		for(int i = 0; i < _renderers.Length; i++)
		{
		  for(int x = 0; x < _renderers[i].materials.Length; x++)
		  {
			 _renderers[i].materials[x].SetColor("_FullColor", targetColor);
			 _renderers[i].materials[x].SetFloat("_FullColorLerpValue", 1);
			 _renderers[i].materials[x].DOFloat(0, "_FullColorLerpValue", 2f);
		  }
		}

	 }

	 public override void Deactivate()
	 {
		base.Deactivate();
	 }

	 public override bool TestInterruption(MotionControllerMotion rMotion)
	 {
		return false;
	 }

#if UNITY_EDITOR

	 /// <summary>
	 /// Allow the motion to render it's own GUI
	 /// </summary>
	 public override bool OnInspectorGUI()
	 {
		bool lIsDirty = false;

		if (EditorHelper.TextField("Action Dress", "", ActionAlias, mMotionController))
		{
		  lIsDirty = true;
		  ActionAlias = EditorHelper.FieldStringValue;
		}

		return lIsDirty;
	 }

#endif

  }
}