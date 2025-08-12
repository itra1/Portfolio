using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

namespace it.Game.Environment.Level2
{
  /// <summary>
  /// Катсцена первого появления
  /// </summary>
  public class StartCutscene : Environment
  {
	 [SerializeField] private Transform _startPosition;
	 [SerializeField] private it.Game.NPC.Enemyes.Enemy _dagot;
	 [SerializeField] private PlayMakerFSM _playMaker;
	 [SerializeField] private VisualEffect _startHole;

	 /// <summary>
	 /// Первисное появление
	 /// </summary>
	 public void StartVisible()
	 {
		if (State != 0)
		  return;

		State = 1;
		_playMaker.SendEvent("StartFSM");
		Save();
	 }

  }
}