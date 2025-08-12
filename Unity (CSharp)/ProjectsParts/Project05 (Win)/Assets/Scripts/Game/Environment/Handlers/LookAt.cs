using System.Collections;

using UnityEngine;

namespace it.Game.Environment.Handlers
{
  public class LookAt : MonoBehaviour
  {
	 [SerializeField]
	 private Transform _target;

	 private void Update()
	 {
		transform.LookAt(_target);
	 }

  }
}