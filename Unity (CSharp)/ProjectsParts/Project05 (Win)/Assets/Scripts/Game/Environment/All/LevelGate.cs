using UnityEngine;
using it.Game.Managers;
using it.Game.Player;
using QFX.IFX;
using DG.Tweening;

namespace it.Game.Environment
{
  public class LevelGate : MonoBehaviourBase
  {

	 public void Gate()
	 {
		it.Game.Managers.GameManager.Instance.NextLevel();
	 }

  }
}