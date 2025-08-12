using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Player
{
  public abstract class PlayerComponent : MonoBehaviourBase
  {
	 private PlayerBehaviour _PlayerBehaviour;

	 public PlayerBehaviour PlayerBehaviour
	 {
		get
		{
		  if (_PlayerBehaviour == null)
			 _PlayerBehaviour = GetComponent<PlayerBehaviour>();

		  return _PlayerBehaviour;
		}
	 }

  }
}