using UnityEngine;
using System.Collections;

namespace it.Game.NPC.Enemyes.Cthulhu
{
  public class Cthulhu : Enemy
  {
    private bool isSet;
    protected override void Start()
    {
      base.Start();
      StartCoroutine(FindPlayer());
    }

    protected override void Update()
    {
      base.Update();
    }

    IEnumerator FindPlayer()
    {
      isSet = false;

      while (!isSet)
      {
        if(it.Game.Player.PlayerBehaviour.Instance != null)
        {
          PlayMakerFSM[] fsms = gameObject.GetComponents<PlayMakerFSM>();
          for(int i = 0; i < fsms.Length; i++)
          {
            var player = fsms[i].FsmVariables.GetFsmGameObject("Player");
            if(player != null)
            {
              player.Value = it.Game.Player.PlayerBehaviour.Instance.gameObject;
            }
          }
          isSet = true;
        }
        yield return null;
      }
    }

  }

}