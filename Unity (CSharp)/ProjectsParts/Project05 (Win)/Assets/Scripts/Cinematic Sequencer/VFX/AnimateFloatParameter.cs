using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Slate.ActionClips;
using UnityEngine.VFX;

namespace Slate.ActionClips.VFX
{
  [Category("VFX")]
  [Description("Анимация VFX параметров")]
  public class AnimateFloatParameter : VFXBaseClip
  {

	 [SerializeField]
	 [HideInInspector]
	 private float _length = 1f;
	 [SerializeField]
	 [HideInInspector]
	 private float _blendIn = 0.2f;
	 [SerializeField]
	 [HideInInspector]
	 private float _blendOut = 0.2f;
	 public string parameterName;
	 [AnimatableParameter]
	 public float value = 1;

	 private UnityEngine.VFX.VisualEffect _vfx;

	 private float lastValue;

	 public override bool isValid
	 {
		get { return base.isValid && HasParameter(parameterName); }
	 }

	 public override string info
	 {
		get { return string.Format("'{0}' Parameter", parameterName); }
	 }

	 public override float length
	 {
		get { return _length; }
		set { _length = value; }
	 }

	 public override float blendIn
	 {
		get { return _blendIn; }
		set { _blendIn = value; }
	 }

	 public override float blendOut
	 {
		get { return _blendOut; }
		set { _blendOut = value; }
	 }

	 protected override void OnEnter()
	 {
		lastValue = actor.GetFloat(parameterName);
	 }

	 protected override void OnUpdate(float deltaTime)
	 {
		actor.SetFloat(parameterName, Mathf.Lerp(lastValue, value, GetClipWeight(deltaTime)));
	 }

	 protected override void OnReverse()
	 {
		actor.SetFloat(parameterName, lastValue);
	 }
  }
}