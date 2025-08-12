using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
  /// <summary>
  /// Бой с боссом пристом
  /// </summary>
  public class PriestBossBattle : MonoBehaviour
  {
	 [UnityTest]
	 public IEnumerator BossBattle()
	 {

		Debug.Log("Start Priest boss battle");
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_5");

		yield return new WaitForSeconds(5);

		var arena = GameObject.FindObjectOfType<it.Game.Environment.Level5.PriestArena.PriestArena>();

		arena.BattleReady();

		yield return new WaitForSeconds(1);

		for (int i = 0; i < arena.Abelisks.Length; i++)
		{
		  yield return new WaitForSeconds(5);
		  arena.Abelisks[i].Activation();
		}
		yield return new WaitForSeconds(10);

		Debug.Log("Stop Priest boss battle");
	 }

	 [UnityTest]
	 public IEnumerator PriestAttack1()
	 {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_5");

		yield return new WaitForSeconds(1);

		var arena = GameObject.FindObjectOfType<it.Game.Environment.Level5.PriestArena.PriestArena>();

		var priest = arena.GetComponentInChildren<it.Game.NPC.Enemyes.Priest>(true);

		priest.gameObject.SetActive(true);
		yield return new WaitForSeconds(3);

		priest.Behaviour.SendEvent("OnTestAttack1");

		yield return new WaitForSeconds(10);

	 }

	 [UnityTest]
	 public IEnumerator PriestAttack2()
	 {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_5");

		yield return new WaitForSeconds(1);

		var arena = GameObject.FindObjectOfType<it.Game.Environment.Level5.PriestArena.PriestArena>();

		var priest = arena.GetComponentInChildren<it.Game.NPC.Enemyes.Priest>(true);

		priest.gameObject.SetActive(true);
		yield return new WaitForSeconds(3);

		priest.Behaviour.SendEvent("OnTestAttack2");

		yield return new WaitForSeconds(15);

	 }

	 [UnityTest]
	 public IEnumerator PriestAttack3()
	 {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_5");

		yield return new WaitForSeconds(1);

		var arena = GameObject.FindObjectOfType<it.Game.Environment.Level5.PriestArena.PriestArena>();

		var priest = arena.GetComponentInChildren<it.Game.NPC.Enemyes.Priest>(true);

		priest.gameObject.SetActive(true);
		yield return new WaitForSeconds(3);

		for (int i = 0; i < 5; i++)
		{
		  yield return Attack3(priest);
		  yield return new WaitForSeconds(15);
		}

		yield return new WaitForSeconds(3);

	 }

	 private IEnumerator Attack3(it.Game.NPC.Enemyes.Priest priest)
	 {

		priest.Behaviour.SendEvent("OnTestAttack3");
		yield return new WaitForSeconds(15);

	 }

  }
}