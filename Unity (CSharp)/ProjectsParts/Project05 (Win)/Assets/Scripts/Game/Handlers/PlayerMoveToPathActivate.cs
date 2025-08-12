using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Game.Player.Actors;

namespace it.Game.Handles
{
  public class PlayerMoveToPathActivate : MonoBehaviourBase
  {
    public void Activate()
    {
      Game.Player.PlayerBehaviour.Instance.MotionController.IsEnabled = false;

      //UnityEngine.AI.NavMeshAgent naw = Game.Player.PlayerBehaviour.Instance.gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
      PlayerActorMovePath move = Game.Player.PlayerBehaviour.Instance.gameObject.AddComponent<PlayerActorMovePath>();
      
      move.SetPath(GetComponent<it.Game.NPC.Helpers.WayPointHelper>().WayPoints);

      move.OnComplete = () =>
      {
        DestroyImmediate(move);
        Game.Player.PlayerBehaviour.Instance.MotionController.IsEnabled = true;
      };
    }

  }
}