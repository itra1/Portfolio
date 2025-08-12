using System.Collections;
using System.Collections.Generic;

using com.ootii.Actors.BoneControllers;

using it.Game.Player.BoneControllers;

using UnityEngine;
namespace Develop
{
  public class TailsCorrect : MonoBehaviour
  {
	 [SerializeField]
	 private AnimationCurve _stiff;
	 [SerializeField]
	 private float _dragLerp = 1;
	 [SerializeField]
	 private float _untwistLerp = 1;
	 [SerializeField]
	 private bool _isFixedUpdateEnabled = true;
	 [SerializeField]
	 private int _fps = 60;
	 [SerializeField]
	 private bool _useBindPosition= true;

#if UNITY_EDITOR

	 [SerializeField]
	 private BoneController _boneController;

	 [ContextMenu("Correct")]
	 public void Correct()
	 {
		if(_boneController == null)
		_boneController = GetComponent<BoneController>();

		List<TailsMotor> _motors = _boneController.GetMotors<TailsMotor>();

		for(int i = 0; i < _motors.Count; i++)
		{
		  _motors[i].Stiffness = _stiff;
		  _motors[i].FixedUpdateFPS = _fps;
		  _motors[i].IsFixedUpdateEnabled = _isFixedUpdateEnabled;
		  for (int b = 0; b < _motors[i]._BoneInfo.Count; b++)
		  {
			 _motors[i]._BoneInfo[b].RotationLerp = _dragLerp;
			 _motors[i]._BoneInfo[b].UntwistLerp = _untwistLerp;
			 _motors[i]._BoneInfo[b].UseBindPosition = _useBindPosition;
		  }
		}
		UnityEditor.EditorUtility.SetDirty(_boneController);
	 }

#endif

  }
}