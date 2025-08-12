using UnityEngine;
#if UNITY_EDITOR
using Handles = UnityEditor.Handles;
#endif

namespace HutongGames.PlayMaker.Actions
{

  [ActionCategory(ActionCategory.Debug)]
  [Tooltip("Draw Gizmos in the Scene View.")]
  public class DebugDrawWipeDisk : FsmStateAction
  {
	 public FsmGameObject source;
	 public FsmFloat radius;
	 public override void OnEnter()
	 {
		base.OnEnter();
		Finish();
	 }
	 public override void OnDrawActionGizmosSelected()
	 {

#if UNITY_EDITOR
		UnityEditor.Handles.DrawWireDisc(source.Value.transform.position, source.Value.transform.up, radius.Value);
#endif

	 }

	 public override void OnDrawActionGizmos()
	 {
#if UNITY_EDITOR
		UnityEditor.Handles.DrawWireDisc(source.Value.transform.position, source.Value.transform.up, radius.Value);
#endif
	 }
  }
}