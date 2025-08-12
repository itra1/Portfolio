using UnityEngine;
using com.ootii.Actors.Navigation;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Messages;
using com.ootii.Actors.AnimationControllers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.NPC.Motions
{
  public class TavrosWayPointsMove : MotionControllerMotion
  {

	 public int PHASE_UNKNOWN = 0;
	 public int PHASE_START = 501;

	 protected it.Game.NPC.NPC _npc;
	 protected UnityEngine.AI.NavMeshAgent _navMeshAgent;
	 protected it.Game.NPC.Helpers.WayPointHelper _wayPointHelper;

	 public TavrosWayPointsMove()
	 : base()
	 {
		_Category = EnumMotionCategories.IDLE;

		_Priority = 4;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "WalkRun"; }
#endif
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public TavrosWayPointsMove(MotionController rController)
		  : base(rController)
	 {
		_Category = EnumMotionCategories.IDLE;

		_Priority = 4;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "WalkRun"; }
#endif
	 }


	 public override void Initialize()
	 {
		_npc = mMotionController.GetComponent<it.Game.NPC.NPC>();
		_navMeshAgent = mMotionController.GetComponent<UnityEngine.AI.NavMeshAgent>();
		_wayPointHelper = mMotionController.GetComponent<it.Game.NPC.Helpers.WayPointHelper>();
	 }


	 public override bool TestActivate()
	 {
		// Don't activate
		return true;
	 }

	 /// <summary>
	 /// Tests if the motion should continue. If it shouldn't, the motion
	 /// is typically disabled
	 /// </summary>
	 /// <returns></returns>
	 public override bool TestUpdate()
	 {
		// Stay
		return true;
	 }

	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_START, 0, Parameter, true);
		// Finalize the activation
		return base.Activate(rPrevMotion);
	 }

	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		mMovement = Vector3.zero;
		mRotation = Quaternion.identity;

		mMotionController.Animator.SetFloat("Speed", 0.5f);
		mMotionController.Animator.SetFloat("Direction", 1);

		//// Smooth the input so we don't start and stop immediately in the blend tree.
		//SmoothInput();

		//// Use the AC to rotate the character towards the input
		//RotateToInput(mMotionController.State.InputFromAvatarAngle, rDeltaTime, ref mRotation);

		//// Force a style change if needed
		//if (_Form <= 0 && mActiveForm != mMotionController.CurrentForm)
		//{
		//  mActiveForm = mMotionController.CurrentForm;
		//  mMotionController.SetAnimatorMotionPhase(mMotionLayer.AnimatorLayerIndex, PHASE_START, mActiveForm, 0, true);
		//}
	 }

  }
}