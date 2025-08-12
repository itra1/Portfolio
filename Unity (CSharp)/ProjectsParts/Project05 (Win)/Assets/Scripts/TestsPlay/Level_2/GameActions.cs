using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using it.Game.Environment.Level2;

namespace Tests
{
  public class GameActions : MonoBehaviour
  {

	 /// <summary>
	 /// Тест работы гриба
	 /// </summary>
	 /// <returns></returns>
	 [UnityTest]
	 public IEnumerator MusheroomTest()
	 {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_2");
		yield return new WaitForSeconds(1);

		GameObject button = GameObject.Find("MushroomsButton");
		var buttonComponent = button.GetComponent<it.Game.Environment.All.HolographicTechnoButton>();

		buttonComponent.Interaction();

		yield return new WaitForSeconds(3);

		MushroomPostament postament = GameObject.FindObjectOfType<MushroomPostament>();
		postament.StartInteraction();

		yield return new WaitForSeconds(15);

	 }

	 /// <summary>
	 /// Тесте Щита
	 /// </summary>
	 /// <returns></returns>
	 [UnityTest]
	 public IEnumerator DogotShield()
	 {

		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_2");
		yield return new WaitForSeconds(1);
		var shield = GameObject.FindObjectOfType<DogotShield>();

		shield.PlayerEnterTrigger();

		yield return new WaitForSeconds(15);
	 }

	 /// <summary>
	 /// Тест кокона на пляже
	 /// </summary>
	 /// <returns></returns>
	 [UnityTest]
	 public IEnumerator CoconActivate()
	 {
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level_2");
		yield return new WaitForSeconds(1);
		var cocon = GameObject.FindObjectOfType<Cocon>();

		var peg = cocon.GetComponentInChildren<it.Game.Environment.Handlers.PegasusController>();
		peg.FromCameraPosition = false;

		cocon.Activate();
		yield return new WaitForSeconds(30);
	 }

  }

}