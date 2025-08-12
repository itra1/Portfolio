using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace it.Game.Utils
{
  public class NavMeshHelpers
  {

	 public static Vector3 CheckPath(UnityEngine.AI.NavMeshAgent agent, Vector3 targetPosition)
	 {
      NavMeshPath path = new NavMeshPath();
      agent.CalculatePath(targetPosition, path);

      if (path.status == NavMeshPathStatus.PathComplete)
      {
        return path.corners[path.corners.Length - 1];
      }
      return Vector3.zero;
    }
  }
}