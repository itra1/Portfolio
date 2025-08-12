using UnityEngine;
using System.Collections;
namespace it.Game.NPC.Enemyes.Boses.SarDayyni
{
  public class SarDayyni : Enemy
  {
	 [SerializeField]
	 private PlayMakerFSM _behaviour;
	 public PlayMakerFSM Behaviour { get => _behaviour; set => _behaviour = value; }
  }
}