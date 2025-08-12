using UnityEngine;
using com.ootii.Actors.Navigation;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Messages;
using com.ootii.Actors.AnimationControllers;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace it.Game.NPC.Enemy.Motions
{
  public class NPCInit : MotionControllerMotion
  {

	 public NPCInit()
	 : base()
	 {
		_Category = EnumMotionCategories.IDLE;

		_Priority = 0;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Init"; }
#endif
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public NPCInit(MotionController rController)
		  : base(rController)
	 {
		_Category = EnumMotionCategories.IDLE;

		_Priority = 0;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "Init"; }
#endif
	 }

	 public override void Initialize()
	 {
	 }

  }
}