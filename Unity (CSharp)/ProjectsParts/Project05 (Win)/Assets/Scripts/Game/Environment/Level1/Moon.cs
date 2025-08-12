using UnityEngine;
using System.Collections;

namespace it.Game.Entitys.Level1
{
  /// <summary>
  /// Луна на первом уровне
  /// </summary>
  public class Moon : MonoBehaviourBase
  {
	 private void Start()
	 {
		var distComp = gameObject.AddComponent<Game.Environment.All.FixedDistanceToTarget>();
		distComp.Target = CameraBehaviour.Instance.transform;
		distComp.Distance = 1000f;

		var lockComp = gameObject.AddComponent<Game.Environment.All.LockToTarget>();
		lockComp.Target = CameraBehaviour.Instance.transform;

	 }
  }
}