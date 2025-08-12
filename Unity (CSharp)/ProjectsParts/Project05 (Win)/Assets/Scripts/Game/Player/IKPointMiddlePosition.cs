using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKPointMiddlePosition : MonoBehaviour
{
  [SerializeField]
  private Transform _point1;
  [SerializeField]
  private Transform _point2;
  [SerializeField]
  private Transform _model;
  [SerializeField]
  private Transform _sourcePoint;

  [SerializeField]
  private Vector3 _offset;


  //// Update is called once per frame
  //void Update()
  //{
  //transform.position = _point1.position + ((_point2.position - _point1.position) / 2)
  //  + (_model.forward * _offset.z)
  //  + (_model.up * _offset.y)
  //  + (_model.right * _offset.x);

  //}

  private void Update()
  {
    return;
    transform.position = Vector3.Lerp(_point1.position, _point2.position,.5f)
    + (_model.forward * _offset.z)
    + (_model.up * _offset.y)
    + (_model.right * _offset.x);
    transform.rotation = Quaternion.LookRotation((transform.position - _sourcePoint.position), _model.up);
  }

}
