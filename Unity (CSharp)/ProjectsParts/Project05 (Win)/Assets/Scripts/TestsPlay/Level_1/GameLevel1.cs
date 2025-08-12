using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using it.Game.Environment.Level1;
using it.Game.Environment.Level1.AncientTemple;

namespace Tests.Level1
{
  public class GameLevel1
  {

	 [UnitySetUp]
	 private IEnumerator SetUp()
	 {
		yield return null;
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_1");
	 }

	 [UnityTest]
	 public IEnumerator ToNight()
	 {
		Debug.Log("Start The Karmamancer boss battle");
		yield return new WaitForSeconds(2);

		AncientTemple tmp = GameObject.FindObjectOfType<AncientTemple>();
		tmp.SetNight();
		yield return new WaitForSeconds(4);
	 }

  }
}