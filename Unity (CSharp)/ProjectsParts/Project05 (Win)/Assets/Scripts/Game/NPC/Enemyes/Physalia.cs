using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.NPC.Enemyes
{
  public class Physalia : Enemy
  {
    [SerializeField]
    public LayerMask _playerLayer;
    private void OnCollisionEnter(Collision collision)
    {
      if ((collision.gameObject.layer & _playerLayer) != 0)
      {
        Debug.Log("Player collision");
      }
    }

    public void ActivateBoneController()
    {
      GetComponentInChildren<com.ootii.Actors.BoneControllers.BoneController>().enabled = true;
    }
    public void DeactivateBoneController()
    {
      GetComponentInChildren<com.ootii.Actors.BoneControllers.BoneController>().enabled = false;
    }

  }
}