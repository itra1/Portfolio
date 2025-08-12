using UnityEngine;
using System.Collections.Generic;
using it.Game.Player;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace it.Game.Environment.All.Animals
{
  public interface IAnimalSpawner
  {

  }
  public interface IAnimalSpawnerAfter 
  {
	 void AfterSpawn(GameObject instance);
  }
}