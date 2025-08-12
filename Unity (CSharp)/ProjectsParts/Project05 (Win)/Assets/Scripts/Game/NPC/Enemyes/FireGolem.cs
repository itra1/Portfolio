using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace it.Game.NPC.Enemyes
{
  public class FireGolem : Enemy
  {
    [SerializeField]
    private PlayMakerFSM _Behaviour;

    public PlayMakerFSM Behaviour { get => _Behaviour; set => _Behaviour = value; }
  }
}