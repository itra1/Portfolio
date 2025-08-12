using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
  public class GishtilBossBattle
  {
	 /// <summary>
	 /// Бой с боссом
	 /// </summary>
	 /// <returns></returns>
	 [UnityTest]
	 public IEnumerator BossBattle()
	 {
		Debug.Log("Start Ghistil boss battle");

		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_6");

		yield return new WaitForSeconds(1);

		var bossBattle = GameObject.FindObjectOfType<it.Game.Environment.Level6.Gishtil.GishtilBattle>();
		Assert.IsNotNull(bossBattle, "Обьект босса не найден");

		var arena = bossBattle.GetComponentInChildren<it.Game.Environment.Level6.Gishtil.GishtilBossArena>();
		Assert.IsNotNull(arena, "Арена не найдена");

		bossBattle.PlayerEnterArena();

		while (bossBattle.IsCutScene)
		  yield return null;

		while (bossBattle.State == 1)
		{
		  yield return new WaitForSeconds(5);
		  arena.Interaction();
		}

		while (bossBattle.IsCutScene)
		  yield return null;

		for (int i = 0; i < arena.Lights.Length; i++)
		{
		  yield return new WaitForSeconds(5);
		  arena.Lights[i]._lightsPortal.FireCintact(true);
		}

		yield return new WaitForSeconds(10);

		Debug.Log("Stop Ghistil boss battle");
	 }

  }
}
