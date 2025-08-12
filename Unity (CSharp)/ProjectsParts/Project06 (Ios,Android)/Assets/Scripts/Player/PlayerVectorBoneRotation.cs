using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVectorBoneRotation : MonoBehaviour
{
  [SerializeField]
  private Transform _vectorBone;

  private float _targetAndle;
  [SerializeField]
  private float _speedRotation;

  private bool _increment;
  private float _actualAngle;

  private void Start() {
    _actualAngle = 0;
  }

  public void SetAngleVector(float angle) {
    _increment = angle > _actualAngle;
    _targetAndle = angle;
  }
  private void Update() {

    if (_targetAndle == _actualAngle)
      return;

    float nextAngle = _actualAngle + _speedRotation * Time.deltaTime * (_increment ? 1 : -1);

    if ((_increment && nextAngle > _targetAndle) || (!_increment && nextAngle < _targetAndle))
      _actualAngle = _targetAndle;
    else
      _actualAngle = nextAngle;

    _vectorBone.localEulerAngles = new Vector3(0, 0, _actualAngle);
  }



}
