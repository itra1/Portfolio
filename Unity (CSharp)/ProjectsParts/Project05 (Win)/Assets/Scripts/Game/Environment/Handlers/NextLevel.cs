using UnityEngine;
using System.Collections;
namespace it.Game.Environment.Handlers
{
  public class NextLevel : MonoBehaviourBase
  {
	 public int levelNum;
	 private bool _isActivate = false;
	 public void StartNextLevel()
	 {
		if (_isActivate)
		  return;
		_isActivate = true;
		it.Game.Managers.GameManager.Instance.NextLevel();
	 }
  }
}