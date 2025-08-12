using UnityEngine;
using System.Collections;
namespace it.Game.NPC.Animals
{
  public class Deer : Animal
  {
	 private bool _isPlayerContact;

	 public bool IsPlayerContact { get => _isPlayerContact; }

	 public void SetPlayerContact(bool isContact)
	 {
		_isPlayerContact = isContact;
	 }
  }
}