using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.Level5.TrapRoom
{
  public class TrapRoomCenter : MonoBehaviourBase
  {

	 [SerializeField]
	 private TrapRoom _trapRoom;

	 public bool IsInteractReady => true;

  }
}