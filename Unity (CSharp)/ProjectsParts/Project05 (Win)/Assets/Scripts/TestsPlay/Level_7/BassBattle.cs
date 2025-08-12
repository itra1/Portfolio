using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
  public class BassBattle
  {
	 [UnityTest]
	 public IEnumerator BattleActivated()
	 {

		Debug.Log("Battle run");

		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_7");

		yield return new WaitForSeconds(5);

		var bossArena = GameObject.FindObjectOfType<it.Game.Environment.Level7.SarDayyni.SarDayyniArena>();

		bossArena.StartBattle();


		for (int i = 0; i < bossArena.ControlPanels.Length; i++)
		{
		  yield return new WaitForSeconds(2);
		  bossArena.ControlPanels[i].Activate();
		}

		yield return null;

		Assert.AreEqual(bossArena.State, 2, "Не вошли во вторую фазу");

		Debug.Log("Phase 2");

		yield return new WaitForSeconds(2);

		var extractor = bossArena.GetComponentInChildren<it.Game.Items.Inventary.SoulExtractorItem>();

		Assert.IsNotNull(extractor, "НЕ нашли экстрактор");

		extractor.GetItem();

		Assert.AreEqual(bossArena.State, 3, "Не перешли в третью фазу");

		for (int i = 0; i < bossArena.CoilElectric.Length; i++)
		{
		  yield return new WaitForSeconds(2);

		  for(int c = 0; c < bossArena.CoilElectric[i].Cails.Length; c++)
		  {
			 bossArena.CoilElectric[i].Cails[c].Interaction();
			 yield return new WaitForSeconds(2);
		  }
		}
		yield return new WaitForSeconds(10);

		Assert.AreEqual(bossArena.State, 4, "Не перешли в четвертую фазу");

		Debug.Log("Phase 5");

		yield return null;

		for (int i = 0; i < bossArena.Obelisks.Length; i++)
		{
		  yield return new WaitForSeconds(2);
		  bossArena.Obelisks[i].Activate();
		}
		Assert.AreEqual(bossArena.State, 5, "Не перешли в пятую фазу");
		Debug.Log("Phase 5");
		yield return new WaitForSeconds(10);

		Debug.Log("Тест закончен");


	 }

	 [UnityTest]
	 public IEnumerator BossAttack()
	 {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_7");

		yield return new WaitForSeconds(1);

		var bossArena = GameObject.FindObjectOfType<it.Game.Environment.Level7.SarDayyni.SarDayyniArena>();

		bossArena.Enemy.Behaviour.SendEvent("StartBattle");

		yield return new WaitForSeconds(300);

	 }

  }
}
