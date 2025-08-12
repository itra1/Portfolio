using UnityEngine;
using com.ootii.Actors;
using com.ootii.Actors.Navigation;
using com.ootii.Geometry;
using com.ootii.Helpers;
using com.ootii.Messages;
using com.ootii.Actors.AnimationControllers;
using RaycastExt = Utilites.Geometry.RaycastExt;

#if UNITY_EDITOR
using UnityEditor;
using com.ootii.Graphics;
#endif

namespace it.Game.Player.MotionControllers.Motions
{
  [MotionName("Wind Effect Pastel")]
  [MotionDescription("WindEffect")]
  public class WindEffect : MotionControllerMotion
  {

	 private Game.Player.PlayerBehaviour _playerBehaviour;

	 private Vector3 _windValue;

	 public WindEffect()
			: base()
	 {
		_Category = EnumMotionCategories.IDLE;

		_Priority = 1;
		_OverrideLayers = true;
		mIsStartable = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "BasicIdle-SM"; }
#endif
	 }

	 /// <summary>
	 /// Controller constructor
	 /// </summary>
	 /// <param name="rController">Controller the motion belongs to</param>
	 public WindEffect(MotionController rController)
		  : base(rController)
	 {
		_Category = EnumMotionCategories.IDLE;

		_Priority = 1;
		_OverrideLayers = true;
		mIsStartable = true;

#if UNITY_EDITOR
		if (_EditorAnimatorSMName.Length == 0) { _EditorAnimatorSMName = "BasicIdle-SM"; }
#endif
	 }

	 public override bool TestActivate()
	 {
		return true;
	 }

	 public override bool TestUpdate()
	 {
		return true;
	 }
	 public override bool Activate(MotionControllerMotion rPrevMotion)
	 {
		if(_playerBehaviour == null)
		  _playerBehaviour = mMotionController.GetComponent<Game.Player.PlayerBehaviour>();

		return base.Activate(rPrevMotion);
	 }
	 public override void Update(float rDeltaTime, int rUpdateIndex)
	 {
		mVelocity = _playerBehaviour.Wind * rDeltaTime;

		if (_windValue == _playerBehaviour.Wind)
		  return;
		_windValue = _playerBehaviour.Wind;


		//for (int i = 0; i < _playerBehaviour.BoneController.Motors.Count; i++)
		//{
		//  var motor =
		//  Game.Player.PlayerBehaviour.Instance.BoneController.Motors[i]
		//	 as com.ootii.Actors.BoneControllers.BoneChainDragMotor;

		//  motor.IsGravityEnabled = true;
		//  motor.Gravity = _playerBehaviour.Wind;
		//}
	 }

	 }
}