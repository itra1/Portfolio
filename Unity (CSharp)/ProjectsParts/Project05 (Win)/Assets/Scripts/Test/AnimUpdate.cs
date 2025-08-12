using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimUpdate : MonoBehaviour
{
  [SerializeField]
  private GameObject _character;
  private void OnAnimatorMove()
  {
	 if (_character == null)
		return;
	 _character.SendMessage("OnAnimatorMove");
  }

  private void OnAnimatorIK(int layerIndex)
  {

	 if (_character == null)
		return;
	 _character.SendMessage("OnAnimatorIK", layerIndex);
  }
}
