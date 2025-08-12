using UnityEngine;
using System.Collections;
namespace it.Game.Handles
{
  public class RotateAround : HandlersBase
  {
	 [SerializeField]
	 private Vector3 _rotateion;

	 public Vector3 Rotateion { get => _rotateion; set => _rotateion = value; }

	 protected override void OnUpdate()
	 {
		transform.rotation *= Quaternion.Euler(_rotateion.x*Time.deltaTime, _rotateion.y * Time.deltaTime, _rotateion.z * Time.deltaTime);
	 }
  }
}