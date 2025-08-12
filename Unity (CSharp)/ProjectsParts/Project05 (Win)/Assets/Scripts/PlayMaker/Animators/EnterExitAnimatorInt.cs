using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Animators
{
  [ActionCategory(ActionCategory.Animator)]
  [Tooltip("Sets the value of an integer parameter")]
  public class EnterExitAnimatorInt : ComponentAction<Animator>
  {
	 [RequiredField]
	 [CheckForComponent(typeof(Animator))]
	 [Tooltip("The target.")]
	 public FsmOwnerDefault gameObject;
	 [RequiredField]
	 [UIHint(UIHint.AnimatorInt)]
	 [Tooltip("The animator parameter")]
	 public FsmString parameter;
	 [Tooltip("The Int value to assign to the animator parameter enter")]
	 public FsmInt ValueEnter;
	 [Tooltip("The Int value to assign to the animator parameter exit")]
	 public FsmInt ValueExit;

	 private Animator animator
	 {
		get { return cachedComponent; }
	 }

	 private string cachedParameter;
	 private int paramID;

	 public override void Reset()
	 {
		base.Reset();
		gameObject = null;
		parameter = null;
		ValueEnter = null;
		ValueExit = null;
	 }

	 public override void OnEnter()
	 {
		paramID = Animator.StringToHash(parameter.Value);
		animator.SetInteger(paramID, ValueEnter.Value);

	 }
	 public override void OnExit()
	 {
		animator.SetInteger(paramID, ValueExit.Value);
	 }
  }
}