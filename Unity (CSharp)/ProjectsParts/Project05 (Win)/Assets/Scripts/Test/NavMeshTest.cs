using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NavMeshTest : MonoBehaviour
{
  public Transform target;
  NavMeshAgent agent;
  NavMeshPath path;
  void Start()
  {
    agent = GetComponent<NavMeshAgent>();

    NavMeshPath path = new NavMeshPath();
    agent.CalculatePath(target.position, path);
    if (path.status == NavMeshPathStatus.PathPartial)
    {
    }

  }

  private void Update()
  {

    path = new NavMeshPath();
    agent.CalculatePath(target.position, path);
    Debug.Log(path.status);
  }

  private void OnDrawGizmos()
  {
    if(path != null && path.corners.Length > 0)
    {

      for(int i = 0; i < path.corners.Length; i++)
      {
        if(i < path.corners.Length-1)
        Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
      }

    }
  }
}
