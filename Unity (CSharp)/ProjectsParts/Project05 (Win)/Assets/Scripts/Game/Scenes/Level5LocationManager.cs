using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Scenes
{
  public class Level5LocationManager : LocationManager
  {
	 public override int LevelIndex => 5;

	 [SerializeField]
	 private GameObject _island;
	 [SerializeField]
	 private GameObject _dungeon;

	 /// <summary>
	 /// Вход на остров
	 /// </summary>
	 public void EnterIsland()
	 {
		_island.SetActive(true);
		_dungeon.SetActive(false);
	 }

	 /// <summary>
	 /// Вход в подземелье
	 /// </summary>
	 public void EnterDungeon()
	 {
		_island.SetActive(false);
		_dungeon.SetActive(true);
	 }

	 
  }
}