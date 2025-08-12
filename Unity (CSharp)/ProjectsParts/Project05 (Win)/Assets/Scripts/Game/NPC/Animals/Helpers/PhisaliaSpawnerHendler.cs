using UnityEngine;
using System.Collections;

namespace it.Game.NPC.Enemyes.Handles
{
  public class PhisaliaSpawnerHendler : MonoBehaviourBase, it.Game.Environment.All.Animals.IAnimalSpawnerAfter
  {
	 [SerializeField]
	 private string _playeMakerName = "Behaviour";
	 [SerializeField]
	 private GameObject[] _wayPoints;

	 public void AfterSpawn(GameObject instance)
	 {
		PlayMakerFSM _playMaker = instance.GetComponent<NPC>().GetFsm(_playeMakerName);
		_playMaker.FsmVariables.GetFsmArray("WayPoints").Values = _wayPoints;
	 }
  }
}