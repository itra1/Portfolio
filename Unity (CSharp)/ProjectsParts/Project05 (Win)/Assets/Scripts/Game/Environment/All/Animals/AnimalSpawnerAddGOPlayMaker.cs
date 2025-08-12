using System.Collections;

using UnityEngine;

namespace it.Game.Environment.All.Animals
{
  public class AnimalSpawnerAddGOPlayMaker : MonoBehaviour, IAnimalSpawnerAfter
  {
	 [SerializeField]
	 private GameObject _gameObject;
	 [SerializeField]
	 private string _variableName;
	 public void AfterSpawn(GameObject instance)
	 {
		PlayMakerFSM pm = instance.GetComponent<PlayMakerFSM>();
		pm.FsmVariables.GetFsmGameObject(_variableName).Value = _gameObject;
	 }
  }
}