using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
  /// <summary>
  /// Бой с кармамансером
  /// </summary>
  public class TheKarmamancerBossArena
  {

	 [UnityTest]
	 public IEnumerator BossBattle()
	 {
		Debug.Log("Start The Karmamancer boss battle");
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_4");

		yield return new WaitForSeconds(5);

		var arena = GameObject.FindObjectOfType<it.Game.Environment.Level4.KarmamancerBossArena>();

		arena.PlayerEnterArena();

		yield return new WaitForSeconds(30);

		Debug.Log("Stop The Karmamancer boss battle");

	 }

	 [UnityTest]
	 public IEnumerator OrbAttack()
	 {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_4");

		yield return new WaitForSeconds(5);

		var arena = GameObject.FindObjectOfType<it.Game.Environment.Level4.KarmamancerBossArena>();

		var enemy = arena.GetComponentInChildren<it.Game.NPC.Enemyes.Boses.Karmamancer.TheKarmamancer>(true);

		Assert.IsNotNull(enemy, "Не найден босс");

		enemy.gameObject.SetActive(true);

		enemy.FsmBehaviour.SendEvent("OnTestOrbAttack");

		yield return new WaitForSeconds(30);

	 }

	 [UnityTest]
	 public IEnumerator BigCast()
	 {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_4");

		yield return new WaitForSeconds(5);

		var arena = GameObject.FindObjectOfType<it.Game.Environment.Level4.KarmamancerBossArena>();

		var enemy = arena.GetComponentInChildren<it.Game.NPC.Enemyes.Boses.Karmamancer.TheKarmamancer>(true);

		Assert.IsNotNull(enemy, "Не найден босс");

		enemy.gameObject.SetActive(true);

		enemy.FsmBehaviour.SendEvent("OnTestBigCast");

		yield return new WaitForSeconds(30);

	 }
  }
}