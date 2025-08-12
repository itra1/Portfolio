using UnityEngine;
using com.ootii.Cameras;
using com.ootii.Geometry;
using com.ootii.Helpers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.ootii.Actors.AnimationControllers.Motions
{
  [MotionName("Sitting Walk Run Pivot")]
  public class SittingWalkRunPivot : MotionControllerMotion, IWalkRunMotion, IPivotMotion
  {
	 public bool IsRunActive => throw new System.NotImplementedException();

	 public bool StartInMove { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
	 public bool StartInWalk { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
	 public bool StartInRun { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
	 public bool DefaultToRun { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
  }
}